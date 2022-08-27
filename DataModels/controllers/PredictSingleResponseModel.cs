using RecommenderEngine.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace RecommenderEngine.DataModels.controllers
{
    public class PredictSingleResponseModel : BaseResponseModel
    {
        /// <example>383</example>
        [SwaggerSchema(Description = "Target User Id", Nullable = false)]
        public long UserId { set; get; }
        /// <example>{id:446 ,score:46.3434234}</example>
        [SwaggerSchema(Description = "Item with prediction score", Nullable = false)]
        public ItemModel Item { set; get; }        
    }
}
