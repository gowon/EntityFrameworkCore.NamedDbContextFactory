namespace EntityFrameworkCore.NamedDbContextFactory
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public interface INamedDbContextFactoryBuilder<TContext> where TContext : DbContext
    {
        IServiceCollection Services { get; }
        ServiceLifetime Lifetime { get; }
    }
}