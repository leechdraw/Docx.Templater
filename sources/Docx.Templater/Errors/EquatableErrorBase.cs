using System;

namespace TemplateEngine.Docx.Errors
{
    public abstract class EquatableBase< T, TCh>:IEquatable<TCh>
        where T:class , IEquatable<T>
        where TCh: class ,T, IEquatable<TCh>
    {
        public bool Equals(TCh other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(T other)
        {
            throw new NotImplementedException();
        }
    }
    internal abstract class EquatableErrorBase<T> : IError, IEquatable<T>
        where T : class, IError
    {
        public bool Equals(IError other)
        {
            if (!(other is T))
                return false;
            return Equals((T) other);
        }

        public abstract string Message { get; }

        public abstract bool Equals(T other);
    }
}