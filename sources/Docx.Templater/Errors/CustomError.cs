using System;

namespace Docx.Templater.Errors
{
    internal class CustomError : IError, IEquatable<CustomError>
    {
        private readonly string _customMessage;

        internal CustomError(string customMessage)
        {
            _customMessage = customMessage;
        }

        public string Message
        {
            get
            {
                return _customMessage;
            }
        }

        public bool Equals(IError other)
        {
            return Equals(other as CustomError);
        }

        public bool Equals(CustomError other)
        {
            return other != null && Message.Equals(other.Message);
        }

        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }
    }
}