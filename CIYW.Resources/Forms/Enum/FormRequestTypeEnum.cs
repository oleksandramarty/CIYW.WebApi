using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Resources.Forms.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FormRequestTypeEnum
{
    [Description("AuthLoginQuery")]AuthLoginQuery = 1,
}