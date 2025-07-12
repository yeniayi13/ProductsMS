using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ProductsMs
{
    [ExcludeFromCodeCoverage]
    public class PresentationAssemblyReference
    {
        internal static readonly Assembly Assembly = typeof(PresentationAssemblyReference).Assembly;
    }
}