namespace Footballers.DataProcessor.ImportDto;

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;


[XmlType("Footballer")]
public class ImportFootballerDTO
{
    [Required] 
    [XmlElement("Name")]
    [MinLength(2)]
    [MaxLength(40)]
    public string Name { get; set; } = null!;

    [Required]
    [XmlElement("ContractStartDate")]
    public string ContractStartDate { get; set; } = null!;

    [Required]
    [XmlElement("ContractEndDate")]
    public string ContractEndDate { get; set; } = null!;

    [Required]
    [XmlElement("BestSkillType")]
    [Range(0,4)]
    public int BestSkillType { get; set; }

    [Required]
    [XmlElement("PositionType")]
    [Range(0,3)]
    public int PositionType { get; set; }
}

