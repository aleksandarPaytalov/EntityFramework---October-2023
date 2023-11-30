using Newtonsoft.Json;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor;


using Data;
using Data.Models;
using ImportDto;
using Utilities;
using System.Text;
using System.Globalization;

using System.ComponentModel.DataAnnotations;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;
using System.Xml.Linq;
using System.Numerics;


public class Deserializer
{
    private const string ErrorMessage = "Invalid data!";

    private const string SuccessfullyImportedProject
        = "Successfully imported project - {0} with {1} tasks.";

    private const string SuccessfullyImportedEmployee
        = "Successfully imported employee - {0} with {1} tasks.";

    public static string ImportProjects(TeisterMaskContext context, string xmlString)
    {
        XmlHelper xmlHelper = new XmlHelper();
        var projectsXml = xmlHelper.Deserialize<ImportProjectDto[]>(xmlString, "Projects");

        

        StringBuilder sb = new StringBuilder();
        HashSet<Project> validProjects = new HashSet<Project>();
        foreach (var p in projectsXml)
        {
            
            DateTime openDate;
            var validOpenDate = DateTime.TryParseExact(p.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,out openDate);
            if (!validOpenDate)
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }
            
            DateTime? dueDate = null;
            if (!string.IsNullOrWhiteSpace(p.DueDate))
            {
                DateTime dueDatecheck;
                bool validDueDate = DateTime.TryParseExact(p.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDatecheck);
                if (!validDueDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                dueDate = dueDatecheck;
            }

            if (!IsValid(p))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }
            
            if (openDate > dueDate)
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Project validProject = new Project()
            {
                Name = p.Name,
                OpenDate = openDate,
                DueDate = dueDate,
            };

            foreach (var t in p.Tasks)
            {
                if (!IsValid(t))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime taskOpenDate;
                bool taskOpenDateIsValid = DateTime.TryParseExact(t.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskOpenDate);
                if (!taskOpenDateIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime taskDueDate;
                bool taskDueDateIsValid = DateTime.TryParseExact(t.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskDueDate);
                if (!taskDueDateIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (taskOpenDate < openDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (dueDate.HasValue && taskDueDate > dueDate.Value)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Task validTask = new Task()
                {
                    Name = t.Name,
                    OpenDate = taskOpenDate,
                    DueDate = taskDueDate,
                    ExecutionType = (ExecutionType)t.ExecutionType,
                    LabelType = (LabelType)t.LabelType
                };

                validProject.Tasks.Add(validTask);
            }

            validProjects.Add(validProject);
            sb.AppendLine(string.Format(SuccessfullyImportedProject, validProject.Name, validProject.Tasks.Count));
        }

        context.AddRange(validProjects);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }

    public static string ImportEmployees(TeisterMaskContext context, string jsonString)
    {
        var employeesJson = JsonConvert.DeserializeObject<ImportEmployeesDto[]>(jsonString);
        StringBuilder sb = new StringBuilder();

        HashSet<Employee> validEmployees = new HashSet<Employee>();
        foreach (var emp in employeesJson!)
        {
            if (!IsValid(emp))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Employee validEmp = new Employee()
            {
                Username = emp.Username,
                Email = emp.Email,
                Phone = emp.Phone
            };

            foreach (var t in emp.Tasks.Distinct())
            {
                if (!IsValid(t))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                //
                if (!context.Tasks.Distinct().Any(ta => ta.Id == t))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                EmployeeTask validT = new EmployeeTask()
                {
                    TaskId = t
                };
                validEmp.EmployeesTasks.Add(validT);
            }

            validEmployees.Add(validEmp);
            sb.AppendLine(string.Format(SuccessfullyImportedEmployee, validEmp.Username,
                validEmp.EmployeesTasks.Distinct().Count()));
        }

        context.AddRange(validEmployees);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }

    private static bool IsValid(object dto)
    {
        var validationContext = new ValidationContext(dto);
        var validationResult = new List<ValidationResult>();

        return Validator.TryValidateObject(dto, validationContext, validationResult, true);
    }
}
