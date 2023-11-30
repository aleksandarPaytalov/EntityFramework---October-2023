using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonerDto
    {
        [JsonProperty("FullName")]
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string FullName { get; set; } = null!;

        [JsonProperty("Nickname")]
        [Required]
        [RegularExpression(@"^(The\s)([A-Z]{1}[a-z]*)$")]
        public string Nickname { get; set; } = null!;


        [JsonProperty("Age")]
        [Required]
        [Range(18,65)]
        public int Age { get; set; }

        [JsonProperty("IncarcerationDate")]
        [Required]
        public string IncarcerationDate { get; set; } = null!;

        //
        [JsonProperty("ReleaseDate")]
        public string? ReleaseDate { get; set; }

        [JsonProperty("Bail")]
        [Range(0, 9999999999999999999)]
        //"79228162514264337593543950335" 
        public decimal? Bail { get; set; }

        [JsonProperty("CellId")]
        public int? CellId { get; set; }

        [JsonProperty("Mails")]
        public ImportMailDto[] Mails { get; set; } = null!;
        //•	Id – integer, Primary Key
        //•	FullName – text with min length 3 and max length 20 (required)
        //•	Nickname – text starting with "The " and a single word only of letters with an uppercase letter for beginning(example: The Prisoner) (required)
        //•	Age – integer in the range[18, 65] (required)
        //•	IncarcerationDate ¬– Date(required)
        //•	ReleaseDate – Date
        //•	Bail – decimal (non-negative, minimum value: 0)
        //•	CellId - integer, foreign key
        //•	Cell – the prisoner's cell
        //•	Mails – collection of type Mail
        //•	PrisonerOfficers - collection of type OfficerPrisoner

    }

    public class ImportMailDto
    {
        [JsonProperty("Description")]
        [Required]
        public string Description { get; set; } = null!;

        [JsonProperty("Sender")]
        [Required]
        public string Sender { get; set; } = null!;

        [JsonProperty("Address")]
        [Required]
        [RegularExpression(@"^([A-Za-z0-9\s]+?)(\sstr\.)$")]
        public string Address { get; set; } = null!;

        

        //      •	Id – integer, Primary Key
        //      •	Description – text(required)
        //    •	Sender – text(required)
        //    •	Address – text, consisting only of letters, spaces and numbers, which ends with "str." (required) (Example: "62 Muir Hill str.")
        //      •	PrisonerId - integer, foreign key(required)
        //    •	Prisoner – the mail's Prisoner (required)

    }
}
