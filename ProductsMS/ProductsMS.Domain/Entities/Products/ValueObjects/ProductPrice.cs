using ProductsMS.Common.Exceptions;
using System.Text.RegularExpressions;

namespace ProductsMs.Domain.Entities.Products.ValueObjects
{
    public partial class ProductPrice
    {
          private const string Pattern = @"^\d+\.\d{2}$";
        private ProductPrice(decimal value) => Value = value;
        //TODO REvisar si necesita el regex
        public static ProductPrice Create(decimal value)
        {
            try
            {
                if (value == default) throw new NullAttributeException("Value is required");
                //if (!BasePriceRegex().IsMatch(value)) throw new InvalidAttributeException("Client ci is invalid");
                if (value < 0) throw new InvalidAttributeException("Value is invalid");

                return new ProductPrice(value);
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
