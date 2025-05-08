using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Product.Request
{
    public record  UpdateProductDto
    {
        public string? Name { get; init; }
        public string? Image { get; init; }
        public decimal Price { get; init; }
        public string? Description { get; init; }
        public int Avilability { get; init; }
        public decimal Stock { get; init; }
        public Guid CategoryId { get; init; }
    }
}
