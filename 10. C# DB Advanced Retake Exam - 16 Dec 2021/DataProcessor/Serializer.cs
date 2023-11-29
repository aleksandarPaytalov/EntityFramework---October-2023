using Artillery.DataProcessor.ExportDto;
using Artillery.Utilities;
using Newtonsoft.Json;

namespace Artillery.DataProcessor;

using Artillery.Data.Models;
using Data;

public class Serializer
{
    public static string ExportShells(ArtilleryContext context, double shellWeight)
    {
        var JsonShells = context.Shells
            .Where(s => s.ShellWeight > shellWeight)
            .ToArray()
            .Select(s => new
            {
                ShellWeight = s.ShellWeight,
                Caliber = s.Caliber,
                Guns = s.Guns
                    .Where(sg => sg.GunType.ToString() == "AntiAircraftGun")
                    .Select(sg => new
                    {
                        GunType = sg.GunType.ToString(),
                        GunWeight = sg.GunWeight,
                        BarrelLength = sg.BarrelLength,
                        Range = sg.Range > 3000 ? "Long-range" : "Regular range"
                    })
                    .OrderByDescending(sg => sg.GunWeight)
                    .ToArray()
            })
            .OrderBy(s => s.ShellWeight)
            .ToArray();

        return JsonConvert.SerializeObject(JsonShells, Formatting.Indented);
    }
    
    public static string ExportGuns(ArtilleryContext context, string manufacturer)
    {
        XmlHelper xmlHelper = new XmlHelper();
        var guns = context.Guns
            .Where(g => g.Manufacturer.ManufacturerName == manufacturer)
            .ToArray()
            .Select(g => new ExportGunDto()
            {
                Manufacturer = g.Manufacturer.ManufacturerName,
                GunType = g.GunType.ToString(),
                GunWeight = g.GunWeight,
                BarrelLength = g.BarrelLength,
                Range = g.Range,
                Countries = g.CountriesGuns
                    .Where(cg => cg.Country.ArmySize > 4500000)
                    .Select(cg => new ExportCountryDto()
                    {
                        Country = cg.Country.CountryName,
                        ArmySize = cg.Country.ArmySize
                    })
                    .OrderBy(cg => cg.ArmySize)
                    .ToArray()
            })
            .OrderBy(g => g.BarrelLength)
            .ToArray();

        return xmlHelper.Serialize(guns, "Guns");
    }
}

