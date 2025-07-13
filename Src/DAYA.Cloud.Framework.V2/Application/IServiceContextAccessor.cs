using System;

namespace DAYA.Cloud.Framework.V2.Application;

public interface IServiceContextAccessor
{
    Guid UserId { get; }

    string FirstName { get; }
    string LastName { get; }
    string EmailAddress { get; }

    string UserName { get; }

    Guid UserObjectId { get; }
}