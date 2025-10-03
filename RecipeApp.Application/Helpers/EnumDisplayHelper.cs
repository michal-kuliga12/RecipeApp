using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Application.Helpers;

public static class EnumDisplayHelper
{
    public static string GetDisplayName(Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        var displayAttribute = fieldInfo?
            .GetCustomAttributes(typeof(DisplayAttribute), false)
            .FirstOrDefault() as DisplayAttribute;

        return displayAttribute?.Name ?? value.ToString();
    }
}
