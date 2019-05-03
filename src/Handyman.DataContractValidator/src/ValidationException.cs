using System;

namespace Handyman.DataContractValidator
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}