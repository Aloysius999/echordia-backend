using System;

namespace Ech.Abstractions.Exceptions
{
    public class InvalidPropertyException : Exception
    {
        public virtual string PropertyName { get; }
        public virtual string ErrorMessage { get; }

        public InvalidPropertyException(string message, string propertyName, string errorMessage)
            : base(message)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }

        public InvalidPropertyException(string message)
            : base(message)
        {
            ErrorMessage = "";
            PropertyName = "";
        }
    }
}
