using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Category.ValueObject;


namespace ProductsMS.Infrastructure.Database.Configuration.Postgres
{

    [ExcludeFromCodeCoverage]
    public class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
        {
                public void Configure(EntityTypeBuilder<CategoryEntity> builder)
                {


                        builder.ToTable("Categories");
                        builder.HasKey(s => s.CategoryId);
                        builder.Property(s => s.CategoryId)
                                .HasConversion(categoryId => categoryId.Value, value => CategoryId.Create(value)!)
                                .IsRequired();
                        builder.Property(s => s.CategoryName)
                                .HasConversion(categoryName => categoryName.Value, value => CategoryName.Create(value)!)
                                .IsRequired();

                }
        }
}