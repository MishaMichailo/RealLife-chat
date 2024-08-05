using Microsoft.AspNetCore.SignalR;
using Real_chat.Data;
using Real_chat.Models;
using Microsoft.EntityFrameworkCore;
using Real_chat.DTO;
using Real_chat.Service;

namespace Real_chat.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string userName, string message);
        Task ReceiveMessageHistory(List<ChatMessageDto> messages);
    }

    public class ChatHub : Hub<IChatClient>
    {
        private readonly ChatContext _context;
        private readonly RoomService _roomService;

        public ChatHub(ChatContext context, RoomService roomService)
        {
            _context = context;
            _roomService = roomService;
        }

        public async Task JoinRoom(string userName, string roomName, string password)
        {
            var room = await _roomService.GetRoomByNameAsync(roomName);

            if (room == null)
            {
                await Clients.Caller.ReceiveMessage("Admin", "Кімната не знайдена.");
                return;
            }

            if (!_roomService.ValidateRoomPassword(room, password))
            {
                await Clients.Caller.ReceiveMessage("Admin", "Неправильний пароль.");
                return;
            }

            var existingConnection = await _context.Connections
                .FirstOrDefaultAsync(c => c.UserId == room.Id && c.UserName == userName);

            if (existingConnection != null)
            {
                existingConnection.ConnectionId = Context.ConnectionId;
            }
            else
            {
                var connection = new UserConnection
                {
                    ConnectionId = Context.ConnectionId,
                    ChatRoom = room.Name,
                    UserName = userName,
                    
                };

                _context.Connections.Add(connection);
            }

            try
            {
                await _context.SaveChangesAsync();
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Name);
                await Clients.Group(room.Name).ReceiveMessage("Admin", $"{userName} приєднався до чату.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                await Clients.Caller.ReceiveMessage("Admin", "An error occurred while joining the room.");
            }
        }

        public async Task SendMessage(string message)
        {
            var connection = await _context.Connections
                .Include(uc => uc.User)
                .FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);

            if (connection != null)
            {
                var chatMessage = new ChatMessage
                {
                    ChatRoom = connection.ChatRoom,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    UserId = connection.User.Id
                };

                _context.ChatMessages.Add(chatMessage);

                try
                {
                    await _context.SaveChangesAsync();
                    await Clients
                        .Group(connection.ChatRoom)
                        .ReceiveMessage(connection.User.UserName, message);
                }
                catch (DbUpdateException dbEx)
                {
                    // Log database update exceptions
                    Console.WriteLine($"Database update error: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    // Log other exceptions
                    Console.WriteLine($"Error sending message: {ex.Message}");
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = await _context.Connections
                .Include(uc => uc.User)
                .FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);

            if (connection != null)
            {
                // Log connection details for debugging
                Console.WriteLine($"User {connection.User?.UserName ?? "Unknown"} disconnected from chat room {connection.ChatRoom}");

                _context.Connections.Remove(connection);
                await _context.SaveChangesAsync();

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom);

                // Only send message if user information is available
                if (connection.User != null)
                {
                    await Clients
                        .Group(connection.ChatRoom)
                        .ReceiveMessage("Admin", $"{connection.User.UserName} покинув чат");
                }
                else
                {
                    await Clients
                        .Group(connection.ChatRoom)
                        .ReceiveMessage("Admin", $"Користувач покинув чат");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
