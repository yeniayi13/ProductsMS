using FluentValidation;
using ProductsMS.Application.Products.Validators;
using ProductsMS.Common.Dtos.Product.Request;

namespace ProductsMS.Application.Products.Validator.Products
{ 
    public class CreateProductEntityValidator : ValidatorBase<CreateProductDto>
    {
        public CreateProductEntityValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("El campo ProductName no puede estar vacío.")
                .MaximumLength(100).WithMessage("El campo ProductName no puede tener más de 100 caracteres.")
                .Matches("^[^0-9]*$").WithMessage("El campo ProductName no puede contener números.");

            RuleFor(x => x.ProductPrice)
                .NotEmpty().WithMessage("El precio del producto es obligatorio.")
                .ExclusiveBetween(0.01m, 100000m).WithMessage("El precio debe estar entre 0.01 y 100000.");

            RuleFor(x => x.ProductAvilability)
                .NotNull().WithMessage("Debe especificar la disponibilidad del producto.");

            RuleFor(x => x.ProductStock)
                .NotEmpty().WithMessage("El stock del producto es obligatorio.")
                .Must(stock => stock >= 0).WithMessage("El stock debe ser mayor a 0.");

            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage("Debe especificar una categoría válida.");

            RuleFor(x => x.ProductUserId)
                .NotNull().WithMessage("Debe asignar el producto a un usuario válido.");
        }
    }

}
