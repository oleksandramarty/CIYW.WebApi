using System.Linq.Expressions;
using AutoMapper;
using CIYW.Elasticsearch;
using CIYW.Elasticsearch.Models.Users;
using CIYW.Interfaces.Strategies;
using CIYW.Kernel.Extensions;
using CIYW.Mediator.Mediator.Users.Requests;
using CIYW.Models.Helpers.Base;
using CIYW.Models.Requests.Common;
using CIYW.Models.Responses.Users;
using Nest;

namespace CIYW.Repositories.Strategies;

public class UsersPageableStrategy: IPageableStrategy<UserResponse, UsersQuery>
{
    private readonly IElasticClient elasticClient;
    private readonly IMapper mapper;

    public UsersPageableStrategy(IElasticClient elasticClient, IMapper mapper)
    {
        this.elasticClient = elasticClient;
        this.mapper = mapper;
    }

    public async Task<ListWithIncludeHelper<UserResponse>> GetPageableAsync(UsersQuery filter, CancellationToken cancellationToken)
    {
        int skip = (filter.Paginator.PageNumber - 1) * filter.Paginator.PageSize;
        
        var result = await this.elasticClient.SearchAsync<UserSearchModel>(s => s
            .Query(q => q
                .Bool(b => this.GetDescriptor(b, filter))
            )
            .Sort(s => this.CreateSortDescriptor(filter.Sort))
            .From(skip)
            .Size(filter.Paginator.PageSize)
        );


        if (!result.IsValid)
        {
            return new ListWithIncludeHelper<UserResponse>
            {
                Entities = new List<UserResponse>(),
                Paginator = filter.Paginator,
                TotalCount = 0
            };
        }
        
        var total = await this.elasticClient.CountAsync<UserSearchModel>(s => s
            .Query(q => q
                .Bool(b => this.GetDescriptor(b, filter))
            ), cancellationToken);

        List<UserSearchModel> entities = result.Documents.ToList();

        return new ListWithIncludeHelper<UserResponse>
        {
            Entities = entities.Select(x => this.mapper.Map<UserSearchModel, UserResponse>(x)).ToList(),
            Paginator = filter.Paginator,
            TotalCount = (int)total.Count
        };
    }
    
    private SortDescriptor<UserSearchModel> CreateSortDescriptor(BaseSortableQuery? filter)
    {
        if (filter == null || filter.Column.NullOrEmpty())
        {
            return null;
        }

        SortDescriptor<UserSearchModel> sort = new SortDescriptor<UserSearchModel>();

        switch (filter.Column)
        {
            case nameof(UserSearchModel.Created):
                return filter.Direction == "asc" ? sort.Ascending(t => t.Created) : sort.Descending(t => t.Created);
            case nameof(UserSearchModel.UserBalance.Amount):
                return filter.Direction == "asc" ? sort.Ascending(t => t.UserBalance.Amount) : sort.Descending(t => t.UserBalance.Amount);
            default:
                return null;
        }
    }

    private BoolQueryDescriptor<UserSearchModel> GetDescriptor(BoolQueryDescriptor<UserSearchModel> t,
        UsersQuery filter)
    {
        t.Must(m => m.ApplyDateRangeFilter<UserSearchModel, DateTime>(r => r.Created, filter.CreatedRange))
            .Must(m => m.ApplyDateRangeFilter<UserSearchModel, DateTime?>(r => r.Updated, filter.UpdatedRange))
            .Should(m =>
            {
                if (filter.Phone.NotNullOrEmpty())
                {
                    m.Match(mq =>
                        mq.Field(f => f.PhoneNumber).Query($"*{filter.Phone}*").Fuzziness(Fuzziness.Auto));
                }
                
                if (filter.Email.NotNullOrEmpty())
                {
                    m.Match(mq =>
                        mq.Field(f => f.Email).Query($"*{filter.Email}*").Fuzziness(Fuzziness.Auto));
                }
                
                if (filter.Login.NotNullOrEmpty())
                {
                    m.Match(mq =>
                        mq.Field(f => f.Login).Query($"*{filter.Login}*").Fuzziness(Fuzziness.Auto));
                }
                
                if (filter.Name.NotNullOrEmpty())
                {
                    m.Bool(b => b
                        .Should(
                            bs => bs.Match(mq => mq.Field(f => f.LastName).Query($"*{filter.Name}*").Fuzziness(Fuzziness.Auto)),
                            bs => bs.Match(mq => mq.Field(f => f.FirstName).Query($"*{filter.Name}*").Fuzziness(Fuzziness.Auto))
                        )
                    );
                }
                
                if (filter.IsBlocked.HasValue)
                {
                    m.Bool(b => b
                        .Must(
                            m.Term(t => t.Field(f => f.IsBlocked).Value(filter.IsBlocked.Value))
                        )
                    );
                }

                return m;
            })
            .Must(m => m.ApplyIdsFilter<UserSearchModel>(u => u.UserBalance.CurrencyId, filter.CurrencyIds))
            .Must(m => m.ApplyIdsFilter<UserSearchModel>(u => u.RoleId, filter.RoleIds))
            .Must(m => m.ApplyIdsFilter<UserSearchModel>(u => u.TariffId, filter.TariffIds));

        return t;
    }
}