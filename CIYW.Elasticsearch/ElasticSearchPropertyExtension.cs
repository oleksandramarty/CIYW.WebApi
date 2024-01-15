using System.Linq.Expressions;
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
}