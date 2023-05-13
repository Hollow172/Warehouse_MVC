using System.ComponentModel.DataAnnotations;

namespace Warehouse.Models
{
    public class NewProduct
    {
        public int id { get; set; }
        [StringLength(8, MinimumLength = 1)]
        public string code { get; set; }
        [StringLength(16)]
        public string name { get; set; }
        public string description { get; set; }
        public Category? category { get; set; }
        public int categoryId { get; set; }
        public float quantity { get; set; }
        public string measure { get; set; }
        public float price { get; set; }
        [Timestamp]
        public byte[]? Version { get; set; }
    }
}
