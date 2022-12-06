using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.DbPrism.Extends
{
    public static class ValidationExtension
    {
        public static string ValidateProperty(this object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return string.Empty;
            try
            {
                var targetType = obj.GetType();
                var propertyValue = targetType.GetProperty(propertyName).GetValue(obj, null);
                var validationContext = new ValidationContext(obj, null, null);
                validationContext.MemberName = propertyName;
                var validationResults = new List<ValidationResult>();

                Validator.TryValidateProperty(propertyValue, validationContext, validationResults);

                if (validationResults.Count > 0)
                {
                    return string.Join(";", validationResults.Select(p => p.ErrorMessage));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return string.Empty;
        }
    }
}
