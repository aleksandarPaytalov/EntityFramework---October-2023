namespace Footballers.DataProcessor.ImportDto;

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;


[XmlType("Coach")]
public class ImportCoachDTO
{
    [Required]
    [XmlElement("Name")]
    [MinLength(2)]
    [MaxLength(40)]
    public string Name { get; set; } = null!;

    [Required]
    [XmlElement("Nationality")]
    public string Nationality { get; set; } = null!;

    [XmlArray("Footballers")]
    public ImportFootballerDTO[] Footballers { get; set; } = null!;
}

