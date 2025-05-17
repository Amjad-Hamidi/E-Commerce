using System.ComponentModel.DataAnnotations;

namespace TShop.API.Validations
{
    public class OverYears(int years) : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime data)
            {
                if (DateTime.Now.Year - data.Year > years)
                    return true;
            }
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be over {years} years old.";
        }
    }
}
