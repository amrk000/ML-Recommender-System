using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommenderEngine.DataModels.ML;

namespace RecommenderEngine.DataModels.controllers
{
    public class TrainRequestModel
    {
        [SwaggerSchema(Description = "Users History Object", Nullable = false)]
        public MLDataModel[] UsersHistory { set; get; }
    }

}