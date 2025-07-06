using System;

namespace DAYA.Cloud.Framework.V2.Application.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message)
        : base(message)
    {
    }
}
