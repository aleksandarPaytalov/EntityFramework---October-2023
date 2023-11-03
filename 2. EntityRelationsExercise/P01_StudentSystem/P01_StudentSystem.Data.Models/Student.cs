using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P01_StudentSystem.Data.Common;

namespace P01_StudentSystem.Data.Models;

public class Student
{
    public Student()
    {
        this.Homeworks = new HashSet<Homework>();
        this.StudentsCourses = new HashSet<StudentCourse>();
    }
    [Key]
    public int StudentId { get; set; }

    [Required]
    //[MaxLength(Validations.StudentNameMaxLength)] - can be skipped here and in the other cases if in "(100)" is written in the next row
    [Column(TypeName = "nvarchar(100)")] 
    public string Name { get; set; } = null!;

    //must be with ? for judge because the property is "not required" 
    [MaxLength(Validations.StudentNumberMaxLength)]
    [Column(TypeName = Validations.VarcharType)]
    public string? PhoneNumber { get; set; }

    public DateTime RegisteredOn { get; set; }

    public DateTime? Birthday { get; set; }

    public virtual ICollection<Homework> Homeworks { get; set; } = null!;
    public virtual ICollection<StudentCourse> StudentsCourses { get; set; }
}
