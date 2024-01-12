using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.User;
using Nest;

namespace CIYW.Kernel.Extensions;

public static class ElasticSearchExtensions
{
    public static void AddElasticsearch(
        this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["ELKConfiguration:Url"];
        
        var user = configuration["ELKConfiguration:User"];
        var pass = configuration["ELKConfiguration:Pass"];
        var usersIndex = configuration["ELKConfiguration:Indexes:Users"];
        var invoicesIndex = configuration["ELKConfiguration:Indexes:Invoices"];
        var productsIndex = configuration["ELKConfiguration:Indexes:Products"];

        var settings = new ConnectionSettings(new Uri(url)).BasicAuthentication(user, pass)
            .PrettyJson()
            .DefaultIndex(usersIndex);

        AddDefaultMappings(settings);

        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);

        CreateIndex<User>(client, usersIndex);
        CreateIndex<Invoice>(client, invoicesIndex);
        CreateIndex<Product>(client, productsIndex);
    }

    private static void AddDefaultMappings(ConnectionSettings settings)
    {
        settings
            .DefaultMappingFor<User>(m => m
                .Ignore(p => p.Tariff)
                .Ignore(p => p.Currency)
                .Ignore(p => p.UserCategories)
                .Ignore(p => p.Invoices)
                .Ignore(p => p.Notes)
                .Ignore(p => p.UserBalance)
                .Ignore(p => p.PasswordHash)
                .Ignore(p => p.SecurityStamp)
                .Ignore(p => p.ConcurrencyStamp)
                .Ignore(p => p.TwoFactorEnabled)
                .Ignore(p => p.LockoutEnd)
                .Ignore(p => p.LockoutEnabled)
                .Ignore(p => p.AccessFailedCount)
            )
            .DefaultMappingFor<Invoice>(m => m
                .Ignore(p => p.User)
                .Ignore(p => p.Category)
                .Ignore(p => p.Currency)
                .Ignore(p => p.Note)
            )
            .DefaultMappingFor<Product>(m => m);
    }

    private static void CreateIndex<T>(IElasticClient client, string indexName) where T: class
    {
        var createIndexResponse = client.Indices.Create(indexName,
            index => index.Map<T>(x => x.AutoMap())
        );
    }
}

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
}