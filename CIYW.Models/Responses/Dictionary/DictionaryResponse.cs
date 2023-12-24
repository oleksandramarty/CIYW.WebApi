namespace CIYW.Models.Responses.Dictionary;

public class DictionaryResponse
{
    public DictionaryResponse(IList<DictionaryItemResponse> items)
    {
        Items = items;
    }
    public IList<DictionaryItemResponse> Items { get; set; }
}