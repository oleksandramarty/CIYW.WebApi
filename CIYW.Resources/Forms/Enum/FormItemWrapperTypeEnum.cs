using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Resources.Forms.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FormItemWrapperTypeEnum
{
    [Description("Full")]Full = 1,
    [Description("Half")]Half = 2,
    [Description("Third")]Third = 3,
    [Description("Quarter")]Quarter = 4
}