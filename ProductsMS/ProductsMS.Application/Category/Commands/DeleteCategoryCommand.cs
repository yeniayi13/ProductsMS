using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Application.Category.Commands
{
    public class DeleteCategoryCommand : IRequest<Guid>
    {
        public Guid CategoryId { get; set; }

        public DeleteCategoryCommand(Guid category)
        {
            CategoryId = category;
        }
    }
}
