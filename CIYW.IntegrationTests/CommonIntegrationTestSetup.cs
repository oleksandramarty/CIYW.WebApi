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

    protected IntegrationTestBase testApplicationFactory;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        this.testApplicationFactory = new IntegrationTestBase();
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