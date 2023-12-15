using System.ComponentModel;

namespace CIYW.Const.Enum;

public enum EntityTypeEnum
{
    [Description("Entity")]Entity = 1,
    [Description("User")]User = 2,
    [Description("Session")]Session = 3,
    [Description("Generic")]Generic = 4,
    [Description("Invoice")]Invoice = 5,
}