using Swashbuckle.AspNetCore.Annotations;
using System;

namespace RecommenderEngine.DataModels.controllers
{
    public class EmailItemModel : ItemModel
    {
        /// <example>Item Full Name/Title </example>
        [SwaggerSchema(Description = "Item Name", Nullable = false)]
        public String Name { get; set; }

        /// <example>Item Subdata</example>
        [SwaggerSchema(Description = "Item Subdata (Ex: price, desc, category)", Nullable = false)]
        public String SubData { get; set; }

        /// <example>Item Image Url</example>
        [SwaggerSchema(Description = "Item Image Url", Nullable = false)]
        public String ImageUrl { get; set; }

        /// <example>Item Page Url</example>
        [SwaggerSchema(Description = "Item Page Url", Nullable = false)]
        public String PageUrl { get; set; }


    }
}
