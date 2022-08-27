using Swashbuckle.AspNetCore.Annotations;
using System.Reflection.Metadata;

namespace RecommenderEngine.DataModels.ML
{
    public class TrainingResultsModel
    {
        public const string STATUS_COMPLETED = "completed";
        public const string STATUS_ERROR = "failed";
        public const string STATUS_TRAINING = "training";
        
        /// <example>5/28/2022 2:10:50 PM (UTC)</example>
        [SwaggerSchema(Nullable = true)]
        public string StartTime { get; set; }
        
        /// <example>4.70s</example>
        [SwaggerSchema(Nullable = true)]
        public string TotalTime { get; set; }

        /// <example>completed</example>
        [SwaggerSchema(Nullable = true)]
        public string Status { get; set; }

        /// <example>Model is Trained and Built Successfully</example>
        [SwaggerSchema(Nullable = true)]
        public string Details { get; set; }

        [SwaggerSchema(Nullable = true)]
        public DataSetModel DataSet { get; set; }

        [SwaggerSchema(Nullable = true)]
        public TrainingConfigsModel TrainingConfigs { get; set; } 
        
        [SwaggerSchema(Nullable = true)]
        public TrainingMetricesModel TrainingMetrices { get; set; }

        public TrainingResultsModel() {
            DataSet = new DataSetModel();
            TrainingConfigs = new TrainingConfigsModel();
            TrainingMetrices = new TrainingMetricesModel();
        }

        public class DataSetModel {
            /// <example>1000</example>
            [SwaggerSchema(Nullable = true)]
            public long RecordsCount { get; set; }
            
            /// <example>0.3</example>
            [SwaggerSchema(Nullable = true)]
            public double SplitRatio { get; set; }

            /// <example>700</example>
            [SwaggerSchema(Nullable = true)]
            public long TrainRecordsCount { get; set; }

            /// <example>300</example>
            [SwaggerSchema(Nullable = true)]
            public long TestRecordsCount { get; set; }
        }

        public class TrainingConfigsModel
        {
            /// <example>10000</example>
            [SwaggerSchema(Nullable = true)]
            public int NumberOfIterations { get; set; }

            /// <example>0.001</example>
            [SwaggerSchema(Nullable = true)]
            public float LearningRate { get; set; }

            /// <example>100</example>
            [SwaggerSchema(Nullable = true)]
            public int ApproximationRank { get; set; }
        }

        public class TrainingMetricesModel
        {
            /// <example>34.50738204637768</example>
            [SwaggerSchema(Nullable = true)]
            public double Rmse { get; set; }

            /// <example>-0.5518134477338905</example>
            [SwaggerSchema(Nullable = true)]
            public double RSquared { get; set; }
        }
    }
}
