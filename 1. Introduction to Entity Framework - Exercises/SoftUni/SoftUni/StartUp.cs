using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            var result = GetEmployeesByFirstNameStartingWithSa(context);
            Console.WriteLine(result);
        }

        //Problem 03. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 04. Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .OrderBy(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(e => e.Salary > 50000)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 05. Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Name} - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //06. Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee? employee = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            employee.Address = newAddress;

            context.SaveChanges();

            var employees = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address!.AddressText)
                .ToArray();

            return string.Join(Environment.NewLine, employees);
        }

        //07. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager!.FirstName,
                    ManagerLastName = e.Manager!.LastName,
                    Projects = e.EmployeesProjects.Where(ep => ep.Project.StartDate.Year >= 2001 &&
                                                    ep.Project.StartDate.Year <= 2003)
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                            EndDate = ep.Project.EndDate.HasValue
                                        ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                                        : "not finished"

                        })
                        .ToArray()
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var ep in e.Projects)
                {
                    sb.AppendLine($"--{ep.ProjectName} - {ep.StartDate} - {ep.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //08. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var addresses = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => new
                {
                    AddressText = a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count

                })
                .ToArray();

            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeesCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        //09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                        .Select(ep => ep.Project.Name)
                        .OrderBy(ep => ep)
                        .ToArray()
                })
                .ToArray();


            foreach (var e in employee)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                foreach (var p in e.Projects)
                {
                    sb.AppendLine(p);
                }
            }

            return sb.ToString().TrimEnd();
        }

        //10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var depts = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(e => e.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    d.Name,
                    FirstNameManager = d.Manager.FirstName,
                    LastNameManager = d.Manager.LastName,
                    EmployeesInTheDepartment = d.Employees
                        .Select(e => new
                        {
                            e.FirstName,
                            e.LastName,
                            e.JobTitle
                        })
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .ToList()
                })
                .ToList();

            foreach (var d in depts)
            {
                sb.AppendLine($"{d.Name} - {d.FirstNameManager} {d.LastNameManager}");
                foreach (var e in d.EmployeesInTheDepartment)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var sortedProjectLastlyStarted = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                })
                .ToArray();

            foreach (var p in sortedProjectLastlyStarted)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate);
            }

            return sb.ToString().TrimEnd();
        }

        //12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesToIncreaseTheirSalaries = context.Employees
                .Where(e => e.Department.Name == "Engineering"
                            || e.Department.Name == "Tool Design"
                            || e.Department.Name == "Marketing"
                            || e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var e in employeesToIncreaseTheirSalaries)
            {
                e.Salary += e.Salary * 0.12m;
            }

            context.SaveChanges();

            foreach (var e in employeesToIncreaseTheirSalaries)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        //13. Find Employees by First Name Starting With Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            //var emp = context.Employees.ToList();
            var employeeNamesThatContainsSa = context.Employees
                //.ToList() -- трябва да е ToList за да може да се сравни със StringComparison.OrdinalIgnoreCase в "Sa" , проверката на следващия ред
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var e in employeeNamesThatContainsSa)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var employeeToDelete = context.EmployeesProjects
                .Where(ep => ep.ProjectId == 2);
            context.EmployeesProjects.RemoveRange(employeeToDelete);

            var projectToDelete = context.Projects
                .Find(2);
            context.Projects.Remove(projectToDelete!);

            context.SaveChanges();

            var projects = context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToList();
            return string.Join(Environment.NewLine, projects);
        }

        //15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            var townToRemove = context.Towns.FirstOrDefault(t => t.Name == "Seattle")!;

            var addressesToRemove = context.Addresses
                .Where(a => a.Town.Name == "Seattle")
                .ToArray();
            
            var employeesAddressSetToNull = context.Employees
                .Where(e => e.Address.Town.Name == "Seattle")
                .ToArray();

            foreach (var e in employeesAddressSetToNull)
            {
                e.AddressId = null;
            }
            context.SaveChanges();

            foreach (var a in addressesToRemove)
            {
                context.Addresses.Remove(a);
            }
            context.SaveChanges();
            
            context.Towns.Remove(townToRemove);
            context.SaveChanges();

            return $"{addressesToRemove.Length} addresses in Seattle were deleted";
        }

    }
}