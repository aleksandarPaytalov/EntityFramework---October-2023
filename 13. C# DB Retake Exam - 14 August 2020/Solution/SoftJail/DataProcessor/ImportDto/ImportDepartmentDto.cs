using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportDepartmentDto
    {
        [JsonProperty("Name")]
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; } = null!;

        [JsonProperty("Cells")]
        public ImportCellDto[] Cells { get; set; } = null!;

        //•	Id – integer, Primary Key
        //•	Name – text with min length 3 and max length 25 (required)
        //•	Cells - collection of type Cell
    }

    public class ImportCellDto
    {
        [JsonProperty("CellNumber")]
        [Required]
        [Range(1,1000)]
        public int CellNumber { get; set; }

        [JsonProperty("HasWindow")]
        [Required]
        public bool HasWindow { get; set; }
    }
    //•	Id – integer, Primary Key
    //•	CellNumber – integer in the range [1, 1000] (required)
    //•	HasWindow – bool (required)
    //•	DepartmentId – integer, foreign key (required)
    //•	Department – the cell's department (required)
    //•	Prisoners – collection of type Prisoner
}






