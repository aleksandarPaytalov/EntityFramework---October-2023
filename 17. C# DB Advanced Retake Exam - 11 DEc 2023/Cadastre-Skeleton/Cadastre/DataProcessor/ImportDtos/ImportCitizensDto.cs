using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastre.DataProcessor.ImportDtos
{
    public class ImportCitizensDto
    {
        [Required]
        [JsonProperty("FirstName")]
        [StringLength(30, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [JsonProperty("LastName")]
        [StringLength(30,MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [JsonProperty("BirthDate")]
        public string BirthDate { get; set; }

        [Required]
        [JsonProperty("MaritalStatus")]
        public string MaritalStatus { get; set; }

        [JsonProperty("Properties")]
        public int[] Properties { get; set; }
    }

    //Id – integer, Primary Key
    //FirstName – text with length[2, 30] (required)
    //LastName – text with length[2, 30] (required)
    //BirthDate – DateTime(required)
    //MaritalStatus - MaritalStatus enum (Unmarried = 0, Married, Divorced, Widowed) (required)
    //PropertiesCitizens - collection of type PropertyCitizen

    public class ImportPropertyCitizenDto
    {
        //!!!
        [Required]
        public int Id { get; set; }
    }

}
