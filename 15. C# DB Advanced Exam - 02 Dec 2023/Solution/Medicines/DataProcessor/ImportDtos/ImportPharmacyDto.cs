

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class ImportPharmacyDto
    {
        [XmlAttribute("non-stop")] 
        [Required] 
        public string IsNonStop { get; set; } = null!;

        [XmlElement("Name")]
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [XmlElement("PhoneNumber")]
        [Required]
        [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$")]
        public string PhoneNumber { get; set; } = null!;

        [XmlArray("Medicines")]
        [Required]
        public ImportMedicineDto[] Medicines { get; set; } = null!;
    }


    //Id – integer, Primary Key
    //Name – text with length[2, 50] (required)
    //PhoneNumber – text with length 14. (required)
    //All phone numbers must have the following structure:
    //three digits enclosed in parentheses, followed by a space, three more digits, a hyphen, and four final digits: 

    //Example -> (123) 456-7890 
    //IsNonStop – bool (required)
    //Medicines - collection of type Medicine

    [XmlType("Medicine")]
    public class ImportMedicineDto
    {
        [XmlAttribute("category")]
        [Required]
        [Range(0,4)]
        public int Category { get; set; }

        [XmlElement("Name")]
        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [XmlElement("Price")]
        [Required]
        [Range(0.01, 1000.00)]
        public decimal Price { get; set; }

        [XmlElement("ProductionDate")]
        [Required]
        public string ProductionDate { get; set; } = null!;

        [XmlElement("ExpiryDate")]
        [Required]
        public string ExpiryDate { get; set; } = null!;

        [XmlElement("Producer")]
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Producer { get; set; } = null!;
    }

    //Id – integer, Primary Key
    //Name – text with length[3, 150] (required)
    //Price – decimal in range[0.01…1000.00] (required)
    //Category – Category enum (Analgesic = 0, Antibiotic, Antiseptic, Sedative, Vaccine) (required)
    //ProductionDate – DateTime(required)
    //ExpiryDate – DateTime(required)
    //Producer – text with length[3, 100] (required)
    //PharmacyId – integer, foreign key(required)
    //Pharmacy – Pharmacy
    //PatientsMedicines - collection of type PatientMedicine

}
