using System;

namespace DAYA.Cloud.Framework.V2.Application;

public interface IServiceContextAccessor
{
    Guid UserId { get; }
    string FullName { get; }
}
