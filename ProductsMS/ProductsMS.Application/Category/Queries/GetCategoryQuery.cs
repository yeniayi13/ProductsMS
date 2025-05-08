using MediatR;
using ProductsMS.Common.Dtos.Category.Response;


namespace ProductsMS.Application.Category.Queries
{
    public class GetCategoryQuery : IRequest<GetCategoryDto>
    {
        public Guid Id { get; set; }

        public GetCategoryQuery(Guid id)
        {
            Id = id;
        }
    }
}
