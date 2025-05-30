using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductsMS.Common.Dtos.Product.Response;

namespace ProductsMS.Core.Service.Auction
{
    public interface IAuctionService
    {
        Task<bool> AuctionExists(Guid productId, Guid userId);
    }
}
