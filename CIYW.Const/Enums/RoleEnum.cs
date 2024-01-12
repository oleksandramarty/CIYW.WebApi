using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CIYW.Const.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleEnum
{
    [Description("SuperAdministratorNabix")]
    USER = 1,

    [Description("AdministratorNabix")]
    ADMIN = 2,
}