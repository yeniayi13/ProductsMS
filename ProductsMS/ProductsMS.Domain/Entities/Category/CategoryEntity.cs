using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Primitives;

namespace ProductsMs.Domain.Entities.Category
{
    public sealed class CategoryEntity: AggregateRoot
    {
        public CategoryId Id { get; private set; }
        public CategoryName Name { get; private set; }

        public List<ProductEntity> Products { get; private set; } = new List<ProductEntity>(); //Navigation Property
        public CategoryEntity(CategoryId id, CategoryName name)
        {
            Id = id;
            Name = name;
        }

        public CategoryEntity() { }

        public static CategoryEntity Update(CategoryEntity category, CategoryName name)
        {
            var updates = new List<Action>()
            {
                () => { if (name != null) category.Name = name; }
            };
            updates.ForEach(update => update());
            return category;
        }
    }
}
