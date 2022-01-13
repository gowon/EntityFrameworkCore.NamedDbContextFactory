namespace EntityFrameworkCore.NamedDbContextFactory
{
    using Microsoft.EntityFrameworkCore;

    public interface INamedDbContextFactory<out TContext> : IDbContextFactory<TContext>
        where TContext : DbContext
    {
        string Name { get; }
    }
}