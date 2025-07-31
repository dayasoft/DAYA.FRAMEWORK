using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DAYA.Cloud.Framework.V2.Authentication.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DAYA.Cloud.Framework.V2.Authentication.Authentication
{
	internal class FakeJwtTokenGenerator : IFakeJwtTokenGenerator
	{
		private readonly IConfiguration _configuration;
		private readonly byte[] _encryptionKey;

		public FakeJwtTokenGenerator(IConfiguration configuration)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_encryptionKey = GetOrGenerateEncryptionKey();
		}

		private byte[] GetOrGenerateEncryptionKey()
		{
			// Try to get key from configuration (Azure Key Vault, environment variables, etc.)
			var keyFromConfig = _configuration["Authentication:JwtSigningKey"];
			
			if (!string.IsNullOrEmpty(keyFromConfig))
			{
				// Validate key length (minimum 256 bits for HMAC-SHA256)
				var keyBytes = Convert.FromBase64String(keyFromConfig);
				if (keyBytes.Length < 32)
				{
					throw new InvalidOperationException("JWT signing key must be at least 256 bits (32 bytes) long");
				}
				return keyBytes;
			}

			// For development/testing only - generate a secure random key
			// WARNING: This should not be used in production as the key will change on each restart
			using var rng = RandomNumberGenerator.Create();
			var key = new byte[64]; // 512 bits for extra security
			rng.GetBytes(key);
			
			// Log warning for development scenarios
			Console.WriteLine("WARNING: Using randomly generated JWT signing key. Configure 'Authentication:JwtSigningKey' for production use.");
			
			return key;
		}

		public string GenerateToken(Dictionary<string, string> claimDictionary, TimeSpan expiry)
		{
			var claims = claimDictionary.Select(x => new Claim(x.Key, x.Value));
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.Add(expiry),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(_encryptionKey),
					SecurityAlgorithms.HmacSha256Signature)
			};
			var secToken = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(secToken);
		}
	}
}