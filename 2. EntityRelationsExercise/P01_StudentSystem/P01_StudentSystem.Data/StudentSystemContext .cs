using P01_StudentSystem.Data.Common;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data;

using Microsoft.EntityFrameworkCore;

public class StudentSystemContext : DbContext
{
    public StudentSystemContext()
    {
        
    }

    public StudentSystemContext(DbContextOptions options)
    : base(options)
    {
        
    }

    // Може и да не са виртуални? Кое е по-правилно?
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Homework> Homeworks { get; set; }
    public virtual DbSet<Resource> Resources { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<StudentCourse> StudentsCourses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(DbConfiguration.SqlConnectionString);
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(sc => new { sc.StudentId, sc.CourseId});
        });
    }
}

