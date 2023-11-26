namespace Footballers.DataProcessor.ImportDto;

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


public class ImportTeamDto
{
    [JsonProperty("Name")]
    [Required]
    [RegularExpression(@"[a-zA-Z0-9\s.\-]{3,40}")]
    public string Name { get; set; } = null!;
    
    [JsonProperty("Nationality")]
    [Required]
    [MinLength(2)]
    [MaxLength(40)]
    public string Nationality { get; set; } = null!;

    [JsonProperty("Trophies")]
    [Required]
    public int Trophies { get; set; }

    [Required]
    [JsonProperty("Footballers")]
    public int[] FootballersId { get; set; } = null!;
}

