using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Resources.Forms.Enum;

[Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
public enum FormItemValidatorTypeEnum
{
    [Description("MinLen")]MinLen = 1,
    [Description("MaxLen")]MaxLen = 2,
    [Description("Required")]Required = 3,
    [Description("Min")]Min = 4,
    [Description("Max")]Max = 5,
}