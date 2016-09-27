using System;

namespace Docx.Templater.Errors
{
    internal interface IError : IEquatable<IError>
    {
        string Message { get; }
    }
}