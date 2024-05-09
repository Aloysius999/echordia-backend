using System;
using System.Runtime.Serialization;
using Ech.Abstractions.Validation;
using System.Collections.Generic;

namespace Ech.Abstractions.Exceptions
{
    public class ValidationErrorException : Exception
    {
        //public ValidationErrorsModel ValidationErrors { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="validationErrors">Validation errors</param>
        public ValidationErrorException(IEnumerable<PropertyValidationError> validationErrors)
            : base("")
        {
            //ValidationErrors = new ValidationErrorsModel();
            //var errors = new List<PropertyValidationErrorModel>();
            //foreach (var validationError in validationErrors)
            //{
            //    var error = new PropertyValidationErrorModel();
            //    error.ErrorMessage = validationError.ErrorMessage;
            //    error.PropertyName = validationError.PropertyName;
            //    errors.Add(error);
            //}
            //ValidationErrors.ValidationErrors = errors.ToArray();
        }

        public ValidationErrorException(string errorMessage)
            : this("Validation Error", "", errorMessage)
        {
            //ValidationErrors = new ValidationErrorsModel();
            //var errors = new List<string>()
            //{
            //    errorMessage
            //};
            //ValidationErrors.Errors = errors.ToArray();
        }

        public ValidationErrorException(string message, string propertyName, string errorMessage)
            : base(message)
        {
            //ValidationErrors = new ValidationErrorsModel();
            //var errors = new List<PropertyValidationErrorModel>();
            //var error = new PropertyValidationErrorModel()
            //{
            //    ErrorMessage = errorMessage,
            //    PropertyName = propertyName
            //};
            //errors.Add(error);
            //ValidationErrors.ValidationErrors = errors.ToArray();
        }
    }
}