using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Project.Cad.Data.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class ApiModelInvalidDataException : Exception
{
    public ApiModelInvalidDataException()
    {
    }

    public ApiModelInvalidDataException(string message) : base(message)
    {
    }

    public ApiModelInvalidDataException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ApiModelInvalidDataException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
