using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Category.Response
{
    public class GetCategoryDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }

        public string? Createdby { get; set; }

        public GetCategoryDto(Guid id, string? name, string? createdby)
        {
            Id = id;
            Name = name;
            Createdby = createdby;
        }
    }
}
