using NUnit.Framework;

namespace CIYW.IntegrationTests;

public class CommonIntegrationTestSetup: IDisposable 
{
    protected HttpClient Client { get; set; }

    protected IntegrationTestBase testApplicationFactory;
    
    private bool withClaims { get; }

    public CommonIntegrationTestSetup(bool withClaims = true)
    {
        this.withClaims = withClaims;
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        this.testApplicationFactory = new IntegrationTestBase(this.withClaims);
        this.Client = this.testApplicationFactory.CreateClient();
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        this.Dispose();
    }
    
    public void Dispose()
    {
        Client.Dispose();
    }
}