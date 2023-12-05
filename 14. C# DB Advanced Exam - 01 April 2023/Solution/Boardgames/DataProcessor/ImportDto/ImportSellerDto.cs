using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellerDto
    {
        [JsonProperty("Name")]
        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [JsonProperty("Address")]
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Address { get; set; } = null!;

        [JsonProperty("Country")] 
        [Required] 
        public string Country { get; set; } = null!;

        [JsonProperty("Website")]
        [Required]
        [RegularExpression("^www\\.[a-zA-Z0-9\\-]+\\.com$")]
        public string Website { get; set; } = null!;

        [JsonProperty("Boardgames")]
        public int[] Boardgames { get; set; }
    }

    //Id – integer, Primary Key
    //Name – text with length[5…20] (required)
    //Address – text with length[2…30] (required)
    //Country – text(required)
    //Website – a string (required). First four characters are "www.", followed by upper and lower letters, digits or '-' and the last three characters are ".com".
    //BoardgamesSellers – collection of type BoardgameSeller

}
