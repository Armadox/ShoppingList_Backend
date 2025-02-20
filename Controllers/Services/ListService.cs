using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers.Services
{
    public class ListService
    {
        private readonly StoreContext _context;

        public ListService(StoreContext context)
        {
            _context = context;
        }

        //GETTING LISTS
        public List<ShoppingList> GetLists(int roomId)
        {
            var lists = _context.ShoppingLists.Where(list => list.RoomId == roomId).Include(list => list.Items).ToList();

            return lists;
        }

        //CREATING LIST
        public ShoppingList CreateList(int roomId, string name, string color, string category)
        {
            var room = _context.Rooms.Include(room => room.Lists).FirstOrDefault(room => room.Id == roomId);
            if (room == null)
            {
                throw new ArgumentException("Room not found.");
            }

            var shoppingList = new ShoppingList
            {
                Name = name,
                Color = color,
                Category = category,
                RoomId = roomId,
                Order = room.Lists.Count + 1
            };

            _context.ShoppingLists.Add(shoppingList);
            _context.SaveChanges();

            return shoppingList;
        }

        //UPDATE LIST
        public ShoppingList UpdateList(int listId, int roomId, string? name, string? color, string? category)
        {
            var list = _context.ShoppingLists.FirstOrDefault(list => list.Id == listId && list.RoomId == roomId);

            if(list == null)
            {
                throw new ArgumentException("List not found.");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                list.Name = name;
            }
            if (!string.IsNullOrWhiteSpace(color))
            {
                list.Color = color;
            }
            if (!string.IsNullOrWhiteSpace(category))
            {
                list.Category = category;
            }

            _context.SaveChanges();
            return list;
        }

        //DELETE LIST
        public ShoppingList DeleteList(int roomId, int listId)
        {
            var list = _context.ShoppingLists.FirstOrDefault(list => list.Id == listId && list.RoomId == roomId);
            Console.WriteLine($"List ID: {listId}, Room ID: {roomId}");
            try{

                if(list == null)
                {
                    throw new ArgumentException("List not found!"); 
                }

                _context.ShoppingLists.Remove(list);
                _context.SaveChanges();

                return list;
            }catch(Exception ex){
                throw new ArgumentException("An error occurred while trying to delete the list.", ex);
            }
        }
    }
}