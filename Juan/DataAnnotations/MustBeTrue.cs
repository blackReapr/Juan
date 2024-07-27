using System.ComponentModel.DataAnnotations;

namespace Juan.DataAnnotations;

public class MustBeTrue : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is bool boolValue && boolValue)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage ?? "The field must be true.");
    }
}
