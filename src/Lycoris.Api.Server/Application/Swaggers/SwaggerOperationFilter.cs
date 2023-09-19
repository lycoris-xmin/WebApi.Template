using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lycoris.Api.Server.Application.Swaggers
{
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var excludeApiKey = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).Union(context.MethodInfo.GetCustomAttributes(true)).OfType<ExcludeSwaggerHeaderAttribute>().Any() ?? false;
            if (excludeApiKey)
            {
                operation.Security = new List<OpenApiSecurityRequirement>();
            }
            else
            {
                operation.Security = new List<OpenApiSecurityRequirement>()
                {
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "ApiKey"
                                    }
                                },
                                new List<string>()
                            }
                        }
                };
            }
        }
    }
}
