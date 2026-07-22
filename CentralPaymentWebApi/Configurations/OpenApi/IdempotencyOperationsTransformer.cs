using CentralPaymentWebApi.Configurations.EndpointsFilter;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CentralPaymentWebApi.Configurations.OpenApi
{
	public sealed class IdempotencyOperationsTransformer : IOpenApiOperationTransformer
	{
		public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
		{
			var hasIdempotency =
						context.Description.ActionDescriptor.EndpointMetadata?
							.OfType<RequireIdempotencyMetadata>()
							.Any();

			if (hasIdempotency.HasValue && hasIdempotency.Value == false)
				return Task.CompletedTask;


			if(operation.Parameters is null)
				operation.Parameters = new List<IOpenApiParameter>();


			operation.Parameters.Add(new OpenApiParameter
			{
				Name = "Idempotency-Key",
				In = ParameterLocation.Header,
				Required = true,
				Description = "Unique client generated request identifier.",
				Schema = new OpenApiSchema
				{
					Type = JsonSchemaType.String
				}
			});

			operation.Responses.TryAdd("409", new OpenApiResponse
			{
				Description = "Duplicate request."
			});

			return Task.CompletedTask;
		}
	}
}
