namespace CIYW.Models.Responses.Dictionary;

public class DictionariesResponse
{
    public DictionaryResponse<Guid> Currencies { get; set; }
    public DictionaryResponse<Guid> Categories { get; set; }
    public DictionaryResponse<Guid> Roles { get; set; }
    public DictionaryResponse<Guid> Tariffs { get; set; }
    public DictionaryResponse<string> InvoiceTypes { get; set; }
}