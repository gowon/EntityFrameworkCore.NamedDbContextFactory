namespace EntityFrameworkCore.NamedDbContextFactory
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class NamedDbContextFactoryExtensions
    {
        public static INamedDbContextFactoryBuilder<TContext> AddNamedDbContext<TContext>(
            this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            // Registering DbContext before any other attempts to play nice with default behaviors/expectations
            // ref: https://github.com/dotnet/efcore/pull/25440
            services.TryAdd(new ServiceDescriptor(typeof(TContext), provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory<TContext>>();
                return factory.CreateDbContext();
            }, lifetime));

            services.AddDbContextFactory<TContext, NamedDbContextFactory<TContext>>(lifetime: lifetime);

            // AddDbContextFactory only registers the interface, not the implementation
            // ref: https://github.com/dotnet/efcore/blob/f9c7b6583ab20e17526fc17a347ea7e5b633327f/src/EFCore/Extensions/EntityFrameworkServiceCollectionExtensions.cs#L801
            services.TryAdd(new ServiceDescriptor(typeof(NamedDbContextFactory<TContext>),
                provider =>
                    provider.GetRequiredService<IDbContextFactory<TContext>>() as NamedDbContextFactory<TContext>,
                lifetime));

            return new NamedDbContextFactoryBuilder<TContext>(services, lifetime);
        }

        public static INamedDbContextFactoryBuilder<TContext> AddNamedFactory<TContext, TFactory>(
            this INamedDbContextFactoryBuilder<TContext> builder, ServiceLifetime? lifetime = null)
            where TContext : DbContext
            where TFactory : class, INamedDbContextFactory<TContext>
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            
            builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(INamedDbContextFactory<TContext>), typeof(TFactory),
                lifetime ?? builder.Lifetime));

            return builder;
        }
    }
}