using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebhookRegistrator
{
    class Program
    {
        static void Main(string[] args)
        {
            var tokenEndpoint = "https://d-bonee-identity.azurewebsites.net"; //identity server endpoint
            var username = ""; //user email
            var password = ""; //password
            var authorizationResponse = RequestTokenFromAuthorizationServer(new Uri(tokenEndpoint), username, password).Result;
            Console.WriteLine(authorizationResponse); //outputing the response

            Console.WriteLine();
            Console.WriteLine("Getting the access token");
            var bearerToken = JObject.Parse(authorizationResponse)["access_token"].ToString();
            Console.WriteLine("access token " + bearerToken);

            Console.WriteLine("Getting the refresh token");
            var refreshToken = JObject.Parse(authorizationResponse)["refresh_token"].ToString();
            Console.WriteLine("refresh token " + refreshToken);

            //after authorization token expiration getting the access token again
            //with refresh token
            Console.WriteLine();
            Console.WriteLine();

            var refreshTokenResponse = RequestRefreshTokenFromAuthorizationServer(new Uri(tokenEndpoint), refreshToken).Result;
            var accessToken = JObject.Parse(refreshTokenResponse)["access_token"].ToString();
            refreshToken = JObject.Parse(refreshTokenResponse)["refresh_token"].ToString();
            Console.WriteLine(accessToken);
            Console.WriteLine();
            Console.WriteLine(refreshToken);


            var TranslationsKeys = new List<string>();
            TranslationsKeys.Add("us");
            TranslationsKeys.Add("ru");
            TranslationsKeys.Add("arm");

            var MenuGroups = new List<MenuGroupWriteModel>
            {
                new MenuGroupWriteModel()
                {
                    Code = "myCode1",
                    Id = new Guid(""),
                    Name = "Group1",
                    ParentId = new Guid("")
                }
            };

            var request = new MenuGroup()
            {
                Language = "en",
                Groups = MenuGroups
            };


            SendProductRequestMessageToWebhook(request, accessToken);


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
                        new KeyValuePair<string, string>("scope", "openid iiko offline_access"),


                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password)
                    });

                tokenRequest.Content = httpContent;
                responseMessage = client.SendAsync(tokenRequest).Result;
            }
            return responseMessage.Content.ReadAsStringAsync().Result;
        }

        public static async Task<string> RequestRefreshTokenFromAuthorizationServer(Uri uriAuthorizationServerUri, string refreshToken)
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

        public static string SendProductRequestMessageToWebhook(MenuGroup menuGroup, string token)
        {
            string uri = "https://d-bonee-iiko.azurewebsites.net/api/Request/menugroup";

            HttpResponseMessage responseMessage = null;
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(menuGroup), Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                try
                {
                    responseMessage = httpClient.SendAsync(request).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception");
                }
            }

            return responseMessage.Content.ReadAsStringAsync().Result;
        }
    }
}
