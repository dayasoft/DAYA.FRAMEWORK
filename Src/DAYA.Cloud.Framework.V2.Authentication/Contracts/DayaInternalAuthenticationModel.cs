namespace DAYA.Cloud.Framework.V2.Authentication.Contracts;

public class DayaInternalAuthenticationModel
{
	public bool IsAuthorized { get; set; }
	public List<DayaClaimItem>? Claims { get; set; }
}