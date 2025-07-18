namespace Daya.Sample.WebAPI.Configuration.Scope
{
    internal static class ContextServiceCollectionExtension
    {
        public static IServiceCollection AddScopeAccessors(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IContextAccessor, ContextAccessor>();

            return services;
        }
    }
}