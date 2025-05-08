

using ProductsMS.Common.Exceptions;

namespace ProductsMs.Domain.Entities.Products.ValueObjects
{
    public class ProductId
    {
        private ProductId(Guid value) => Value = value;

        public static ProductId Create()
        {
            return new ProductId(Guid.NewGuid());
        }
        public static ProductId? Create(Guid value)
        {
            if (value == Guid.Empty) throw new NullAttributeException("Product id is required");

            return new ProductId(value);
        }

        public static ProductId Create(object value)
        {
            throw new NotImplementedException();
        }

        public Guid Value { get; init; }
    }
}
