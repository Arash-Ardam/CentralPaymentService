using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Abstractions
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class ApiControllerBase : ControllerBase
	{
		public IActionResult ActionResult;

		public IActionResult HandleOutput<T>(ApplicationResponse<T> response) 
		{
			return response.Status switch
			{
				ApplicarionResultStatus.Done => Ok(response.Data),
				ApplicarionResultStatus.Created => Created(string.Empty,response.Message),
				ApplicarionResultStatus.Accepted => Accepted(response.Message),
				ApplicarionResultStatus.NotFound => NotFound(response.Message),
				ApplicarionResultStatus.ValidationError => BadRequest(response.Message),
				ApplicarionResultStatus.Exception => StatusCode(500,response.Message),
				_ => BadRequest("an unhandled error")
			};
		}

		public IActionResult HandleOutput(ApplicationResponse response)
		{
			return response.Status switch
			{
				ApplicarionResultStatus.Created => Created(string.Empty, response.Message),
				ApplicarionResultStatus.Accepted => Accepted(response.Message),
				ApplicarionResultStatus.NotFound => NotFound(response.Message),
				ApplicarionResultStatus.ValidationError => BadRequest(response.Message),
				ApplicarionResultStatus.Exception => StatusCode(500, response.Message),
				_ => BadRequest("an unhandled error")
			};
		}
	}
}
