namespace Daya.Sample.Domain.UserAccounts.Dto
{
    public record UserAccountProfileDto(Guid UserId, string FirstName, string LastName, string EmailAddress);
}