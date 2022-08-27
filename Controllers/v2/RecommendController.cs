using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.ML;
using Microsoft.ML.Recommender;
using System.Text.Json;
using Swashbuckle.AspNetCore.Annotations;
using RecommenderEngine.DataModels.ML;
using RecommenderEngine.DataModels.controllers;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace RecommenderEngine.Controllers.v2
{

    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(BaseResponseModel), Description = "Error: Model is missing. Please train the model first")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseModel), Description = "Error: UserId cannot be negative")]

    public class RecommendController : ControllerBase
    {

        private PredictionEnginePool<MLDataModel, MLRatingPrediction> predictionEnginePool;

        public RecommendController(PredictionEnginePool<MLDataModel, MLRatingPrediction> predictionEnginePool)
        {
            this.predictionEnginePool = predictionEnginePool;
        }

        /// <summary>
        /// Predict one item for a specific user
        /// </summary>

        [HttpPost]
        [MapToApiVersion("2.0")]
        [Route("api/v{version:apiVersion}/[controller]/PredictSingle")]
        [ApiExplorerSettings(GroupName = "v2")]

        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PredictSingleResponseModel), Description = "Success: item with predicted score")]
        public ActionResult<string> PredictSingle([FromBody, SwaggerRequestBody(Description = "\"score\" attribute isn't required. you can ignore it")] PredictSingleRequestModel requestBody)
        {
            if (!MLManager.DoesModelExists())
            {

                BaseResponseModel baseResponseModel = new BaseResponseModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = PredictMultiResponseModel.STATUS_ERROR,
                    Message = "Error: Model is missing. Please train the model first.",
                };

                return NotFound(baseResponseModel);
            }

            if (requestBody.UserId < 0)
            {

                BaseResponseModel baseResponseModel = new BaseResponseModel
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = PredictMultiResponseModel.STATUS_ERROR,
                    Message = "UserId cannot be negative.",
                };

                return BadRequest(baseResponseModel);
            }

            MLDataModel dataModel = new MLDataModel { userId = requestBody.UserId, itemId = requestBody.Item.Id };

            MLRatingPrediction prediction = MLManager.Predict(dataModel, predictionEnginePool);

            float score = 0;

            //Check Item value to know if it could be predicted
            if (prediction.Score > float.MinValue && prediction.Score < float.MaxValue)
                score = prediction.Score;

            ItemModel item = new ItemModel { Id = requestBody.Item.Id, Score = score };

            PredictSingleResponseModel predictSingleResponseModel = new PredictSingleResponseModel
            {
                Code = StatusCodes.Status200OK,
                Status = PredictMultiResponseModel.STATUS_SUCCESS,
                Message = "Single Prediction is Finished Successfully",
                UserId = requestBody.UserId,
                Item = item
            };

            return Ok(predictSingleResponseModel);

        }

        /// <summary>
        /// Predict list of items for a specific user and sort them by estimated score 
        /// </summary>

        [HttpPost]
        [MapToApiVersion("2.0")]
        [Route("api/v{version:apiVersion}/[controller]/PredictMulti")]
        [ApiExplorerSettings(GroupName = "v2")]

        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PredictMultiResponseModel), Description = "Success: List of items with predicted scores sorted dsc by score")]
        public ActionResult<string> PredictMulti([FromBody, SwaggerRequestBody(Description = "\"score\" attribute isn't required. you can ignore it")] PredictMultiRequestModel requestBody)
        {
            if (!MLManager.DoesModelExists())
            {

                BaseResponseModel baseResponseModel = new BaseResponseModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = PredictMultiResponseModel.STATUS_ERROR,
                    Message = "Error: Model is missing. Please train the model first.",
                };

                return NotFound(baseResponseModel);
            }

            if (requestBody.UserId < 0)
            {

                BaseResponseModel baseResponseModel = new BaseResponseModel
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = PredictMultiResponseModel.STATUS_ERROR,
                    Message = "UserId cannot be negative.",
                };

                return BadRequest(baseResponseModel);
            }

            List<ItemModel> items = new List<ItemModel>();

            foreach (ItemModel item in requestBody.Items)
            {
                if (item.Id < 0) continue;

                MLDataModel dataModel = new MLDataModel { userId = requestBody.UserId, itemId = item.Id };

                MLRatingPrediction prediction = MLManager.Predict(dataModel, predictionEnginePool);

                float score = 0;

                //Check Item value to know if it could be predicted
                if (prediction.Score > float.MinValue && prediction.Score < float.MaxValue)
                    score = prediction.Score;

                item.Score = score;

                items.Add(item);
            }

            items = items.OrderBy(item => -item.Score).ToList();

            PredictMultiResponseModel predictMultiResponseModel = new PredictMultiResponseModel
            {
                Code = StatusCodes.Status200OK,
                Status = PredictMultiResponseModel.STATUS_SUCCESS,
                Message = "Multi Prediction is Finished Successfully",
                UserId = requestBody.UserId,
                Items = items
            };

            return Ok(predictMultiResponseModel);

        }

        /// <summary>
        /// Predict list of items for a specific user pick top 5 and send by email
        /// </summary>

        [HttpPost]
        [MapToApiVersion("2.0")]
        [Route("api/v{version:apiVersion}/[controller]/SendEmail")]
        [ApiExplorerSettings(GroupName = "v2")]
        
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(BaseResponseModel), Description = "Success: Email is Sent To User Successfully")]
        [SwaggerResponse(StatusCodes.Status429TooManyRequests, Type = typeof(BaseResponseModel), Description = "Error: SendinBlue Max Limit Reached")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Type = typeof(BaseResponseModel), Description = "Error: SendinBlue Acccount Auth Failed")]
        public ActionResult<string> SendEmail([FromBody, SwaggerRequestBody(Description = "\"score\" attribute isn't required. you can ignore it")] SendEmailRequestModel requestBody)
        {

            if (!MLManager.DoesModelExists())
            {

                BaseResponseModel baseResponseModel = new BaseResponseModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = PredictMultiResponseModel.STATUS_ERROR,
                    Message = "Error: Model is missing. Please train the model first.",
                };

                return NotFound(baseResponseModel);
            }

            if (requestBody.UserId < 0)
            {

                BaseResponseModel baseResponseModel = new BaseResponseModel
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = PredictMultiResponseModel.STATUS_ERROR,
                    Message = "UserId cannot be negative.",
                };

                return BadRequest(baseResponseModel);
            }

            List<EmailItemModel> items = new List<EmailItemModel>();

            foreach (EmailItemModel item in requestBody.Items)
            {
                if (item.Id < 0) continue;

                MLDataModel dataModel = new MLDataModel { userId = requestBody.UserId, itemId = item.Id };

                MLRatingPrediction prediction = MLManager.Predict(dataModel, predictionEnginePool);

                float score = 0;

                //Check Item value to know if it could be predicted
                if (prediction.Score > float.MinValue && prediction.Score < float.MaxValue)
                    score = prediction.Score;

                item.Score = score;

                items.Add(item);
            }

            items = items.OrderBy(item => -item.Score).ToList();

            if (items.Count > 5) items.RemoveRange(5, items.Count - 5);


            HttpResponseMessage response = SendinBlueEmailSender.SendEmail(requestBody.UserEmail,requestBody.UserName, items);

            int code = (int) response.StatusCode;
            String status;
            String message;

            switch (code) {
                case StatusCodes.Status201Created:
                    status = BaseResponseModel.STATUS_SUCCESS;
                    message = "Email is Sent To User Successfully";
                    break;

                case StatusCodes.Status429TooManyRequests:
                    status = BaseResponseModel.STATUS_ERROR;
                    message = "Email Service Max Limit Reached";
                    break;

                case StatusCodes.Status401Unauthorized:
                    status = BaseResponseModel.STATUS_ERROR;
                    message = "Email Service Acccount Auth Failed. ";
                    break;

                default:
                    status = BaseResponseModel.STATUS_ERROR;
                    message = "Email Service Error Code: " + code;
                    break;
            }

            BaseResponseModel responseModel = new BaseResponseModel
            {
                Code = code,
                Status = status,
                Message = message,
            };

            return Ok(responseModel);

        }

    }
      
}
