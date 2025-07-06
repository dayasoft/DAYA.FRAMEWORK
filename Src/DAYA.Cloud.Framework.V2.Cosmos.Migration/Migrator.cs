namespace DAYA.Cloud.Framework.V2.Cosmos.Migration;

public abstract class Migrator
{
    public abstract Task MigrateAsync();
    public abstract string Version { get; }
    public abstract int Order { get; }
    public abstract string Description { get; }
}
