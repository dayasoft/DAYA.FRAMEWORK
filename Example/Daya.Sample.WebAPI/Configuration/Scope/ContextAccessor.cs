using DAYA.Cloud.Framework.V2.Application;

public class ContextAccessor : IContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextAccessor(IHttpContextAccessor executionContextAccessor)
    {
        _httpContextAccessor = executionContextAccessor;
    }

    private Guid UserId()
    {
        try
        {
            return Guid.Parse(FromClaim(DayaClaimTypes.UserId));
        }
        catch
        {
            return default;
        }
    }

    private string FirstName()
    {
        return FromClaim(DayaClaimTypes.FirstName);
    }

    private string LastName()
    {
        return FromClaim(DayaClaimTypes.LastName);
    }

    private string EmailAddress()
    {
        return FromClaim(DayaClaimTypes.EmailAddress);
    }

    private int UserType()
    {
        return int.Parse(FromClaim(DayaClaimTypes.UserType));
    }

    private string Name()
    {
        try
        {
            return FromClaim(DayaClaimTypes.UserName);
        }
        catch
        {
            return FromClaim("name");
        }
    }

    private string FromClaim(string key)
    {
        var claims = _httpContextAccessor?.HttpContext?.User.Claims;

        return claims
            .Single(x => string.Equals(x.Type, key, StringComparison.OrdinalIgnoreCase))
            .Value;
    }

    Guid IServiceContextAccessor.UserId => UserId();
    string IServiceContextAccessor.FullName => Name();
    string IContextAccessor.FirstName => FirstName();
    string IContextAccessor.LastName => LastName();
    string IContextAccessor.EmailAddress => EmailAddress();
    int IContextAccessor.UserType => UserType();
}