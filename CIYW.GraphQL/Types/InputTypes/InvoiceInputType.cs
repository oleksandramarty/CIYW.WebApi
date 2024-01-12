using CIYW.Const.Enums;
using GraphQL.Types;

namespace CIYW.GraphQL.Types.InputTypes;

public class InvoiceInputType: InputObjectGraphType
{
    public InvoiceInputType()
    {
        Name = "InvoiceInput";
        Field<NonNullGraphType<StringGraphType>>("name");
        Field<NonNullGraphType<DecimalGraphType>>("amount");
        Field<NonNullGraphType<IdGraphType>>("categoryId");
        Field<NonNullGraphType<IdGraphType>>("currencyId");
        Field<NonNullGraphType<DateTimeGraphType>>("date");
        Field<NonNullGraphType<EnumerationGraphType<InvoiceTypeEnum>>>("type");
        Field<NoteInputType>("note");
    }
}