using Real_chat.Data;
using Real_chat.Models;
using Microsoft.EntityFrameworkCore;

namespace Real_chat.Service
{
    public class RoomService
    {
        private readonly ChatContext _context;

        public RoomService(ChatContext context)
        {
            _context = context;
        }

        public async Task<Room> CreateRoom(string name, string password)
        {
            var room = new Room
            {
                Name = name,
                Password = BCrypt.Net.BCrypt.HashPassword(password) 
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }

        public async Task<Room> GetRoomByNameAsync(string name)
        {
            return await _context.Rooms.FirstOrDefaultAsync(r => r.Name == name);
        }

        public bool ValidateRoomPassword(Room room, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, room.Password);
        }
        public async Task<bool> RoomExists(string name)
        {
            // Логіка перевірки наявності кімнати
            return await _context.Rooms.AnyAsync(r => r.Name == name);
        }

    }
}
