using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Infrastructure.Exceptions
{

    [ExcludeFromCodeCoverage]

    public class ProductInAuctionException : Exception
    {
        public ProductInAuctionException() { }

        public ProductInAuctionException(string message)
            : base(message) { }

        public ProductInAuctionException(string message, Exception inner)
            : base(message, inner) { }
    }
}
