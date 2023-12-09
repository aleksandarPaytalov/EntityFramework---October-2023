

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace VaporStore.DataProcessor.ImportDto
{
    public class ImportGameDeveloperGenreTagDto
    {
        [JsonProperty("Name")]
        [Required]
        public string Name { get; set; } = null!;

        [JsonProperty("Price")]
        [Required]
        [Range(0,9999999999999999999)]
        public decimal Price { get; set; }

        [JsonProperty("ReleaseDate")]
        [Required]
        public string ReleaseDate { get; set; } = null!;

        [JsonProperty("Developer")]
        [Required]
        public string Developer { get; set; } = null!;

        [JsonProperty("Genre")]
        [Required]
        public string Genre { get; set; } = null!;

        [JsonProperty("Tags")]
        [Required]
        public string[] Tags { get; set; } = null!;
        //•	Id – integer, Primary Key
        //•	Name – text(required)
        //•	Price – decimal (non-negative, minimum value: 0) (required)
        //•	ReleaseDate – Date(required)
        //•	DeveloperId – integer, foreign key(required)
        //•	Developer – the game's developer (required)
        //•	GenreId – integer, foreign key(required)
        //•	Genre – the game's genre (required)
        //•	Purchases - collection of type Purchase
        //•	GameTags - collection of type GameTag.Each game must have at least one tag.

    }

}
