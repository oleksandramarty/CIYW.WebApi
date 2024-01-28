using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Resources.Forms.Enum;

[Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
public enum FormItemInputDateTypeEnum
{
    [Description("String")]String = 1,
    [Description("Number")]Number = 2,
    [Description("Boolean")]Boolean = 3,
    [Description("Guid")]Guid = 4
}