using System.Linq.Expressions;
using CIYW.Models.Requests.Common;
using Nest;

namespace CIYW.Elasticsearch;

public static class ElasticSearchPropertyExtension
{
    public static PropertiesDescriptor<T> AddText<T, TValue>(
        this PropertiesDescriptor<T> pt, 
        Expression<Func<T, TValue>> objectPath,
        string name = "keyword",
        int ignoreAbove = 256) where T: class
    {
        return pt.Text(t => t
            .Name(objectPath)
            .Fields(f => f.Keyword(k => k.Name(name).IgnoreAbove(ignoreAbove)))
        );
    }
    
    public static PropertiesDescriptor<T> AddId<T, TValue>(
        this PropertiesDescriptor<T> pt, 
        Expression<Func<T, TValue>> objectPath,
        bool index = false) where T: class
    {
        return pt.Text(t => t
            .Name(objectPath)
            .Index(index)
        );
    }
    
    public static PropertiesDescriptor<T> AddDate<T, TValue>(
        this PropertiesDescriptor<T> pt, 
        Expression<Func<T, TValue>> objectPath,
        string format = "strict_date_optional_time||epoch_millis") where T: class
    {
        return pt.Date(d => d
            .Name(objectPath)
            .Format(format)
        );
    }
    
    public static PropertiesDescriptor<T> AddNumber<T, TValue>(
        this PropertiesDescriptor<T> pt, 
        Expression<Func<T, TValue>> objectPath,
        string format = "strict_date_optional_time||epoch_millis") where T: class
    {
        return pt.Number(n => n
            .Name(objectPath)
            .Type(NumberType.Double)
        );
    }
    
    public static PropertiesDescriptor<T> AddRef<T, TValue>(
        this PropertiesDescriptor<T> pt, 
        Expression<Func<T, TValue>> objectPath,
        string format = "strict_date_optional_time||epoch_millis") where T: class
    {
        return pt.Keyword(k => k
            .Name(objectPath)
        );
    }
    
    public static PropertiesDescriptor<T> AddObject<T, TValue, TObject>(
        this PropertiesDescriptor<T> pt, 
        Expression<Func<TObject, TValue>> objectPath,
        string fieldName,
        string name = "keyword",
        int ignoreAbove = 256) 
        where T: class
        where TObject: class
    {
        return pt.Object<TObject>(o => o
            .Name(fieldName)
            .Properties(cp => cp
                .Text(ct => ct
                    .Name(objectPath)
                    .Fields(f => f.Keyword(k => k.Name(name).IgnoreAbove(ignoreAbove)))
                )
            )
        );
    }
    
    public static QueryContainerDescriptor<T> ApplyDateRangeFilter<T, TDateTime>(this QueryContainerDescriptor<T> m, Expression<Func<T, TDateTime>> objectPath, BaseDateRangeQuery? range) where T: class
    {
        if (range != null && (range.DateFrom.HasValue || range.DateTo.HasValue))
        {
            if (range.DateFrom.HasValue && range.DateTo.HasValue)
            {
                m.DateRange(r => r.Field(objectPath)
                    .GreaterThanOrEquals(range.DateFrom.Value)
                    .LessThanOrEquals(range.DateTo.Value)
                );
                return m;
            }
            if (range.DateFrom.HasValue && !range.DateTo.HasValue)
            {
                m.DateRange(r => r.Field(objectPath)
                    .GreaterThanOrEquals(range.DateFrom.Value)
                );
                return m;
            }
            if (!range.DateFrom.HasValue && range.DateTo.HasValue)
            {
                m.DateRange(r => r.Field(objectPath)
                    .LessThanOrEquals(range.DateTo.Value)
                );
                return m;
            }
        }

        return m;
    }

    public static QueryContainerDescriptor<T> ApplyIdsFilter<T>(this QueryContainerDescriptor<T> m,
        Expression<Func<T, Guid>> objectPath, BaseIdsListQuery? query) where T : class
    {
        if (query == null)
        {
            return m;
        }
        
        if (query.Ids.Any())
        {
            m.Bool(b => b
                .Must(
                    m.Term(t => t.Field(objectPath).Value(query.Ids.First()))
                )
            );
        }

        return m;
    }
}