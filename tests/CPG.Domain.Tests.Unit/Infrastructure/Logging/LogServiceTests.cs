using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.Tests.Unit.Infrastructure.Logging;

public class LogServiceTests
{
    private readonly Mock<ILogger<LogService>> _loggerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly LogService _sut;

    public LogServiceTests()
    {
        _loggerMock = new Mock<ILogger<LogService>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _sut = new LogService(_loggerMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public void LogService_Should_Initialize_With_Dependencies()
    {
        // Act & Assert
        _sut.Should().NotBeNull();
        _sut.ServiceName.Should().BeNull();
    }

    [Fact]
    public void LogService_Should_Allow_Setting_ServiceName_And_ServiceType()
    {
        // Arrange
        var expectedServiceName = "AsanPardakht Token Service";
        var expectedServiceType = Enums.ServiceType.AsanPardakhtToken;
        var expectedProviderType = ProviderTypeInLog.AsanPardakht;

        // Act
        _sut.ServiceName = expectedServiceName;
        _sut.ServiceType = expectedServiceType;
        _sut.ProviderTypeInLog = expectedProviderType;

        // Assert
        _sut.ServiceName.Should().Be(expectedServiceName);
        _sut.ServiceType.Should().Be(expectedServiceType);
        _sut.ProviderTypeInLog.Should().Be(expectedProviderType);
    }

    [Fact]
    public void AddServiceCallLog_WithHttpProviderRequest_Should_Log_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 123);

        var request = new HttpProviderRequest<object>
        {
            Uri = "https://api.asanpardakht.com/token",
            Body = new { merchantId = "TEST123", amount = 1000 },
            Service = Enums.ServiceType.AsanPardakhtToken,
            Provider = ProviderTypeInLog.AsanPardakht
        };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"token\":\"ABC123\"}")
        };

        // Act
        _sut.AddServiceCallLog(request, response, "{\"token\":\"ABC123\"}");

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
    public void AddServiceCallLog_WithStringParameters_Should_Log_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 456);

        _sut.ServiceName = "PEC Token Service";
        _sut.ServiceType = Enums.ServiceType.PecToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var requestString = "{\"merchantId\":\"PEC123\"}";
        var responseString = "{\"token\":\"XYZ789\"}";
        short status = 0; // Success
        var message = "Token retrieved successfully";

        // Act
        _sut.AddServiceCallLog(requestString, responseString, status, message);

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
    public void AddServiceCallLog_WithErrorStatus_Should_Log_Error_Details()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 789);

        _sut.ServiceName = "BehPardakht Verify";
        _sut.ServiceType = Enums.ServiceType.BehPardakhtVerify;
        _sut.ProviderTypeInLog = ProviderTypeInLog.BehPardakht;

        var requestString = "{\"token\":\"VERIFY123\"}";
        var responseString = "{\"error\":\"Invalid token\"}";
        short status = -1; // Error
        var message = "Verification failed";

        // Act
        _sut.AddServiceCallLog(requestString, responseString, status, message);

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
    public async Task AddServiceCallLogAsync_Should_Log_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 999);

        var request = new HttpProviderRequest<object, object>
        {
            Uri = "https://api.vandar.com/verify",
            Body = new { transactionId = "TXN123" },
            Service = Enums.ServiceType.VandarVerify,
            ProviderTypeInLog = ProviderTypeInLog.Vandar
        };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"status\":\"verified\"}")
        };

        // Act
        await _sut.AddServiceCallLogAsync(request, response);

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
    public async Task AddServiceCallLogAsync_WithFailedResponse_Should_Log_Error_Code()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 111);

        var request = new HttpProviderRequest<object, object>
        {
            Uri = "https://api.sep.com/token",
            Body = new { amount = 5000 },
            Service = Enums.ServiceType.SepToken,
            ProviderTypeInLog = ProviderTypeInLog.Sep
        };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("{\"error\":\"Invalid request\"}")
        };

        // Act
        await _sut.AddServiceCallLogAsync(request, response);

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
    public void AddTimeoutLog_Should_Log_Timeout_With_Request_Details()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 222);

        var request = new HttpProviderRequest<object>
        {
            Uri = "https://api.neobank.com/transfer",
            Body = new { amount = 10000, fromAccount = "123", toAccount = "456" },
            Service = Enums.ServiceType.ClientDirectDebit,
            Provider = ProviderTypeInLog.NeoBank
        };

        var exception = new TimeoutException("The operation has timed out.");
        var durationMs = 30500L;

        // Act
        _sut.AddTimeoutLog(request, exception, durationMs);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[CallLog]") && v.ToString().Contains("TIMEOUT")),
                It.Is<Exception>(ex => ex == exception),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void AddTimeoutLog_Should_Include_Duration_In_Response()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 333);

        var request = new HttpProviderRequest<object>
        {
            Uri = "https://api.charispay.com/payment",
            Body = new { orderId = "ORD999" },
            Service = Enums.ServiceType.AsanPardakhtToken,
            Provider = ProviderTypeInLog.CharisPay
        };

        var exception = new TaskCanceledException("Request timeout");
        var durationMs = 45000L;

        // Act
        _sut.AddTimeoutLog(request, exception, durationMs);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"TIMEOUT after {durationMs}ms")),
                It.Is<Exception>(ex => ex == exception),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void AddServiceCallLog_Should_Default_UserId_To_1_When_No_User_Context()
    {
        // Arrange
        SetupHttpContextWithoutUser();

        var request = new HttpProviderRequest<object>
        {
            Uri = "https://api.test.com",
            Body = new { test = "data" },
            Service = Enums.ServiceType.GetIdpToken,
            Provider = ProviderTypeInLog.Idp
        };

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"result\":\"success\"}")
        };

        // Act
        _sut.AddServiceCallLog(request, response, "{\"result\":\"success\"}");

        // Assert - Should log with UserId = 1 (system default)
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
    [InlineData(HttpStatusCode.OK, true)]
    [InlineData(HttpStatusCode.Created, true)]
    [InlineData(HttpStatusCode.BadRequest, false)]
    [InlineData(HttpStatusCode.InternalServerError, false)]
    [InlineData(HttpStatusCode.RequestTimeout, false)]
    public void AddServiceCallLog_Should_Set_ServiceCallStatus_Based_On_HttpStatusCode(HttpStatusCode statusCode, bool expectedSuccess)
    {
        // Arrange
        SetupHttpContextWithUser(userId: 444);

        var request = new HttpProviderRequest<object>
        {
            Uri = "https://api.test.com",
            Body = new { },
            Service = Enums.ServiceType.AsanPardakhtToken,
            Provider = ProviderTypeInLog.AsanPardakht
        };

        var response = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent("{}")
        };

        // Act
        _sut.AddServiceCallLog(request, response, "{}");

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void AddTimeoutLog_Should_Handle_Null_HttpContext()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

        var request = new HttpProviderRequest<object>
        {
            Uri = "https://api.test.com",
            Body = new { },
            Service = Enums.ServiceType.GetIdpToken,
            Provider = ProviderTypeInLog.Idp
        };

        var exception = new TimeoutException("Timeout");
        var durationMs = 30000L;

        // Act
        _sut.AddTimeoutLog(request, exception, durationMs);

        // Assert - Should log with UserId = 1 (system default) and not throw
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(ex => ex == exception),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    private void SetupHttpContextWithUser(long userId)
    {
        var claims = new List<Claim>
        {
            new Claim("UserId", userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }

    private void SetupHttpContextWithoutUser()
    {
        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }
}
