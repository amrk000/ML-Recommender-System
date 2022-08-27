using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderEngine.DataModels.ML
{
    public class MLRatingPrediction
    {
        public float Label { set; get; }
        public float Score { set; get; }
    }
}

