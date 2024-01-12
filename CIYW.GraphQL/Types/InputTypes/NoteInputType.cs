using GraphQL.Types;

namespace CIYW.GraphQL.Types.InputTypes;

public class NoteInputType: InputObjectGraphType
{
    public NoteInputType()
    {
        Name = "NoteInput";
        Field<NonNullGraphType<StringGraphType>>("name");
        Field<NonNullGraphType<StringGraphType>>("body");
        Field<IdGraphType>("invoiceId");
    }
}