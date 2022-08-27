using RecommenderEngine.DataModels.controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;

namespace RecommenderEngine
{
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

        static SendinBlueEmailSender() {
            
            client = new HttpClient();
            client.BaseAddress = new Uri("https://api.sendinblue.com/v3/smtp/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("api-key", apiKey);
        }

        private class EmailRequestBody {
            public TheSender Sender { get; set; }
            public Recievers[] To { get; set; }
  
            public String Subject { get; set; }
            public String HtmlContent { get; set; }

            public EmailRequestBody() {
                Sender = new TheSender();
                To = new Recievers[1];
                To[0] = new Recievers();
            }
            
            public class TheSender
            {
                public String Email { get; set; }
                public String Name { get; set; }
            }

            public class Recievers
            {
                public String Email { get; set; }
                public String Name { get; set; }
            }

        }

        public static HttpResponseMessage SendEmail(String userEmail, String userName,List<EmailItemModel> items) {
            emailRequestBody = new EmailRequestBody();
            emailRequestBody.Sender.Email = senderEmail;
            emailRequestBody.Sender.Name = senderName;
            emailRequestBody.To[0].Email = userEmail;
            emailRequestBody.To[0].Name = userName;
            emailRequestBody.Subject = "Recommended For You";
            emailRequestBody.HtmlContent = GenerateHtml(items);
 
            HttpResponseMessage response = client.PostAsJsonAsync("email", emailRequestBody).Result;

            return response;
        }

        private static String GenerateHtml(List<EmailItemModel> items) {
            String emailHtml = File.ReadAllText("EmailDesign.html");
            String itemHtml = File.ReadAllText("EmailItemDesign.html");

            String itemsListHtml = "";

            foreach (EmailItemModel item in items) {
                itemsListHtml += itemHtml.Replace("{itemTitle}", item.Name)
                                         .Replace("{itemData}", item.SubData)
                                         .Replace("{itemImage}", item.ImageUrl)
                                         .Replace("{itemUrl}", item.PageUrl);
                itemsListHtml += "\n";
            }

            emailHtml = emailHtml.Replace("{items}", itemsListHtml);

            return emailHtml;
        }
    }
}
