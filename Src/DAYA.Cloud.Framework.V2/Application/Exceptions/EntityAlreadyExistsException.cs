using System;

namespace DAYA.Cloud.Framework.V2.Application.Exceptions;

public class EntityAlreadyExistsException : Exception
{
    public EntityAlreadyExistsException(string message)
        : base(message)
    {
    }
}
