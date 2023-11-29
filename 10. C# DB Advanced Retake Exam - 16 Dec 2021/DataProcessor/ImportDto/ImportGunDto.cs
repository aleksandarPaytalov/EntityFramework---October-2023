using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Artillery.Data.Models.Enums;
using Newtonsoft.Json;

namespace Artillery.DataProcessor.ImportDto

{
    public class ImportGunDto
    {
        [Required]
        [JsonProperty("ManufacturerId")]
        public int ManufacturerId { get; set; }

        [Required]
        [JsonProperty("GunWeight")]
        [Range(100, 1350000)]
        public int GunWeight { get; set; }

        [Required]
        [JsonProperty("BarrelLength")]
        [Range(2.00,35.00)]
        public double BarrelLength { get; set; }

        [JsonProperty("NumberBuild")]
        public int? NumberBuild { get; set; }

        [Required]
        [JsonProperty("Range")]
        [Range(1,100000)]
        public int Range { get; set; }

        [Required] 
        [JsonProperty("GunType")] 
        public string GunType { get; set; } = null!;

        [Required]
        [JsonProperty("ShellId")]
        public int ShellId { get; set; }

        [Required] 
        [JsonProperty("Countries")] 
        public ImportCountryGunDto[] Countries { get; set; } = null!;
    }

    public class ImportCountryGunDto
    {
        [Required]
        [JsonProperty("Id")]
        public int Id { get; set; }
    }
}

//GunWeight– integer in range [100…1_350_000] (required)
//BarrelLength – double in range [2.00….35.00] (required)
//NumberBuild – integer
//Range – integer in range [1….100_000] (required)
//GunType – enumeration of GunType, with possible values (Howitzer, Mortar, FieldGun, AntiAircraftGun, MountainGun, AntiTankGun) (required)
//ShellId – integer, foreign key (required)
//CountriesGuns – a collection of CountryGun

