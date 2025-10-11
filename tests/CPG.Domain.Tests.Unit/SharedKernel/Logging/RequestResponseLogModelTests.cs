using CPG.Domain.SharedKernel.Logging;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.Tests.Unit.SharedKernel.Logging;

public class RequestResponseLogModelTests
{
    [Fact]
    public void RequestResponseLogModel_Should_Initialize_With_Default_Values()
    {
        // Act
        var logModel = new RequestResponseLogModel();

        // Assert
        logModel.Should().NotBeNull();
        logModel.UserAgent.Should().BeNull();
        logModel.IP.Should().BeNull();
        logModel.Host.Should().BeNull();
        logModel.ServiceName.Should().BeNull();
        logModel.RequestMethod.Should().BeNull();
        logModel.RequestBody.Should().BeNull();
        logModel.RequestQueryString.Should().BeNull();
        logModel.ResponseStatus.Should().BeNull();
        logModel.ResponseBody.Should().BeNull();
        logModel.CompanyId.Should().BeNull();
        logModel.ApplicationId.Should().BeNull();
        logModel.UserId.Should().BeNull();
        logModel.ClientId.Should().BeNull();
        logModel.StackTrace.Should().BeNull();
        logModel.ErrorCode.Should().BeNull();
        logModel.RoutValues.Should().BeNull();
        logModel.MobilePhone.Should().BeNull();
    }

    [Fact]
    public void RequestResponseLogModel_Should_Set_All_Properties_For_Client_Request()
    {
        // Arrange
        var expectedUserAgent = "Mozilla/5.0";
        var expectedIP = "192.168.1.100";
        var expectedHost = "api.cpg.com";
        var expectedServiceName = "PaymentRequest";
        var expectedRequestTime = DateTime.UtcNow;
        var expectedRequestMethod = "POST";
        var expectedRequestBody = new { amount = 1000, orderId = "ORD123" };
        var expectedRequestQueryString = "?callback=https://merchant.com/callback";
        var expectedResponseTime = DateTime.UtcNow.AddSeconds(2);
        var expectedResponseStatus = "200";
        var expectedResponseBody = new { token = "TOKEN123", redirectUrl = "https://gateway.com" };
        var expectedAuditType = AuditType.Client;
        var expectedCompanyId = 456L;
        var expectedApplicationId = 789L;
        var expectedUserId = 123L;
        var expectedClientId = "client_123";
        var expectedIsSuccess = true;
        var expectedStartDateTime = DateTime.UtcNow;
        var expectedEndDateTime = DateTime.UtcNow.AddSeconds(2);
        var expectedDurationMs = 2000L;
        var expectedRoutValues = new KeyValuePair<string, object>[]
        {
            new("controller", "Payment"),
            new("action", "PaymentRequest")
        };

        // Act
        var logModel = new RequestResponseLogModel
        {
            UserAgent = expectedUserAgent,
            IP = expectedIP,
            Host = expectedHost,
            ServiceName = expectedServiceName,
            RequestTime = expectedRequestTime,
            RequestMethod = expectedRequestMethod,
            RequestBody = expectedRequestBody,
            RequestQueryString = expectedRequestQueryString,
            ResponseTime = expectedResponseTime,
            ResponseStatus = expectedResponseStatus,
            ResponseBody = expectedResponseBody,
            AuditType = expectedAuditType,
            CompanyId = expectedCompanyId,
            ApplicationId = expectedApplicationId,
            UserId = expectedUserId,
            ClientId = expectedClientId,
            IsSuccess = expectedIsSuccess,
            StartDateTime = expectedStartDateTime,
            EndDateTime = expectedEndDateTime,
            DurationMs = expectedDurationMs,
            RoutValues = expectedRoutValues
        };

        // Assert
        logModel.UserAgent.Should().Be(expectedUserAgent);
        logModel.IP.Should().Be(expectedIP);
        logModel.Host.Should().Be(expectedHost);
        logModel.ServiceName.Should().Be(expectedServiceName);
        logModel.RequestTime.Should().Be(expectedRequestTime);
        logModel.RequestMethod.Should().Be(expectedRequestMethod);
        logModel.RequestBody.Should().BeEquivalentTo(expectedRequestBody);
        logModel.RequestQueryString.Should().Be(expectedRequestQueryString);
        logModel.ResponseTime.Should().Be(expectedResponseTime);
        logModel.ResponseStatus.Should().Be(expectedResponseStatus);
        logModel.ResponseBody.Should().BeEquivalentTo(expectedResponseBody);
        logModel.AuditType.Should().Be(expectedAuditType);
        logModel.CompanyId.Should().Be(expectedCompanyId);
        logModel.ApplicationId.Should().Be(expectedApplicationId);
        logModel.UserId.Should().Be(expectedUserId);
        logModel.ClientId.Should().Be(expectedClientId);
        logModel.IsSuccess.Should().Be(expectedIsSuccess);
        logModel.StartDateTime.Should().Be(expectedStartDateTime);
        logModel.EndDateTime.Should().Be(expectedEndDateTime);
        logModel.DurationMs.Should().Be(expectedDurationMs);
        logModel.RoutValues.Should().BeEquivalentTo(expectedRoutValues);
    }

    [Theory]
    [InlineData(AuditType.Client)]
    [InlineData(AuditType.Provider)]
    [InlineData(AuditType.User)]
    [InlineData(AuditType.Develop)]
    public void RequestResponseLogModel_Should_Support_All_AuditTypes(AuditType auditType)
    {
        // Arrange & Act
        var logModel = new RequestResponseLogModel
        {
            AuditType = auditType
        };

        // Assert
        logModel.AuditType.Should().Be(auditType);
    }

    [Fact]
    public void RequestResponseLogModel_Should_Handle_User_Request_Without_Authentication()
    {
        // Arrange & Act
        var logModel = new RequestResponseLogModel
        {
            ServiceName = "Login",
            AuditType = AuditType.User,
            IP = "192.168.1.100",
            RequestMethod = "POST",
            IsSuccess = true
            // No CompanyId, ApplicationId, UserId, or ClientId set
        };

        // Assert
        logModel.AuditType.Should().Be(AuditType.User);
        logModel.CompanyId.Should().BeNull();
        logModel.ApplicationId.Should().BeNull();
        logModel.UserId.Should().BeNull();
        logModel.ClientId.Should().BeNull();
    }

    [Fact]
    public void RequestResponseLogModel_Should_Capture_Error_With_StackTrace()
    {
        // Arrange
        var expectedStackTrace = "at CPG.Application.Handler.Handle() in Handler.cs:line 42";
        var expectedErrorCode = "PAYMENT_001";

        // Act
        var logModel = new RequestResponseLogModel
        {
            IsSuccess = false,
            StackTrace = expectedStackTrace,
            ErrorCode = expectedErrorCode,
            ResponseStatus = "500",
            ResponseBody = "Internal server error occurred"
        };

        // Assert
        logModel.IsSuccess.Should().BeFalse();
        logModel.StackTrace.Should().Be(expectedStackTrace);
        logModel.ErrorCode.Should().Be(expectedErrorCode);
        logModel.ResponseStatus.Should().Be("500");
    }

    [Fact]
    public void RequestResponseLogModel_Should_Calculate_Duration_From_Timestamps()
    {
        // Arrange
        var startDateTime = new DateTime(2025, 10, 11, 10, 0, 0);
        var endDateTime = new DateTime(2025, 10, 11, 10, 0, 5); // 5 seconds later
        var expectedDurationMs = 5000L;

        // Act
        var logModel = new RequestResponseLogModel
        {
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            DurationMs = expectedDurationMs
        };

        // Assert
        logModel.StartDateTime.Should().Be(startDateTime);
        logModel.EndDateTime.Should().Be(endDateTime);
        logModel.DurationMs.Should().Be(expectedDurationMs);

        // Verify the duration matches the timestamps
        var actualDuration = (endDateTime - startDateTime).TotalMilliseconds;
        logModel.DurationMs.Should().Be((long)actualDuration);
    }

    [Fact]
    public void RequestResponseLogModel_Should_Store_RouteValues_As_KeyValuePairs()
    {
        // Arrange
        var routeValues = new KeyValuePair<string, object>[]
        {
            new("controller", "Payment"),
            new("action", "PaymentRequest"),
            new("version", "1.0"),
            new("id", 123)
        };

        // Act
        var logModel = new RequestResponseLogModel
        {
            RoutValues = routeValues
        };

        // Assert
        logModel.RoutValues.Should().HaveCount(4);
        logModel.RoutValues.Should().Contain(kv => kv.Key == "controller" && kv.Value.ToString() == "Payment");
        logModel.RoutValues.Should().Contain(kv => kv.Key == "action" && kv.Value.ToString() == "PaymentRequest");
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("PATCH")]
    public void RequestResponseLogModel_Should_Support_All_HTTP_Methods(string httpMethod)
    {
        // Arrange & Act
        var logModel = new RequestResponseLogModel
        {
            RequestMethod = httpMethod
        };

        // Assert
        logModel.RequestMethod.Should().Be(httpMethod);
    }

    [Theory]
    [InlineData("200", true)]
    [InlineData("201", true)]
    [InlineData("400", false)]
    [InlineData("401", false)]
    [InlineData("500", false)]
    public void RequestResponseLogModel_Should_Match_IsSuccess_With_ResponseStatus(string statusCode, bool isSuccess)
    {
        // Arrange & Act
        var logModel = new RequestResponseLogModel
        {
            ResponseStatus = statusCode,
            IsSuccess = isSuccess
        };

        // Assert
        logModel.ResponseStatus.Should().Be(statusCode);
        logModel.IsSuccess.Should().Be(isSuccess);
    }

    [Fact]
    public void RequestResponseLogModel_Should_Store_MobilePhone_For_Authentication()
    {
        // Arrange
        var expectedMobilePhone = "09123456789";

        // Act
        var logModel = new RequestResponseLogModel
        {
            MobilePhone = expectedMobilePhone,
            AuditType = AuditType.User
        };

        // Assert
        logModel.MobilePhone.Should().Be(expectedMobilePhone);
    }

    [Fact]
    public void RequestResponseLogModel_Should_Handle_Complex_RequestBody()
    {
        // Arrange
        var complexRequestBody = new
        {
            paymentRequest = new
            {
                amount = 1000000,
                orderId = "ORD-123-456",
                callbackUrl = "https://merchant.com/callback",
                additionalData = new Dictionary<string, object>
                {
                    { "customerId", 789 },
                    { "productIds", new[] { 1, 2, 3 } },
                    { "metadata", new { source = "mobile", version = "2.0" } }
                }
            }
        };

        // Act
        var logModel = new RequestResponseLogModel
        {
            RequestBody = complexRequestBody
        };

        // Assert
        logModel.RequestBody.Should().NotBeNull();
        logModel.RequestBody.Should().BeEquivalentTo(complexRequestBody);
    }

    [Fact]
    public void RequestResponseLogModel_Should_Handle_Complex_ResponseBody()
    {
        // Arrange
        var complexResponseBody = new
        {
            success = true,
            data = new
            {
                token = "TOKEN-ABC-123",
                redirectUrl = "https://gateway.com/pay",
                expiresAt = DateTime.UtcNow.AddMinutes(15)
            },
            metadata = new
            {
                transactionId = "TXN-999",
                timestamp = DateTime.UtcNow
            }
        };

        // Act
        var logModel = new RequestResponseLogModel
        {
            ResponseBody = complexResponseBody
        };

        // Assert
        logModel.ResponseBody.Should().NotBeNull();
        logModel.ResponseBody.Should().BeEquivalentTo(complexResponseBody);
    }
}
