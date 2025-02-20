using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public required string Password { get; set; }

        [JsonIgnore]
        public ICollection<ShoppingList> Lists { get; set; } = new List<ShoppingList>();
    }
}