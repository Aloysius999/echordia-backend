using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Ech.Abstractions.Validation
{
    public class DataAnnotationsValidator
    {
        public static IEnumerable<PropertyValidationError> GetValidationErrors<T>(T model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var _ = Validator.TryValidateObject(model, context, results, true);
            var query = from r in results
                        select new PropertyValidationError() { ErrorMessage = r.ErrorMessage, PropertyName = r.MemberNames.First() };
            return query;
        }

        public static bool IsValid<T>(T model)
        {
            return !GetValidationErrors(model).Any();
        }
    }
}
