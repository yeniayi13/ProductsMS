using MediatR;
using ProductsMs.Domain.Entities.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Application.Category.Queries
{
    public class GetCategoryNameQuery:IRequest<CategoryEntity>
    {
        public string Name { get; set; }


        public GetCategoryNameQuery(string name)
        {
            Name = name;
        }
    }
}
