namespace CIYW.Elasticsearch.Models.Currency;

public class CurrencySearchModel
{
    public Guid Id { get; set; }
    public string IsoCode { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
}