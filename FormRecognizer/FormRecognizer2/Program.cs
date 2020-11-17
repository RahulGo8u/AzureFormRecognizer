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
    public class Program
    {
        static string apiKey = "cca73eb1d2064462ba62d32598fcd2c5";
        static string imgUrl =@"https://signaturestorageacct.blob.core.windows.net/chequestorage/IMG_8097.JPG";
        static string endpointURI = @"https://adcbformrecognizer.cognitiveservices.azure.com/formrecognizer/v2.1-preview.1/custom/models";
        static string sasURI = @"https://signaturestorageacct.blob.core.windows.net/chequestorage?sv=2019-12-12&ss=b&srt=sco&st=2020-11-16T18%3A52%3A04Z&se=2020-11-17T18%3A52%3A04Z&sp=rwl&sig=bZGWoGGMEu%2FsaZWLH90incofFRtZWgN5bEFKLn4DEPQ%3D";
        static string uri;
        static bool trainModel = false;
        static void Main(string[] args)
        {
            PerformFormRecognization();
            Console.ReadLine();
        }
        public async static void PerformFormRecognization()
        {
            string responseText="";
            if (trainModel)
            {
                uri = await TrainModel();
                var customModel = await GetCustomModel(uri);
            }
            else
            {
                string modelid = "913add5d-6361-4327-8bbd-ea4d535983d8";
                uri = @"https://adcbformrecognizer.cognitiveservices.azure.com/formrecognizer/v2.1-preview.1/custom/models/" + modelid;
            }            
            var analyzeURI = await AnalyzeForm(uri);
            if (!(string.IsNullOrEmpty(analyzeURI)))
            {
                while (true)
                {
                    responseText = await AnalyzeFormResult(analyzeURI);
                    if (responseText.Contains("succeeded"))
                    {
                        ParseResponse(responseText);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }               
            }
            else
            {
                Console.WriteLine("Error on Analyzing Form. URI is empty");
            }
            Console.ReadLine();
        }

        private static void ParseResponse(string responseText)
        {
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseText);
            if (myDeserializedClass.status.Equals("succeeded"))
            {
                foreach (var docFields in myDeserializedClass.analyzeResult.documentResults)
                {
                    Console.WriteLine("Attribute Details from Cheque");
                    foreach (var prop in docFields.fields.GetType().GetProperties())
                    {
                        Console.WriteLine("Attribute Name: {0}", prop.Name);
                        dynamic propValue = prop.GetValue(docFields.fields, null);
                        if (propValue != null)
                        {
                            Console.WriteLine("Text: {0}", propValue.text);
                            double confidence = propValue.confidence * 100;
                            Console.WriteLine("Confidence: {0}%", confidence);
                        }
                        else
                        {
                            Console.WriteLine("Attribute Details are not available from OCR");
                        }
                        Console.WriteLine("************");
                    }
                }
            }
            else
            {
                Console.WriteLine("Error while fetching response text. Please try again");
            }            
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
                    useLabelFile = true
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
                Thread.Sleep(2000);
                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                return responseBodyAsText;
            };
        }
    }
}