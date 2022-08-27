using Swashbuckle.AspNetCore.Annotations;

namespace RecommenderEngine.DataModels.controllers
{
    public class ItemModel
    {
        /// <example>446</example>
        [SwaggerSchema(Description = "Item Id", Nullable = false)]
        public int Id { get; set; }
        /// <example>0.00</example>
        [SwaggerSchema(Description = "Item Score (NOT REQUIRED in request body)", Nullable = false)]
        public float Score { get; set; }
        
    }
}
