
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;
using ProductsMS.Domain.Entities.Products.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace ProductsMS.Infrastructure.Database.Configuration.Postgres
{

    [ExcludeFromCodeCoverage]
    public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
        {
                public void Configure(EntityTypeBuilder<ProductEntity> builder)
                {


                        builder.ToTable("Products");
                        builder.HasKey(s => s.ProductId);
                        builder.Property(s => s.ProductId)
                                .HasConversion(productId => productId.Value, value => ProductId.Create(value)!)
                                .IsRequired();

                        builder.Property(s => s.ProductName)
                                .HasConversion(productName => productName.Value, value => ProductName.Create(value)!)
                                .IsRequired();

                        builder.Property(s => s.ProductDescription)
                                .HasConversion(productDescription => productDescription.Value, value => ProductDescription.Create(value)!)
                                .IsRequired();

                        builder.Property(s => s.ProductImage)
                              .HasConversion(productImage => productImage.Base64Data, value => ProductImage.FromBase64(value)!)
                              .IsRequired();

                        builder.Property(s => s.ProductStock)
                              .HasConversion(productStock => productStock.Value, value => ProductStock.Create(value)!)
                              .IsRequired();

                        builder.Property(s => s.ProductPrice)
                                            .HasConversion(productPrice => productPrice.Value, value => ProductPrice.Create(value)!)
                                            .IsRequired();

                        builder.Property(s => s.ProductAvilability)
                                .HasConversion<string>()
                                .IsRequired();
                         builder.Property(p => p.CategoryId)
                                .IsRequired(); // La clave foránea no puede ser nula

                                    builder.HasOne(p => p.Category)
                        .WithMany(c => c.Products)
                        .HasForeignKey(p => p.CategoryId)
                        .OnDelete(DeleteBehavior.Cascade) // Ajusta según tu lógica de negocio
                        .IsRequired();
            builder.Property(s => s.ProductUserId)
                               .HasConversion(ProductUserId => ProductUserId.Value, value => ProductUserId.Create(value)! )
                               .IsRequired();

        }
        }
}