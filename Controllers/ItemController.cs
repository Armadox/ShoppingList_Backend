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
    [Route("api/room/{roomId}/list/item")]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _itemService;

        private readonly IHubContext<ListHub> _hubContext;

        public ItemController(ItemService itemService, IHubContext<ListHub> hubContext)
        {
            _itemService = itemService;
            _hubContext = hubContext;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateItem(int roomId, [FromBody] Item item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                return BadRequest("Name of Item is required!");
            }

            var newItem = _itemService.CreateItem(item.ShoppingListId, item.Name, item.Price, item.Store, item.Brand, item.Quantity);

            await _hubContext.Clients.Group(roomId.ToString()).SendAsync($"createdItem-{item.ShoppingListId}", newItem);

            return Ok(new
            {
                message = "Successfully created the Item!",
                listId = newItem.ShoppingListId,
                itemId = newItem.Id,
                itemName = newItem.Name,
                itemPrice = newItem.Price,
                itemStore = newItem.Store,
                itemBrand = newItem.Brand,
                itemQuantity = newItem.Quantity
            });
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateItem(int roomId, [FromBody] Item item)
        {
            var updatedItem = _itemService.UpdateItem(item.ShoppingListId, item.Id, item.Name, item.Price, item.Store, item.Brand, item.Quantity);

            await _hubContext.Clients.Group(roomId.ToString()).SendAsync($"updatedItem-{item.ShoppingListId}", updatedItem);


            return Ok(new
            {
                message = "Successfully updated the Item!",
                listId = updatedItem.ShoppingListId,
                itemId = updatedItem.Id,
                itemName = updatedItem.Name,
                itemPrice = updatedItem.Price,
                itemStore = updatedItem.Store,
                itemBrand = updatedItem.Brand,
                itemQuantity = updatedItem.Quantity
            });
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteItem(int roomId, [FromBody] Item item)
        {
            var deletedItem = _itemService.DeleteItem(item.ShoppingListId, item.Id);

            if(deletedItem.Price != null || deletedItem.Quantity != null || deletedItem.Brand != null || deletedItem.Store != null){
                var signalItem = new Item {
                    Id = deletedItem.Id,
                    ShoppingListId = deletedItem.ShoppingListId,
                    Name = deletedItem.Name,
                    Price = null,
                    Store = null,
                    Brand = null,
                    Quantity = null
                };
                await _hubContext.Clients.Group(roomId.ToString()).SendAsync($"deletedItem-{item.ShoppingListId}", signalItem);
            }else{
                await _hubContext.Clients.Group(roomId.ToString()).SendAsync($"deletedItem-{item.ShoppingListId}", deletedItem.Id);
            }

            

            return Ok(new
            {
                message = "Successfully deleted the Item!",
                listId = deletedItem.ShoppingListId,
                itemId = deletedItem.Id,
                itemName = deletedItem.Name,
                itemPrice = deletedItem.Price,
                itemStore = deletedItem.Store,
                itemBrand = deletedItem.Brand,
                itemQuantity = deletedItem.Quantity
            });
        }
    }
}