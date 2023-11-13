namespace MusicHub.Data.Models;

using System.ComponentModel.DataAnnotations;

public class Performer
{
    public Performer()
    {
        this.PerformerSongs = new HashSet<SongPerformer>();
    }
    [Key]
    public int Id { get; set; }

    //In EF 3.1.X we need to write a [Required] 
    //In EF 6.X everything is required by default and if we put ? we set it to be nullable 
    [MaxLength(DataCommonValidations.PerformerFirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [MaxLength(DataCommonValidations.PerformerLastNameMaxLength)]
    public string LastName { get; set; } = null!;

    public int Age { get; set; }

    public decimal NetWorth { get; set; }

    public virtual ICollection<SongPerformer> PerformerSongs { get; set; }
}

