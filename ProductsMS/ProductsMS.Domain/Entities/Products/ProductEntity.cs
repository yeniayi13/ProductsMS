using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Enum;
using ProductsMS.Common.Primitives;

namespace ProductsMs.Domain.Entities.Products
{
    public sealed class ProductEntity: AggregateRoot
    {
        public ProductId Id { get; private set; }
        public ProductName Name { get; private set; }
        public ProductImage Image { get; private set; }
        public ProductPrice Price { get; private set; }
        public ProductDescription Description { get; private set; }

        public CategoryId CategoryId { get; private set; } //FK

        public CategoryEntity Category { get; private set; } //Navigation Property

        public ProductAvilability Avilability { get; private set; }

        public ProductStock Stock { get; private set; } 
        public ProductEntity(ProductId id, ProductName name, ProductImage image, ProductPrice price, ProductDescription description, ProductAvilability avilability, ProductStock stock, CategoryId categoryId)
        {
            Id = id;
            Name = name;
            Image = image;
            Price = price;
            Description = description;
            Avilability = avilability;
            Stock = stock;
            CategoryId = categoryId;

        }

        public ProductEntity() { }


        //actualiza las propiedades de un objeto
        public static ProductEntity Update(ProductEntity product, ProductName name, ProductImage image, ProductPrice price, ProductDescription description, ProductAvilability avilability, ProductStock stock)
        {
            var updates = new List<Action>()
            {
                () => { if (name != null) product.Name = name; },
                () => { if (image != null) product.Image = image; },
                () => { if (price != null) product.Price = price; },
                () => { if (description != null) product.Description = description; },
                () => { if (stock != null) product.Stock = stock; },
                () => { if (avilability != null) product.Avilability = avilability; },

            };
            updates.ForEach(update => update());
            return product;
        }

    }
}
