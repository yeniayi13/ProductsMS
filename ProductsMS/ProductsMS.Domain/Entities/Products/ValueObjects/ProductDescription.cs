
using ProductsMS.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMs.Domain.Entities.Products.ValueObjects
{
    public partial class ProductDescription
    {
        private ProductDescription(string value) => Value = value;

        public static ProductDescription Create(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) throw new NullAttributeException("Product description is required");

                return new ProductDescription(value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string Value { get; init; } //*init no permite setear desde afuera, solo desde el constructor
    }
}
