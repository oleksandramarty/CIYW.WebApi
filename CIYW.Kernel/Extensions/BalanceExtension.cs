using CIYW.Const.Enum;
using CIYW.Const.Errors;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.User;
using CIYW.Kernel.Exceptions;

namespace CIYW.Kernel.Extensions;

public static class BalanceExtension
{
    public static void AddInvoice(this UserBalance balance, Invoice invoice)
    {
        balance.UpdateBalance(invoice);
        balance.UpdateBalanceTime();
    }
    
    public static void UpdateInvoice(this UserBalance balance, Invoice invoice, Invoice updatedInvoice)
    {
        balance.UpdateBalance(invoice, -1);
        balance.UpdateBalance(updatedInvoice);
        balance.UpdateBalanceTime();
    }
    
    public static void DeleteInvoice(this UserBalance balance, Invoice invoice)
    {
        balance.UpdateBalance(invoice, -1);
        balance.UpdateBalanceTime();
    }
    
    private static void UpdateBalance(this UserBalance balance, Invoice invoice, int sign = 1)
    {
        balance.Amount = balance.Amount + (sign) *
            (invoice.Type == InvoiceTypeEnum.Income ? invoice.Amount : (-1) * invoice.Amount);
    }

    private static void UpdateBalanceTime(this UserBalance balance)
    {
        balance.Updated = DateTime.UtcNow;
    }
}