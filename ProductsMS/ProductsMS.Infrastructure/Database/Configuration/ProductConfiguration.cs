
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsMs.Domain.Entities.Products;
using ProductsMs.Domain.Entities.Products.ValueObjects;

namespace ProductsMs.Infrastructure.Database.Configuration
{
        public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
        {
                public void Configure(EntityTypeBuilder<ProductEntity> builder)
                {


                        builder.ToTable("Products");
                        builder.HasKey(s => s.Id);
                        builder.Property(s => s.Id)
                                .HasConversion(productId => productId.Value, value => ProductId.Create(value)!)
                                .IsRequired();
                        builder.Property(s => s.Name)
                                .HasConversion(productName => productName.Value, value => ProductName.Create(value)!)
                                .IsRequired();
                        builder.Property(s => s.Description)
                                .HasConversion(productDescription => productDescription.Value, value => ProductDescription.Create(value)!)
                                .IsRequired();
                        builder.Property(s => s.Image)
                              .HasConversion(productImage => productImage.Url, value => ProductImage.Create(value)!)
                              .IsRequired();
                        builder.Property(s => s.Stock)
                              .HasConversion(productStock => productStock.Value, value => ProductStock.Create(value)!)
                              .IsRequired();

            builder.Property(s => s.Price)
                                .HasConversion(productPrice => productPrice.Value, value => ProductPrice.Create(value)!)
                                .IsRequired();
                        builder.Property(s => s.Avilability)
                                .HasConversion<string>()
                                .IsRequired();
                         builder.Property(p => p.CategoryId)
                                .IsRequired(); // La clave foránea no puede ser nula

                        builder.HasOne(p => p.Category) // Relación con Categoria
                            .WithMany(c => c.Products)  // Categoria tiene muchos Productos
                            .HasForeignKey(p => p.CategoryId) // Definimos la clave foránea
                            .IsRequired(); // Aseguramos que la relación sea obligatoria



        }
        }
}