using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;


namespace ProductsMs.Infrastructure.Database.Configuration
{
        public class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
        {
                public void Configure(EntityTypeBuilder<CategoryEntity> builder)
                {


                        builder.ToTable("Categories");
                        builder.HasKey(s => s.Id);
                        builder.Property(s => s.Id)
                                .HasConversion(categoryId => categoryId.Value, value => CategoryId.Create(value)!)
                                .IsRequired();
                        builder.Property(s => s.Name)
                                .HasConversion(categoryName => categoryName.Value, value => CategoryName.Create(value)!)
                                .IsRequired();

                }
        }
}