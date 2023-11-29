using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto;

[XmlType("Country")]
public class ImportCountryDto
{
    [Required]
    [XmlElement("CountryName")]
    [MinLength(4)]
    [MaxLength(60)]
    public string CountryName { get; set; } = null!;

    [Required]
    [XmlElement("ArmySize")]
    [Range(50000,10000000)]
    public int ArmySize { get; set; }

    
}

    //• CountryName – text with length[4, 60] (required)
    //•	ArmySize – integer in the range[50_000….10_000_000] (required)
