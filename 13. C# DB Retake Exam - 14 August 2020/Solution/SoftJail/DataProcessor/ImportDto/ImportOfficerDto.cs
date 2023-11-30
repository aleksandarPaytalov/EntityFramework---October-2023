using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficerDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; } = null!;

        [XmlElement("Money")]
        [Required]
        [Range(0, 9999999999999999999)]
        public decimal Money { get; set; }

        [XmlElement("Position")]
        [Required]
        public string Position { get; set; } = null!;

        [XmlElement("Weapon")]
        [Required]
        public string Weapon { get; set; } = null!;

        [XmlElement("DepartmentId")]
        [Required]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public ImportOfficerPrisonerDto[] Prisoners { get; set; }
    }
    //•	Id – integer, Primary Key
    //•	FullName – text with min length 3 and max length 30 (required)
    //•	Salary – decimal (non-negative, minimum value: 0) (required)
    //•	Position – Position enumeration with possible values: "Overseer, Guard, Watcher, Labour" (required)
    //•	Weapon – Weapon enumeration with possible values: "Knife, FlashPulse, ChainRifle, Pistol, Sniper" (required)
    //•	DepartmentId – integer, foreign key(required)
    //•	Department – the officer's department (required)
    //•	OfficerPrisoners – collection of type OfficerPrisoner

    [XmlType("Prisoner")]
    public class ImportOfficerPrisonerDto
    {
        [XmlAttribute("id")]
        [Required]
        public int PrisonerId { get; set; }
    }
}
