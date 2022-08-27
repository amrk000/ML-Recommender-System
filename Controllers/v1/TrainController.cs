using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.ML;
using Microsoft.ML.Recommender;
using Swashbuckle.AspNetCore.Annotations;
using RecommenderEngine.DataModels.ML;
using RecommenderEngine.DataModels.controllers;
using RecommenderEngine.DataModels.controllers.v1;

namespace RecommenderEngine.Controllers.v1
{

    [ApiController]
    [ApiVersion("1.0")]

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(String), Description = "Success: Model Training is Done and Returned Detailed Results for Backend Log")]
    [SwaggerResponse(StatusCodes.Status423Locked, Type = typeof(BaseResponseModel), Description = "Locked: Model Training is in progress")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(String), Description = "Error: Invalid Request Body")]
    public class TrainController : ControllerBase
    {
        [HttpPost]
        [MapToApiVersion("1.0")]
        [Route("api/[controller]")]
        [ApiExplorerSettings(GroupName = "v0")]
        public ActionResult<string> Post([FromBody] MLDataModel[] input)
        {
            if (MLManager.IsTrainingInProgress()) return StatusCode(StatusCodes.Status423Locked, "Processing: Model Training is in progress please wait");

            String response = "";

            try
            {
               response = MLManager.TrainAndBuildModel(input);
            }
            catch (Exception e) {
                return BadRequest("Bad Request: Invalid Request Body");
            }

            return Ok(response);
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiExplorerSettings(GroupName = "v1")]

        [SwaggerOperation("Train ML Model using users history dataset")]

        public ActionResult<string> PostV1([FromBody] MLDataModel[] input)
        {
            return Post(input);
        }
    }
}
