﻿namespace Footballers.Data.Models;

using Enums;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


public class Footballer
{
    public Footballer()
    {
        this.TeamsFootballers = new HashSet<TeamFootballer>();
    }
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Required]
    public DateTime ContractStartDate { get; set; }

    [Required]
    public DateTime ContractEndDate { get; set; }

    [Required]
    public PositionType PositionType { get; set; }

    [Required]
    public BestSkillType BestSkillType { get; set; }

    [Required]
    [ForeignKey(nameof(Coach))]
    public int CoachId { get; set; }

    public Coach Coach { get; set; } = null!;

    public virtual ICollection<TeamFootballer> TeamsFootballers { get; set; } = null!;

}
