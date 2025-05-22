using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Product.Request
{
    public record  CreateProductDto
    {
        public Guid ProductId { get; init; } = Guid.NewGuid(); // Genera un nuevo GUID por defecto
        public string? ProductName { get; init; } 
        public string? ProductImage { get; init; } 
        public decimal ProductPrice { get; init; }
        public string? ProductDescription { get; init; } 
        public string? ProductAvilability { get; init; }
        public decimal ProductStock { get; init; }
        public Guid CategoryId { get; init; }
        public Guid ProductUserId { get;  init; }// Genera un nuevo GUID por defecto
    }
}
