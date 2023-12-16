namespace CIYW.Models.Responses.Dictionary;

public class DictionaryResponse
{
    public IList<DictionaryItemResponse> Currencies { get; set; }
    public IList<DictionaryItemResponse> Categories { get; set; }
    public IList<DictionaryItemResponse> Roles { get; set; }
    public IList<DictionaryItemResponse> Tariffs { get; set; }
}