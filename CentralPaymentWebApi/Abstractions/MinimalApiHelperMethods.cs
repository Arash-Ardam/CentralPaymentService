using Application.Abstractions;

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
	}
}
