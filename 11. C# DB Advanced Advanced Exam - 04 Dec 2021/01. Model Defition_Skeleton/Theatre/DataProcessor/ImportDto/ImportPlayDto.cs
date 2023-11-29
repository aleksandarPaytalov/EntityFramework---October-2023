namespace Theatre.DataProcessor.ImportDto;

using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;


[XmlType("Play")]
public class ImportPlayDto
{
    [XmlElement("Title")]
    [Required]
    [MinLength(4)]
    [MaxLength(50)]
    public string Title { get; set; } = null!;

    [XmlElement("Duration")] 
    [Required]
    public string Duration { get; set; } = null!;

    [XmlElement("Raiting")]
    [Required]
    [Range(0.00,10.00)]
    public float Rating { get; set; }

    [XmlElement("Genre")] 
    [Required] 
    public string Genre { get; set; } = null!;

    [XmlElement("Description")]
    [Required]
    [MaxLength(700)]
    public string Description { get; set; } = null!;

    [XmlElement("Screenwriter")]
    [Required]
    [MinLength(4)]
    [MaxLength(30)]
    public string Screenwriter { get; set; } = null!;

}


//Title – text with length [4, 50] (required)
//Duration – TimeSpan in format {hours:minutes: seconds}, with a minimum length of 1 hour. (required)
//Rating – float in the range [0.00….10.00] (required)
//Genre – enumeration of type Genre, with possible values (Drama, Comedy, Romance, Musical) (required)
//Description – text with length up to 700 characters (required)
//Screenwriter – text with length [4, 30] (required)

