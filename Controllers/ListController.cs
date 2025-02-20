using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using API.Controllers.Services;
using API.Data;
using API.Entities;
using API.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/room/{roomId}/list")]
    public class ListController : ControllerBase
    {
        private readonly ListService _listsService;
        private readonly IHubContext<ListHub> _hubContext;

        public ListController(ListService listsService, IHubContext<ListHub> hubContext)
        {
            _listsService = listsService;
            _hubContext = hubContext;
        }

        public class UpdateListRequest
        {
            public required int Id {get; set;}
            public string? Name {get; set;}
            public string? Color {get; set;}
            public string? Category {get; set;}
        }

        public class DeleteListRequest
        {
            public required int Id {get; set;}
        }

        [HttpGet]
        public ActionResult<List<ShoppingList>> GetLists(int roomId)
        {
            var lists = _listsService.GetLists(roomId);

            if (lists == null || lists.Count == 0)
            {
                return NotFound("No lists found for this room.");
            }

            return Ok(lists);
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateList(int roomId, [FromBody] ShoppingList list)
        {
            if (string.IsNullOrWhiteSpace(list.Name))
            {
                return BadRequest("Name of List is required!");
            }

            var shoppingList = _listsService.CreateList(roomId, list.Name, list.Color, list.Category);

            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("NewList", shoppingList);

            return Ok(new
            {
                message = "Successfully created the List!",
                roomId = shoppingList.RoomId,
                listId = shoppingList.Id,
                listName = shoppingList.Name,
                listColor = shoppingList.Color,
                listCategory = shoppingList.Category,
            });
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateList(int roomId, [FromBody] UpdateListRequest request)
        {
            var shoppingList = _listsService.UpdateList(request.Id, roomId, request.Name, request.Color, request.Category);

            Console.WriteLine("SHOPPING LIST: ", shoppingList);

            await _hubContext.Clients.Group(roomId.ToString()).SendAsync($"editedList-{request.Id}", shoppingList);

            return Ok(new
            {
                message = "List Updated!",
                listId = shoppingList.Id,
                listRoomId = shoppingList.RoomId,
                listName = shoppingList.Name,
                listColor = shoppingList.Color,
                listCategory = shoppingList.Category,
            });
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteList(int roomId, [FromBody] DeleteListRequest request)
        {
            var deletedList = _listsService.DeleteList(roomId, request.Id);

            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("DeletedList", deletedList.Id);

            return Ok(new
            {
                message = "List deleted.",
                listId = deletedList.Id,
                listRoomId = deletedList.RoomId,
                listName = deletedList.Name,
                listColor = deletedList.Color,
                listCategory = deletedList.Category,
            });
        }
    }
}