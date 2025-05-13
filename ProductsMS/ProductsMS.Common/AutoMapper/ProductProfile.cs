using AutoMapper;
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMs.Domain.Entities.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Dtos.Product.Response;

namespace ProductsMS.Common.AutoMapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductEntity, GetProductDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.Value))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName.Value))
            .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.ProductImage.Url))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.ProductPrice.Value))
            .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.ProductDescription.Value))
            .ForMember(dest => dest.ProductStock, opt => opt.MapFrom(src => src.ProductStock.Value))
            .ForMember(dest => dest.ProductUserId, opt => opt.MapFrom(src => src.ProductUserId.Value))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId.Value))
            .ForMember(dest => dest.ProductAvilability, opt => opt.MapFrom(src => src.ProductAvilability.ToString())) // Convertir Enum a string
            .ReverseMap();
        }
    }
}
