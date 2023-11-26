using System.ComponentModel.DataAnnotations;
using Invoices.Data.Models.Enums;
using Newtonsoft.Json;

namespace Invoices.DataProcessor.ImportDto;

public class ImportProductDto
{
    [JsonProperty("Name")]
    [Required]
    [MinLength(9)]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [JsonProperty("Price")]
    [Required]
    [Range(5.00,1000.00)]
    public decimal Price { get; set; }

    [JsonProperty("CategoryType")]
    [Required]
    [Range(0, 4)]
    public int CategoryType { get; set; }


    [JsonProperty("Clients")] 
    public int[] Clients { get; set; } = null!;
}

