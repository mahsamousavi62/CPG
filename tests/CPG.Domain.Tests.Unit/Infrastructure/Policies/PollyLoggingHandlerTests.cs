using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Policies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CPG.Domain.Tests.Unit.Infrastructure.Policies;

public class PollyLoggingHandlerTests
{
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<HttpMessageHandler> _innerHandlerMock;
    private readonly PollyLoggingHandler _sut;
    private readonly HttpClient _httpClient;

    public PollyLoggingHandlerTests()
    {
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _innerHandlerMock = new Mock<HttpMessageHandler>();

        _sut = new PollyLoggingHandler(_auditLogServiceMock.Object, _httpContextAccessorMock.Object)
        {
            InnerHandler = _innerHandlerMock.Object
        };

        _httpClient = new HttpClient(_sut);
    }

    [Fact]
    public void PollyLoggingHandler_Should_Require_AuditLogService()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PollyLoggingHandler(null, _httpContextAccessorMock.Object));
    }

    [Fact]
    public async Task SendAsync_Should_Pass_Through_Successful_Response()
    {
        // Arrange
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"token\":\"ABC123\"}")
        };

        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.test.com/token");

        // Act
        var response = await _httpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("ABC123");

        // LogProviderCall should not be called for successful responses (no errors)
        _auditLogServiceMock.Verify(
            x => x.LogProviderCall(It.IsAny<ProviderCallLog>()),
            Times.Never);
    }

    [Fact]
    public async Task SendAsync_Should_Log_Timeout_On_TaskCanceledException()
    {
        // Arrange
        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Request timeout"));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.test.com/payment")
        {
            Content = new StringContent("{\"amount\":1000}")
        };

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _httpClient.SendAsync(request));

        // Verify LogProviderCall was called for timeout
        _auditLogServiceMock.Verify(
            x => x.LogProviderCall(It.Is<ProviderCallLog>(log => log.IsTimeout == true)),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_Should_Log_Timeout_On_OperationCanceledException()
    {
        // Arrange
        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException("Operation timeout"));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.test.com/verify")
        {
            Content = new StringContent("{\"transactionId\":\"TXN123\"}")
        };

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _httpClient.SendAsync(request));

        // Verify LogProviderCall was called for timeout
        _auditLogServiceMock.Verify(
            x => x.LogProviderCall(It.Is<ProviderCallLog>(log => log.IsTimeout == true)),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_Should_NOT_Log_Timeout_When_User_Cancelled()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel(); // User cancellation

        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("User cancelled"));

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.test.com/status");

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _httpClient.SendAsync(request, cts.Token));

        // LogProviderCall should NOT be called for user cancellation
        _auditLogServiceMock.Verify(
            x => x.LogProviderCall(It.IsAny<ProviderCallLog>()),
            Times.Never);
    }

    [Fact]
    public async Task SendAsync_Should_Measure_Duration_For_Timeout()
    {
        // Arrange
        long? capturedDuration = null;
        _auditLogServiceMock
            .Setup(x => x.LogProviderCall(It.IsAny<ProviderCallLog>()))
            .Callback<ProviderCallLog>(log => capturedDuration = log.DurationMs);

        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(async () =>
            {
                await Task.Delay(100); // Simulate some processing time
                throw new TaskCanceledException("Timeout");
            });

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.test.com/long-operation");

        // Act
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _httpClient.SendAsync(request));

        // Assert
        capturedDuration.Should().NotBeNull();
        capturedDuration.Should().BeGreaterThanOrEqualTo(100); // At least 100ms
    }

    [Fact]
    public async Task SendAsync_Should_Capture_Request_Body_For_Timeout_Log()
    {
        // Arrange
        string capturedRequestBody = null;
        _auditLogServiceMock
            .Setup(x => x.LogProviderCall(It.IsAny<ProviderCallLog>()))
            .Callback<ProviderCallLog>(log => capturedRequestBody = log.RequestBody);

        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Timeout"));

        var requestBody = "{\"amount\":5000,\"orderId\":\"ORD-999\"}";
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.test.com/payment")
        {
            Content = new StringContent(requestBody)
        };

        // Act
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _httpClient.SendAsync(request));

        // Assert
        capturedRequestBody.Should().NotBeNullOrEmpty();
        capturedRequestBody.Should().Contain("amount");
    }

    [Fact]
    public async Task SendAsync_Should_Rethrow_Exception_After_Logging()
    {
        // Arrange
        var expectedException = new TaskCanceledException("Request timeout");

        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(expectedException);

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.test.com");

        // Act
        var actualException = await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _httpClient.SendAsync(request));

        // Assert
        actualException.Should().Be(expectedException);
    }

    [Fact]
    public async Task SendAsync_Should_Handle_Request_Without_Content()
    {
        // Arrange
        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Timeout"));

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.test.com/status");
        // No content

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _httpClient.SendAsync(request));

        // Should still log provider call for timeout
        _auditLogServiceMock.Verify(
            x => x.LogProviderCall(It.Is<ProviderCallLog>(log => log.IsTimeout == true)),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_Should_NOT_Interfere_With_Other_Exceptions()
    {
        // Arrange
        var expectedException = new HttpRequestException("Network error");

        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(expectedException);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.test.com/payment");

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await _httpClient.SendAsync(request));

        // Should NOT log timeout for non-timeout exceptions
        _auditLogServiceMock.Verify(
            x => x.LogProviderCall(It.IsAny<ProviderCallLog>()),
            Times.Never);

        actualException.Should().Be(expectedException);
    }

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Created)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task SendAsync_Should_Pass_Through_All_Status_Codes(HttpStatusCode statusCode)
    {
        // Arrange
        var expectedResponse = new HttpResponseMessage(statusCode);

        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.test.com");

        // Act
        var response = await _httpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(statusCode);

        // Should not log provider call for successful responses
        _auditLogServiceMock.Verify(
            x => x.LogProviderCall(It.IsAny<ProviderCallLog>()),
            Times.Never);
    }
}
