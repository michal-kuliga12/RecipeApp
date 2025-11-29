using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RecipeApp.Domain.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequireIDorString : ValidationAttribute
    {
        private string _idPropertyName;
        private string _referencePropertyName;

        public RequireIDorString(string idPropertyName, string referencePropertyName)
        {
            _idPropertyName = idPropertyName;
            _referencePropertyName = referencePropertyName;
            this.ErrorMessage = "Należy podać jedną z wartości: {0} lub {1}";
        }
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, _idPropertyName, _referencePropertyName);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            Type type = validationContext.ObjectType;

            PropertyInfo? idPropertyInfo = type.GetProperty(_idPropertyName);
            PropertyInfo? referencePropertyInfo = type.GetProperty(_referencePropertyName);

            if (idPropertyInfo is null || referencePropertyInfo is null)
                throw new ArgumentException($"Właściwości ('{_idPropertyName}', '{_referencePropertyName}') nie zostały znalezione w typie '{type.Name}'.");

            object? idValue = idPropertyInfo.GetValue(instance);
            object? referenceValue = referencePropertyInfo.GetValue(instance);

            bool idIsPresent = idValue is Guid idGuid && (idGuid != Guid.Empty);
            bool referenceIsPresent = referenceValue is String refString && !String.IsNullOrWhiteSpace(refString);

            if (idIsPresent ^ referenceIsPresent)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { _idPropertyName, _referencePropertyName });
        }
    }
}
