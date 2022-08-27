using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RecommenderEngine.DataModels.ML;
using RecommenderEngine.DataModels.controllers;
using RecommenderEngine.DataModels.controllers.v1;

namespace RecommenderEngine.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(String), Description = "Success: \"System is Online. ML Model Status: ..\"")]

    public class PingController : ControllerBase
    {

        [HttpGet]
        [MapToApiVersion("1.0")]
        [Route("api/[controller]")]
        [ApiExplorerSettings(GroupName = "v0")]
        public ActionResult<string> Get()
        {
            String modelStatus="ML Model is Ready for Prediction.";
            
            if (!MLManager.DoesModelExists()) modelStatus = "ML Model is Missing!";

            return Ok("System is Online.\nML Model Status: " + modelStatus);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiExplorerSettings(GroupName = "v1")]
        [SwaggerOperation("Ping server to check if it's online")]
        public ActionResult<string> GetV1()
        {
            return Get();
        }
    }
}
