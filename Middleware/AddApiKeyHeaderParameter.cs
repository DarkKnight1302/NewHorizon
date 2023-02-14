using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AddApiKeyHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiKeyRequired = context.MethodInfo.GetCustomAttributes(true)
            .OfType<ApiKeyRequiredAttribute>()
            .Any();

        if (apiKeyRequired)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Api-Key",
                In = ParameterLocation.Header,
                Description = "API key required for this API",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}
