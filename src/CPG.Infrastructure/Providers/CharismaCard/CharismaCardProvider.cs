using CPG.Application.Auth;
using CPG.Application.Shared.Resource;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Communication;
using CPG.Domain.SharedKernel.Communication.CharismaCard;
using CPG.Domain.SharedKernel.Communication.CharismaCard.Models;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Logging;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Providers.CharismaCard;

public class CharismaCardProvider(IHttpProvider httpProvider,
	   IHttpClientFactory factory, IConfiguration configuration, IAuthService authService,
	IHttpContextAccessor httpContextAccessor, ILogService logService, ICurrentUser currentUser) : ICharismaCardService
{

	private readonly IHttpClientFactory factory = factory;
	private readonly IConfiguration configuration = configuration;
	private readonly IAuthService authService = authService;
	private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
	private readonly ILogService _logService = logService;
	private readonly ICurrentUser currentUser = currentUser;

	public async Task<Result<CharismaCardUserDepositBalanceResponse>> GetUserDepositBalance(string nationalCode)
	{
		var charismaCardConfig = configuration.GetSection("Infrastructure:CharismaCard").Get<CharismaCardConfig>();
		var appConfig = authService.GetJwtConfig();
		var accessTokenResult = await ExchangeToken(appConfig);
		if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
			return Result<CharismaCardUserDepositBalanceResponse>.Failure(new Error("2201001", accessTokenResult.Error));
		try
		{
			var headers = new List<(string Key, string Value)>
			{
				("Authorization", $"Bearer {accessTokenResult.Data.AccessToken}")
			};
			var httpRequest = new HttpProviderRequest<dynamic>
			{
				BaseAddress = charismaCardConfig.BaseUrl,
				Uri = charismaCardConfig.GetUserDepositBalanceUrl,
				Body = new { NationalCode = nationalCode },
				HeaderParameters = headers,
				Provider = Enums.ProviderTypeInLog.CharismaCard,
				Service = Enums.ServiceType.GetUserDepositBalance
			};
			var response = await httpProvider.GetAsync<RequestBase, CharismaCardUserDepositBalanceResponse, CharismaCardBaseResponse<List<CharismaCardData>>>(httpRequest, null, UserDepositBalanceErrorHandler, UserDepositBalanceDecoder);
			return Result<CharismaCardUserDepositBalanceResponse>.SuccessResult(response);
		}
		catch (Exception ex)
		{
			var callLog = CallLogModel.CreateError(
				serviceName: "GetUserDepositBalance",
				providerName: "CharismaCard",
				requestUri: charismaCardConfig.GetUserDepositBalanceUrl,
				requestBody: System.Text.Json.JsonSerializer.Serialize(new { nationalCode }),
				responseBody: ex.Message,
				exception: ex,
				serviceType: Enums.ServiceType.GetUserDepositBalance,
				providerType: Enums.ProviderTypeInLog.CharismaCard,
				auditType: Enums.AuditType.Provider,
				userId: currentUser.UserId
			);
			_logService.LogError(callLog);
			return Result<CharismaCardUserDepositBalanceResponse>.Failure(new Error("2451000", GlobalResource.UnexpectedError));
		}
	}


	private async Task<ResultData<TokenResponse>> ExchangeToken(JwtConfigViewModel appConfig)

	{
		var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
		var client = factory.CreateClient();
		var httpClient = factory.CreateClient("idpClient");
		var disco = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
		{
			Address = appConfig.Authority,
			Policy = { RequireHttps = false }
		});
		if (disco.IsError)
			return new ResultData<TokenResponse> { Error = disco.Error, OperationResult = Enums.OperationResult.Failed };

		var request = new TokenExchangeTokenRequest
		{
			Address = disco.TokenEndpoint,
			ClientId = appConfig.ServerApiKey,
			ClientSecret = appConfig.ServerApiSecret,
			Scope = appConfig.CharismaCardScope,
			SubjectToken = token,
			SubjectTokenType = OidcConstants.TokenTypeIdentifiers.AccessToken,

			Parameters =
		{
			{ "exchange_style", "impersonation" }
		}
		};
		var response = await client.RequestTokenExchangeTokenAsync(request);

		if (response.IsError)
			throw new Exception(response.Raw);

		return new ResultData<TokenResponse> { Data = response, OperationResult = Enums.OperationResult.Succeeded };
	}

	public async Task<Result<DirectDebitResponse>> DirectDebitRequest(DirectDebitRequest request)
	{
		var charismaCardConfig = configuration.GetSection("Infrastructure:CharismaCard").Get<CharismaCardConfig>();
		var appConfig = authService.GetJwtConfig();
		var accessTokenResult = await ExchangeToken(appConfig);
		if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
			return Result<DirectDebitResponse>.Failure(new Error("2201001", accessTokenResult.Error));
		try
		{
			var headers = new List<(string Key, string Value)>
			{
				("Authorization", $"Bearer {accessTokenResult.Data.AccessToken}")
			};
			var httpRequest = new HttpProviderRequest<DirectDebitRequest>
			{
				BaseAddress = charismaCardConfig.BaseUrl,
				Uri = charismaCardConfig.DirectDebitRequestUrl,
				HeaderParameters = headers,
				Body = request,
				Provider = Enums.ProviderTypeInLog.CharismaCard,
				Service = Enums.ServiceType.ClientDirectDebit
			};
			var directDebitResponse = await httpProvider.PostAsync<DirectDebitRequest, DirectDebitResponse, CharismaCardBaseResponse<DirectDebitData>, DirectDebitRequest>(
				httpRequest,
				request,
				DirectDebitErrorHandler,
				DirectDebitDecoder);
			var successLog = CallLogModel.CreateSuccess(
				serviceName: "ClientDirectDebit",
				providerName: "CharismaCard",
				requestUri: charismaCardConfig.DirectDebitRequestUrl,
				requestBody: System.Text.Json.JsonSerializer.Serialize(request),
				responseBody: System.Text.Json.JsonSerializer.Serialize(directDebitResponse),
				serviceType: Enums.ServiceType.ClientDirectDebit,
				providerType: Enums.ProviderTypeInLog.CharismaCard,
				auditType: Enums.AuditType.Provider,
				userId: currentUser.UserId
			);
			_logService.LogInformation(successLog);

			if (directDebitResponse.IsSuccess)
			{
				return Result<DirectDebitResponse>.SuccessResult(directDebitResponse);
			}
			else
			{
				return Result<DirectDebitResponse>.Failure(new Error("2452001", GlobalResource.DirectDeditException));

			}
		}
		catch (Exception ex)
		{
			var callLog = CallLogModel.CreateError(
				serviceName: "ClientDirectDebit",
				providerName: "CharismaCard",
				requestUri: charismaCardConfig.DirectDebitRequestUrl,
				requestBody: System.Text.Json.JsonSerializer.Serialize(request),
				responseBody: ex.Message,
				exception: ex,
				serviceType: Enums.ServiceType.ClientDirectDebit,
				providerType: Enums.ProviderTypeInLog.CharismaCard,
				auditType: Enums.AuditType.Provider,
				userId: currentUser.UserId
			);
			_logService.LogError(callLog);
			return Result<DirectDebitResponse>.Failure(new Error("2452000", GlobalResource.UnexpectedError));
		}
	}

	public async Task<Result<DirectDebitResultResponse>> DirectDebitInquiry(DirectDebitResultRequest request)
	{
		var charismaCardConfig = configuration.GetSection("Infrastructure:CharismaCard").Get<CharismaCardConfig>();
		var appConfig = authService.GetJwtConfig();
		var accessTokenResult = await ExchangeToken(appConfig);
		if (accessTokenResult.OperationResult == Enums.OperationResult.Failed)
			return Result<DirectDebitResultResponse>.Failure(new Error("2201001", accessTokenResult.Error));
		try
		{
			var headers = new List<(string Key, string Value)>
			{
				("Authorization", $"Bearer {accessTokenResult.Data.AccessToken}")
			};
			var httpRequest = new HttpProviderRequest<dynamic>
			{
				BaseAddress = charismaCardConfig.BaseUrl,
				Uri = charismaCardConfig.DirectDebitResultUrl,
				Body = request,
				HeaderParameters = headers,
				Provider = Enums.ProviderTypeInLog.CharismaCard,
				Service = Enums.ServiceType.ClientDirectDebit
			};
			var response = await httpProvider.GetAsync<DirectDebitResultRequest, DirectDebitResultResponse,
													CharismaCardBaseResponse<DirectDebitResultData>>(httpRequest, null, DirectDebitInquiryErrrorHandler, DirectDebitInquiryDecoder);
			if (response.IsSuccess)
			{
				return Result<DirectDebitResultResponse>.SuccessResult(response);
			}
			else
			{
				return Result<DirectDebitResultResponse>.Failure(new Error("2453001", GlobalResource.DirectDebitResponseException));
			}
		}
		catch (Exception ex)
		{
			var callLog = CallLogModel.CreateError(
				serviceName: "ClientDirectDebitInquiry",
				providerName: "CharismaCard",
				requestUri: charismaCardConfig.DirectDebitResultUrl,
				requestBody: System.Text.Json.JsonSerializer.Serialize(request),
				responseBody: ex.Message,
				exception: ex,
				serviceType: Enums.ServiceType.ClientDirectDebit,
				providerType: Enums.ProviderTypeInLog.CharismaCard,
				auditType: Enums.AuditType.Provider,
				userId: currentUser.UserId
			);
			_logService.LogError(callLog);
			return Result<DirectDebitResultResponse>.Failure(new Error("2453000", GlobalResource.UnexpectedError));
		}
	}

	private Task<DirectDebitResultResponse> DirectDebitInquiryErrrorHandler(dynamic baseRequest, DirectDebitResultResponse response, CharismaCardBaseResponse<DirectDebitResultData> error, short statusCode)
	{
		var result = response ?? new DirectDebitResultResponse();
		result.StatusCode = statusCode;
		if (error != null)
		{
			result.IsSuccess = error.IsSuccess;
			result.IsFailure = error.IsFailure;
			result.Data = error.Data;
			result.Error = error.Error;
		}
		return Task.FromResult<DirectDebitResultResponse>(result);
	}

	private DirectDebitResultResponse DirectDebitInquiryDecoder(string responseString)
	{
		if (string.IsNullOrEmpty(responseString))
		{
			return new DirectDebitResultResponse
			{
				StatusCode = (short)System.Net.HttpStatusCode.NotFound,
				IsSuccess = false,
				IsFailure = true,
				Error = new CharismaCardError
				{
					Code = ((int)System.Net.HttpStatusCode.NoContent).ToString(),
					Description = System.Net.HttpStatusCode.NoContent.ToString()
				}
			};
		}
		if (!string.IsNullOrEmpty(responseString) && responseString.Contains("Unauthorized"))
		{
			return new DirectDebitResultResponse
			{
				StatusCode = (short)System.Net.HttpStatusCode.Unauthorized,
				IsSuccess = false,
				IsFailure = true,
				Error = new CharismaCardError
				{
					Code = ((int)System.Net.HttpStatusCode.Unauthorized).ToString(),
					Description = responseString
				}
			};
		}
		var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		return JsonSerializer.Deserialize<DirectDebitResultResponse>(responseString, options)!;
	}

	private Task<DirectDebitResponse> DirectDebitErrorHandler(DirectDebitRequest baseRequest, DirectDebitResponse response, CharismaCardBaseResponse<DirectDebitData>? error, short statusCode)
	{
		var result = response ?? new DirectDebitResponse();
		result.StatusCode = statusCode;
		if (error != null)
		{
			result.IsSuccess = error.IsSuccess;
			result.IsFailure = error.IsFailure;
			result.Data = error.Data;
			result.Error = error.Error;
		}
		return Task.FromResult<DirectDebitResponse>(result);
	}

	private DirectDebitResponse DirectDebitDecoder(string responseString)
	{
		if (string.IsNullOrEmpty(responseString))
		{
			return new DirectDebitResponse
			{
				StatusCode = (short)System.Net.HttpStatusCode.NotFound,
				IsSuccess = false,
				IsFailure = true,
				Error = new CharismaCardError
				{
					Code = ((int)System.Net.HttpStatusCode.NoContent).ToString(),
					Description = System.Net.HttpStatusCode.NoContent.ToString()
				}
			};
		}
		// Handle plain Unauthorized response
		else if (!string.IsNullOrEmpty(responseString) && responseString.Contains("Unauthorized"))
		{
			return new DirectDebitResponse
			{
				StatusCode = (short)System.Net.HttpStatusCode.Unauthorized,
				IsSuccess = false,
				IsFailure = true,
				Error = new CharismaCardError
				{
					Code = ((int)System.Net.HttpStatusCode.Unauthorized).ToString(),
					Description = responseString
				}
			};
		}
		// Deserialize normal JSON response
		var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		return JsonSerializer.Deserialize<DirectDebitResponse>(responseString, options)!;
	}

	private Task<CharismaCardUserDepositBalanceResponse> UserDepositBalanceErrorHandler(RequestBase baseRequest, CharismaCardUserDepositBalanceResponse response, CharismaCardBaseResponse<List<CharismaCardData>> error, short statusCode)
	{
		var result = response ?? new CharismaCardUserDepositBalanceResponse();
		result.StatusCode = statusCode;
		if (error != null)
		{
			result.IsSuccess = error.IsSuccess;
			result.IsFailure = error.IsFailure;
			result.Data = error.Data;
			result.Error = error.Error;
		}
		return Task.FromResult(result);
	}

	private CharismaCardUserDepositBalanceResponse UserDepositBalanceDecoder(string responseString)
	{
		if (string.IsNullOrEmpty(responseString))
		{
			return new CharismaCardUserDepositBalanceResponse
			{
				StatusCode = (short)System.Net.HttpStatusCode.NoContent,
				IsSuccess = false,
				IsFailure = true,
				Error = new CharismaCardError
				{
					Code = ((int)System.Net.HttpStatusCode.NoContent).ToString(),
					Description = System.Net.HttpStatusCode.NoContent.ToString()
				}
			};
		}
		if (!string.IsNullOrEmpty(responseString) && responseString.Contains("Unauthorized"))
		{
			return new CharismaCardUserDepositBalanceResponse
			{
				StatusCode = (short)System.Net.HttpStatusCode.Unauthorized,
				IsSuccess = false,
				IsFailure = true,
				Error = new CharismaCardError
				{
					Code = ((int)System.Net.HttpStatusCode.Unauthorized).ToString(),
					Description = responseString
				}
			};
		}
		var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		return JsonSerializer.Deserialize<CharismaCardUserDepositBalanceResponse>(responseString, options)!;
	}
}