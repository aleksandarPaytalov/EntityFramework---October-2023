using System.ComponentModel.DataAnnotations.Schema;
using P01_StudentSystem.Data.Common;

namespace P01_StudentSystem.Data.Models;

using System.ComponentModel.DataAnnotations;

public class Course
{
    public Course()
    {
        this.Resources = new HashSet<Resource>();
        this.Homeworks = new HashSet<Homework>();
        this.StudentsCourses = new HashSet<StudentCourse>();
    }
    [Key]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(Validations.CourseMaxNameLength)]
    [Column(TypeName = Validations.NvarcharType)]
    public string Name { get; set; } = null!;

    [MaxLength(Validations.CourseMaxDescriptionLength)]
    [Column(TypeName = Validations.NvarcharType)]
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Resource> Resources { get; set; } = null!;

    public virtual ICollection<Homework> Homeworks { get; set; } = null!;

    public virtual ICollection<StudentCourse> StudentsCourses { get; set; }
}

