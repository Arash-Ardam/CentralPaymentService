using Application.Abstractions;
using CentralPaymentWebApi.Abstractions;
using Infrastructure.Helpers;
using Infrastructure.Services.Idempotency;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace CentralPaymentWebApi.Handlers
{
	public class IdempotencyHandler : IIdempotencyHandler
	{
		private readonly IIdempotencyService _service;

		private IResult? _cachedResult;

		private IdempotencyDto? _request;

		public IdempotencyHandler(IIdempotencyService service)
		{
			_service = service;
		}

		public async Task<bool> BeginRequestAsync(EndpointFilterInvocationContext context)
		{
			var key = context.HttpContext.Request.Headers["Idempotency-Key"].ToString();

			if (string.IsNullOrWhiteSpace(key))
			{
				_cachedResult =
					Results.BadRequest("Idempotency-Key header is required.");

				return false;
			}

			var bodyObject = context.Arguments.Where(a => 
				a is not HttpContext &&
				a is not CancellationToken &&
				a is not ClaimsPrincipal &&
				a is not IServiceProvider&&
				a is not IServiceCollection)
				.ToArray();

			var body = HashConvertor.ConvertToHash(bodyObject);

			if (await _service.AnyWithDifferentBodyAsync(key, body))
			{
				_cachedResult =
					Results.Conflict("کلید تکراری با Body متفاوت ارسال شده است.");

				return false;
			}

			var request =
				await _service.GetIdempotencyRequestAsync(key, body);

			if (request == null)
			{
				_request =
					await _service.AddIdempotentRequest(
						new CreateIdempotencyRequestDto
						{
							Key = key,
							RequestBody = body
						});

				return true;
			}

			if (request.Status == IdempotencyStatus.Pending)
			{
				_cachedResult =
					Results.Conflict("Request is already processing.");

				return false;
			}

			if (request.ExpiresAt <= DateTimeOffset.UtcNow)
			{
				await _service.RemoveIdempotencyRequest(key);

				_request =
					await _service.AddIdempotentRequest(
						new CreateIdempotencyRequestDto
						{
							Key = key,
							RequestBody = body
						});

				return true;
			}

			_cachedResult =
				Results.Content(
					request.ResponseBody,
					"application/json",
					statusCode: request.StatusCode);

			return false;
		}

		public IResult? GetCachedResult()
			=> _cachedResult;

		public async Task<IResult> EndRequestAsync<T>(object? endpointResult)
		{
			if (endpointResult is not ApiResponse<T> response)
				return Results.Conflict("Endpoint result type conflicts");


			var updateIdempotentRequestDto = new UpdateIdempotencyRequestDto
			{
				Id = _request.Id,
				Key = _request.Key,
				RequestBody = _request.RequestBody,
			};

			switch (response.AppResponse.Status)
			{
				case ApplicationResultStatus.Done:
					{
						updateIdempotentRequestDto.Status = IdempotencyStatus.Completed;
						updateIdempotentRequestDto.StatusCode = StatusCodes.Status200OK;
						updateIdempotentRequestDto.ResponseBody = JsonSerializer.Serialize(response.AppResponse.Data);
					}
					break;
				case ApplicationResultStatus.Created:
					{
						updateIdempotentRequestDto.Status = IdempotencyStatus.Completed;
						updateIdempotentRequestDto.StatusCode = StatusCodes.Status201Created;
						updateIdempotentRequestDto.ResponseBody = response.AppResponse.Message;
					}
					break;
				case ApplicationResultStatus.Accepted:
					{
						updateIdempotentRequestDto.Status = IdempotencyStatus.Completed;
						updateIdempotentRequestDto.StatusCode = StatusCodes.Status202Accepted;
						updateIdempotentRequestDto.ResponseBody = JsonSerializer.Serialize(response.AppResponse.Data);
					}
					break;
				case ApplicationResultStatus.ValidationError:
					{
						updateIdempotentRequestDto.Status = IdempotencyStatus.Rejected;
						updateIdempotentRequestDto.StatusCode = StatusCodes.Status400BadRequest;
						updateIdempotentRequestDto.ResponseBody = response.AppResponse.Message;
					}
					break;
				case ApplicationResultStatus.NotFound:
					{
						updateIdempotentRequestDto.Status = IdempotencyStatus.Rejected;
						updateIdempotentRequestDto.StatusCode = StatusCodes.Status404NotFound;
						updateIdempotentRequestDto.ResponseBody = response.AppResponse.Message;
					}
					break;
				case ApplicationResultStatus.Exception:
					{
						updateIdempotentRequestDto.Status = IdempotencyStatus.Failed;
						updateIdempotentRequestDto.StatusCode = StatusCodes.Status500InternalServerError;
						updateIdempotentRequestDto.ResponseBody = response.AppResponse.Message;
					}
					break;
				default:
					{
						updateIdempotentRequestDto.Status = IdempotencyStatus.Failed;
						updateIdempotentRequestDto.StatusCode = StatusCodes.Status500InternalServerError;
						updateIdempotentRequestDto.ResponseBody = "an unhandled error";
					}
					break;
			}

			await _service.UpdateIdempotentRequest(updateIdempotentRequestDto);

			return response.HttpResult;
		}

		public async Task<IResult> EndRequestAsync(object? endpointResult)
		{
			if (endpointResult is not ApiResponse response)
				return Results.Conflict("Endpoint result type conflicts");




			await _service.UpdateIdempotentRequest(new UpdateIdempotencyRequestDto
			{
				Id = _request.Id,
				Key = _request.Key,
				RequestBody = _request.RequestBody,


				Status = response.AppResponse.Status switch 
				{
					ApplicationResultStatus.Created or ApplicationResultStatus.Accepted => IdempotencyStatus.Completed,
					ApplicationResultStatus.ValidationError or ApplicationResultStatus.NotFound => IdempotencyStatus.Rejected,
					ApplicationResultStatus.Exception => IdempotencyStatus.Failed,
					_ => IdempotencyStatus.Failed,
				},
				StatusCode = response.AppResponse.Status switch
				{
					ApplicationResultStatus.Created => StatusCodes.Status201Created,
					ApplicationResultStatus.Accepted => StatusCodes.Status202Accepted,
					ApplicationResultStatus.ValidationError => StatusCodes.Status400BadRequest ,
					ApplicationResultStatus.NotFound => StatusCodes.Status404NotFound,
					ApplicationResultStatus.Exception => StatusCodes.Status500InternalServerError,
					_ => StatusCodes.Status500InternalServerError
				},
				ResponseBody = response.AppResponse.Message ?? "An unhandled error happend"
			});

			return response.HttpResult;
		}

	}
}
