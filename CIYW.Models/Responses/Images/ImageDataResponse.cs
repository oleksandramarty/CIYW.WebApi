using CIYW.Const.Enums;

namespace CIYW.Models.Responses.Images;

public class ImageDataResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public FileTypeEnum Type { get; set; }
    public byte[] Data { get; set; }
}