using System.ComponentModel.DataAnnotations;

namespace WebApiCasino.Validaciones
{
    public class NombresConMayuscula : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            char[] spearator = { ' ' };

            String[] strlist = value.ToString().Split(spearator);

            foreach (String s in strlist)
            {
                Console.WriteLine(s);
                var letra = s.ToString()[0].ToString();
                if (letra != letra.ToUpper())
                {
                    return new ValidationResult("La primera letra de cada palabra debe ser mayuscula");
                }

            }

            return ValidationResult.Success;
        }
    }
}
