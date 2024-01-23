using CIYW.Const.Enums;
using CIYW.Models.Responses.Base;

namespace CIYW.Models.Responses.Images;

public class ImageDataResponse: BaseEntityResponse
{
    public Guid EntityId { get; set; }
    public FileTypeEnum Type { get; set; }
    public string Name { get; set; }
    public byte[] Data { get; set; }
}