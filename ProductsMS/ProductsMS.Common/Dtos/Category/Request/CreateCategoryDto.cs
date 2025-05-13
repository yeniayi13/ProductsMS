using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Category.Request
{
    public record CreateCategoryDto
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string? CategoryName { get; init; }
    }
}
