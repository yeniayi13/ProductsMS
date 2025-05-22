using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsMS.Common.Exceptions
{

    [ExcludeFromCodeCoverage]

    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException() { }

        public ProductNotFoundException(string message)
            : base(message) { }

        public ProductNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}