namespace DAYA.Cloud.Framework.V2.Authentication.Contracts
{
	public interface IFakeJwtTokenGenerator
	{
		string GenerateToken(Dictionary<string, string> claims, TimeSpan expiry);
	}
}
