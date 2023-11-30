
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Project")]
    public class ImportProjectDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        [XmlElement("OpenDate")]
        [Required]
        public string OpenDate { get; set; } = null!;

        [XmlElement("DueDate")]
        public string? DueDate { get; set; }

        [XmlArray("Tasks")]
        public ImportTaskDto[] Tasks { get; set; } = null!;

    }
    //Project
    //    •	Id – integer, Primary Key
    //    •	Name – text with length [2, 40] (required)
    //    •	OpenDate – date and time (required)
    //    •	DueDate – date and time (can be null)
    //    •	Tasks – collection of type Task

    [XmlType("Task")]
    public class ImportTaskDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        [XmlElement("OpenDate")]
        [Required]
        public string OpenDate { get; set; } = null!;

        [XmlElement("DueDate")]
        [Required]
        public string DueDate { get; set; } = null!;

        [XmlElement("ExecutionType")]
        [Required]
        public int ExecutionType { get; set; }

        [XmlElement("LabelType")]
        [Required]
        public int LabelType { get; set; }
    }
}

//Task
//    •	Id – integer, Primary Key
//    •	Name – text with length [2, 40] (required)
//    •	OpenDate – date and time (required)
//    •	DueDate – date and time (required)
//    •	ExecutionType – enumeration of type ExecutionType, with possible values (ProductBacklog, SprintBacklog, InProgress, Finished) (required)
//    •	LabelType – enumeration of type LabelType, with possible values (Priority, CSharpAdvanced, JavaAdvanced, EntityFramework, Hibernate) (required)
//    •	ProjectId – integer, foreign key (required)
//    •	Project – Project 
//    •	EmployeesTasks – collection of type EmployeeTask
