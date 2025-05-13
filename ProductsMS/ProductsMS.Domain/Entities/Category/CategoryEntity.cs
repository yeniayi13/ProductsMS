using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Common.Primitives;

namespace ProductsMs.Domain.Entities.Category
{
    public  class CategoryEntity: AggregateRoot
    {
        public CategoryId CategoryId { get; private set; }
        public CategoryName CategoryName { get; private set; }

        public List<ProductEntity> Products { get; private set; }  //Navigation Property
        public CategoryEntity(CategoryId categoryId, CategoryName categoryName)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
        }

        public CategoryEntity() { }

        public static CategoryEntity Update(CategoryEntity category, CategoryName name)
        {
            var updates = new List<Action>()
            {
                () => { if (name != null) category.CategoryName = name; }
            };
            updates.ForEach(update => update());
            return category;
        }
    }
}
