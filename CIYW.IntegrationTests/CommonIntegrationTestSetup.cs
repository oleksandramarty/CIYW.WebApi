using CIYW.Domain.Initialization;
using NUnit.Framework;

namespace CIYW.IntegrationTests;

public class CommonIntegrationTestSetup: IDisposable 
{
    protected HttpClient Client { get; set; }

    protected IntegrationTestBase testApplicationFactory;
    
    private Guid? claimUserId { get; }

    public CommonIntegrationTestSetup()
    {
        this.claimUserId = InitConst.MockUserId;
    }

    public CommonIntegrationTestSetup(Guid? claimUserId)
    {
        this.claimUserId = claimUserId;
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        this.testApplicationFactory = new IntegrationTestBase(this.claimUserId);
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