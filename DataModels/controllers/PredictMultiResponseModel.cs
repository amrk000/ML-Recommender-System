using RecommenderEngine.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace RecommenderEngine.DataModels.controllers
{
    public class PredictMultiResponseModel : BaseResponseModel
    {
        /// <example>383</example>
        [SwaggerSchema(Description = "Target User Id", Nullable = false)]
        public long UserId { set; get; }
        /// <example>[{id:446 ,score:46.3434234}, {id:242 ,score:41.2312323}, {id:627 ,score:35.54345445}, {id:23 ,score:87.234234234}, {id:2 ,score:11.2323221}, {id:93 ,score:0.0}]</example>
        [SwaggerSchema(Description = "Items with prediction score sorted dsc by score", Nullable = false)]
        public List<ItemModel> Items { set; get; }        
    }
}
