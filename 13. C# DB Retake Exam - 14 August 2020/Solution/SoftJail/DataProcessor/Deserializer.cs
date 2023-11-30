using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using SoftJail.Data.Models;
using SoftJail.DataProcessor.ImportDto;
using SoftJail.Utilities;

namespace SoftJail.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Xml.Linq;
    using Data;
    using SoftJail.Data.Models.Enums;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        private const string SuccessfullyImportedDepartment = "Imported {0} with {1} cells";

        private const string SuccessfullyImportedPrisoner = "Imported {0} {1} years old";

        private const string SuccessfullyImportedOfficer = "Imported {0} ({1} prisoners)";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departmentsCellsJson = JsonConvert.DeserializeObject<ImportDepartmentDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            HashSet<Department> validDepartments = new HashSet<Department>();
            foreach (var d in departmentsCellsJson!)
            {
                if (!IsValid(d))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!d.Cells.Any())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Department dept = new Department()
                {
                    Name = d.Name
                };

                bool isValidDept = true;
                foreach (var cell in d.Cells)
                {
                    if (!IsValid(cell))
                    {
                        isValidDept = false;
                        break;
                    }

                    Cell validCell = new Cell()
                    {
                        CellNumber = cell.CellNumber,
                        HasWindow = cell.HasWindow
                    };
                    dept.Cells.Add(validCell);
                }

                if (!isValidDept)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validDepartments.Add(dept);
                sb.AppendLine(string.Format(SuccessfullyImportedDepartment, dept.Name, dept.Cells.Count));
            }

            context.AddRange(validDepartments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonersMailsJson = JsonConvert.DeserializeObject<ImportPrisonerDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            HashSet<Prisoner> validPrisoners = new HashSet<Prisoner>();
            foreach (var p in prisonersMailsJson!)
            {
                if (!IsValid(p))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime IncarcerationDate;
                var incDateValidation = DateTime.TryParseExact(p.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out IncarcerationDate);
                if (!incDateValidation)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? releaseDate = null;
                if (!string.IsNullOrWhiteSpace(p.ReleaseDate))
                {
                    DateTime releaseDateValue;
                    bool releaseDateValidation = DateTime.TryParseExact(p.ReleaseDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDateValue);

                    if (!releaseDateValidation)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    releaseDate = releaseDateValue;
                }

                Prisoner prisoner = new Prisoner()
                {
                    FullName = p.FullName,
                    Nickname = p.Nickname,
                    Age = p.Age,
                    IncarcerationDate = IncarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = p.Bail,
                    CellId = p.CellId
                };

                bool prisonerIsValid = true;
                foreach (var pMail in p.Mails)
                {
                    if (!IsValid(pMail))
                    {
                        prisonerIsValid = false;
                        break;
                    }

                    Mail validMail = new Mail()
                    {
                        Description = pMail.Description,
                        Sender = pMail.Sender,
                        Address = pMail.Address
                    };
                    prisoner.Mails.Add(validMail);
                }

                if (!prisonerIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validPrisoners.Add(prisoner);
                sb.AppendLine(string.Format(SuccessfullyImportedPrisoner, prisoner.FullName, prisoner.Age));
            }

            context.AddRange(validPrisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            var officersPrisoners = xmlHelper.Deserialize<ImportOfficerDto[]>(xmlString, "Officers");

            StringBuilder sb = new StringBuilder();
            HashSet<Officer> validOfficers = new HashSet<Officer>();
            foreach (var o in officersPrisoners)
            {
                if (!IsValid(o))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                object position;
                bool isPositionValid = Enum.TryParse(typeof(Position), o.Position, out position!);
                if (!isPositionValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                object weapon;
                bool isWeaponValid = Enum.TryParse(typeof(Weapon), o.Weapon, out weapon!);
                if (!isWeaponValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Officer validOfficer = new Officer()
                {
                    FullName = o.Name,
                    Salary = o.Money,
                    Position = (Position)position,
                    Weapon = (Weapon)weapon,
                    DepartmentId = o.DepartmentId
                };

                foreach (var pId in o.Prisoners)
                {
                    OfficerPrisoner prisonerId = new OfficerPrisoner()
                    {
                        Officer = validOfficer,
                        PrisonerId = pId.PrisonerId
                    };

                    validOfficer.OfficerPrisoners.Add(prisonerId);
                }

                validOfficers.Add(validOfficer);
                sb.AppendLine(string.Format(SuccessfullyImportedOfficer, validOfficer.FullName,
                    validOfficer.OfficerPrisoners.Count));
            }

            context.AddRange(validOfficers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}