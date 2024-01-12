using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Const.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityTypeEnum
{
    [Description("Tariff")]TARIFF = 1,
    [Description("Category")]CATEGORY = 2,
    [Description("Currency")]CURRENCY = 3,
    [Description("Role")]ROLE = 4,
    [Description("InvoiceType")]INVOICE_TYPE = 5,
}