using System.ComponentModel;

namespace CIYW.Const.Enum;

public enum EntityTypeEnum
{
    [Description("Пользователь")]User = 1,
    [Description("Сессия")]Session = 2,
    [Description("Generic")]Generic = 2,
}