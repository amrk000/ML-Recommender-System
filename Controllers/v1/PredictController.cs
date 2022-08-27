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
using RecommenderEngine.DataModels.controllers.v1;

namespace RecommenderEngine.Controllers.v1
{

    [ApiController]
    [ApiVersion("1.0")]

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<PredictItemModel>), Description = "Success: List of items with predicted scores ordered dsc by score")]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(String), Description = "Error: Model is missing. Please train the model first")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(String), Description = "Error: UserId cannot be negative")]
    
    public class PredictController : ControllerBase
    {
        private PredictionEnginePool<MLDataModel, MLRatingPrediction> predictionEnginePool;
        
        public PredictController(PredictionEnginePool<MLDataModel, MLRatingPrediction> predictionEnginePool)
        {
            this.predictionEnginePool = predictionEnginePool;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [Route("api/[controller]")]
        [ApiExplorerSettings(GroupName = "v0")]
        public ActionResult<string> Get([FromBody] PredictRequestModel input)
        {
            if (!MLManager.DoesModelExists())
            {
                this.Response.StatusCode = StatusCodes.Status404NotFound;
                return "Error: Model is missing. Please train the model first.";
            }
            
            if (input.UserId < 0) return BadRequest("UserId cannot be negative.");

            List<PredictItemModel> items = new List<PredictItemModel>();

            foreach (long item in input.Items)
            {
                if (item < 0) continue;

                MLDataModel dataModel = new MLDataModel { userId = input.UserId, itemId = item };

                MLRatingPrediction prediction = MLManager.Predict(dataModel, predictionEnginePool);

                float score = 0;
                
                //Check Item value to know if it could be predicted
                if (prediction.Score > float.MinValue && prediction.Score < float.MaxValue)
                score = prediction.Score;

                PredictItemModel predictedItemRate = new PredictItemModel();
                predictedItemRate.Item = item;
                predictedItemRate.Score = score;

                items.Add(predictedItemRate);
            }

            items = items.OrderBy(item => -item.Score).ToList();

            return Ok(JsonSerializer.Serialize(items));
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiExplorerSettings(GroupName = "v1")]
        
        [SwaggerOperation("Predict Items to recommend for user")]
        
        public ActionResult<string> GetV1([FromBody] PredictRequestModel input)
        {
            return Get(input);
        }
    }
}
