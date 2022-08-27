using Microsoft.ML.Data;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderEngine.DataModels.ML
{
    public class MLDataModel
    {
        /// <example>627</example>
        [LoadColumn(0)]
        [SwaggerSchema(Description = "userId", Nullable = false)]
        public long userId { set; get; }
        
        /// <example>54</example>
        [LoadColumn(1)]
        [SwaggerSchema(Description = "itemId", Nullable = false)]
        public long itemId { set; get; }

        /// <example>79.3234678675</example>
        [LoadColumn(2)]
        [SwaggerSchema(Description = "Label: known item score for the user", Nullable = false)]
        public float Label { set; get; }
    }
}

