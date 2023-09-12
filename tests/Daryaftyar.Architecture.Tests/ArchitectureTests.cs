using NetArchTest.Rules;

namespace Daryaftyar.Architecture.Tests;

public class ArchitectureTests
{
    #region Namespaces

    private const string DomainNamespace = "Daryaftyar.Domain";
    private const string ApplicationNamespace = "Daryaftyar.Application";
    private const string InfrastructureNamespace = "Daryaftyar.Infrastructure";
    private const string InfrastructureAuthorizationNamespace = "Daryaftyar.Infrastructure.Authorization";
    private const string InfrastructurePersistenceNamespace = "Daryaftyar.Infrastructure.Persistence";
    private const string WebNamespace = "Daryaftyar.API";
    private const string PresentationNamespace = "Daryaftyar.Presentation";

    #endregion

    #region Dependency Checking

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        //Arrange 
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        var otherProjcets = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            InfrastructureAuthorizationNamespace,
            InfrastructurePersistenceNamespace,
            PresentationNamespace,
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
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var otherProjcets = new[]
        {
            InfrastructureNamespace,
            InfrastructureAuthorizationNamespace,
            InfrastructurePersistenceNamespace,
            PresentationNamespace,
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
        var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

        var otherProjcets = new[]
        {
            PresentationNamespace,
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
        var assembly = typeof(Infrastructure.Authorization.AssemblyReference).Assembly;

        var otherProjcets = new[]
        {
            PresentationNamespace,
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
        var assembly = typeof(Infrastructure.Persistence.AssemblyReference).Assembly;

        var otherProjcets = new[]
        {
            PresentationNamespace,
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
    public void Presentation_Should_Not_HaveDependencyOnOtherProjects()
    {
        //Arrange 
        var assembly = typeof(Presentation.AssemblyReference).Assembly;

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
    public void Controllers_Should_HaveDependencyOnMediatR()
    {
        //Arrange 
        var assembly = typeof(Presentation.AssemblyReference).Assembly;

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
        var assembly = typeof(Application.AssemblyReference).Assembly;

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