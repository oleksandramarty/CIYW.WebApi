using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Const.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FileTypeEnum
{
    [Description("UserImage")]USER_IMAGE = 1,
    [Description("CategoryIco")]CATEGORY_ICO = 2,
}