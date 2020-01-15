using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebhookRegistrator
{
    class Program
    {
    //    public var _clientScopes = new string[] {
    //    "openid',
    //    "offline_access',
    //    "profile',
    //    "accounting',
    //    "virtualcard',
    //    "client',
    //    "partner',
    //    "locations',
    //    "filestore',
    //    "userprovider',
    //    "cardgenerator',
    //    "advertisement',
    //    "loyaltyservices',
    //    "catalog',
    //    "translation',
    //    "talkurl',
    //    "orders',
    //    "feedback',
    //    "basket',
    //    "orders.signalrhub',
    //    "hotactions',
    //    "subscriptions',
    //    "imagecloud',
    //    "smarttranslate',
    //    "payment',
    //    "webhook',
    //    "webboneeagg'
    //};
        static void Main(string[] args)
        {
            var token = RequestTokenFromAuthorizationServer(new Uri("http://localhost:52489/"), "arthar9613@gmail.com", "Mersoft1*").Result;
            Console.WriteLine(token); //getting authorization token with refresh token
            
            //updating authorization token with refresh token
            var tokenFromRefreshToken = RequestRefreshTokenFromAuthorizationServer(new Uri("http://localhost:52489/"), "arthar9613@gmail.com", "Mersoft1*", "137054b208d5ee245ae1826e622422604952501cc5b3856c6618f899136f3fc5").Result;
            var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjZCN0FDQzUyMDMwNUJGREI0RjcyNTJEQUVCMjE3N0NDMDkxRkFBRTEiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJhM3JNVWdNRnY5dFBjbExhNnlGM3pBa2ZxdUUifQ.eyJuYmYiOjE1NzkwOTEzODUsImV4cCI6MTU3OTE3Nzc4NSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjQ4OSIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjUyNDg5L3Jlc291cmNlcyIsImNhdGFsb2ciLCJmaWxlc3RvcmUiXSwiY2xpZW50X2lkIjoidmNtY2xpZW50Iiwic3ViIjoiNGYwYjkwY2EtMWNhYS00ODRkLTg5OTMtNDg4Zjk3MTU5MWVmIiwiYXV0aF90aW1lIjoxNTc5MDkxMzg1LCJpZHAiOiJsb2NhbCIsInByZWZlcnJlZF91c2VybmFtZSI6ImFydGhhcjk2MTNAZ21haWwuY29tIiwidHlwZSI6IjIiLCJ0ZWFtIjoiNGYwYjkwY2EtMWNhYS00ODRkLTg5OTMtNDg4Zjk3MTU5MWVmIiwiZW1haWwiOiJhcnRoYXI5NjEzQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJyb2xlIjoiUGFydG5lciIsInNjb3BlIjpbImNhdGFsb2ciLCJmaWxlc3RvcmUiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsicHdkIl19.hPMphSdZ3o6slc - DypOdAR8ieMbidl4PvJ38EdL - QSdt6D9RGPULdCiX4JPKK3N0qAfO0XneTFHMbqJLKT - uHutljHYGsGcMkckfJpsqyIIjVCaObsPloF6 - rK0EjK4kuO_gkIZkXwqAZVIpVuKnKOSeOi51o2qyOjZcdNJOwlg1QRZ41XiAgyDvtRdlV - yDQfT4oqPiin72cKsFtRXcM2ZpP7I4_m9HAxF92c4v2uQ2aBYHSVb5tSCR9FbRDFFZ38kpgd5OJr89XG1LVcBySxCs4sKIvz3qnnDboet4RGxvcuXfWlgTHIaf - 9BW8oTzgNbU030dvUwn1jTLuwA1Ig";


            var TranslationsKeys = new List<string>();
            TranslationsKeys.Add("us");
            TranslationsKeys.Add("ru");

            var request = new ClientRequestReadModel()
            {
                ItemId = new Guid("0AB82EF6-D357-48B6-A13C-009F606C2079"),
                Translations = TranslationsKeys
            };
            var a = SendRequestMessageToWebhook(request, token);

            Console.WriteLine(a);

            Thread.Sleep(50000);
        }
        public static async Task<string> RequestTokenFromAuthorizationServer(Uri uriAuthorizationServerUri, string username, string password)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            using (HttpClient client = new HttpClient())
            {
                // returns DiscoveryResponse 
                client.BaseAddress = uriAuthorizationServerUri;
                var disco = client.GetDiscoveryDocumentAsync(uriAuthorizationServerUri.ToString()).Result;
                HttpRequestMessage tokenRequest = new HttpRequestMessage(HttpMethod.Post, disco.TokenEndpoint);
                //tokenRequest.Properties = new IDictionary<string, string>();
                HttpContent httpContent = new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("client_id", "vcmclient"),
                        new KeyValuePair<string, string>("scope", "filestore catalog offline_access"),
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password)
                    });
                
                tokenRequest.Content = httpContent;
                responseMessage =  client.SendAsync(tokenRequest).Result;
            }
            return responseMessage.Content.ReadAsStringAsync().Result;
        }

        public static async Task<string> RequestRefreshTokenFromAuthorizationServer(Uri uriAuthorizationServerUri, string username, string password, string refreshToken)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = uriAuthorizationServerUri;
                var disco = client.GetDiscoveryDocumentAsync(uriAuthorizationServerUri.ToString()).Result;
                HttpRequestMessage tokenRequest = new HttpRequestMessage(HttpMethod.Post, disco.TokenEndpoint);
                HttpContent httpContent = new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("client_id", "vcmclient"),
                        new KeyValuePair<string, string>("refresh_token", refreshToken),
                     });

                tokenRequest.Content = httpContent;
                responseMessage = client.SendAsync(tokenRequest).Result;
            }
            return responseMessage.Content.ReadAsStringAsync().Result;
        }

        public class ClientRequestReadModel
        {
            public Guid ItemId { get; set; }
            public List<string> Translations { get; set; }
        }
        public static  string SendMessage(ClientRequestReadModel message, string token)
        {
            string uri = "http://localhost:57424/api/v1/WebhookCatalog/GetWebhookProduct";

            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(message), UnicodeEncoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                responseMessage = httpClient.SendAsync(request).Result;
            }

            return responseMessage.Content.ReadAsStringAsync().Result;
        }
    }
}
