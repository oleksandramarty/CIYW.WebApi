using CIYW.Const.Enums;

namespace CIYW.Models.Requests.Common;

public class BaseFileFilterQuery: BaseFilterQuery
{
    public FileTypeEnum Type { get; set; }
}