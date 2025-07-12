using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Product.Response
{
    public class GetProductDto
    {
       // [JsonIgnore]
        public Guid ProductId { get; init; }
        public string? ProductName { get; init; }
        public string? ProductImage { get; init; }
        public decimal? ProductPrice { get; init; }
        public string? ProductDescription { get; init; }
        public string? ProductAvilability { get; init; }
        public decimal? ProductStock { get; init; }
        public Guid CategoryId { get; init; }
       //1 [JsonIgnore]
        public Guid ProductUserId { get; init; }
        
       // public bool IsDeleted { get; set; }
    }
}
