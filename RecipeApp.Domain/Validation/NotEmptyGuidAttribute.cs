using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Domain.Validation;

public class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is Guid guid && guid != Guid.Empty;
    }
}
