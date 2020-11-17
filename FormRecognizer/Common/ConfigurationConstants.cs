using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ConfigurationConstants
    {
        public static string apiKey = "cca73eb1d2064462ba62d32598fcd2c5";
        public static string imgUrl = @"https://signaturestorageacct.blob.core.windows.net/chequestorage/IMG_8097.JPG";
        public static string endpointURI = @"https://adcbformrecognizer.cognitiveservices.azure.com/formrecognizer/v2.1-preview.1/custom/models";
        public static string sasURI = @"https://signaturestorageacct.blob.core.windows.net/chequestorage?sv=2019-12-12&ss=b&srt=sco&st=2020-11-16T18%3A52%3A04Z&se=2020-11-17T18%3A52%3A04Z&sp=rwl&sig=bZGWoGGMEu%2FsaZWLH90incofFRtZWgN5bEFKLn4DEPQ%3D";
        public static bool trainModel = false;
        public static string modelid = "913add5d-6361-4327-8bbd-ea4d535983d8";
        public static string uri = @"https://adcbformrecognizer.cognitiveservices.azure.com/formrecognizer/v2.1-preview.1/custom/models/" + modelid;
    }
}
