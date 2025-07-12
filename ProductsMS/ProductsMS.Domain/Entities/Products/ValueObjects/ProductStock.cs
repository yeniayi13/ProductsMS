//using ProductsMS.Common.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace ProductsMs.Domain.Entities.Products.ValueObjects
{

    [ExcludeFromCodeCoverage]
    public partial class ProductStock
    {
        private const string Pattern = @"^\d+$";
        private ProductStock(decimal value) => Value = value;
        //TODO REvisar si necesita el regex
        public static ProductStock Create(decimal value)
        {
            try
            {
                //if (value == default) throw new NullAttributeException("Value is required");
                //if (!BasePriceRegex().IsMatch(value)) throw new InvalidAttributeException("Client ci is invalid");
               // if (value < 0) throw new InvalidAttributeException("Value is invalid");

                return new ProductStock(value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public decimal Value { get; init; } //*init no permite setear desde afuera, solo desde el constructor

        [GeneratedRegex(Pattern)]
        private static partial Regex BasePriceRegex();
    }
}

