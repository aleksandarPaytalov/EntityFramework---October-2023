using FastFood.Core.ViewModels.Items;

namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Categories;
    using FastFood.Core.ViewModels.Employees;
    using FastFood.Models;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Categories
            CreateMap<CreateCategoryInputModel, Category>()
                .ForMember(c => c.Name, x => x.MapFrom(y => y.CategoryName));
            CreateMap<Category, CategoryAllViewModel>();

            //Positions
            CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(r => r.PositionId, p => p.MapFrom(x => x.Id));
            CreateMap<RegisterEmployeeInputModel, Employee>();

            CreateMap<Category, CreateItemViewModel>()
                .ForMember(i => i.CategoryId, c=> c.MapFrom(x=>x.Id));

            CreateMap<CreateItemInputModel, Item>();
            CreateMap<Item, ItemsAllViewModels>()
                .ForMember(i => i.Category, it => it.MapFrom(x=>x.Name));
        }
    }
}
 