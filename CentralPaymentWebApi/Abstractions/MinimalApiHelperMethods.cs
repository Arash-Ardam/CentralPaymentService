using Application.Abstractions;
using Azure;
using Infrastructure.Services.Idempotency;
using System.Text.Json;

namespace CentralPaymentWebApi.Abstractions
{
	public static class MinimalApiHelperMethods
	{
		public static IResult HandleOutput<T>(this ApplicationResponse<T> response)
		{
			return response.Status switch
			{
				ApplicationResultStatus.Done => Results.Ok(response.Data),
				ApplicationResultStatus.Created => Results.Created(string.Empty, response.Message),
				ApplicationResultStatus.Accepted => Results.Accepted(string.Empty, response.Data),
				ApplicationResultStatus.NotFound => Results.NotFound(response.Message),
				ApplicationResultStatus.ValidationError => Results.BadRequest(response.Message),
				ApplicationResultStatus.Exception => Results.InternalServerError(response.Message),
				_ => Results.BadRequest("an unhandled error")
			};
		}

		public static IResult HandleOutput(this ApplicationResponse response)
		{
			return response.Status switch
			{
				ApplicationResultStatus.Created => Results.Created(string.Empty, response.Message),
				ApplicationResultStatus.Accepted => Results.Accepted(string.Empty, response.Message),
				ApplicationResultStatus.NotFound => Results.NotFound(response.Message),
				ApplicationResultStatus.ValidationError => Results.BadRequest(response.Message),
				ApplicationResultStatus.Exception => Results.InternalServerError(response.Message),
				_ => Results.BadRequest("an unhandled error")
			};
		}

		public static (IdempotencyDto idempotencyDto, IResult apiResponse) HandleIdempotencyResponse(this ApplicationResponse appResponse)
		{
			return appResponse.Status switch
			{
				ApplicationResultStatus.Created => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Completed,
					StatusCode = StatusCodes.Status201Created,
					ResponseBody = appResponse.Message
				},
				Results.Created(string.Empty, appResponse.Message)
				),

				ApplicationResultStatus.Accepted => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Completed,
					StatusCode = StatusCodes.Status202Accepted,
					ResponseBody = appResponse.Message
				},
				Results.Accepted(string.Empty, appResponse.Message)
				),

				ApplicationResultStatus.ValidationError =>(
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Rejected,
					StatusCode = StatusCodes.Status400BadRequest,
					ResponseBody = appResponse.Message
				},
				Results.BadRequest(appResponse.Message)
				),

				ApplicationResultStatus.NotFound => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Rejected,
					StatusCode = StatusCodes.Status404NotFound,
					ResponseBody = appResponse.Message
				},

				Results.NotFound(appResponse.Message)
				),

				ApplicationResultStatus.Exception => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Failed,
					StatusCode = StatusCodes.Status500InternalServerError,
					ResponseBody = appResponse.Message
				},
				Results.InternalServerError(appResponse.Message)
				),

				_ => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Failed,
					StatusCode = StatusCodes.Status500InternalServerError,
					ResponseBody = "An unhandled error happend"
				},
				Results.BadRequest("an unhandled error")
				)
			};

		}




		public static (IdempotencyDto idempotencyDto, IResult apiResponse) HandleIdempotencyResponse<T>(this ApplicationResponse<T> appResponse)
		{
			return appResponse.Status switch
			{

				ApplicationResultStatus.Done => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Completed,
					StatusCode = StatusCodes.Status200OK,
					ResponseBody = JsonSerializer.Serialize(appResponse.Data)
				},
				Results.Ok(appResponse.Data)
				),

				ApplicationResultStatus.Created => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Completed,
					StatusCode = StatusCodes.Status201Created,
					ResponseBody = appResponse.Message
				},
				Results.Created(string.Empty, appResponse.Message)
				),

				ApplicationResultStatus.Accepted => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Completed,
					StatusCode = StatusCodes.Status202Accepted,
					ResponseBody = JsonSerializer.Serialize(appResponse.Data)
				},
				Results.Accepted(string.Empty, appResponse.Data)
				),

				ApplicationResultStatus.ValidationError => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Rejected,
					StatusCode = StatusCodes.Status400BadRequest,
					ResponseBody = appResponse.Message
				},
				Results.BadRequest(appResponse.Message)
				),

				ApplicationResultStatus.NotFound => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Rejected,
					StatusCode = StatusCodes.Status404NotFound,
					ResponseBody = appResponse.Message
				},

				Results.NotFound(appResponse.Message)
				),

				ApplicationResultStatus.Exception => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Failed,
					StatusCode = StatusCodes.Status500InternalServerError,
					ResponseBody = appResponse.Message
				},
				Results.InternalServerError(appResponse.Message)
				),

				_ => (
				new IdempotencyDto
				{
					Status = IdempotencyStatus.Failed,
					StatusCode = StatusCodes.Status500InternalServerError,
					ResponseBody = "An unhandled error happend"
				},
				Results.BadRequest("an unhandled error")
				)
			};
		}



		public static ApiResponse HandleApiResponse(this ApplicationResponse response)
		{
			return response.Status switch
			{
				ApplicationResultStatus.Created => new ApiResponse 
				{
					HttpResult = Results.Created(string.Empty, response.Message),
					AppResponse = response
				},
				ApplicationResultStatus.Accepted => new ApiResponse 
				{
					HttpResult = Results.Accepted(string.Empty, response.Message),
					AppResponse = response
				},
				ApplicationResultStatus.NotFound => new ApiResponse 
				{
					HttpResult = Results.NotFound(response.Message),
					AppResponse = response
				},
				ApplicationResultStatus.ValidationError => new ApiResponse 
				{
					HttpResult = Results.BadRequest(response.Message),
					AppResponse = response
				},
				ApplicationResultStatus.Exception => new ApiResponse 
				{
					HttpResult = Results.InternalServerError(response.Message),
					AppResponse = response
				},
				_ => new ApiResponse
				{
					HttpResult = Results.BadRequest("an unhandled error"),
					AppResponse = response
				}
			};
		}

		public static ApiResponse<T> HandleApiResponse<T>(this ApplicationResponse<T> response)
		{
			return response.Status switch
			{
				ApplicationResultStatus.Done => new ApiResponse<T>
				{
					HttpResult = Results.Ok(response.Data),
					AppResponse = response
				},
				ApplicationResultStatus.Created => new ApiResponse<T>
				{
					HttpResult = Results.Created(string.Empty, response.Message),
					AppResponse = response
				},
				ApplicationResultStatus.Accepted => new ApiResponse<T>
				{
					HttpResult = Results.Accepted(string.Empty, response.Data),
					AppResponse = response
				},
				ApplicationResultStatus.NotFound => new ApiResponse<T>
				{
					HttpResult = Results.NotFound(response.Message),
					AppResponse = response
				},
				ApplicationResultStatus.ValidationError => new ApiResponse<T>
				{
					HttpResult = Results.BadRequest(response.Message),
					AppResponse = response
				},
				ApplicationResultStatus.Exception => new ApiResponse<T>
				{
					HttpResult = Results.InternalServerError(response.Message),
					AppResponse = response
				},
				_ => new ApiResponse<T>
				{
					HttpResult = Results.BadRequest("an unhandled error"),
					AppResponse = response
				}
			};
		}
	}
}
