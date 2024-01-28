using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Resources.Forms.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FormTypeEnum
{
    [Description("AuthLogin")]AUTH_LOGIN = 1,
}