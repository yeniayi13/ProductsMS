using ProductsMS.Common.Exceptions;
using System.Text.RegularExpressions;

namespace ProductsMs.Domain.Entities.Products.ValueObjects
{
    public partial class ProductName
    {
        private const string Pattern = @"^[a-zA-Z]+$";
        private ProductName(string value) => Value = value;

        public static ProductName Create(string value)
        {
            try
            {
               if (string.IsNullOrEmpty(value)) throw new NullAttributeException("Product Name is required");
               if (!NameRegex().IsMatch(value)) throw new InvalidAttributeException("Product Name is invalid");

                return new ProductName(value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string Value { get; init; } //*init no permite setear desde afuera, solo desde el constructor
        [GeneratedRegex(Pattern)]
        private static partial Regex NameRegex();
    }
}
