using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderEngine.DataModels.controllers.v1
{
    public class PredictRequestModel
    {
        /// <example>383</example>
        public long UserId { set; get; }
        /// <example>[446, 242, 627, 23, 2, 93]</example>
        public long[] Items { set; get; }
    }

}