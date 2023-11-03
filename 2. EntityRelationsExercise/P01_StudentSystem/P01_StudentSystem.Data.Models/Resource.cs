using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P01_StudentSystem.Data.Common;
using P01_StudentSystem.Data.Models.Enums;

namespace P01_StudentSystem.Data.Models;

public class Resource
{
    [Key]
    public int ResourceId { get; set; }

    [Required]
    [MaxLength(Validations.CourseMaxNameLength)]
    [Column(TypeName = Validations.NvarcharType)]
    public string Name { get; set; } = null!;

    [MaxLength(Validations.ResourcesUrlMaxLength)] 
    [Column(TypeName = Validations.VarcharType)]
    public string? Url { get; set; }

    public ResourceTypeEnums ResourceType { get; set; }

    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;
}

