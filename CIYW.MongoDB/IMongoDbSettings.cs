namespace CIYW.MongoDB;

public interface IMongoDbSettings
{
    string ConnectionString { get; set; }
    string DatabaseNameImages { get; set; }
    string CollectionNameImages { get; set; }
}