using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using FluentValidation.Results;

namespace bimeh_back.Components.Tools
{
    public class ValidationResult : FluentValidation.Results.ValidationResult
    {
        public ValidationResult()
        {
        }

        public ValidationResult(IEnumerable<FluentValidation.Results.ValidationFailure> failures) : base(failures)
        {
        }

        public static ValidationResult FromFluentValidationResult(FluentValidation.Results.ValidationResult result)
        {
            return new ValidationResult(result.Errors);
        }

        public IEnumerable<ValidationFailure> Messages()
        {
            return Errors.Select(x => new ValidationFailure {
                PropertyName = x.PropertyName,
                ErrorMessage = x.ErrorMessage,
            });
        }

        public bool Failed()
        {
            return !IsValid;
        }
    }
}