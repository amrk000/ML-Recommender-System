![ML Recommender](https://user-images.githubusercontent.com/63168118/187040645-3a5ebd77-ef04-4f1a-befe-f62977987b3d.png)

# ML Recommender System (Engine)
### Machine Learning Recommender System with REST API. Can be used to recommend products, movies, books or any items based on users history.

## Details:
- ML Type: Collaborative Filtering (Supervised)
- ML Algorithm:
    - Matrix Factoriaztion: NMF Algorithm
    - Regression: Stochastic gradient descent (SGD)
- Swagger Docs provided for rest api
- Built using Microsoft ML.Net Framework & ASP.NET Core

---

## Basic Idea:
**Collaborative** Filtering model can recommend items to user as similar users are interested in that item unlike **Content Based** approach that needs to understand the characteristics of this item. Depending on our users’ items rating the recommender system can do data analysis to be able to predict a specific item rating value for the targeted user. By making multiple items prediction the system can decide what items are most likely to be liked by that user so they get high priority in recommendations.

<div align="center">
<img width="50%" height="50%" src="https://user-images.githubusercontent.com/63168118/187041424-622f0e22-10b6-487b-8296-f9084c6e910e.png"/>
</div>

---

## REST API:
![Screenshot 2022-08-27 193713](https://user-images.githubusercontent.com/63168118/187041628-4b331ae6-b467-4191-b1b5-89e3d4cd5c63.png)

The system contains 7 routes:
- ```Train:``` train and build ML Model using given dataset.
- ```LastTrainingResults:``` get the results of latest training done.
- ```LastTrainingDataset:``` get the data set used to build the current running model.
- ```Ping:``` to check Server.
- ```PredictSingle:``` predict the score of a single item for the target user.
- ```PredictMulti:``` predict the score of a multiple items and sort them for the target user.
- ```SendEmail:``` predict the score of a multiple items, sort them and send the top 5 to the target user via email (Based on SendinBlue Service).

---

## Training Dataset Schema:
-	User Id: (Long/Int64)
-	Item Id: (Long/Int64)
-	Label: (Float)

The training dataset represents a matrix of m (Users) X n (Items) with each score of an item for a specific user.

---

## Project Setup:
### - Machine Learning: ```MLManager.cs``` Class
```C#
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
        }
```

### - Email Client: ```SendinBlueEmailSender.cs``` Class
goto https://www.sendinblue.com/ to get API Key and add a sender email to use for email sending service then add your data in the client in project.
```C#
 //Email Recommendation Client Based on SendinBlue Service
    // https://www.sendinblue.com/

    //SendinBlue Resposne Codes

    //201 -> sent
    //429 -> maximum rate reached
    //401 -> unAuthorized
    //400 -> badRequest
    public static class SendinBlueEmailSender
    {
        private static HttpClient client;
        private static EmailRequestBody emailRequestBody { get; set; }

        //Replace with your data
        private const string apiKey = "YOUR_SendinBlue_API_KEY";
        private const string senderEmail = "SENDER_EMAIL";
        private const string senderName = "SENDER_NAME";
```

### - Email Template:
Edit ```EmailDesign.html``` and ```EmailItemDesign.html``` files to customize email style.

Note: Make sure to keep these variables: ```{items}, {itemTitle}, {itemData}, {itemImage}, {itemUrl}``` to be replaced with data.

---

## Postman Collection to test REST API with sample data:
### Included in Project Files [Here]("#")
