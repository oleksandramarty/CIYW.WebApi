using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Const.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InvoiceTypeEnum
{
    [Description("Income")]INCOME = 1,
    [Description("Expense")]EXPENSE = 2,
}