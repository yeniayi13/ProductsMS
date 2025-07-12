using MediatR;
using ProductsMs.Domain.Entities.Category;
using ProductsMS.Common.Dtos.Category.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Application.Category.Queries
{
    public class GetCategoryNameQuery:IRequest<GetCategoryDto>
    {
        public string Name { get; set; }


        public GetCategoryNameQuery(string name)
        {
            Name = name;
        }
    }
}
