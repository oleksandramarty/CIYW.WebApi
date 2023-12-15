using System.ComponentModel;

namespace CIYW.Const.Enum;

public enum InvoiceTypeEnum
{
    [Description("Приход")]Income = 1,
    [Description("Расход")]Expense = 2,
}