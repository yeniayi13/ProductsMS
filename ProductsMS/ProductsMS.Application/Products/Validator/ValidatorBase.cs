using FluentValidation;
using ProductsMS.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Application.Products.Validators
{
    public class ValidatorBase<T> : AbstractValidator<T>
    {
        public virtual async Task<bool> ValidateRequest(T request)
        {
            var result = await ValidateAsync(request);
            if (!result.IsValid)
            {
                throw new ValidatorException(result.Errors.ToString());
            }

            return result.IsValid;
        }
    }
}
