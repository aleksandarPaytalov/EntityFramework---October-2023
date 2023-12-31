﻿namespace Invoices.Data.Models;

using System.ComponentModel.DataAnnotations;

public class Client
{
    public Client()
    {
        this.Invoices = new HashSet<Invoice>();
        this.Addresses = new HashSet<Address>();
        this.ProductsClients = new HashSet<ProductClient>();
    }
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(15)]
    public string NumberVat { get; set; } = null!;

    public ICollection<Invoice> Invoices { get; set; }

    public ICollection<Address> Addresses { get; set; }

    public ICollection<ProductClient> ProductsClients { get; set; }
}

