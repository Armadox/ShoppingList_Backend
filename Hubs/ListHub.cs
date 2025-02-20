using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using API.Entities;

namespace API.Hubs
{
    public class ListHub : Hub
    {
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            Console.WriteLine($"Client {Context.ConnectionId} joined room {roomId}");
        }
        
        public async Task NewListSignal(int roomId, ShoppingList list)
        {
            await Clients.Others.SendAsync(roomId.ToString(), list);
        }

    }
}