using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAYA.Cloud.Framework.V2.Authentication.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace DAYA.Cloud.Framework.V2.Authentication.Authentication
{
	internal class FakeJwtTokenGenerator : IFakeJwtTokenGenerator
	{
		internal static byte[] EncryptionKey = Encoding.ASCII.GetBytes("JYa65i0tITZnnjnZJYa65i0tITZnnjnZJYa65i0tITZnnjnZ");

		public string GenerateToken(Dictionary<string, string> claimDictionary, TimeSpan expiry)
		{
			var claims = claimDictionary.Select(x => new Claim(x.Key, x.Value));
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.Add(expiry),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(EncryptionKey),
					SecurityAlgorithms.HmacSha256Signature)
			};
			var secToken = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(secToken);
		}
	}
}