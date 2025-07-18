using System.Runtime.InteropServices;
using Xunit;

namespace Daya.Sample.IntegrationTests.SeedWork
{
    public class SkipeOnAzureFact : FactAttribute
    {
        public SkipeOnAzureFact()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && IsFromPipeLineBuild())
            {
                Skip = "Ignore on Azure Pipeline";
            }
        }

        private static bool IsFromPipeLineBuild()
            => Environment.GetEnvironmentVariable("TF_BUILD") != null;
    }

    public class SkipeOnAzureTheory : TheoryAttribute
    {
        public SkipeOnAzureTheory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && IsFromPipeLineBuild())
            {
                Skip = "Ignore on Azure Pipeline";
            }
        }

        private static bool IsFromPipeLineBuild()
            => Environment.GetEnvironmentVariable("TF_BUILD") != null;
    }
}