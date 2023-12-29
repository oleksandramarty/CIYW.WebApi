using System.Security.Claims;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CIYW.Auth;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using CIYW.Mediator;
using CIYW.Repositories;
using CIYW.UnitTests;
using CYIW.Mapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using Xunit;

namespace CIYW.IntegrationTests;

public class CommonIntegrationTestSetup: IDisposable 
{
    protected HttpClient Client { get; set; }
    protected DataContext dbContext;
    protected User mockUser;

    protected IntegrationTestBase testApplicationFactory;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        this.mockUser = MockHelper.GetMockUser();
        this.testApplicationFactory = new IntegrationTestBase(mockUser);
        this.Client = this.testApplicationFactory.CreateClient();
        using var scope = this.testApplicationFactory.Services.CreateScope();
        this.dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        this.mockUser = MockHelper.GetMockUser();
        this.mockUser.PhoneNumber = "21215476981";
        this.mockUser.CurrencyId = InitConst.CurrencyUsdId;
        this.mockUser.Currency = null;
        this.mockUser.TariffId = InitConst.FreeTariffId;
        this.mockUser.Tariff = null;
        this.mockUser.UserBalance.CurrencyId = InitConst.CurrencyUsdId;
        
        SetUpDatabase();
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        this.dbContext.Database.EnsureDeleted();
        this.Dispose();
    }
    
    public void Dispose()
    {
        Client.Dispose();
    }

    private void SetUpDatabase()
    {
        var Users = this.dbContext.Users.ToList();
        DbInitializer.Initialize(this.dbContext, false);
        List<IdentityUserLogin<Guid>> logins = this.mockUser.CreateUserLogins();
        this.dbContext.Users.Add(mockUser);
        this.dbContext.UserLogins.AddRange(logins);
        this.dbContext.SaveChanges();
    }
}