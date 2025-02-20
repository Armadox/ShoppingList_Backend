using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal? Price { get; set; }
        public string? Store { get; set; }
        public string? Brand { get; set; }
        public int? Quantity { get; set; }

        [NotMapped]
        public decimal Result => (Price ?? 0) * (Quantity ?? 0);

        [ForeignKey("ShoppingListId")]
        public int ShoppingListId { get; set; }

        [JsonIgnore]
        public ShoppingList? ShoppingList { get; set; }
    }
}