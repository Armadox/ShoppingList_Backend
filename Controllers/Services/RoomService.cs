using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Utility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using API.Controllers.Services;

namespace API.Controllers.Services
{
    public class RoomService
    {
        private readonly StoreContext _context;

        public RoomService(StoreContext context)
        {
            _context = context;
        }

        //CREATE ROOM
        public Room CreateRoom(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var room = new Room
            {
                Password = hashedPassword,
                Code = RoomCodeGenerator.GenerateUniqueRoomCode(_context)
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();
            return room;
        }

        //UPDATE ROOM
        public Room UpdateRoom(string code, string currentPassword, string newPassword)
        {
            var room = _context.Rooms.SingleOrDefault(r => r.Code == code);
            if(room == null)
            {
                throw new ArgumentException("Room not found!");
            }

            Console.WriteLine($"Current Password: {currentPassword}");
            Console.WriteLine($"Stored Hash: {room.Password}");

            if(!BCrypt.Net.BCrypt.Verify(currentPassword, room.Password))
            {
                throw new ArgumentException("Wrong Password!"); 
            }

            room.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _context.SaveChanges();

            return room;
        }

        //DELETE ROOM
        public Room DeleteRoom(int roomId, string code, string password)
        {
            var room = _context.Rooms.FirstOrDefault(req => req.Id == roomId);

            if(room == null){throw new ArgumentException("Room doesnt exist!");}
            if(room.Code != code){throw new ArgumentException("Room Code is wrong!");}
            if(!BCrypt.Net.BCrypt.Verify(password, room.Password)){throw new ArgumentException("Room Password is wrong! :", BCrypt.Net.BCrypt.HashPassword(password));}

            try{
                _context.Rooms.Remove(room);
                _context.SaveChanges();
                return room;
            }catch(Exception ex){
                throw new ArgumentException("An error occurred while trying to delete the room.", ex);
            }
        }

        //JOIN ROOM
        public Room? JoinRoom(string code, string password)
        {
            var room = _context.Rooms.Include(req => req.Lists).ThenInclude(l => l.Items).FirstOrDefault(req => req.Code == code);

            if (room == null)
            {
                throw new ArgumentException("Room can't be found!"); 
            }

            if (BCrypt.Net.BCrypt.Verify(password, room.Password) || password == room.Password)
            {
                return room;
            }

            throw new ArgumentException("Password wrong!"); 
        }
    }
}