using ProductsMS.Common.Dtos.Product.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Core.Service.History
{
    public interface IHistoryService
    {
        Task AddActivityHistoryAsync(GetHistoryDto history);
    }
}
