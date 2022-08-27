using Swashbuckle.AspNetCore.Annotations;
using System.Reflection.Metadata;

namespace RecommenderEngine.DataModels.controllers
{
    public class BaseResponseModel
    {
        public const string STATUS_SUCCESS = "success";
        public const string STATUS_ERROR = "error";

        /// <example>000</example>
        [SwaggerSchema(Description = "HTTP Status Code", Nullable = false)]
        public int Code { get; set; }
        /// <example>status values: (success, error)</example>
        [SwaggerSchema(Description = "Request Status (success, error)", Nullable = false)]
        public string Status { get; set; }
        /// <example>message content</example>
        [SwaggerSchema(Description = "Response Message", Nullable = false)]
        public string Message { get; set; }
    }
}
