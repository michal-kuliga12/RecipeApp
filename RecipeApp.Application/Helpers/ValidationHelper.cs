using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Application.Helpers;

public static class ValidationHelper
{
    public static bool ValidateModel(object? obj)
    {
        if (obj == null)
            return false;

        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(obj, context, results, true);

        return isValid;
    }

}
