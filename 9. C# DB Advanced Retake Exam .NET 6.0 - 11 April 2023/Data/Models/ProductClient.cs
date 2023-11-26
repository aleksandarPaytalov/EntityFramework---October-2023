using System.ComponentModel.DataAnnotations.Schema;

namespace Invoices.Data.Models;

using System.ComponentModel.DataAnnotations;

public class ProductClient
{
    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Client))]
    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

}

