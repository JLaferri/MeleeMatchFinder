using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ClientApplication.View
{
    public enum ValidationParseType
    {
        Double, PositiveDouble, NegativeDouble, Integer, PositiveInteger, NegativeInteger
    }

    public class SimpleValidation : ValidationRule
    {
        public int? MaxLength { get; set; }
        public int? MinimumLength { get; set; }
        public ValidationParseType? ParseType { get; set; }

        public bool NullIsEmptyString { get; set; }

        public SimpleValidation()
        {
            NullIsEmptyString = true;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string input;

            if (value == null && NullIsEmptyString) input = string.Empty;
            else if (value is string) input = (string)value;
            else return new ValidationResult(false, "Input must be a string.");

            if (MaxLength.HasValue && input.Length > MaxLength)
            {
                return new ValidationResult(false, string.Format("Input string is greater than the max length of {0}.", MaxLength));
            }
            
            if (MinimumLength.HasValue && input.Length < MinimumLength)
            {
                return new ValidationResult(false, string.Format("Input string is less than the minimum length of {0}.", MinimumLength));
            }
            
            if (ParseType.HasValue)
            {
                double parsedDouble;
                int parsedInt;

                switch (ParseType.Value)
                {
                    case ValidationParseType.Double:
                        if (!double.TryParse(input, out parsedDouble))
                            return new ValidationResult(false, "Input string could not be interpretted as a real number.");
                        break;
                    case ValidationParseType.PositiveDouble:
                        if (!double.TryParse(input, out parsedDouble) || parsedDouble < 0)
                            return new ValidationResult(false, "Input string could not be interpretted as a positive real number.");
                        break;
                    case ValidationParseType.NegativeDouble:
                        if (!double.TryParse(input, out parsedDouble) || parsedDouble > 0)
                            return new ValidationResult(false, "Input string could not be interpretted as a negative real number.");
                        break;
                    case ValidationParseType.Integer:
                        if (!int.TryParse(input, out parsedInt))
                            return new ValidationResult(false, "Input string could not be interpretted as an integer number.");
                        break;
                    case ValidationParseType.PositiveInteger:
                        if (!int.TryParse(input, out parsedInt) || parsedInt < 0)
                            return new ValidationResult(false, "Input string could not be interpretted as a positive integer number.");
                        break;
                    case ValidationParseType.NegativeInteger:
                        if (!int.TryParse(input, out parsedInt) || parsedInt > 0)
                            return new ValidationResult(false, "Input string could not be interpretted as a negative integer number.");
                        break;
                }
            }

            return new ValidationResult(true, null);
        }
    }
}
