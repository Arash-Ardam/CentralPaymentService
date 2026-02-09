using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CentralPaymentWebApi.Abstractions
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class ApiControllerBase : ControllerBase
	{
		public IActionResult HandleOutput<T>(ApplicationResponse<T> response) 
		{
			return response.Status switch
			{
				ApplicationResultStatus.Done => Ok(response.Data),
				ApplicationResultStatus.Created => Created(string.Empty,response.Message),
				ApplicationResultStatus.Accepted => Accepted(string.Empty,response.Message),
				ApplicationResultStatus.NotFound => NotFound(response.Message),
				ApplicationResultStatus.ValidationError => BadRequest(response.Message),
				ApplicationResultStatus.Exception => StatusCode(500,response.Message),
				_ => BadRequest("an unhandled error")
			};
		}

		public IActionResult HandleOutput(ApplicationResponse response)
		{
			return response.Status switch
			{
				ApplicationResultStatus.Created => Created(string.Empty, response.Message),
				ApplicationResultStatus.Accepted => Accepted(string.Empty,response.Message),
				ApplicationResultStatus.NotFound => NotFound(response.Message),
				ApplicationResultStatus.ValidationError => BadRequest(response.Message),
				ApplicationResultStatus.Exception => StatusCode(500, response.Message),
				_ => BadRequest("an unhandled error")
			};
		}
	}
}
