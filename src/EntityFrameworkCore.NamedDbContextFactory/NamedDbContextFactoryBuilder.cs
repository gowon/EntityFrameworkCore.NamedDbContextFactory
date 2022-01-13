namespace EntityFrameworkCore.NamedDbContextFactory
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class NamedDbContextFactoryBuilder<TContext> : INamedDbContextFactoryBuilder<TContext> where TContext : DbContext
    {
        internal NamedDbContextFactoryBuilder(IServiceCollection services, ServiceLifetime lifetime)
        {
            Services = services;
            Lifetime = lifetime;
        }

        public IServiceCollection Services { get; }
        public ServiceLifetime Lifetime { get; }
    }
}