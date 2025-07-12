using ProductsMS.Domain.Entities.Products.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMS.Common.Dtos.Product.Response;

namespace ProductsMS.Core.Service.User
{
    public interface IUserService
    {
        Task<GetUser> AuctioneerExists(Guid productUserId);

    }
}
