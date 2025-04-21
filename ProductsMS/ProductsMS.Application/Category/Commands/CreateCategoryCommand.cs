using MediatR;
using ProductsMS.Common.Dtos.Category.Request;

namespace ProductsMS.Application.Category.Commands
{
    public class CreateCategoryCommand :IRequest<Guid>
    {
        public CreateCategoryDto Category { get; set; }
        public CreateCategoryCommand(CreateCategoryDto category)
        {
            Category = category;
        }
    }
}
