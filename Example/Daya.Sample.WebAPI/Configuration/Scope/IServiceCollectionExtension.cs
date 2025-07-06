internal static class IServiceCollectionExtension
{
    public static IServiceCollection AddScopeAccessors(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IContextAccessor, ContextAccessor>();

        return services;
    }
}