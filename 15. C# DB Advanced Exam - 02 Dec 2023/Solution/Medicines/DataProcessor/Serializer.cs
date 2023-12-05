using System.Globalization;
using AutoMapper.Execution;
using Medicines.Data.Models.Enums;
using Medicines.DataProcessor.ExportDtos;
using Medicines.Utilities;
using Newtonsoft.Json;

namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using System.Diagnostics;
    using System.Xml.Linq;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            var productionDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var patients = context.Patients
                .ToArray()
                .Where(p => p.PatientsMedicines.Any(pm => pm.Medicine.ProductionDate > productionDate))
                .Select(p => new ExportPatientDto()
                {
                    Name = p.FullName,
                    Gender = p.Gender.ToString().ToLower(),
                    AgeGroup = p.AgeGroup.ToString(),
                    Medicines = p.PatientsMedicines
                        .Where(pm => pm.Medicine.ProductionDate > productionDate)
                        .OrderByDescending(pm => pm.Medicine.ExpiryDate)
                        .ThenBy(pm => pm.Medicine.Price)
                        .Select(pm => new ExportMedicineDto()
                        {
                            Name = pm.Medicine.Name,
                            Price = pm.Medicine.Price.ToString("f2"),
                            Category = pm.Medicine.Category.ToString().ToLower(),
                            Producer = pm.Medicine.Producer,
                            BestBefore = pm.Medicine.ExpiryDate.ToString("yyyy-MM-dd"),
                        })
                        .ToArray()
                })
                .OrderByDescending(p => p.Medicines.Length)
                .ThenBy(p => p.Name)
                .ToArray();

            XmlHelper xmlHelper = new XmlHelper();

            return xmlHelper.Serialize<ExportPatientDto[]>(patients, "Patients");
        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var medicines = context.Medicines
                .ToArray()
                .Where(m => m.Category == (Category)medicineCategory && m.Pharmacy.IsNonStop)
                .Select(m => new
                {
                    Name = m.Name,
                    Price = m.Price.ToString("f2"),
                    Pharmacy = new
                    {
                        Name = m.Pharmacy.Name,
                        PhoneNumber = m.Pharmacy.PhoneNumber
                    }
                })
                .OrderBy(m => m.Price)
                .ThenBy(m => m.Name)
                .ToArray();

            return JsonConvert.SerializeObject(medicines, Formatting.Indented);
        }
    }
}
