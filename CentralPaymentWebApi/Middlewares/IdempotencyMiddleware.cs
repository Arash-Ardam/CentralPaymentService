using Application.OrderManagement.Services;
using Infrastructure.Helpers;
using Infrastructure.Services.Idempotency;

namespace CentralPaymentWebApi.Middlewares
{
	public class IdempotencyMiddleware
	{
		public RequestDelegate Next { get; }

		public IdempotencyMiddleware(RequestDelegate next)
		{
			Next = next ?? throw new ArgumentNullException(nameof(next));
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var tenantContext = context.RequestServices.GetService<ITenantContext>();
			var idempotencyService = context.RequestServices.GetService<IIdempotencyService>();

			if (context.User.IsInRole("User"))
			{
				context.Request.EnableBuffering();

				var requestBody = await new StreamReader(
					context.Request.Body,
					leaveOpen: true).ReadToEndAsync();

				context.Request.Body.Position = 0;

				var requestHash = HashConvertor.ConvertToHash(requestBody);
				var idempotencyKey = $"{tenantContext.Current.TenantName}_{requestHash}";
								
				if(await idempotencyService.AnyWithDifferentBodyAsync(idempotencyKey, requestBody))
				{
					context.Response.StatusCode = StatusCodes.Status409Conflict;
					await context.Response.WriteAsync("کلید فرستاده شده تکراری می باشد");
					return;
				}

				var idempotentRequest = await idempotencyService.GetIdempotencyRequestAsync(idempotencyKey, requestBody);
				if(idempotentRequest != null)
				{
					if(idempotentRequest.Status != IdempotencyStatus.Pending)
					{
						if (idempotentRequest.ExpiresAt > DateTimeOffset.UtcNow)
						{
							context.Response.StatusCode = idempotentRequest.StatusCode;
							await context.Response.WriteAsync(idempotentRequest.ResponseBody);
						}
						else
						{
							await idempotencyService.RemoveIdempotencyRequest(idempotencyKey);
							await Next(context);
						}
					}
					else
					{
						context.Response.StatusCode = StatusCodes.Status409Conflict;
						await context.Response.WriteAsync("درخواست ارسال شده تکراری و در حال پردازش می باشد.");
					}
					return;
				}
			}


			await Next(context);
		}
	}
}
