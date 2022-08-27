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

namespace RecommenderEngine.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseResponseModel), Description = "Success")]

    public class PingController : ControllerBase
    {
        [HttpGet]
        [MapToApiVersion("2.0")]
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiExplorerSettings(GroupName = "v2")]
        [SwaggerOperation("Ping server to check if it's online")]
        public ActionResult<BaseResponseModel> Get()
        {
            String modelStatus = "Ready for Prediction.";

            if (!MLManager.DoesModelExists()) modelStatus = "Model is Missing!";

            BaseResponseModel baseResponse = new BaseResponseModel()
            {
                Code = StatusCodes.Status200OK,
                Status = BaseResponseModel.STATUS_SUCCESS,
                Message = "System is Online. ML Model Status: " + modelStatus
            };

            return Ok(baseResponse);
        }
    }
}
