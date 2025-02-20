using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Entities
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Color { get; set; } = "#FFFFFF";
        public string Category { get; set; } = "Default";
        public int Order {get; set;}
        public ICollection<Item> Items { get; set; } = new List<Item>();

        [ForeignKey("RoomId")]
        public int RoomId {get; set;}

        [JsonIgnore]
        public Room? Room { get; set; }
    }
}