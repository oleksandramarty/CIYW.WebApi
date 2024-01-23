using CIYW.Elasticsearch.Models.Currencies;
using CIYW.Elasticsearch.Models.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace CIYW.Elasticsearch;

public static class ElasticSearchExtension
{
    public static void AddElasticsearch(
        this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["ELKConfiguration:Url"];
        
        var user = configuration["ELKConfiguration:User"];
        var pass = configuration["ELKConfiguration:Pass"];
        var usersIndex = configuration["ELKConfiguration:Indexes:Users"];

        var settings = new ConnectionSettings(new Uri(url)).BasicAuthentication(user, pass)
            .PrettyJson()
            .DefaultIndex(usersIndex)
            .EnableDebugMode()
            .EnableApiVersioningHeader()
            .RequestTimeout(TimeSpan.FromMinutes(2))
            .DefaultMappingFor<UserSearchModel>(i => i)
            .DefaultMappingFor<UserBalanceSearchModel>(i => i)
            .DefaultMappingFor<CurrencySearchModel>(i => i);

        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);
        
        CreateIndex(client, usersIndex);
    }
    
    private static void CreateIndex(IElasticClient client, string indexName)
    {
        var createIndexResponse = client.Indices.Create(indexName, index => index
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(1)
            )
            .Map<UserSearchModel>(x => x
                .AutoMap()
                .Properties(p => p
                    .Date(k => k
                            .Name("Created")
                    )
                    .Object<UserBalanceSearchModel>(ob => ob
                        .Name("UserBalance")
                        .AutoMap()
                        .Properties(pb => pb
                            .Number(n => n
                                    .Name("Amount")
                                    .Type(NumberType.Double)
                            )
                        )
                    )
                )
            )
        );
    }
}