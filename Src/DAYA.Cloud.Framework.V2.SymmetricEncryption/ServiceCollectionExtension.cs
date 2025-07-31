using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace DAYA.Cloud.Framework.V2.SymmetricEncryption;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSymmetricEncryption(this IServiceCollection services, string hexKey)
    {
        services.AddTransient<ISymmetricEncryption, SymmetricEncryption>();

        services.AddSingleton<IBinaryToTextConverter, Base64>();

        services.AddTransient<SymmetricAlgorithm, AesCryptoServiceProvider>();

        services.AddSingleton(new SymmetricAlgorithmConfig(hexKey)
        {
            CipherMode = CipherMode.CBC, // Use CBC instead of insecure ECB
            PaddingMode = PaddingMode.PKCS7
        });

        return services;
    }
}