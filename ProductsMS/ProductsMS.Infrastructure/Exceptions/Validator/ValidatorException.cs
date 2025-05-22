using System.Diagnostics.CodeAnalysis;

namespace ProductsMS.Infrastructure.Exceptions
{
    [ExcludeFromCodeCoverage]

    public class ValidatorException : Exception
    {
        public ValidatorException(List<global::FluentValidation.Results.ValidationFailure> errors)
        {
            Errors = errors;
        }

        public ValidatorException(string message) : base(message)
        {
        }

        public ValidatorException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public List<global::FluentValidation.Results.ValidationFailure> Errors { get; }
    }
}
