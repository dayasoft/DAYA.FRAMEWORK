using System;

namespace DAYA.Cloud.Framework.V2.Application.Exceptions;

public class InconsistencyException : Exception
{
    public InconsistencyException(string message)
        : base(message)
    {
    }
}
