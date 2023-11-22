using CarDealer.Models;

namespace CarDealer.DTOs.Import
{
    public class PartsDTO
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int SupplierId { get; set; }
    }
}
