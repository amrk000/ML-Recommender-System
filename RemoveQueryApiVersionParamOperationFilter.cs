using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace RecommenderEngine
{
    internal class RemoveQueryApiVersionParamOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.FirstOrDefault(p => p.Name == "version" && p.In == ParameterLocation.Path);
            if (versionParameter != null) operation.Parameters.Remove(versionParameter);
        }
    }
}