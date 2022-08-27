using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace RecommenderEngine.DataModels.controllers
{
    public class PredictMultiRequestModel
    {
        /// <example>383</example>
        [SwaggerSchema(Description = "Target User Id", Nullable = false)]
        public long UserId { set; get; }
        /// <example>[{id:446 ,score:0.0}, {id:242 ,score:0.0}, {id:627 ,score:0.0}, {id:23 ,score:0.0}, {id:2 ,score:0.0}, {id:93 ,score:0.0}]</example>
        [SwaggerSchema(Description = "Items to Predict and Sort for User", Nullable = false)]
        public List<ItemModel> Items { set; get; }
    }
}
