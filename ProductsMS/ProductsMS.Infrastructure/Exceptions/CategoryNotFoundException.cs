using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Exceptions
{
    [ExcludeFromCodeCoverage]

    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException() { }

        public CategoryNotFoundException(string message)
            : base(message) { }

        public CategoryNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}
