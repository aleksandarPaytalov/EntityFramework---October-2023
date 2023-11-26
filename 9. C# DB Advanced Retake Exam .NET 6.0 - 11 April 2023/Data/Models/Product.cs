namespace Invoices.Data.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Enums;

public class Product
{
    public Product()
    {
        this.ProductsClients = new HashSet<ProductClient>();
    }

    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    //
    [Required]
    public decimal Price { get; set; }

    [Required]
    public  CategoryType CategoryType { get; set; }

    public ICollection<ProductClient> ProductsClients { get; set; }
}

