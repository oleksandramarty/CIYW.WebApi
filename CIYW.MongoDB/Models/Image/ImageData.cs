using CIYW.Const.Enums;

namespace CIYW.MongoDB.Models.Image;

public class ImageData
{
    public ImageData(Guid userId, FileTypeEnum type, byte[] data)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Type = type;
        Data = data;

    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public FileTypeEnum Type { get; set; }
    public byte[] Data { get; set; }
}