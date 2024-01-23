using CIYW.Const.Enums;

namespace CIYW.MongoDB.Models.Images;

public class ImageData
{
    public ImageData(Guid entityId, FileTypeEnum type, string name, byte[] data)
    {
        Id = Guid.NewGuid();
        EntityId = entityId;
        Type = type;
        Name = name;
        Data = data;

    }

    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public FileTypeEnum Type { get; set; }
    public string Name { get; set; }
    public byte[] Data { get; set; }
}