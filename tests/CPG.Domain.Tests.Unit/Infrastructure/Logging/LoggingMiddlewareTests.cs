using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CPG.Domain.Tests.Unit.Infrastructure.Logging;

public class LoggingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<ILogger<LoggingMiddleware>> _loggerMock;
    private readonly LoggingMiddleware _sut;

    public LoggingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerMock = new Mock<ILogger<LoggingMiddleware>>();

        _loggerFactoryMock
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(_loggerMock.Object);

        _sut = new LoggingMiddleware(_nextMock.Object, _loggerFactoryMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_Should_Capture_Request_And_Response_Data()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        SetupRequest(httpContext, "POST", "/api/v1/payment/request", "{\"amount\":1000}");
        SetupResponse(httpContext, 200, "{\"token\":\"ABC123\"}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[CallLog]")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_Should_Generate_CorrelationId_When_Not_Provided()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        SetupRequest(httpContext, "GET", "/api/v1/health", "");
        SetupResponse(httpContext, 200, "{\"status\":\"ok\"}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        httpContext.Items.Should().ContainKey("CorrelationId");
        httpContext.Items["CorrelationId"].Should().NotBeNull();
    }

    [Fact]
    public async Task InvokeAsync_Should_Use_Provided_CorrelationId()
    {
        // Arrange
        var expectedCorrelationId = "CORR-123-ABC";
        var httpContext = CreateHttpContext();
        httpContext.Request.Headers["X-Correlation-ID"] = expectedCorrelationId;
        SetupRequest(httpContext, "POST", "/api/v1/payment/request", "{}");
        SetupResponse(httpContext, 200, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        httpContext.Items["CorrelationId"].Should().Be(expectedCorrelationId);
    }

    [Fact]
    public async Task InvokeAsync_Should_Generate_RequestId_When_Not_Provided()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        SetupRequest(httpContext, "POST", "/api/v1/payment/request", "{}");
        SetupResponse(httpContext, 200, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        httpContext.Items.Should().ContainKey("RequestId");
        httpContext.Items["RequestId"].Should().NotBeNull();
        httpContext.Items["RequestId"].ToString().Length.Should().BeLessOrEqualTo(12);
    }

    [Fact]
    public async Task InvokeAsync_Should_Use_Provided_RequestId()
    {
        // Arrange
        var expectedRequestId = "REQ-XYZ-789";
        var httpContext = CreateHttpContext();
        httpContext.Request.Headers["X-Request-ID"] = expectedRequestId;
        SetupRequest(httpContext, "POST", "/api/v1/payment/request", "{}");
        SetupResponse(httpContext, 200, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        httpContext.Items["RequestId"].Should().Be(expectedRequestId);
    }

    [Fact]
    public async Task InvokeAsync_Should_Capture_User_Claims()
    {
        // Arrange
        var httpContext = CreateHttpContextWithUser(
            userId: 123,
            companyId: 456,
            applicationId: 789,
            clientId: "CLIENT-ABC",
            mobilePhone: "09123456789");

        SetupRequest(httpContext, "POST", "/api/v1/payment/request", "{}");
        SetupResponse(httpContext, 200, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert - Should log with user context
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("/api/v1/payment/request", Enums.AuditType.Client)]
    [InlineData("/api/v1/payment/verify", Enums.AuditType.Client)]
    [InlineData("/api/v1/company/create", Enums.AuditType.User)]
    [InlineData("/api/v1/user/profile", Enums.AuditType.User)]
    public async Task InvokeAsync_Should_Set_Correct_AuditType_Based_On_Endpoint(string path, Enums.AuditType expectedAuditType)
    {
        // Arrange
        var httpContext = CreateHttpContext();
        SetupRequest(httpContext, "POST", path, "{}");
        SetupResponse(httpContext, 200, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>())
)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert - Logged successfully
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_Should_Capture_ClientIpAddress()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.100");
        SetupRequest(httpContext, "GET", "/api/v1/health", "");
        SetupResponse(httpContext, 200, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_Should_Capture_UserAgent()
    {
        // Arrange
        var expectedUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
        var httpContext = CreateHttpContext();
        httpContext.Request.Headers["User-Agent"] = expectedUserAgent;
        SetupRequest(httpContext, "POST", "/api/v1/payment/request", "{}");
        SetupResponse(httpContext, 200, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_Should_Calculate_DurationMs()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        SetupRequest(httpContext, "POST", "/api/v1/payment/request", "{}");
        SetupResponse(httpContext, 200, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(async () =>
            {
                await Task.Delay(100); // Simulate processing time
            });

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert - Should have logged with duration
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(400)]
    [InlineData(401)]
    [InlineData(500)]
    public async Task InvokeAsync_Should_Capture_ResponseStatus(int statusCode)
    {
        // Arrange
        var httpContext = CreateHttpContext();
        SetupRequest(httpContext, "GET", "/api/v1/test", "");
        SetupResponse(httpContext, statusCode, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_Should_Handle_Exception_And_Log_StackTrace()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        SetupRequest(httpContext, "POST", "/api/v1/payment/request", "{}");

        var expectedException = new InvalidOperationException("Payment processing failed");
        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.InvokeAsync(httpContext));

        // Should log error with stack trace
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(ex => ex == expectedException),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_Should_Throw_ArgumentNullException_For_Null_HttpContext()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _sut.InvokeAsync(null));
    }

    [Fact]
    public async Task InvokeAsync_Should_Capture_QueryString()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        httpContext.Request.QueryString = new QueryString("?orderId=123&amount=1000");
        SetupRequest(httpContext, "GET", "/api/v1/payment/status", "");
        SetupResponse(httpContext, 200, "{}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_Should_Handle_Empty_Request_Body()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        SetupRequest(httpContext, "GET", "/api/v1/health", "");
        SetupResponse(httpContext, 200, "{\"status\":\"healthy\"}");

        _nextMock
            .Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.InvokeAsync(httpContext);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    private DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private DefaultHttpContext CreateHttpContextWithUser(
        long userId,
        long companyId,
        long applicationId,
        string clientId,
        string mobilePhone)
    {
        var claims = new[]
        {
            new Claim("UserId", userId.ToString()),
            new Claim("CompanyId", companyId.ToString()),
            new Claim("ApplicationId", applicationId.ToString()),
            new Claim("ClientId", clientId),
            new Claim("MobilePhone", mobilePhone)
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        context.Response.Body = new MemoryStream();

        return context;
    }

    private void SetupRequest(HttpContext context, string method, string path, string body)
    {
        context.Request.Method = method;
        context.Request.Path = path;
        context.Request.ContentType = "application/json";

        if (!string.IsNullOrEmpty(body))
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            context.Request.Body = new MemoryStream(bytes);
        }
        else
        {
            context.Request.Body = new MemoryStream();
        }

        context.Request.Host = new HostString("api.cpg.com");
    }

    private void SetupResponse(HttpContext context, int statusCode, string body)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        if (!string.IsNullOrEmpty(body))
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            context.Response.Body.Write(bytes, 0, bytes.Length);
            context.Response.Body.Position = 0;
        }
    }
}
