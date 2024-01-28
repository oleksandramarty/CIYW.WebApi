using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Resources.Forms.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FormItemValidatorTypeEnum
{
    [Description("MinLen")]MinLen = 1,
    [Description("MaxLen")]MaxLen = 2,
    [Description("Required")]Required = 3,
    [Description("Min")]Min = 4,
    [Description("Max")]Max = 5,
    [Description("Email")]Email = 6,
    [Description("AtLeastOne")]AtLeastOne = 7,
    [Description("Pattern")]Pattern = 8,
}