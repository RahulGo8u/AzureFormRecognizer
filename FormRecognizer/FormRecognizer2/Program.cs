using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FormRecognizer2
{
    public class SourceFilter
    {
        [JsonProperty("prefix")]
        public string prefix { get; set; }

        [JsonProperty("includeSubFolders")]
        public bool includeSubFolders { get; set; }
    }

    public class PostObject
    {
        [JsonProperty("sourceFilter")]
        public SourceFilter SourceFilter { get; set; }
        public bool useLabelFile { get; set; }
        public string source { get; set; }
    }
    public class PostChequeObject
    {
        public string source { get; set; }
    }

    public class Program
    {
        static string apiKey = "cca73eb1d2064462ba62d32598fcd2c5";
        static string imgUrl =@"https://signaturestorageacct.blob.core.windows.net/chequestorage/IMG_8097.JPG";
        static string endpointURI = @"https://adcbformrecognizer.cognitiveservices.azure.com/formrecognizer/v2.0/custom/models";
        static string sasURI = @"https://signaturestorageacct.blob.core.windows.net/chequestorage?sv=2019-12-12&ss=b&srt=sco&st=2020-11-17T04%3A31%3A06Z&se=2020-11-20T04%3A31%3A00Z&sp=rwftlcp&sig=sBSmbbd8wgyB%2ByS9IC8%2Fq3C%2F%2FzXMCkwUCsTWoxe6yxs%3D";
        static void Main(string[] args)
        {
            PerformFormRecognization();
            Console.ReadLine();
        }
        public async static void PerformFormRecognization()
        {
            var uri = await TrainModel();
            var customModel = await GetCustomModel(uri);
            //var uri = @"https://adcbformrecognizer.cognitiveservices.azure.com/formrecognizer/v2.0/custom/models/8ea466e9-48d2-48bf-a444-4101258a823d";
            var analyzeURI = await AnalyzeForm(uri);
            var responseText = await AnalyzeFormResult(analyzeURI);
            Console.WriteLine("************");
            Console.WriteLine(responseText);
            Console.WriteLine("************");
            Console.ReadLine();
        }
        public static async Task<string> TrainModel()
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
            {
                PostObject postObject = new PostObject
                {
                    source = sasURI,
                    SourceFilter = new SourceFilter
                    {
                        includeSubFolders = false,
                        prefix = ""
                    },
                    useLabelFile = false
                };

                var myContent = JsonConvert.SerializeObject(postObject);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var builder = new UriBuilder(new Uri(endpointURI));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
                request.Content = new StringContent(myContent, Encoding.UTF8, "application/json");//CONTENT-TYPE header

                HttpResponseMessage response = await client.SendAsync(request);
                Thread.Sleep(2000);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var location = response.Headers.Location.ToString();
                    return location;
                }
            };
            return "";
        }

        public static async Task<string> GetCustomModel(string uri)
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var builder = new UriBuilder(new Uri(uri));

                HttpResponseMessage response = await client.GetAsync(builder.Uri);
                response.EnsureSuccessStatusCode();
                var responseBodyAsText = await response.Content.ReadAsStringAsync();

                Thread.Sleep(2000);
            };
            return "";
        }

        public static async Task<string> AnalyzeForm(string uri)
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
            {
                PostChequeObject postObject = new PostChequeObject
                {
                    source = imgUrl,
                };

                var myContent = JsonConvert.SerializeObject(postObject);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var builder = new UriBuilder(new Uri(uri + "/analyze"));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
                request.Content = new StringContent(myContent, Encoding.UTF8, "application/json");//CONTENT-TYPE header

                HttpResponseMessage response = await client.SendAsync(request);
                Thread.Sleep(2000);
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    foreach (var item in response.Headers)
                    {
                        if (item.Key.Equals("Operation-Location"))
                        {
                            return item.Value.FirstOrDefault().ToString();
                        }
                    }
                }
            };
            return "";
        }
        public static async Task<string> AnalyzeFormResult(string uri)
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var builder = new UriBuilder(new Uri(uri));

                HttpResponseMessage response = await client.GetAsync(builder.Uri);
                Thread.Sleep(2000);
                response.EnsureSuccessStatusCode();
                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                return responseBodyAsText;
            };
        }

    }
}