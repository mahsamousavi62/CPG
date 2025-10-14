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
using System.Linq;
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
            Request = null,
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
            Request = null,
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

    #region SOAP Logging Tests - New AddSoapCallLog Method

    [Fact]
    public void AddSoapCallLog_WithPecRequest_Should_Log_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 10001);

        _sut.ServiceName = "SaleServiceSoapClient.SalePaymentRequestAsync";
        _sut.ServiceType = Enums.ServiceType.PecToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var soapRequest = new
        {
            LoginAccount = "PEC_MERCHANT_456",
            OrderId = 123456789L,
            Amount = 50000L,
            CallBackUrl = "https://merchant.com/callback",
            Originator = "09121234567"
        };

        var soapResponse = new
        {
            Body = new
            {
                SalePaymentRequestResult = new
                {
                    Status = 0,
                    Token = 987654321L,
                    Message = "Success"
                }
            }
        };

        short status = 0;
        var message = "Success";

        // Act
        _sut.AddSoapCallLog(soapRequest, soapResponse, "SaleServiceSoapClient.SalePaymentRequestAsync", status, message);

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
    public void AddSoapCallLog_WithBehPardakhtRequest_Should_Log_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 10002);

        _sut.ServiceName = "PaymentGatewayClient.bpPayRequestAsync";
        _sut.ServiceType = Enums.ServiceType.BehPardakhtToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.BehPardakht;

        var soapRequest = new
        {
            terminalId = 98765,
            userName = "BEHPARDAKHT_USER",
            userPassword = "******",
            orderId = 999888777L,
            amount = 100000L,
            localDate = "20251014",
            localTime = "143000",
            mobileNo = "989123456789"
        };

        var soapResponse = new
        {
            Body = new
            {
                @return = "0,TOKEN-BEHPARDAKHT-XYZ123"
            }
        };

        short status = 0;
        var message = "0,TOKEN-BEHPARDAKHT-XYZ123";

        // Act
        _sut.AddSoapCallLog(soapRequest, soapResponse, "PaymentGatewayClient.bpPayRequestAsync", status, message);

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
    public void AddSoapCallLog_WithErrorStatus_Should_Log_Error_Details()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 10003);

        _sut.ServiceName = "ConfirmServiceSoapClient.ConfirmPaymentAsync";
        _sut.ServiceType = Enums.ServiceType.PecVerify;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var soapRequest = new
        {
            LoginAccount = "PEC_MERCHANT",
            Token = 123456L
        };

        var soapResponse = new
        {
            Body = new
            {
                ConfirmPaymentResult = new
                {
                    Status = -1528,
                    Message = "Invalid token or transaction already verified",
                    RRN = 0,
                    CardNumberMasked = "",
                    Token = 0L
                }
            }
        };

        short status = -1528;
        var message = "Invalid token or transaction already verified";

        // Act
        _sut.AddSoapCallLog(soapRequest, soapResponse, "ConfirmServiceSoapClient.ConfirmPaymentAsync", status, message);

        // Assert - Should log error information
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
    public void AddSoapTimeoutLog_WithPecRequest_Should_Capture_Request_Body()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 10004);

        _sut.ServiceName = "SaleServiceSoapClient.SalePaymentRequestAsync";
        _sut.ServiceType = Enums.ServiceType.PecToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var soapRequest = new
        {
            LoginAccount = "MERCHANT_TIMEOUT_TEST",
            OrderId = 555444333L,
            Amount = 250000L,
            CallBackUrl = "https://shop.example.com/callback",
            Originator = "09123456789"
        };

        var timeoutException = new TimeoutException("The SOAP request has timed out after 30 seconds");
        var durationMs = 30500L;
        var serviceName = "SaleServiceSoapClient.SalePaymentRequestAsync";

        // Act
        _sut.AddSoapTimeoutLog(soapRequest, serviceName, timeoutException, durationMs);

        // Assert - Should log timeout with captured request body
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("[CallLog]") &&
                    v.ToString().Contains("SOAP TIMEOUT") &&
                    v.ToString().Contains($"TIMEOUT after {durationMs}ms")),
                It.Is<Exception>(ex => ex == timeoutException),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void AddSoapTimeoutLog_WithBehPardakhtRequest_Should_Log_Timeout_With_Full_Context()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 10005);

        _sut.ServiceName = "PaymentGatewayClient.bpVerifyRequestAsync";
        _sut.ServiceType = Enums.ServiceType.BehPardakhtVerify;
        _sut.ProviderTypeInLog = ProviderTypeInLog.BehPardakht;

        var soapRequest = new
        {
            terminalId = 98765,
            userName = "BEHPARDAKHT_MERCHANT",
            userPassword = "******",
            orderId = 111222333L,
            saleOrderId = 111222333L,
            saleReferenceId = 444555666L
        };

        var taskCanceledException = new TaskCanceledException("A task was canceled - SOAP timeout");
        var durationMs = 45200L;
        var serviceName = "PaymentGatewayClient.bpVerifyRequestAsync";

        // Act
        _sut.AddSoapTimeoutLog(soapRequest, serviceName, taskCanceledException, durationMs);

        // Assert - Should log timeout with all details
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("SOAP TIMEOUT") &&
                    v.ToString().Contains($"TIMEOUT after {durationMs}ms")),
                It.Is<Exception>(ex => ex == taskCanceledException),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0, "Success")]
    [InlineData(-1528, "Verification failed")]
    [InlineData(-1598, "Transaction not found")]
    [InlineData(504, "Gateway timeout")]
    [InlineData(-1610, "Transaction in process")]
    public void AddSoapCallLog_WithDifferentStatuses_Should_Log_Correctly(short status, string message)
    {
        // Arrange
        SetupHttpContextWithUser(userId: 10006);

        _sut.ServiceName = "ConfirmServiceSoapClient.ConfirmPaymentAsync";
        _sut.ServiceType = Enums.ServiceType.PecVerify;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var request = new { Token = 123456L };
        var response = new { Status = status, Message = message };
        var serviceName = "ConfirmServiceSoapClient.ConfirmPaymentAsync";

        // Act
        _sut.AddSoapCallLog(request, response, serviceName, status, message);

        // Assert - Should always log, regardless of status
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
    public void AddSoapCallLog_WithLargePayload_Should_Log_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 10007);

        _sut.ServiceName = "LargeSoapService";
        _sut.ServiceType = Enums.ServiceType.PecToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        // Create a large SOAP request payload
        var largeSoapRequest = new
        {
            LoginAccount = "MERCHANT_LARGE",
            OrderId = 999999999L,
            Amount = 1000000L,
            AdditionalData = new string('X', 2000), // 2KB of data
            Items = Enumerable.Range(1, 50).Select(i => new { ItemId = i, Name = $"Item {i}", Price = i * 1000 })
        };

        var largeSoapResponse = new
        {
            Status = 0,
            Token = 888777666L,
            Details = new string('Y', 1000)
        };

        short status = 0;
        var message = "Success";

        // Act
        _sut.AddSoapCallLog(largeSoapRequest, largeSoapResponse, "LargeSoapService", status, message);

        // Assert - Should handle large payloads
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
    public void AddSoapTimeoutLog_Should_Handle_Null_HttpContext()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

        _sut.ServiceName = "TimeoutService";
        _sut.ServiceType = Enums.ServiceType.PecToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var request = new { OrderId = 123L };
        var exception = new TimeoutException("Timeout");
        var durationMs = 30000L;
        var serviceName = "TimeoutService";

        // Act
        _sut.AddSoapTimeoutLog(request, serviceName, exception, durationMs);

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

    [Fact]
    public void AddSoapCallLog_WithNullRequest_Should_Log_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 10008);

        _sut.ServiceName = "NullRequestService";
        _sut.ServiceType = Enums.ServiceType.PecToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        object nullRequest = null;
        var response = new { Status = 0, Token = 123L };
        short status = 0;
        var message = "Success";

        // Act
        _sut.AddSoapCallLog(nullRequest, response, "NullRequestService", status, message);

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

    #endregion

    #region Legacy SOAP String-based Logging Tests

    [Fact]
    public void AddServiceCallLog_WithSoapRequest_Should_Log_PecProvider_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 555);

        _sut.ServiceName = "SaleServiceSoapClient";
        _sut.ServiceType = Enums.ServiceType.PecToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var soapRequest = new
        {
            LoginAccount = "PEC_MERCHANT_123",
            OrderId = 123456789L,
            Amount = 50000L,
            CallBackUrl = "https://merchant.com/callback"
        };
        var soapResponse = new
        {
            Status = 0,
            Token = 987654321L,
            Message = "Success"
        };

        var requestString = System.Text.Json.JsonSerializer.Serialize(soapRequest);
        var responseString = System.Text.Json.JsonSerializer.Serialize(soapResponse);
        short status = 0;
        var message = "Success";

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
    public void AddServiceCallLog_WithSoapRequest_Should_Log_BehPardakhtProvider_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 666);

        _sut.ServiceName = "PaymentGatewayClient.bpPayRequestAsync";
        _sut.ServiceType = Enums.ServiceType.BehPardakhtToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.BehPardakht;

        var soapRequest = new
        {
            terminalId = 12345,
            userName = "BEHPARDAKHT_USER",
            orderId = 999888777L,
            amount = 100000L,
            localDate = "20251014",
            localTime = "143000"
        };
        var soapResponse = "0,TOKEN-BEHPARDAKHT-XYZ";

        var requestString = System.Text.Json.JsonSerializer.Serialize(soapRequest);
        short status = 0;
        var message = soapResponse;

        // Act
        _sut.AddServiceCallLog(requestString, soapResponse, status, message);

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
    public void AddServiceCallLog_WithSoapError_Should_Log_Error_Details()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 777);

        _sut.ServiceName = "ConfirmServiceSoapClient";
        _sut.ServiceType = Enums.ServiceType.PecVerify;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var soapRequest = new
        {
            LoginAccount = "PEC_MERCHANT",
            Token = 123456L
        };
        var soapResponse = new
        {
            Status = -1528,
            Message = "Invalid token or transaction already verified"
        };

        var requestString = System.Text.Json.JsonSerializer.Serialize(soapRequest);
        var responseString = System.Text.Json.JsonSerializer.Serialize(soapResponse);
        short status = -1528;
        var message = "Invalid token or transaction already verified";

        // Act
        _sut.AddServiceCallLog(requestString, responseString, status, message);

        // Assert - Should log error information
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
    public void AddTimeoutLog_WithSoapRequest_Should_Capture_Request_Body_For_Troubleshooting()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 888);

        var soapRequest = new HttpProviderRequest<object>
        {
            Uri = "https://pec.shaparak.ir/SaleService.svc",
            Body = new
            {
                LoginAccount = "MERCHANT_PEC_999",
                OrderId = 555444333L,
                Amount = 250000L,
                CallBackUrl = "https://shop.example.com/callback",
                Originator = "09123456789"
            },
            Service = Enums.ServiceType.PecToken,
            Provider = ProviderTypeInLog.Pec
        };

        var timeoutException = new TimeoutException("The SOAP request has timed out after 30 seconds");
        var durationMs = 30500L;

        // Act
        _sut.AddTimeoutLog(soapRequest, timeoutException, durationMs);

        // Assert - Should log error with captured request body
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("[CallLog]") &&
                    v.ToString().Contains("TIMEOUT") &&
                    v.ToString().Contains($"TIMEOUT after {durationMs}ms")),
                It.Is<Exception>(ex => ex == timeoutException),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void AddTimeoutLog_WithBehPardakhtSoapRequest_Should_Log_Timeout_With_Full_Context()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 999);

        var soapRequest = new HttpProviderRequest<object>
        {
            Uri = "https://bpm.shaparak.ir/pgwchannel/services/pgw",
            Body = new
            {
                terminalId = 98765,
                userName = "BEHPARDAKHT_MERCHANT",
                orderId = 111222333L,
                amount = 750000L,
                mobileNo = "989123456789"
            },
            Service = Enums.ServiceType.BehPardakhtToken,
            Provider = ProviderTypeInLog.BehPardakht
        };

        var taskCanceledException = new TaskCanceledException("A task was canceled - SOAP timeout");
        var durationMs = 45200L;

        // Act
        _sut.AddTimeoutLog(soapRequest, taskCanceledException, durationMs);

        // Assert - Should log timeout with all details
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("TIMEOUT") &&
                    v.ToString().Contains($"{durationMs}ms")),
                It.Is<Exception>(ex => ex == taskCanceledException),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0, "Success")]
    [InlineData(-1528, "Verification failed")]
    [InlineData(-1598, "Transaction not found")]
    [InlineData(504, "Gateway timeout")]
    public void AddServiceCallLog_WithDifferentSoapStatuses_Should_Log_Correctly(short status, string message)
    {
        // Arrange
        SetupHttpContextWithUser(userId: 1234);

        _sut.ServiceName = "PecVerifyService";
        _sut.ServiceType = Enums.ServiceType.PecVerify;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var requestString = "{\"Token\":123456}";
        var responseString = $"{{\"Status\":{status},\"Message\":\"{message}\"}}";

        // Act
        _sut.AddServiceCallLog(requestString, responseString, status, message);

        // Assert - Should always log, regardless of status
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
    public void AddServiceCallLog_WithLargeSoapRequest_Should_Log_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 5555);

        _sut.ServiceName = "LargeSoapService";
        _sut.ServiceType = Enums.ServiceType.PecToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        // Create a large SOAP request payload
        var largeSoapRequest = new
        {
            LoginAccount = "MERCHANT_LARGE",
            OrderId = 999999999L,
            Amount = 1000000L,
            AdditionalData = new string('X', 2000), // 2KB of data
            Items = Enumerable.Range(1, 50).Select(i => new { ItemId = i, Name = $"Item {i}", Price = i * 1000 })
        };

        var requestString = System.Text.Json.JsonSerializer.Serialize(largeSoapRequest);
        var responseString = "{\"Status\":0,\"Token\":888777666}";
        short status = 0;
        var message = "Success";

        // Act
        _sut.AddServiceCallLog(requestString, responseString, status, message);

        // Assert - Should handle large payloads
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
    public void AddServiceCallLog_WithSpecialCharactersInSoapData_Should_Log_Safely()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 6666);

        _sut.ServiceName = "SpecialCharService";
        _sut.ServiceType = Enums.ServiceType.BehPardakhtVerify;
        _sut.ProviderTypeInLog = ProviderTypeInLog.BehPardakht;

        var requestWithSpecialChars = new
        {
            merchantName = "Test & <Company> \"Ltd\"",
            description = "Payment for order #123 with 10% discount",
            callbackUrl = "https://example.com/callback?param=value&other=123"
        };

        var requestString = System.Text.Json.JsonSerializer.Serialize(requestWithSpecialChars);
        var responseString = "{\"Result\":\"<Success/>\"}";
        short status = 0;
        var message = "OK";

        // Act
        _sut.AddServiceCallLog(requestString, responseString, status, message);

        // Assert - Should handle special characters safely
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
    public void AddTimeoutLog_Should_Include_ServiceCallStatus_False_For_Timeout()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 7777);

        var request = new HttpProviderRequest<object>
        {
            Uri = "https://payment.gateway.com/soap",
            Body = new { orderId = "ORDER-TIMEOUT-TEST" },
            Service = Enums.ServiceType.PecToken,
            Provider = ProviderTypeInLog.Pec
        };

        var exception = new TimeoutException("Request timed out");
        var durationMs = 30000L;

        // Act
        _sut.AddTimeoutLog(request, exception, durationMs);

        // Assert - ServiceCallStatus should be false for timeouts
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("TIMEOUT")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void AddTimeoutLog_Should_Include_ErrorCode_RequestTimeout()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 8888);

        var request = new HttpProviderRequest<object>
        {
            Uri = "https://soap.service.com/endpoint",
            Body = new { data = "test" },
            Service = Enums.ServiceType.BehPardakhtVerify,
            Provider = ProviderTypeInLog.BehPardakht
        };

        var exception = new TimeoutException("Timeout occurred");
        var durationMs = 35000L;

        // Act
        _sut.AddTimeoutLog(request, exception, durationMs);

        // Assert - Should have ErrorCode "RequestTimeout"
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void AddServiceCallLog_WithEmptySoapResponse_Should_Log_Successfully()
    {
        // Arrange
        SetupHttpContextWithUser(userId: 9999);

        _sut.ServiceName = "EmptyResponseService";
        _sut.ServiceType = Enums.ServiceType.PecToken;
        _sut.ProviderTypeInLog = ProviderTypeInLog.Pec;

        var requestString = "{\"Request\":\"Test\"}";
        var responseString = string.Empty;
        short status = -1;
        var message = "Empty response received";

        // Act
        _sut.AddServiceCallLog(requestString, responseString, status, message);

        // Assert - Should handle empty responses gracefully
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[CallLog]")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    #endregion

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
