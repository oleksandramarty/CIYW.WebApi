namespace CIYW.MongoDB;

public class MongoDbSettings: IMongoDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseNameImages { get; set; }
    public string CollectionNameImages { get; set; }
}