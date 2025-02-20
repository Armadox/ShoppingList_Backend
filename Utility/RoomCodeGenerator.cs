using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;

namespace API.Utility
{
    public class RoomCodeGenerator
    {
        public static string GenerateUniqueRoomCode(StoreContext context)
        {
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numbers = "0123456789";
            string roomCode;

            do
            {
                var randomLetters = new string(Enumerable.Range(0, 2).Select(x => letters[Random.Shared.Next(letters.Length)]).ToArray());
                var randomNumbers = new string(Enumerable.Range(0, 6).Select(x => numbers[Random.Shared.Next(numbers.Length)]).ToArray());
                roomCode = randomLetters + randomNumbers;

            } while (context.Rooms.Any(r => r.Code == roomCode));

            return roomCode;
        }

    }
}