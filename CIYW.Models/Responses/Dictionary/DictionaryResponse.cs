namespace CIYW.Models.Responses.Dictionary;

public class DictionaryResponse<TId>
{
    public DictionaryResponse(IList<DictionaryItemResponse<TId>> items)
    {
        Items = items;
    }
    public IList<DictionaryItemResponse<TId>> Items { get; set; }
}