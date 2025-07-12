using AutoMapper;
using ProductsMs.Domain.Entities.Category;
using ProductsMS.Common.Dtos.Category.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductsMS.Common.AutoMapper
{
    [ExcludeFromCodeCoverage]
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryEntity, GetCategoryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName.Value)) // Conversión específica
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId.Value))
                .ReverseMap();
        }
    }
}
