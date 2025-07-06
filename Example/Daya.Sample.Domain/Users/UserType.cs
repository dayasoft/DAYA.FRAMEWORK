public enum UserType
{
    /// <summary>
    /// None = 0
    /// </summary>
    None = 0,

    /// <summary>
    /// PlatformAdmin = 1
    /// </summary>
    PlatformAdmin = 1 << 0,

    /// <summary>
    /// PlatformUser = 2
    /// </summary>
    PlatformUser = 1 << 1,

    /// <summary>
    /// B2BAdmin = 4
    /// </summary>
    B2BAdmin = 1 << 2,

    /// <summary>
    /// B2BUser = 8
    /// </summary>
    B2BUser = 1 << 3,
}