using DAYA.Cloud.Framework.V2.Application;

public interface IContextAccessor : IServiceContextAccessor
{
    string FirstName { get; }
    string LastName { get; }
    string EmailAddress { get; }
    int UserType { get; }
}