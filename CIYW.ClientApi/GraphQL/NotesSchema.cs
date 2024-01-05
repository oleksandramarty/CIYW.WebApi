using GraphQL.Types;

namespace CIYW.ClientApi.GraphQL;

public class NotesSchema: Schema
{
    public NotesSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Query = serviceProvider.GetRequiredService<NotesQuery>();
    }
}