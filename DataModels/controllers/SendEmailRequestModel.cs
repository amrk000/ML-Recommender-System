using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace RecommenderEngine.DataModels.controllers
{
    public class SendEmailRequestModel
    {
        /// <example>383</example>
        [SwaggerSchema(Description = "Target User Id", Nullable = false)]
        public long UserId { set; get; }

        /// <example>user@email.com</example>
        [SwaggerSchema(Description = "Target User Email", Nullable = false)]
        public String UserEmail { set; get; }

        /// <example>Hello World</example>
        [SwaggerSchema(Description = "Target User Full Name", Nullable = false)]
        public String UserName { set; get; }

        /// <example>[{id:446 ,score:0.0, name:"item name", subData:"subdata of item", imageUrl:"...", pageUrl:"..."}, {id:242 ,score:0.0, name:"item name", subData:"subdata of item", imageUrl:"...", pageUrl:"..."}, {id:627 ,score:0.0, name:"item name", subData:"subdata of item", imageUrl:"...", pageUrl:"..."}, {id:23 ,score:0.0, name:"item name", subData:"subdata of item", imageUrl:"...", pageUrl:"..."}, {id:2 ,score:0.0, name:"item name", subData:"subdata of item", imageUrl:"...", pageUrl:"..."}, {id:93 ,score:0.0, name:"item name", subData:"subdata of item", imageUrl:"...", pageUrl:"..."}]</example>
        [SwaggerSchema(Description = "Items to Predict and Sort Then Send Top 5 by Email to User", Nullable = false)]
        public List<EmailItemModel> Items { set; get; }
    }
}
