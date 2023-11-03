using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P01_StudentSystem.Data.Common;
using P01_StudentSystem.Data.Models.Enums;

namespace P01_StudentSystem.Data.Models;

public class Homework
{
    [Key]
    public int HomeworkId { get; set; }

    [Required]
    [MaxLength(Validations.HomeworkContentMaxLength)]
    [Column(TypeName = Validations.VarcharType)]
    public string Content { get; set; } = null!;

     public ContentTypeEnums ContentType { get; set; }

     public DateTime SubmissionTime { get; set; }


     [ForeignKey(nameof(Student))]
     public int StudentId { get; set; }
     public virtual Student Student { get; set; } = null!;


     [ForeignKey(nameof(Course))]
     public int CourseId { get; set; }
     public virtual Course Course { get; set; } = null!;
}

