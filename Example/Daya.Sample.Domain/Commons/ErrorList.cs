namespace Daya.Sample.Domain.Commons
{
    public static class ErrorList
    {
        // User Accounts - A1XXX
        [DayaError("User account with the same email address already exist")]
        public const string UserAccountAlreadyExist = "A1000";
    }
}