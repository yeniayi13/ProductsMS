using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Category.Request
{
    public record UpdateCategoryDto
    {
        public string? CategoryName { get; init; }
    }
}
