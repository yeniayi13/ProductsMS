//using ProductsMS.Common.Exceptions;
using System.Text.RegularExpressions;

namespace ProductsMs.Domain.Entities.Category.ValueObject
{
    public partial class CategoryName
    {
        private const string Pattern = @"^[a-zA-Z]+$";
        private CategoryName(string value) => Value = value;

        public static CategoryName Create(string value)
        {
            try
            {
                //if (string.IsNullOrEmpty(value)) throw new NullAttributeException("Category Name is required");
                //if (!NameRegex().IsMatch(value)) throw new InvalidAttributeException("Category Name is invalid");

                return new CategoryName(value);
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
