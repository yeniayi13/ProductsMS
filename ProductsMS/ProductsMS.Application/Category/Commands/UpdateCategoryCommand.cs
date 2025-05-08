using MediatR;
using ProductsMS.Common.Dtos.Category.Request;


namespace ProductsMS.Application.Category.Commands
{
    public class UpdateCategoryCommand : IRequest<Guid>
    {
        public Guid CategoryId { get; set; }
        public UpdateCategoryDto Category { get; set; }


        public UpdateCategoryCommand(Guid id, UpdateCategoryDto category)
        {
            CategoryId = id;
            Category = category;
        }
    }
}
