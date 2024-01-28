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
        CreateIndex(client, "usersIntegrationTests");
    }
    
    private static void CreateIndex(IElasticClient client, string indexName)
    {
        var createIndexResponse = client.Indices.Create(indexName, index => index
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(1)
                .Analysis(an => an
                    .Analyzers(a => a
                        .Custom("partial_match_analyzer", ca => ca
                            .Tokenizer("partial_match_tokenizer")
                            .Filters("lowercase")
                        )
                    )
                    .Tokenizers(t => t
                        .NGram("partial_match_tokenizer", ts => ts
                            .MinGram(2)
                            .MaxGram(20)  // Adjust max gram based on your requirements
                            .TokenChars(TokenChar.Letter, TokenChar.Digit)
                        )
                    )
                )
            )
            .Map<UserSearchModel>(x => x
                .AutoMap()
                .Properties(p => p
                    .Text(t => t
                        .Name(u => u.Login)
                        .Analyzer("partial_match_analyzer")
                    )
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