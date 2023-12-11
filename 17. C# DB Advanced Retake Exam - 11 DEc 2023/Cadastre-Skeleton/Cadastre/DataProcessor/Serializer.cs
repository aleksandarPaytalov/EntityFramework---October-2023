using System.Globalization;
using Cadastre.Data;
using Cadastre.Data.Enumerations;
using System.Net;
using Cadastre.DataProcessor.ExportDtos;
using Cadastre.Utilities;
using Newtonsoft.Json;
using DateTime = System.DateTime;

namespace Cadastre.DataProcessor
{
    public class Serializer
    {
        public static string ExportPropertiesWithOwners(CadastreContext dbContext)
        {
            DateTime dateToCompare = new DateTime(2000, 1, 1);

            var properties = dbContext.Properties
                .ToArray()
                .Where(p => p.DateOfAcquisition >= dateToCompare)
                .OrderByDescending(p => p.DateOfAcquisition)
                .ThenBy(p => p.PropertyIdentifier)
                .Select(p => new
                {
                    PropertyIdentifier = p.PropertyIdentifier,
                    Area = p.Area,
                    Address = p.Address,
                    DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Owners = p.PropertiesCitizens
                        .Select(o => new
                        {
                            LastName = o.Citizen.LastName,
                            MaritalStatus = o.Citizen.MaritalStatus.ToString()
                        })
                        .OrderBy(o => o.LastName)
                        .ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(properties, Formatting.Indented);
        }

        public static string ExportFilteredPropertiesWithDistrict(CadastreContext dbContext)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var properties = dbContext.Properties
                .ToArray()
                .Where(p => p.Area >= 100)
                .OrderByDescending(p => p.Area)
                .ThenBy(p => p.DateOfAcquisition)
                .Select(p => new ExportPropertyDto()
                {
                    PostCode = p.District .PostalCode,
                    PropertyIdentifier = p.PropertyIdentifier,
                    Area = p.Area,
                    DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                })
                .ToArray();

            return xmlHelper.Serialize(properties, "Properties");
        }
    }
}
