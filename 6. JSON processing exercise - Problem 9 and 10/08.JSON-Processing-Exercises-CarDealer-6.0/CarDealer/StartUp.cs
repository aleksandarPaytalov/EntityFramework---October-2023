using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext carDealerContext = new CarDealerContext();

            //9. Import Suppliers
            //var supplierJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(carDealerContext,supplierJson));

            //10. Import Parts
            //var partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //Console.WriteLine(ImportParts(carDealerContext,partsJson));;
            //Console.WriteLine(ImportParts(carDealerContext,partsJson));
        }

        //9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            supplierDTO[] supplierDTOs = JsonConvert.DeserializeObject<supplierDTO[]>(inputJson);

            Supplier[] suppliers = mapper.Map<Supplier[]>(supplierDTOs);

            context.Suppliers.AddRange(suppliers);
            //context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            PartsDTO[] partsDtos = JsonConvert.DeserializeObject<PartsDTO[]>(inputJson);
            Part[] parts = mapper.Map<Part[]>(partsDtos);

            int[] suppliersId = context.Suppliers
                .Select(s => s.Id)
                .ToArray();
            Part[] partsWithValidSuppliersId = parts.Where(p => suppliersId.Contains(p.SupplierId)).ToArray();
            context.AddRange(partsWithValidSuppliersId);
            context.SaveChanges();

            return $"Successfully imported {partsWithValidSuppliersId.Count()}";


            //10. Import Parts - 2nd solution with anonymous types

            //public static string ImportParts(CarDealerContext context, string inputJson)
            //{
            //    var partsJson = JsonConvert.DeserializeObject<Part[]>(inputJson);
            //
            //    var partsWithValidSuppliersId = partsJson!.Where(s => s.SupplierId != null).ToArray();
            //
            //    context.AddRange(partsWithValidSuppliersId);
            //    context.SaveChanges();
            //
            //    return $"Successfully imported {partsWithValidSuppliersId.Count()}";
            //}
        }

       

    }


}