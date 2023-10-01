using NetArchTest.Rules;

namespace CPG.Architecture.Tests;

public class ArchitectureTests
{
    #region Namespaces

    private const string DomainNamespace = "CPG.Domain";
    private const string ApplicationNamespace = "CPG.Application";
    private const string InfrastructureNamespace = "CPG.Infrastructure";
    private const string InfrastructureAuthorizationNamespace = "CPG.Infrastructure.Authorization";
    private const string InfrastructurePersistenceNamespace = "CPG.Infrastructure.Persistence";
    private const string WebNamespace = "CPG.API";

    #endregion

    #region Dependency Checking

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        //Arrange 
        var assembly = typeof(Domain.DomainAssembly).Assembly;

        var otherProjcets = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            InfrastructureAuthorizationNamespace,
            InfrastructurePersistenceNamespace,
            WebNamespace,
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjcets)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        //Arrange 
        var assembly = typeof(Application.ApplicationAssembly).Assembly;

        var otherProjcets = new[]
        {
            InfrastructureNamespace,
            InfrastructureAuthorizationNamespace,
            InfrastructurePersistenceNamespace,
            WebNamespace,
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjcets)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        //Arrange 
        var assembly = typeof(Infrastructure.InfrastructureAssembly).Assembly;

        var otherProjcets = new[]
        {
            WebNamespace,
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjcets)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    [Fact]
    public void InfrastructureAuthorization_Should_Not_HaveDependencyOnOtherProjects()
    {
        //Arrange 
        var assembly = typeof(Infrastructure.Authorization.InfrastructureAuthorizationAssembly).Assembly;

        var otherProjcets = new[]
        {
            WebNamespace,
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjcets)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }
    
    [Fact]
    public void InfrastructurePersistence_Should_Not_HaveDependencyOnOtherProjects()
    {
        //Arrange 
        var assembly = typeof(Infrastructure.Persistence.InfrastructurePersistenceAssembly).Assembly;

        var otherProjcets = new[]
        {
            WebNamespace,
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjcets)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOnMediatR()
    {
        //Arrange 
        var assembly = typeof(API.WebApiAssembly).Assembly;

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    [Fact]
    public void Handlers_Should_HaveDependencyOnDomain()
    {
        //Arrange 
        var assembly = typeof(Application.ApplicationAssembly).Assembly;

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    #endregion
}