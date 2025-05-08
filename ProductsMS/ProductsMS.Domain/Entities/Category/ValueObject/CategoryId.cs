using ProductsMS.Common.Exceptions;

namespace ProductsMs.Domain.Entities.Category.ValueObject
{
    public class CategoryId
    {
        private CategoryId(Guid value) => Value = value;

        public static CategoryId Create()
        {
            return new CategoryId(Guid.NewGuid());
        }
        public static CategoryId? Create(Guid value)
        {
             if (value == Guid.Empty) throw new NullAttributeException("Category id is required");

            return new CategoryId(value);
        }

        public static CategoryId Create(object value)
        {
            throw new NotImplementedException();
        }

        public Guid Value { get; init; }
    }
}
