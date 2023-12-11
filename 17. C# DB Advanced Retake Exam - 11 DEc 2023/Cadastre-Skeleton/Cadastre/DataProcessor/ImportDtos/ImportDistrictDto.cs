using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ImportDtos
{
    [XmlType("District")]
    public class ImportDistrictDto
    {
        [XmlElement("Name")]
        [Required]
        [StringLength(80, MinimumLength = 2)]
        public string Name { get; set; }

        [XmlElement("PostalCode")]
        [Required]
        [RegularExpression("^[A-Z]{2}-\\d{5}$")]
        public string PostalCode { get; set; }

        [Required]
        [XmlAttribute("Region")]
        public string Region { get; set; }

        [XmlArray("Properties")]
        public ImportPropertyDto[] Properties { get; set; }
    }

    //Id – integer, Primary Key
    //Name – text with length[2, 80] (required)
    //PostalCode – text with length 8. All postal codes must have the following structure:starting with two capital letters, followed by e dash '-', followed by five digits.Example: SF-10000 (required)
    //Region – Region enum (SouthEast = 0, SouthWest, NorthEast, NorthWest) (required)
    //Properties - collection of type Property

    [XmlType("Property")]
    public class ImportPropertyDto
    {
        [Required]
        [XmlElement("PropertyIdentifier")]
        [StringLength(20, MinimumLength = 16)]
        public string PropertyIdentifier { get; set; }

        //
        [Required]
        [XmlElement("Area")]
        [Range(0, 2147483647)]
        public int Area { get; set; }

        [XmlElement("Details")]
        [StringLength(500, MinimumLength = 5)]
        public string? Details { get; set; }

        [Required]
        [XmlElement("Address")]
        [StringLength(200, MinimumLength = 5)]
        public string Address { get; set; }

        [Required]
        [XmlElement("DateOfAcquisition")]
        public string DateOfAcquisition { get; set; }
    }

    //Id – integer, Primary Key
    //PropertyIdentifier – text with length[16, 20] (required)
    //Area – int not negative(required)
    //Details - text with length[5, 500] (not required)
    //Address – text with length[5, 200] (required)
    //DateOfAcquisition – DateTime(required)
    //DistrictId – integer, foreign key(required)
    //District – District
    //PropertiesCitizens - collection of type PropertyCitizen

}
