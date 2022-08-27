using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace RecommenderEngine.DataModels.controllers
{
    public class PredictSingleRequestModel
    {
        /// <example>383</example>
        [SwaggerSchema(Description = "Target User Id", Nullable = false)]
        public long UserId { set; get; }
        /// <example>{id:446 ,score:0.0}</example>
        [SwaggerSchema(Description = "Item to Predict for User", Nullable = false)]
        public ItemModel Item { set; get; }
    }
}
