using System;

namespace DAYA.Cloud.Framework.V2.Domain;

public class TypedIdInitializationException : Exception
{
    public TypedIdInitializationException(string message)
        : base(message)
    {
    }
}
