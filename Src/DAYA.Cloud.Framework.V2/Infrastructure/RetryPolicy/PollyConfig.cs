using System;

namespace DAYA.Cloud.Framework.V2.Infrastructure.RetryPolicy;

public class PollyConfig
{
    public TimeSpan[] SleepDurations { get; set; }
}
