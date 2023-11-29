using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Theatre.DataProcessor.ImportDto
{
    public class ImportTheatreDto
    {
        [JsonProperty("Name")]
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Name { get; set; } = null!;

        [JsonProperty("NumberOfHalls")]
        [Required]
        [Range(1,10)]
        public sbyte NumberOfHalls { get; set; }

        [JsonProperty("Director")]
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Director { get; set; } = null!;

        [JsonProperty("Tickets")] 
        public ImportTicketDto[] Tickets { get; set; } = null!;
    }
}

//Name – text with length [4, 30] (required)
//NumberOfHalls – sbyte between [1…10] (required)
//Director – text with length [4, 30] (required)

public class ImportTicketDto
{
    [JsonProperty("Price")]
    [Required]
    [Range(1.00,100.00)]
    public decimal Price { get; set; }

    [JsonProperty("RowNumber")]
    [Required]
    [Range(1,10)]
    public sbyte RowNumber { get; set; }

    [JsonProperty("PlayId")]
    [Required]
    public int PlayId { get; set; }
}

//Price – decimal in the range [1.00….100.00] (required)
//RowNumber – sbyte in range [1….10] (required)
//PLayId

