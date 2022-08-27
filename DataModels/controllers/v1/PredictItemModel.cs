using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderEngine.DataModels.controllers.v1
{
    public class PredictItemModel
    {
        /// <example>627</example>
        public long Item { set; get; }
        /// <example>50.392300123</example>
        public float Score { set; get; }
    }
}
