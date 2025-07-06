namespace DAYA.Cloud.Framework.V2.Infrastructure.ContextAccessors
{
    /// <summary>
    /// Provides access to the current user context
    /// </summary>
    public interface IUserContextAccessor
    {
        /// <summary>
        /// Gets the current user identifier
        /// </summary>
        string GetUserId();

        /// <summary>
        /// Gets the current user name
        /// </summary>
        string GetUserName();
    }
}
