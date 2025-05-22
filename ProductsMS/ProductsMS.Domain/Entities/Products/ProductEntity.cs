using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Common.Enum;
using ProductsMS.Common.Primitives;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace ProductsMs.Domain.Entities.Products
{
  
    public sealed class ProductEntity: AggregateRoot
    {
        public ProductId ProductId { get; private set; }
        public ProductName ProductName { get;  set; }
        public ProductImage ProductImage { get; private set; }
        public ProductPrice ProductPrice { get; private set; }
        public ProductDescription ProductDescription { get; private set; }

        public CategoryId CategoryId { get; private set; } //FK

        public CategoryEntity Category { get; private set; } //Navigation Property

        public ProductAvilability ProductAvilability { get; private set; }

        public ProductStock ProductStock { get; private set; } 

        public ProductUserId ProductUserId { get; private set; } //FK

        

        public ProductEntity(ProductId productId, ProductName productName, ProductImage productImage, ProductPrice productPrice, ProductDescription productDescription, ProductAvilability productavilability, ProductStock productStock, CategoryId categoryId, ProductUserId productUserId)
        {
            ProductId = productId;
            ProductName = productName;
            ProductImage = productImage;
            ProductPrice = productPrice;
            ProductDescription = productDescription;
            ProductAvilability = productavilability;
            ProductStock = productStock;
            CategoryId = categoryId;
            ProductUserId = productUserId;

        }

        public ProductEntity(
            ProductId productId,
            ProductName productName,
            ProductImage productImage,
            ProductPrice productPrice,
            ProductDescription productDescription,
            ProductAvilability productAvilability,
            ProductStock productStock,
            CategoryId categoryId,
            ProductUserId productUserId,
            string createdBy,
            DateTime createdAt,
            string updatedBy,
            DateTime updatedAt)
        {
            ProductId = productId;
            ProductName = productName;
            ProductImage = productImage;
            ProductPrice = productPrice;
            ProductDescription = productDescription;
            ProductAvilability = productAvilability;
            ProductStock = productStock;
            CategoryId = categoryId;
            ProductUserId = productUserId;
            CreatedBy = createdBy;
            UpdatedBy = updatedBy;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }


        


        public ProductEntity(ProductId productId)
        {
            ProductId = productId;
            
        }

        public ProductEntity() { }


        //actualiza las propiedades de un objeto
        public static ProductEntity Update(ProductEntity product, ProductName name, ProductImage image, ProductPrice price, ProductDescription description, ProductAvilability avilability, ProductStock stock,CategoryId categoryId, ProductUserId productUserId)
        {

               var updates = new List<Action>()
                {
                    () => { if (name != null) product.ProductName = name; },
                    () => { if (image != null) product.ProductImage = image; },
                    () => { if (price != null) product.ProductPrice = price; },
                    () => { if (description != null) product.ProductDescription = description; },
                    () => { if (stock != null) product.ProductStock = stock; },
                    () =>
                    {
                        if (avilability != null)
                        {
                            if (Enum.TryParse<ProductAvilability>(avilability.ToString(), out var availabilityEnum))
                            {
                                product.ProductAvilability = availabilityEnum;
                            }
                            else
                            {
                                Console.WriteLine($"Error: No se pudo convertir '{avilability}' a ProductAvailability.");
                            }
                        }
                    },
                    () => { if (product.CategoryId != null) product.CategoryId = categoryId; },
                    () => { if (product.ProductUserId != null) product.ProductUserId = productUserId; }
                };

            updates.ForEach(update => update());
            return product;
        }

    }
}
