
using ProductsMs.Domain.Entities.Products.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Domain.Entities.Products.ValueObjects
{
    public class ProductUserId
    {
        private ProductUserId(Guid value) => Value = value;

        public static ProductUserId Create()
        {
            return new ProductUserId(Guid.NewGuid());
        }
        public static ProductUserId? Create(Guid value)
        {
            //if (value == Guid.Empty) throw new NullAttributeException("Product userId is required");

            return new ProductUserId(value);
        }

        public static ProductUserId Create(object value)
        {
            throw new NotImplementedException();
        }

        public Guid Value { get; init; }
    }
}
