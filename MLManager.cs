using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.Extensions.ML;
using System.Runtime.CompilerServices;
using static Microsoft.ML.DataOperationsCatalog;
using RecommenderEngine.DataModels.ML;
using System.Text.Json;

namespace RecommenderEngine
{
    public static class MLManager
    {
        private static MLContext mlContext;

        private static String modelName;
        private static String modelPath;

        private static String lastDatasetJsonPath;
        private static String lastDatasetCsvPath;

        private static String lastTrainingResultsJsonPath;
        private static String lastTrainingResultsStringPath;

        private static String trainingResults;
        private static TrainingResultsModel trainingResultsModel;

        private static double datasetSplitRatio;
        private static int numberOfIterations;
        private static float learningRate;
        private static int approximationRank;

        private static bool training;

        static MLManager()
        {
            mlContext = new MLContext(); //ML Initialization
            
            //Setting ML Model Name & Path
            modelName = "RecommenderModel";
            modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RecommenderModel.zip").ToString();

            //Setting Last Training Dataset Files
            lastDatasetJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastDatasetBackup.json").ToString();
            lastDatasetCsvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastDatasetBackup.csv").ToString();

            //Setting Last Training Results Files
            lastTrainingResultsJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastTrainingResults.json").ToString();
            lastTrainingResultsStringPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastTrainingResults.txt").ToString();

            trainingResults = "";
            trainingResultsModel = new TrainingResultsModel();

            //ML Training Algorithm Configs
            datasetSplitRatio = 0.20; //Split Ratio of Dataset (Train / Test) 0.20 or 0.30 is recommended
            numberOfIterations = 5000; //The Total training iterations count
            learningRate = 0.001f; //The rate of weights adjustments
            approximationRank = 10; //K-Rank of matrix factorization

            training = false;
        }

        public static String GetModelName() {
            return modelName;
        }

        public static String GetModelPath()
        {
            return modelPath;
        }

        public static bool DoesModelExists()
        {
            return File.Exists(modelPath);
        }

        public static bool DoesLastDatasetJsonExists()
        {
            return File.Exists(lastDatasetJsonPath);
        }

        public static String GetLastDatasetJson()
        {
            return File.ReadAllText(lastDatasetJsonPath);
        }

        public static bool DoesLastDatasetCsvExists()
        {
            return File.Exists(lastDatasetCsvPath);
        }

        public static String GetLastDatasetCsv()
        {
            return File.ReadAllText(lastDatasetCsvPath);
        }

        public static String GetLastTrainResults()
        {
            if (trainingResults.Length > 0) return trainingResults;
            else if(File.Exists(lastTrainingResultsStringPath)) {
                trainingResults = File.ReadAllText(lastTrainingResultsStringPath);
                return trainingResults;
            }
            else return "No Previous Training Results Found";
        }

        public static TrainingResultsModel GetLastTrainResultsModel()
        {
            if (trainingResultsModel.Status == null && File.Exists(lastTrainingResultsJsonPath))
            {
                trainingResultsModel = JsonSerializer.Deserialize<TrainingResultsModel>(File.ReadAllText(lastTrainingResultsJsonPath));
                return trainingResultsModel;
            }

            return trainingResultsModel;
        }

        public static bool IsTrainingInProgress()
        {
            return training;
        }

        public static MLRatingPrediction Predict(MLDataModel dataModel, PredictionEnginePool<MLDataModel, MLRatingPrediction> predictionEnginePool)
        {
            return predictionEnginePool.Predict(modelName: modelName, dataModel);
        }

        public static string TrainAndBuildModel(MLDataModel[] DataSet)
        {
            training = true;

            try
            {
                double startTime = DateTime.UtcNow.TimeOfDay.TotalSeconds;

                trainingResults = "Model Training Request | " + DateTime.UtcNow.ToString() + " (UTC)\n";

                trainingResultsModel = new TrainingResultsModel();

                trainingResultsModel.StartTime = DateTime.UtcNow.ToString() + " (UTC)";

                trainingResultsModel.Status = TrainingResultsModel.STATUS_TRAINING;
                trainingResultsModel.Details = "Training is In Progress";
               
                IDataView dataView = LoadData(DataSet);
                TrainTestData trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: datasetSplitRatio);
                
                ITransformer model = BuildAndTrainModel(trainTestData.TrainSet);

                EvaluateModel(trainTestData.TestSet, model);

                SaveModel(trainTestData.TrainSet.Schema, model);

                double endTime = DateTime.UtcNow.TimeOfDay.TotalSeconds;
                trainingResults += "Total Training Time: " + String.Format("{0:0.00}", (endTime - startTime)) + "s";
                
                trainingResultsModel.TotalTime = String.Format("{0:0.00}", (endTime - startTime)) + "s";

                SaveLastTrainingResults();

                BackupDatasetCsv(dataView);
                BackupDatasetJson(DataSet);
            }
            catch (Exception e) {
                trainingResults += "\nModel Training Failed !";
                trainingResults += "\nError: " + e.Message;

                trainingResultsModel.Status = TrainingResultsModel.STATUS_ERROR;
                trainingResultsModel.Details = "Model Training Failed !";
            }

            training = false;
            
            return trainingResults;
        }

        private static IDataView LoadData(MLDataModel[] DataSet)
        {
            
            IDataView dataView = mlContext.Data.LoadFromEnumerable<MLDataModel>(DataSet);
            
            long dataRowsCount = dataView.GetRowCount().Value;

            trainingResults += "\nDataSet ("+ dataRowsCount + " Records | Split Ratio: "+ datasetSplitRatio + "): "+ "\nTrainig Data Records: " + dataRowsCount * (1-datasetSplitRatio) + "\nTesting Data Records: " + dataRowsCount*datasetSplitRatio + "\n";
            
            trainingResultsModel.DataSet.RecordsCount = dataRowsCount;
            trainingResultsModel.DataSet.SplitRatio = datasetSplitRatio;
            trainingResultsModel.DataSet.TrainRecordsCount = (long)(dataRowsCount * (1 - datasetSplitRatio));
            trainingResultsModel.DataSet.TestRecordsCount = (long)(dataRowsCount * datasetSplitRatio);

            return dataView;
        }

        private static ITransformer BuildAndTrainModel(IDataView trainingDataView)
        {
            IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "itemIdEncoded", inputColumnName: "itemId"));

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "itemIdEncoded",
                LabelColumnName = "Label",
                NumberOfIterations = numberOfIterations, //default 20
                LearningRate = learningRate, //default 0.1
                ApproximationRank = approximationRank //default 8
            };

            trainingResults += "\nTraining Configs:\nNumber Of Iterations: " + numberOfIterations + "\nLearning Rate: " + learningRate + "\nApproximation Rank: " + approximationRank + "\n";

            trainingResultsModel.TrainingConfigs.NumberOfIterations = numberOfIterations;
            trainingResultsModel.TrainingConfigs.LearningRate = learningRate;
            trainingResultsModel.TrainingConfigs.ApproximationRank = approximationRank;

            var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

            ITransformer model = trainerEstimator.Fit(trainingDataView);
            
            return model;
        }

        private static void EvaluateModel(IDataView testDataView, ITransformer model)
        {
            trainingResults += "\nTraining Metrices: \n";
            var prediction = model.Transform(testDataView);
            var trainingMetrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");
            
            trainingResults += "RMSE: " + trainingMetrics.RootMeanSquaredError.ToString() + " (Lower is Better)\n";
            trainingResults += "RSquared: " + trainingMetrics.RSquared.ToString() + " (Higher is Better , Max: 1.0)\n";

            trainingResultsModel.TrainingMetrices.Rmse = trainingMetrics.RootMeanSquaredError;
            trainingResultsModel.TrainingMetrices.RSquared = trainingMetrics.RSquared;

        }

        private static void SaveModel(DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            mlContext.Model.Save(model, trainingDataViewSchema, modelPath);
            
            trainingResults += "\nModel is Trained & Built Successfully\n";

            trainingResultsModel.Status = TrainingResultsModel.STATUS_COMPLETED;
            trainingResultsModel.Details = "Model is Trained & Built Successfully";
        }

        private static void BackupDatasetJson(MLDataModel[] DataSet)
        {
            File.WriteAllText(lastDatasetJsonPath, JsonSerializer.Serialize(DataSet));
            
        }

        private static void BackupDatasetCsv(IDataView dataset)
        {
            mlContext.Data.SaveAsText(dataset, File.CreateText(lastDatasetCsvPath).BaseStream, separatorChar: ',', headerRow: true, schema: false);
        }

        private static void SaveLastTrainingResults() {
            File.WriteAllText(lastTrainingResultsStringPath, trainingResults);
            File.WriteAllText(lastTrainingResultsJsonPath, JsonSerializer.Serialize(trainingResultsModel));
        }

    }
}
