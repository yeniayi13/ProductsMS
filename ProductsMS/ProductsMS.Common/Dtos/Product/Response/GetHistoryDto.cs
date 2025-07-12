using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Product.Response
{
    public class GetHistoryDto
    {
        public Guid Id { get;  set; }
        public Guid UserId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
