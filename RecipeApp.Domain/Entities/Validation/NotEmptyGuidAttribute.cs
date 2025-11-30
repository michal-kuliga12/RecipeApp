using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Domain.Entities.Validation;

public class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is Guid guid && guid != Guid.Empty;
    }
}
