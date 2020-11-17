using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SelectionMark
    {
        public List<int> boundingBox { get; set; }
        public double confidence { get; set; }
        public string state { get; set; }
    }

    public class ReadResult
    {
        public int page { get; set; }
        public double angle { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string unit { get; set; }
        public List<SelectionMark> selectionMarks { get; set; }
    }

    public class PageResult
    {
        public int page { get; set; }
        public List<object> tables { get; set; }
    }

    public class Payee
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class AmountInAED
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class ChequeDate
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class MICR
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class AmountInText
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class TestAccount
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class AccountNo
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class BranchName
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string text { get; set; }
        public int page { get; set; }
        public List<double> boundingBox { get; set; }
        public double confidence { get; set; }
    }

    public class Fields
    {
        public Payee Payee { get; set; }
        public AmountInAED AmountInAED { get; set; }
        public ChequeDate ChequeDate { get; set; }
        public MICR MICR { get; set; }
        public AmountInText AmountInText { get; set; }
        public TestAccount TestAccount { get; set; }
        public AccountNo AccountNo { get; set; }
        public BranchName BranchName { get; set; }
        //public object ChequeNo { get; set; }
        //public object POBoxNo { get; set; }
    }

    public class DocumentResult
    {
        public string docType { get; set; }
        public string modelId { get; set; }
        public List<int> pageRange { get; set; }
        public Fields fields { get; set; }
        public double docTypeConfidence { get; set; }
    }

    public class AnalyzeResult
    {
        public string version { get; set; }
        public List<ReadResult> readResults { get; set; }
        public List<PageResult> pageResults { get; set; }
        public List<DocumentResult> documentResults { get; set; }
        public List<object> errors { get; set; }
    }

    public class Root
    {
        public string status { get; set; }
        public DateTime createdDateTime { get; set; }
        public DateTime lastUpdatedDateTime { get; set; }
        public AnalyzeResult analyzeResult { get; set; }
    }


    //Body Class of Request
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
    public class SourceFilter
    {
        [JsonProperty("prefix")]
        public string prefix { get; set; }

        [JsonProperty("includeSubFolders")]
        public bool includeSubFolders { get; set; }
    }




}
