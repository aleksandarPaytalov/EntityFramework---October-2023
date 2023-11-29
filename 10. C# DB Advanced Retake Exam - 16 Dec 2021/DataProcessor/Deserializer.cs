namespace Artillery.DataProcessor;

using System.Text;
using Data.Models;
using ImportDto;
using Utilities;
using Newtonsoft.Json;

using Data.Models.Enums;
using Data;
using System.ComponentModel.DataAnnotations;

public class Deserializer
{
    private const string ErrorMessage =
        "Invalid data.";
    private const string SuccessfulImportCountry =
        "Successfully import {0} with {1} army personnel.";
    private const string SuccessfulImportManufacturer =
        "Successfully import manufacturer {0} founded in {1}.";
    private const string SuccessfulImportShell =
        "Successfully import shell caliber #{0} weight {1} kg.";
    private const string SuccessfulImportGun =
        "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

    public static string ImportCountries(ArtilleryContext context, string xmlString)
    {
        XmlHelper xmlHelper = new XmlHelper();
        var countries = xmlHelper.Deserialize<ImportCountryDto[]>(xmlString, "Countries");

        StringBuilder sb = new StringBuilder();
        HashSet<Country> validCountries = new HashSet<Country>();
        foreach (var c in countries)
        {
            if (!IsValid(c))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Country validCountry = new Country()
            {
                CountryName = c.CountryName,
                ArmySize = c.ArmySize
            };

            validCountries.Add(validCountry);
            sb.AppendLine(string.Format(SuccessfulImportCountry, validCountry.CountryName, validCountry.ArmySize));
        }

        context.AddRange(validCountries);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }

    public static string ImportManufacturers(ArtilleryContext context, string xmlString)
    {
        XmlHelper xmlHelper = new XmlHelper();
        var manufacturers = xmlHelper.Deserialize<ImportManufacturerDto[]>(xmlString, "Manufacturers");

        StringBuilder sb = new StringBuilder();
        HashSet<Manufacturer> validManufacturers = new HashSet<Manufacturer>();
        foreach (var m in manufacturers)
        {
            if (!IsValid(m) || validManufacturers.Any(vm => vm.ManufacturerName == m.ManufacturerName))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Manufacturer manufacturer = new Manufacturer()
            {
                ManufacturerName = m.ManufacturerName,
                Founded = m.Founded
            };

            validManufacturers.Add(manufacturer);

            string[] manufacturerInfo = m.Founded.Split(", ").ToArray();
            string manufacturerTownAndCountry = manufacturerInfo[manufacturerInfo.Length - 2] + ", " + manufacturerInfo[manufacturerInfo.Length - 1];
            sb.AppendLine(string.Format(SuccessfulImportManufacturer, m.ManufacturerName, manufacturerTownAndCountry));
        }

        context.AddRange(validManufacturers);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }

    public static string ImportShells(ArtilleryContext context, string xmlString)
    {
        XmlHelper xmlHelper = new XmlHelper();
        var shellsXml = xmlHelper.Deserialize<ImportShellDto[]>(xmlString, "Shells");

        StringBuilder sb = new StringBuilder();
        HashSet<Shell> validShells = new HashSet<Shell>();
        foreach (var shell in shellsXml)
        {
            if (!IsValid(shell))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Shell validShell = new Shell()
            {
                ShellWeight = shell.ShellWeight,
                Caliber = shell.Caliber
            };

            validShells.Add(validShell);
            sb.AppendLine(string.Format(SuccessfulImportShell, validShell.Caliber, validShell.ShellWeight));
        }

        context.AddRange(validShells);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }

    public static string ImportGuns(ArtilleryContext context, string jsonString)
    {
        var gunsJson = JsonConvert.DeserializeObject<ImportGunDto[]>(jsonString);

        StringBuilder sb = new StringBuilder();
        HashSet<Gun> validGuns = new HashSet<Gun>();
        foreach (var gun in gunsJson!)
        {
            if (!IsValid(gun) || !Enum.TryParse(gun.GunType, out GunType gunType))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Gun validGun = new Gun()
            {
                ManufacturerId = gun.ManufacturerId,
                GunWeight = gun.GunWeight,
                BarrelLength = gun.BarrelLength,
                NumberBuild = gun.NumberBuild,
                Range = gun.Range,
                GunType = gunType,
                ShellId = gun.ShellId
            };

            validGuns.Add(validGun);

            foreach (var c in gun.Countries)
            {
                CountryGun gunCountryIds = new CountryGun()
                {
                    CountryId = c.Id,
                    GunId = validGun.Id
                };

                validGun.CountriesGuns.Add(gunCountryIds);
            }

            validGuns.Add(validGun);
            sb.AppendLine(string.Format(SuccessfulImportGun, validGun.GunType,validGun.GunWeight, validGun.BarrelLength));
        }

        context.AddRange(validGuns);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }
    private static bool IsValid(object obj)
    {
        var validator = new ValidationContext(obj);
        var validationRes = new List<ValidationResult>();

        var result = Validator.TryValidateObject(obj, validator, validationRes, true);
        return result;
    }
}
