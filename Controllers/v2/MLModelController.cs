using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.ML;
using Microsoft.ML.Recommender;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.ComponentModel;
using System.Text.Json;
using RecommenderEngine.DataModels.ML;
using RecommenderEngine.DataModels.controllers;

namespace RecommenderEngine.Controllers.v2
{

    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]

    public class MLModelController : ControllerBase
    {
        /// <summary>
        /// Train ML Model using users history dataset
        /// </summary>

        [HttpPost]
        [MapToApiVersion("2.0")]
        [Route("api/v{version:apiVersion}/[controller]/Train")]
        [ApiExplorerSettings(GroupName = "v2")]

        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseResponseModel), Description = "Success: Model Training is Started")]
        [SwaggerResponse(StatusCodes.Status423Locked, Type = typeof(BaseResponseModel), Description = "Locked: Model Training is in progress")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseModel), Description = "Error: Invalid Request Body")]

        public ActionResult<string> Train([FromBody] TrainRequestModel trainRequestModel)
        {

            if (trainRequestModel.UsersHistory == null)
            {

                BaseResponseModel baseResponseModel = new BaseResponseModel
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = BaseResponseModel.STATUS_ERROR,
                    Message = "Invalid Request Body",
                };

                return BadRequest(baseResponseModel);
            }
            

            if (MLManager.IsTrainingInProgress())
            {
                
                BaseResponseModel baseResponseModel = new BaseResponseModel
                {
                    Code = StatusCodes.Status423Locked,
                    Status = BaseResponseModel.STATUS_ERROR,
                    Message = "Processing: Model Training is in progress please wait",
                };
                
                return StatusCode(StatusCodes.Status423Locked,baseResponseModel);
            }
            

            Task.Factory.StartNew(() =>
            {
               MLManager.TrainAndBuildModel(trainRequestModel.UsersHistory);
            });

            BaseResponseModel baseResponse = new BaseResponseModel()
            {
                Code = StatusCodes.Status200OK,
                Status = BaseResponseModel.STATUS_SUCCESS,
                Message = "Model Training Started Successfully"
            };

            return Ok(baseResponse);
        }

        /// <summary>
        /// Get Last Training Results
        /// </summary>

        [HttpGet]
        [MapToApiVersion("2.0")]
        [Route("api/v{version:apiVersion}/[controller]/LastTrainingResults")]
        [ApiExplorerSettings(GroupName = "v2")]
        [Produces("text/plain", "application/json")]

        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TrainingResultsModel), Description = "Success")]
        [SwaggerResponse(StatusCodes.Status423Locked, Type = typeof(BaseResponseModel), Description = "Locked: File is being processed")]

        public ActionResult<string> LastTrainingResults([FromQuery, SwaggerParameter(Description = "set returned format (value: string, json)", Required = false)] string format)
        {

            if (MLManager.IsTrainingInProgress())
                return StatusCode(StatusCodes.Status423Locked, "Training is in progress. File can't be accessed.");

            if (format != null && format.ToLower() == "string") return Ok(MLManager.GetLastTrainResults());
            else return Ok(MLManager.GetLastTrainResultsModel());
 
        }

        /// <summary>
        /// Get Last Training Dataset
        /// </summary>

        [HttpGet]
        [MapToApiVersion("2.0")]
        [Route("api/v{version:apiVersion}/[controller]/LastTrainingDataset")]
        [ApiExplorerSettings(GroupName = "v2")]
        [Produces("text/plain","application/json")]

        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(MLDataModel[]), Description = "Success")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(BaseResponseModel), Description = "Error: dataset backup file not found")]
        [SwaggerResponse(StatusCodes.Status423Locked, Type = typeof(BaseResponseModel), Description = "Locked: File is being processed")]

        public ActionResult<string> LastTrainingDataset([FromQuery, SwaggerParameter(Description = "set returned format (value: csv, json)", Required = false)] string format)
        {
           
            if (format != null && format.ToLower() == "csv")
            {
                if (!MLManager.DoesLastDatasetCsvExists())
                {

                    BaseResponseModel baseResponseModel = new BaseResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = PredictMultiResponseModel.STATUS_ERROR,
                        Message = "Error: Dataset csv backup file is missing.",
                    };

                    return NotFound(baseResponseModel);
                }

                if (MLManager.IsTrainingInProgress())
                    return StatusCode(StatusCodes.Status423Locked, "Training is in progress. File can't be accessed.");

                return Ok(MLManager.GetLastDatasetCsv());

            }
            else {
                if (!MLManager.DoesLastDatasetJsonExists())
                {

                    BaseResponseModel baseResponseModel = new BaseResponseModel
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = PredictMultiResponseModel.STATUS_ERROR,
                        Message = "Error: Dataset json backup file is missing.",
                    };

                    return NotFound(baseResponseModel);
                }

                if (MLManager.IsTrainingInProgress())
                    return StatusCode(StatusCodes.Status423Locked, "Training is in progress. File can't be accessed.");

                return Ok(MLManager.GetLastDatasetJson());

            }

        }
    }
}
