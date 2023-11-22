using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
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
            //Console.WriteLine(ImportParts(carDealerContext,partsJson));

            //11. Import Cars
            //var carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //Console.WriteLine(ImportCars(carDealerContext, carsJson));

            //12. Import Customers
            //var customersJson = File.ReadAllText("../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(carDealerContext, customersJson));

            //13. Import Sales
            var salesJson = File.ReadAllText("../../../Datasets/sales.json");
            Console.WriteLine(ImportSales(carDealerContext, salesJson));
        }

        //9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            supplierDTO[] supplierDto = JsonConvert.DeserializeObject<supplierDTO[]>(inputJson)!;

            Supplier[] suppliers = mapper.Map<Supplier[]>(supplierDto);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            //creating a mapper 
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            //converting a Json file to a DTO
            PartsDTO[] partsDtos = JsonConvert.DeserializeObject<PartsDTO[]>(inputJson)!;

            //mapping the part from their DTO
            List<Part> parts = new List<Part>();
            foreach (var p in partsDtos)
            {
                if (context.Suppliers.Any(s => s.Id == p.SupplierId))
                {
                    parts.Add(mapper.Map<Part>(p));
                }
            }
            
            //adding the part to Database
            context.AddRange(parts);
            context.SaveChanges();
            
            //output
            return $"Successfully imported {parts.Count()}.";


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

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            List<CarDTO> carsInput = JsonConvert.DeserializeObject<List<CarDTO>>(inputJson);

            List<Car> allCars = new List<Car>();
            foreach (var carJson in carsInput)
            {
                Car car = new Car()
                {
                    Make = carJson.Make,
                    Model = carJson.Model,
                    TravelledDistance = carJson.TraveledDistance
                };
                foreach (var partId in carJson.PartsId.Distinct())
                {
                    car.PartsCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    });
                }

                allCars.Add(car);
            }

            context.Cars.AddRange(allCars);
            //context.SaveChanges();
            return $"Successfully imported {allCars.Count()}.";

        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            CustomerDTO[] customerDtos = JsonConvert.DeserializeObject<CustomerDTO[]>(inputJson)!;

            List<Customer> customers = new List<Customer>();
            foreach (var c in customerDtos)
            {
                customers.Add(mapper.Map<Customer>(c));
            }
            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            //creating a mapper 
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());

            //converting a Json file to DTO  
            SaleDTO[] salesDto = JsonConvert.DeserializeObject<SaleDTO[]>(inputJson)!;

            //creating a new mapper who will help map the sales Dto's
            IMapper mapper = new Mapper(config);

            //mapping the sales from their Dto
            Sale[] sales = mapper.Map<Sale[]>(salesDto);

            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

    }
}