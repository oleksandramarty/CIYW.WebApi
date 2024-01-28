using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Const.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityTypeEnum
{
    [Description("User")]USER = 1,
    [Description("Tariff")]TARIFF = 1,
}