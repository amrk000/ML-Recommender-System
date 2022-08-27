using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace RecommenderEngine
{
    //Swagger swashbuckle versioning path filter
    internal class ReplaceVersionWithExactValueInPathFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

            var paths = new OpenApiPaths();
            foreach (var path in swaggerDoc.Paths)
            {   
                paths.Add(path.Key.Replace("v{version}", swaggerDoc.Info.Version), path.Value);
            }
            swaggerDoc.Paths = paths;

        }
    }
}