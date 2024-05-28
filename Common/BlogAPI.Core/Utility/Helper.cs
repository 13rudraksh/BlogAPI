using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlogAPI.Core.Utility
{
    [ExcludeFromCodeCoverage]
    public class Helper
    {
        public class MinWordsAttribute : ValidationAttribute
        {
            private readonly int _minWords;

            public MinWordsAttribute(int minWords)
            {
                _minWords = minWords;
            }

            protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
            {
                if (value == null)
                {
                    return new ValidationResult($"The field {validationContext.DisplayName} is required.");
                }

                if (value is string strValue)
                {
                    var words = strValue.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length < _minWords)
                    {
                        return new ValidationResult($"The field {validationContext.DisplayName} must be at least {_minWords} words.");
                    }
                }

                return ValidationResult.Success!;
            }
        }
    }
}