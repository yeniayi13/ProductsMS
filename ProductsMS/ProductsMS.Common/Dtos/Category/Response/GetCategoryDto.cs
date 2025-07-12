using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Category.Response
{
    public class GetCategoryDto
    {
        public bool IsDeleted;

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }


       
    }
}
