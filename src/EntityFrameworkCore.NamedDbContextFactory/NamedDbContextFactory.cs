namespace EntityFrameworkCore.NamedDbContextFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class NamedDbContextFactory<TContext> : IDbContextFactory<TContext> where TContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, INamedDbContextFactory<TContext>> _factories;

        public string ConfigurationKey => $"DbContextProviders:{typeof(TContext).Name}";

        public NamedDbContextFactory(IServiceProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            _configuration = provider.GetRequiredService<IConfiguration>();

            var factories = provider.GetServices<INamedDbContextFactory<TContext>>();
            _factories = factories.ToDictionary(contextProvider => contextProvider.Name);
        }

        TContext IDbContextFactory<TContext>.CreateDbContext()
        {
            // ref: https://stackoverflow.com/a/65022730/7644876
            var name = _configuration.GetValue<string>(ConfigurationKey, null) ??
                       throw new InvalidOperationException($"No named provider specified for '{ConfigurationKey}'.");
            return CreateDbContext(name);
        }

        public TContext CreateDbContext(string name)
        {
            var dbContextProvider = _factories[name];
            return dbContextProvider.CreateDbContext();
        }
    }
}