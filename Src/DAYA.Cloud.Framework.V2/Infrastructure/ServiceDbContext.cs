using System;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.InternalCommands;
using DAYA.Cloud.Framework.V2.Application.Outbox;
using Microsoft.EntityFrameworkCore;

namespace DAYA.Cloud.Framework.V2.Infrastructure;

public class ServiceDbContext : DbContext
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<InternalCommandMessage> InternalCommands { get; set; }
    private bool _commited;

    public ServiceDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_commited)
        {
            throw new Exception("can not commit twice within a scope in DbContext");
        }
        _commited = true;
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        if (_commited)
        {
            throw new Exception("can not commit twice within a scope in DbContext");
        }
        _commited = true;
        return base.SaveChanges();
    }
}