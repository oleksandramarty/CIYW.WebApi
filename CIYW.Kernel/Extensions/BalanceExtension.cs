using CIYW.Const.Enum;
using CIYW.Domain.Models.User;

namespace CIYW.Kernel.Extensions;

public static class BalanceExtension
{
    public static void AddInvoice(this UserBalance balance, InvoiceTypeEnum type, decimal amount)
    {
        balance.UpdateBalance(type, amount);
        balance.UpdateBalanceTime();
    }
    
    public static void UpdateInvoice(this UserBalance balance, InvoiceTypeEnum type, decimal amount, InvoiceTypeEnum updatedType, decimal updatedAmount)
    {
        balance.UpdateBalance(type, amount, -1);
        balance.UpdateBalance(updatedType, updatedAmount);
        balance.UpdateBalanceTime();
    }
    
    public static void DeleteInvoice(this UserBalance balance, InvoiceTypeEnum type, decimal amount)
    {
        balance.UpdateBalance(type, amount, -1);
        balance.UpdateBalanceTime();
    }
    
    private static void UpdateBalance(this UserBalance balance, InvoiceTypeEnum type, decimal amount, int sign = 1)
    {
        balance.Amount = balance.Amount + (sign) *
            (type == InvoiceTypeEnum.Income ? amount : (-1) * amount);
    }

    private static void UpdateBalanceTime(this UserBalance balance)
    {
        balance.Updated = DateTime.UtcNow;
    }
}