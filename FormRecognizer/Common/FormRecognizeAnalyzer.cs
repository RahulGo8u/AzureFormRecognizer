using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static class FormRecognizeAnalyzer
    {
        public async static Task<List<ResultDTO>> PerformFormRecognization()
        {
            List<ResultDTO> lstResultDTO = new List<ResultDTO>();            
            var analyzeURI = await AnalyzeForm(ConfigurationConstants.uri);
            if (!(string.IsNullOrEmpty(analyzeURI)))
            {
                while (true)
                {
                    var responseText = await AnalyzeFormResult(analyzeURI);
                    if (responseText.Contains("succeeded"))
                    {
                        lstResultDTO = ParseResponse(responseText);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }            
            return lstResultDTO;
        }

        private static List<ResultDTO> ParseResponse(string responseText)
        {
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseText);
            List<ResultDTO> lstResultDTO = new List<ResultDTO>();
            if (myDeserializedClass.status.Equals("succeeded"))
            {
                foreach (var docFields in myDeserializedClass.analyzeResult.documentResults)
                {
                    foreach (var prop in docFields.fields.GetType().GetProperties())
                    {
                        var resultDTO = new ResultDTO();
                        resultDTO.Name = prop.Name;
                        dynamic propValue = prop.GetValue(docFields.fields, null);
                        if (propValue != null)
                        {
                            resultDTO.Value = propValue.text;
                            double confidence = propValue.confidence * 100;
                            resultDTO.Confidence = confidence.ToString();
                        }                       
                        lstResultDTO.Add(resultDTO);
                    }

                }
            }            
            return lstResultDTO;
        }
        public static async Task<string> AnalyzeForm(string uri)
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
            {
                PostChequeObject postObject = new PostChequeObject
                {
                    source = ConfigurationConstants.imgUrl,
                };

                var myContent = JsonConvert.SerializeObject(postObject);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationConstants.apiKey);
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
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationConstants.apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var builder = new UriBuilder(new Uri(uri));
                HttpResponseMessage response = await client.GetAsync(builder.Uri);
                //Thread.Sleep(2000);
                response.EnsureSuccessStatusCode();
                //Thread.Sleep(2000);
                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                return responseBodyAsText;
            };
        }
    }
}
