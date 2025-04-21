using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Product.Response
{
    public class GetProductDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Image { get; init; }
        public decimal? Price { get; init; }
        public string? Description { get; init; }
        public string? Avilability { get; init; }
        public decimal? Stock { get; init; }
        public Guid CategoryId { get; init; }

        public string? CreatedBy { get; set; }
        public GetProductDto(Guid id, string? name, string? image, decimal? price, string? description, string? avilability, decimal? stock, Guid categoryId, string? createdby)
        {
            Id = id;
            Name = name;
            Image = image;
            Price = price;
            Description = description;
            Avilability = avilability;
            Stock = stock;
            CategoryId = categoryId;
            CreatedBy = createdby;
        }
    }
}
