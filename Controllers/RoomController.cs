using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using API.Controllers.Services;

namespace API.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService;
        
        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        public class CreateRoomRequest
        {
            public required string Password { get; set; }
        }

        public class UpdateRoomRequest
        {
            public required string Code { get; set; }
            public required string CurrentPassword { get; set; }
            public required string NewPassword {get; set;}
        }

        public class DeleteRoomRequest
        {
            public required int Id {get; set;}
            public required string Code { get; set; }
            public required string Password { get; set; }
        }
        
        [HttpPost("create")]
        public ActionResult CreateRoom([FromBody] CreateRoomRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Password is required!");
            }

            var room = _roomService.CreateRoom(request.Password);

            return Ok(new 
            { 
                message = "Room created successfully!",
                roomId = room.Id, 
                roomCode = room.Code,
                roomPassword = room.Password
            });
        }

        [HttpPut("update")]
        public ActionResult UpdateRoom([FromBody] UpdateRoomRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("Code, current password, and new password are required!");
            }

            var updatedRoom = _roomService.UpdateRoom(request.Code, request.CurrentPassword, request.NewPassword);

            if (updatedRoom == null)
            {
                return NotFound("Room not found or incorrect password!");
            }

            return Ok(new
            {
                message = "Room password updated successfully!",
                roomId = updatedRoom.Id,
                roomCode = updatedRoom.Code,
                roomPassword = updatedRoom.Password
            });
        }
        
        [HttpDelete("delete")]
        public ActionResult DeleteRoom([FromBody] DeleteRoomRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Code and password are required to delete the room!");
            }
            var deletedRoom = _roomService.DeleteRoom(request.Id, request.Code, request.Password);

            return Ok(new
            {
                message = "Room deleted.",
                roomId = deletedRoom.Id,
                roomCode = deletedRoom.Code,
                roomPassword = deletedRoom.Password
            });
        }

        [HttpPost("join")]
        public ActionResult JoinRoom([FromBody] Room request)
        {
            if(request.Code == null || request.Password == null)
            {
                return BadRequest("Please check your code and password!");
            }

            var room = _roomService.JoinRoom(request.Code, request.Password);

            if (room == null)
            {
                return NotFound("Room not found or incorrect password!");
            }

            return Ok(new
            {
                message = "Successfully joined the room!",
                roomId = room.Id,
                roomCode = room.Code,
                roomPassword = room.Password
            });
        }
    }
}