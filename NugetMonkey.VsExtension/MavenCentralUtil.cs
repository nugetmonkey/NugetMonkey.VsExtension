using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NugetMonkey.VsExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NugetMonkey.VsExtension
{
    public class MavenCentralUtil
    {
        private const string TEXT_SEARCH_LATEST_VERSION = "http://search.maven.org/solrsearch/select?q=g:\"{0}\"+AND+a:\"{1}\"+AND+v:\"\"&rows=1&wt=json";

        private static void ArrangeHeaders(HttpClient cl)
        {
            cl.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
        }
        public static RootObject GetReleases(string url)
        {
            using (var httpClient = new HttpClient())
            {
                ArrangeHeaders(httpClient);

                var response = httpClient.GetStringAsync(new Uri(url)).Result;
                return JsonConvert.DeserializeObject<RootObject>(response);
            }
        }
        public static List<Doc> GetAllVersions(string groupId, String artifactId)
        {
            var ret = new List<Doc>();
            var r = GetReleases(String.Format(TEXT_SEARCH_LATEST_VERSION, groupId, artifactId));
            var docs = r.response.docs;
            ret.AddRange(docs);
            return ret;
        }
        public static List<Doc> GetAllVersions(Doc doc)
        {
            var splits = ParseVersion(doc.id);
            return GetAllVersions(splits[0],splits[1]);
        }
        public static String[] ParseVersion(String id)
        {
            return id.Split(":".ToCharArray());
        }
        public static Doc GetLatestVersion(string groupId, String artifactId)
        {
            var docs = GetAllVersions(groupId, artifactId);
            if (docs.Count > 0)
            {
                return docs.First();
            }
            return null;
        }
        public static List<Doc> GetUpdates()
        {
            List<Doc> lstNew = new List<Doc>();
            AdditionalDeps deps = DependencyUtil.GetInstalledDependencies();
            if (deps != null && deps.AdditionalProjectDependencies != null)
            {
                foreach (string item in deps.AdditionalProjectDependencies)
                {
                    var splits = ParseVersion(item) ;
                    var doc = MavenCentralUtil.GetLatestVersion(splits[0], splits[1]);
                    if (doc != null)
                    {
                        var thereSplits = ParseVersion(doc.id) ;
                        if (thereSplits[2].ToLowerInvariant() != splits[2].ToLowerInvariant())
                        {
                            lstNew.Add(doc);
                        }
                    }
                }
            }
            return lstNew;
        }
    }
}
