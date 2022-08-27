using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.ML;
using System;
using System.Reflection;
using System.IO;
using RecommenderEngine.DataModels.ML;
using Microsoft.AspNetCore.Mvc;

namespace RecommenderEngine
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

            });

            services.AddControllers();

            services.AddMvcCore().AddApiExplorer();

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Machine Learning Recommender System",
                    Version = "v2.0",
                    Description = "REST API Docs to integrate system with backend"
                });

                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Machine Learning Recommender System (Deprecated)",
                    Version = "v1.0",
                    Description = "REST API Docs to integrate system with backend",
                });
                    
                swagger.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
                swagger.OperationFilter<RemoveQueryApiVersionParamOperationFilter>();

                swagger.EnableAnnotations();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);

            });

            services.AddPredictionEnginePool<MLDataModel, MLRatingPrediction>()            
              .FromFile(modelName: MLManager.GetModelName(), filePath: MLManager.GetModelPath(), watchForChanges: true);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger();
            
            app.UseSwaggerUI(swagger => {
                swagger.SwaggerEndpoint("/swagger/v2/swagger.json", "REST API v2");
                swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "REST API v1 (Deprecated)");
            }
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

        }
    }
}
