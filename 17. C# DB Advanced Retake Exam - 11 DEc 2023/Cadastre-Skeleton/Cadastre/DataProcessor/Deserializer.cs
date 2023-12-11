using System.Globalization;
using System.Text;
using Cadastre.Data.Enumerations;
using Cadastre.Data.Models;
using Cadastre.DataProcessor.ImportDtos;
using Cadastre.Utilities;
using Newtonsoft.Json;

namespace Cadastre.DataProcessor
{
    using Cadastre.Data;
    using System.ComponentModel.DataAnnotations;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid Data!";
        private const string SuccessfullyImportedDistrict =
            "Successfully imported district - {0} with {1} properties.";
        private const string SuccessfullyImportedCitizen =
            "Succefully imported citizen - {0} {1} with {2} properties.";

        public static string ImportDistricts(CadastreContext dbContext, string xmlDocument)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var districts = xmlHelper.Deserialize<ImportDistrictDto[]>(xmlDocument, "Districts");

            StringBuilder sb = new StringBuilder();
            HashSet<District> validDistricts = new HashSet<District>();
            foreach (var d in districts)
            {
                if (!IsValid(d))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //check if the Region is valid
                Region region;
                var validRegion = Enum.TryParse(d.Region, true, out region);
                if (!validRegion)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //check if district with same name Exist
                //!!
                var districtExist = validDistricts.FirstOrDefault(x => x.Name == d.Name);
                if (districtExist != null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                District district = new District()
                {
                    Name = d.Name,
                    PostalCode = d.PostalCode,
                    Region = region
                };

                foreach (var p in d.Properties)
                {
                    if (!IsValid(p))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    // check if the date is valid
                    DateTime date;
                    var dateIsValid = DateTime.TryParseExact(p.DateOfAcquisition, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                    if (!dateIsValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    //Check if the property Identifier Exist in current district
                    var identifierExistInThisDistrict =
                        district.Properties.FirstOrDefault(i => i.PropertyIdentifier == p.PropertyIdentifier);
                    if (identifierExistInThisDistrict != null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    //Check if the property Identifier Exist in collection
                    var identifierExistInThisCollection = validDistricts.FirstOrDefault(dis =>
                        dis.Properties.Any(i => i.PropertyIdentifier == p.PropertyIdentifier));
                    if (identifierExistInThisCollection != null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    // check if the property with same address already exist
                    var addressExistInDistrict = district.Properties.FirstOrDefault(a => a.Address == p.Address);
                    if (addressExistInDistrict != null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    // check if the property with same address already exist in collection
                    var addressExistInCollection =
                        validDistricts.FirstOrDefault(dis => dis.Properties.Any(n => n.Address == p.Address));
                    if (addressExistInCollection != null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Property property = new Property()
                    {
                        PropertyIdentifier = p.PropertyIdentifier,
                        Area = p.Area,
                        Details = p.Details,
                        Address = p.Address,
                        DateOfAcquisition = date
                    };

                    district.Properties.Add(property);
                }

                validDistricts.Add(district);
                sb.AppendLine(string.Format(SuccessfullyImportedDistrict, district.Name, district.Properties.Count));
            }

            dbContext.AddRange(validDistricts);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCitizens(CadastreContext dbContext, string jsonDocument)
        {
            var citizens = JsonConvert.DeserializeObject<ImportCitizensDto[]>(jsonDocument);

            StringBuilder sb = new StringBuilder();
            HashSet<Citizen> validCitizens = new HashSet<Citizen>();
            foreach (var citizenDto in citizens)
            {
                if (!IsValid(citizenDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //Check if the Status is valid
                MaritalStatus status;
                var statusIsValid = Enum.TryParse(citizenDto.MaritalStatus, true, out status);
                if (!statusIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //check if the Date is Valid
                DateTime date;
                var dateIsValid = DateTime.TryParseExact(citizenDto.BirthDate, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                if (!dateIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Citizen citizen = new Citizen()
                {
                    FirstName = citizenDto.FirstName,
                    LastName = citizenDto.LastName,
                    BirthDate = date,
                    MaritalStatus = status
                };

                //Its possible to have duplicate Ids so we need to use Ditinct() or Maybe the Id do not exist in database
                foreach (var propertyDto in citizenDto.Properties)
                {
                    var idExist = dbContext.Properties.FirstOrDefault(i => i.Id == propertyDto);
                    // maybe we dont have to trow an Error message and just continue with next Id
                    if (idExist == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    PropertyCitizen property = new PropertyCitizen()
                    {
                        PropertyId = propertyDto,
                        Citizen = citizen
                    };

                    citizen.PropertiesCitizens.Add(property);
                }

                validCitizens.Add(citizen);
                sb.AppendLine(string.Format(SuccessfullyImportedCitizen, citizen.FirstName, citizen.LastName,
                    citizen.PropertiesCitizens.Count));
            }

            dbContext.AddRange(validCitizens);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
