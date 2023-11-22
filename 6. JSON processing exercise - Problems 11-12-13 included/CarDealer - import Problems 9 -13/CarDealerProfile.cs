using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<supplierDTO, Supplier>();
            CreateMap<PartsDTO, Part>();
            //CreateMap<CarDTO, Car>();
            CreateMap<CustomerDTO, Customer>();
            CreateMap<SaleDTO, Sale>();
        }
    }
}
