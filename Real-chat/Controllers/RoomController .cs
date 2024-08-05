using Microsoft.AspNetCore.Mvc;
using Real_chat.Service;
using Real_chat.DTO;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly RoomService _roomService;

    public RoomController(RoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] RoomDto roomDto)
    {
        if (await _roomService.RoomExists(roomDto.Name))
        {
            return Conflict("Room already exists"); 
        }

        var room = await _roomService.CreateRoom(roomDto.Name, roomDto.Password);
        if (room != null)
        {
            return Ok();
        }
        return BadRequest("Failed to create room");
    }
}