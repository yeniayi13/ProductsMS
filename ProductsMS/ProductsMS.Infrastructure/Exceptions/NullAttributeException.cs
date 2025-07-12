using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsMS.Common.Exceptions
{

    [ExcludeFromCodeCoverage]

    public class NullAttributeException : Exception
    {
        public NullAttributeException() { }

        public NullAttributeException(string message)
            : base(message) { }

        public NullAttributeException(string message, Exception inner)
            : base(message, inner) { }
    }
}