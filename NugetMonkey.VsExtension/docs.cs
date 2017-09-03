using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetMonkey.VsExtension
{
    public class Params
    {
        public string spellcheck { get; set; }
        public string fl { get; set; }
        public string sort { get; set; }
        public string indent { get; set; }
        public string q { get; set; }
        public string qf { get; set; }
        [JsonProperty("Feedback_IM&SR")]
        public string spellcheckCount { get; set; }
        public string wt { get; set; }
        public string rows { get; set; }
        public string version { get; set; }
        public string defType { get; set; }
    }

    public class ResponseHeader
    {
        public int status { get; set; }
        public int QTime { get; set; }
        public Params @params { get; set; }
    }

    public class Doc
    {
        public string id { get; set; }
        public string g { get; set; }
        public string a { get; set; }
        public string latestVersion { get; set; }
        public string repositoryId { get; set; }
        public string p { get; set; }
        public object timestamp { get; set; }
        public int versionCount { get; set; }
        public List<string> text { get; set; }
        public List<string> ec { get; set; }
    }

    public class Response
    {
        public int numFound { get; set; }
        public int start { get; set; }
        public List<Doc> docs { get; set; }
    }

    public class Spellcheck
    {
        public List<object> suggestions { get; set; }
    }

    public class RootObject
    {
        public ResponseHeader responseHeader { get; set; }
        public Response response { get; set; }
        public Spellcheck spellcheck { get; set; }
    }
}
