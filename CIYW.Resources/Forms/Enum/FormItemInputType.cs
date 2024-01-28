using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Resources.Forms.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FormItemInputType
{
    [Description("Select")]Select = 1,
    [Description("Date")]Date = 2,
    [Description("Daterange")]Daterange = 3,
    [Description("Checkbox")]Checkbox = 4,
    [Description("Radio")]Radio = 5,
    [Description("Text")]Text = 6
}