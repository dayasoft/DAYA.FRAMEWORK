using System;

namespace DAYA.Cloud.Framework.V2.Domain;

public static class Clock
{
    private static DateTime? _customDate;

    public static DateTime Now
    {
        get
        {
            if (_customDate.HasValue)
            {
                return _customDate.Value;
            }

            return DateTime.UtcNow;
        }
    }

    public static void SetCustomDate(DateTime customDate)
    {
        _customDate = customDate;
    }

    public static void Reset()
    {
        _customDate = null;
    }
}
