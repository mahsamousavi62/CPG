using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using FluentAssertions;
using System;
using Xunit;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.Tests.Unit.SharedKernel.Logging;

public class CallLogModelTests
{
    [Fact]
    public void CallLogModel_Should_Initialize_With_Default_Values()
    {
        // Act
        var callLog = new CallLogModel();

        // Assert
        callLog.Should().NotBeNull();
        callLog.ServiceType.Should().BeNull();
        callLog.AuditType.Should().BeNull();
        callLog.RequestBody.Should().BeNull();
        callLog.ServiceCallStatus.Should().BeNull();
        callLog.ServiceCallUrl.Should().BeNull();
        callLog.ResponseBody.Should().BeNull();
        callLog.ErrorCode.Should().BeNull();
        callLog.ErrorType.Should().BeNull();
        callLog.ProviderType.Should().BeNull();
        callLog.CorrolationId.Should().BeNull();
    }

    [Fact]
    public void CallLogModel_Should_Set_All_Properties_Correctly()
    {
        // Arrange
        var expectedServiceType = Enums.ServiceType.AsanPardakhtToken;
        var expectedAuditType = AuditType.Provider;
        var expectedRequestBody = "{\"amount\":1000}";
        var expectedServiceCallStatus = true;
        var expectedServiceCallUrl = "https://api.example.com/token";
        var expectedServiceCallDate = DateTime.UtcNow;
        var expectedResponseBody = "{\"token\":\"abc123\"}";
        var expectedErrorCode = "200";
        var expectedErrorType = "Success";
        var expectedCreationDate = DateTime.UtcNow;
        var expectedCreationUserId = 123L;
        var expectedProviderType = ProviderTypeInLog.AsanPardakht;
        var expectedCorrelationId = "correlation-123";

        // Act
        var callLog = new CallLogModel
        {
            ServiceType = expectedServiceType,
            AuditType = expectedAuditType,
            RequestBody = expectedRequestBody,
            ServiceCallStatus = expectedServiceCallStatus,
            ServiceCallUrl = expectedServiceCallUrl,
            ServiceCallDate = expectedServiceCallDate,
            ResponseBody = expectedResponseBody,
            ErrorCode = expectedErrorCode,
            ErrorType = expectedErrorType,
            CreationDate = expectedCreationDate,
            CreationUserId = expectedCreationUserId,
            ProviderType = expectedProviderType,
            CorrolationId = expectedCorrelationId
        };

        // Assert
        callLog.ServiceType.Should().Be(expectedServiceType);
        callLog.AuditType.Should().Be(expectedAuditType);
        callLog.RequestBody.Should().Be(expectedRequestBody);
        callLog.ServiceCallStatus.Should().Be(expectedServiceCallStatus);
        callLog.ServiceCallUrl.Should().Be(expectedServiceCallUrl);
        callLog.ServiceCallDate.Should().Be(expectedServiceCallDate);
        callLog.ResponseBody.Should().Be(expectedResponseBody);
        callLog.ErrorCode.Should().Be(expectedErrorCode);
        callLog.ErrorType.Should().Be(expectedErrorType);
        callLog.CreationDate.Should().Be(expectedCreationDate);
        callLog.CreationUserId.Should().Be(expectedCreationUserId);
        callLog.ProviderType.Should().Be(expectedProviderType);
        callLog.CorrolationId.Should().Be(expectedCorrelationId);
    }

    [Fact]
    public void CallLogModel_Should_Handle_Failed_Service_Call()
    {
        // Arrange & Act
        var callLog = new CallLogModel
        {
            ServiceCallStatus = false,
            ErrorCode = "500",
            ErrorType = "InternalServerError",
            ResponseBody = "Error occurred"
        };

        // Assert
        callLog.ServiceCallStatus.Should().BeFalse();
        callLog.ErrorCode.Should().Be("500");
        callLog.ErrorType.Should().Be("InternalServerError");
        callLog.ResponseBody.Should().Be("Error occurred");
    }

    [Theory]
    [InlineData(AuditType.Client)]
    [InlineData(AuditType.Provider)]
    [InlineData(AuditType.User)]
    [InlineData(AuditType.Develop)]
    public void CallLogModel_Should_Support_All_AuditTypes(AuditType auditType)
    {
        // Arrange & Act
        var callLog = new CallLogModel
        {
            AuditType = auditType
        };

        // Assert
        callLog.AuditType.Should().Be(auditType);
    }

    [Theory]
    [InlineData(ProviderTypeInLog.AsanPardakht)]
    [InlineData(ProviderTypeInLog.Pec)]
    [InlineData(ProviderTypeInLog.BehPardakht)]
    [InlineData(ProviderTypeInLog.NeoBank)]
    [InlineData(ProviderTypeInLog.CharisPay)]
    [InlineData(ProviderTypeInLog.CharismaCard)]
    public void CallLogModel_Should_Support_All_ProviderTypes(ProviderTypeInLog providerType)
    {
        // Arrange & Act
        var callLog = new CallLogModel
        {
            ProviderType = providerType
        };

        // Assert
        callLog.ProviderType.Should().Be(providerType);
    }

    [Fact]
    public void CallLogModel_Should_Handle_Timeout_Scenario()
    {
        // Arrange & Act
        var callLog = new CallLogModel
        {
            ServiceCallStatus = false,
            ErrorCode = "RequestTimeout",
            ErrorType = "Timeout",
            ResponseBody = "TIMEOUT after 30000ms - The operation has timed out",
            ServiceCallDate = DateTime.UtcNow,
            CreationDate = DateTime.UtcNow
        };

        // Assert
        callLog.ServiceCallStatus.Should().BeFalse();
        callLog.ErrorCode.Should().Be("RequestTimeout");
        callLog.ErrorType.Should().Be("Timeout");
        callLog.ResponseBody.Should().Contain("TIMEOUT");
    }

    [Fact]
    public void CallLogModel_Should_Store_UserId_As_Long()
    {
        // Arrange
        var expectedUserId = 9999999999L; // Large user ID

        // Act
        var callLog = new CallLogModel
        {
            CreationUserId = expectedUserId
        };

        // Assert
        callLog.CreationUserId.Should().Be(expectedUserId);
    }

    [Fact]
    public void CallLogModel_Should_Allow_Null_Optional_Fields()
    {
        // Arrange & Act
        var callLog = new CallLogModel
        {
            ServiceCallUrl = "https://api.example.com",
            ServiceCallDate = DateTime.UtcNow,
            CreationDate = DateTime.UtcNow,
            CreationUserId = 1,
            CorrolationId = "corr-123"
            // All other fields remain null
        };

        // Assert
        callLog.ServiceType.Should().BeNull();
        callLog.AuditType.Should().BeNull();
        callLog.RequestBody.Should().BeNull();
        callLog.ServiceCallStatus.Should().BeNull();
        callLog.ResponseBody.Should().BeNull();
        callLog.ErrorCode.Should().BeNull();
        callLog.ErrorType.Should().BeNull();
        callLog.ProviderType.Should().BeNull();
    }
}
