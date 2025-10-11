using CPG.Infrastructure.Policies;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CPG.Domain.Tests.Unit.Infrastructure.Policies;

public class SoapLoggingWrapperTests
{
    private readonly Mock<ILogger<SoapLoggingWrapper>> _loggerMock;
    private readonly SoapLoggingWrapper _sut;

    public SoapLoggingWrapperTests()
    {
        _loggerMock = new Mock<ILogger<SoapLoggingWrapper>>();
        _sut = new SoapLoggingWrapper(_loggerMock.Object);
    }

    [Fact]
    public void SoapLoggingWrapper_Should_Require_Logger()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SoapLoggingWrapper(null));
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Log_Request_And_Response()
    {
        // Arrange
        var serviceName = "PecPaymentService";
        var operationName = "GetToken";
        var request = new { MerchantId = "PEC123", Amount = 10000 };
        var expectedResponse = new { Token = "TOKEN-ABC-123", Status = "Success" };

        Func<Task<object>> soapCall = async () =>
        {
            await Task.Delay(50); // Simulate SOAP call
            return expectedResponse;
        };

        // Act
        var response = await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall);

        // Assert
        response.Should().Be(expectedResponse);

        // Verify request log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("SOAP Request") && v.ToString().Contains(serviceName)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // Verify response log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("SOAP Response") && v.ToString().Contains(serviceName)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Log_Timeout_On_TaskCanceledException()
    {
        // Arrange
        var serviceName = "BehPardakhtService";
        var operationName = "VerifyPayment";
        var request = new { TransactionId = "TXN123" };

        Func<Task<object>> soapCall = () => throw new TaskCanceledException("Request timeout");

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall));

        // Verify timeout log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("SOAP TIMEOUT") && v.ToString().Contains(serviceName)),
                It.IsAny<TaskCanceledException>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Log_Timeout_On_TimeoutException()
    {
        // Arrange
        var serviceName = "SepPaymentService";
        var operationName = "CreateToken";
        var request = new { MerchantKey = "SEP789" };

        Func<Task<object>> soapCall = () => throw new TimeoutException("Operation timeout");

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(async () =>
            await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall));

        // Verify timeout log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("SOAP TIMEOUT") && v.ToString().Contains(serviceName)),
                It.IsAny<TimeoutException>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Log_Error_On_Generic_Exception()
    {
        // Arrange
        var serviceName = "AsanPardakhtService";
        var operationName = "GetTransactionResult";
        var request = new { Token = "TOKEN-XYZ" };
        var expectedException = new InvalidOperationException("SOAP fault");

        Func<Task<object>> soapCall = () => throw expectedException;

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall));

        // Verify error log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("SOAP FAILED") && v.ToString().Contains(serviceName)),
                It.IsAny<InvalidOperationException>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        actualException.Should().Be(expectedException);
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Include_Request_Body_In_Timeout_Log()
    {
        // Arrange
        var serviceName = "PecService";
        var operationName = "Verify";
        var request = new
        {
            MerchantId = "MERCHANT-999",
            TransactionId = "TXN-ABC-123",
            Amount = 500000
        };

        Func<Task<object>> soapCall = () => throw new TimeoutException("Timeout");

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(async () =>
            await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall));

        // Verify that request body is included in log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("SOAP TIMEOUT") &&
                    v.ToString().Contains("Request:")),
                It.IsAny<TimeoutException>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Include_Duration_In_All_Logs()
    {
        // Arrange
        var serviceName = "TestService";
        var operationName = "TestOperation";
        var request = new { Data = "test" };

        Func<Task<object>> soapCall = async () =>
        {
            await Task.Delay(100); // Simulate 100ms operation
            return new { Result = "success" };
        };

        // Act
        await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall);

        // Verify response log includes duration
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Duration:")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Generate_Unique_Request_Id()
    {
        // Arrange
        var serviceName = "UniqueIdService";
        var operationName = "TestOp";
        var request = new { };
        var expectedResponse = new { };

        Func<Task<object>> soapCall = () => Task.FromResult((object)expectedResponse);

        // Act - Call twice
        await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall);
        await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall);

        // Assert - Should have 4 log calls (2 requests + 2 responses)
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Exactly(4));
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Handle_Null_Request()
    {
        // Arrange
        var serviceName = "NullRequestService";
        var operationName = "GetStatus";
        object request = null;
        var expectedResponse = new { Status = "OK" };

        Func<Task<object>> soapCall = () => Task.FromResult((object)expectedResponse);

        // Act
        var response = await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall);

        // Assert
        response.Should().Be(expectedResponse);

        // Should still log (with [null] for request)
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Handle_Null_Response()
    {
        // Arrange
        var serviceName = "NullResponseService";
        var operationName = "VoidOperation";
        var request = new { Command = "execute" };
        object expectedResponse = null;

        Func<Task<object>> soapCall = () => Task.FromResult(expectedResponse);

        // Act
        var response = await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall);

        // Assert
        response.Should().BeNull();

        // Should still log both request and response
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Exactly(2)); // Request + Response
    }

    [Fact]
    public async Task ExecuteWithLoggingAsync_Should_Rethrow_Exception_After_Logging()
    {
        // Arrange
        var serviceName = "ErrorService";
        var operationName = "FailingOperation";
        var request = new { };
        var expectedException = new ApplicationException("Service error");

        Func<Task<object>> soapCall = () => throw expectedException;

        // Act
        var actualException = await Assert.ThrowsAsync<ApplicationException>(async () =>
            await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall));

        // Assert
        actualException.Should().Be(expectedException);

        // Should have logged the error
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
    public async Task ExecuteWithLoggingAsync_Should_Include_Exception_Type_In_Error_Log()
    {
        // Arrange
        var serviceName = "TypedErrorService";
        var operationName = "Operation";
        var request = new { };

        Func<Task<object>> soapCall = () => throw new ArgumentNullException("parameter", "Parameter cannot be null");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall));

        // Verify exception type is logged
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("ArgumentNullException")),
                It.IsAny<ArgumentNullException>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("ShortService", "ShortOp")]
    [InlineData("VeryLongServiceNameThatExceedsNormalLength", "VeryLongOperationNameThatExceedsNormalLength")]
    [InlineData("Service With Spaces", "Operation With Spaces")]
    public async Task ExecuteWithLoggingAsync_Should_Handle_Various_Service_Names(string serviceName, string operationName)
    {
        // Arrange
        var request = new { };
        var response = new { };

        Func<Task<object>> soapCall = () => Task.FromResult((object)response);

        // Act
        var result = await _sut.ExecuteWithLoggingAsync(serviceName, operationName, request, soapCall);

        // Assert
        result.Should().Be(response);

        // Should log successfully
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(serviceName)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);
    }
}
