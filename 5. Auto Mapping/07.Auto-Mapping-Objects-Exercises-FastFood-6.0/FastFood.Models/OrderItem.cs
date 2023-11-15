namespace FastFood.Models
{
    using System.ComponentModel.DataAnnotations;

    public class OrderItem
    {
        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public int ItemId { get; set; }

       public Item Item { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}