using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers.Services
{
    public class ItemService
    {
        private readonly StoreContext _context;

        public ItemService(StoreContext context)
        {
            _context = context;
        }

        //CREATE ITEM
        public Item CreateItem(int listId, string name, decimal? price, string? store, string? brand, int? quantity)
        {
            var shoppingList = _context.ShoppingLists.FirstOrDefault(list => list.Id == listId);
            if (shoppingList == null)
            {
                throw new ArgumentException("Shopping list not found.");
            }

            var item = new Item
            {
                Name = name,
                Price = price,
                Store = store,
                Brand = brand,
                Quantity = quantity,
                ShoppingListId = listId
            };

            _context.Items.Add(item);
            _context.SaveChanges();

            return item;
        }

        //UPDATE ITEM
        public Item UpdateItem(int listId, int itemId, string? name, decimal? price, string? store, string? brand, int? quantity)
        {
            
            var item = _context.Items.FirstOrDefault(item => item.Id == itemId && item.ShoppingListId == listId);
            Console.WriteLine($"LIST ID: {listId}, ITEM ID: {itemId}");

            if(item == null)
            {
                throw new ArgumentException("item not found.");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                item.Name = name;
            }

            if (price != null)
            {
                item.Price = price;
            }

            if (!string.IsNullOrWhiteSpace(store))
            {
                item.Store = store;
            }

            if (!string.IsNullOrWhiteSpace(brand))
            {
                item.Brand = brand;
            }

            if (quantity != null)
            {
                item.Quantity = quantity;
            }

            _context.SaveChanges();

            return item;
        }

        //DELETE ITEM
        public Item DeleteItem(int listId, int itemId)
        {
            var item = _context.Items.FirstOrDefault(item => item.ShoppingListId == listId && item.Id == itemId);
            if (item == null)
            {
                throw new ArgumentException("Item not found on the list.");
            }

            if (item.Price != null || item.Store != null || item.Brand != null || item.Quantity != null)
            {
                var oldItem = new Item 
                {
                    Id = item.Id,
                    ShoppingListId = item.ShoppingListId,
                    Name = item.Name,
                    Price = item.Price,
                    Store = item.Store,
                    Brand = item.Brand,
                    Quantity = item.Quantity
                };

                item.Price = null;
                item.Store = null;
                item.Brand = null;
                item.Quantity = null;

                _context.SaveChanges();
                return oldItem;
            }

            _context.Items.Remove(item);
            _context.SaveChanges();

            return item;
        }
    }
}