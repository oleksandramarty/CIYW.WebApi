using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Resources.Forms.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FormItemInputType
{
    [Description("select")]Select = 1,
    [Description("date")]Date = 2,
    [Description("daterange")]Daterange = 3,
    [Description("checkbox")]Checkbox = 4,
    [Description("radio")]Radio = 5,
    [Description("text")]Text = 6,
    [Description("password")]Password = 7,
}