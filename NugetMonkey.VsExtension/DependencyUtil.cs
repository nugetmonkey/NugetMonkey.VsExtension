using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NugetMonkeyVsExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NugetMonkey.VsExtension
{
    public class DependencyUtil
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        private const string TEXT_DEPS_FILE = "AdditionalJavaDependencies.json";
        public static AdditionalDeps GetInstalledDependencies()
        {
            var project = VSUtils.GetSelectedProject();
            if (project != null)
            {
                string filePath = project.Properties.Item("FullPath").Value.ToString();
                var depFile = filePath + "\\" + TEXT_DEPS_FILE;
                if (File.Exists(depFile))
                {
                    return JsonConvert.DeserializeObject<AdditionalDeps>(File.ReadAllText(depFile), settings);
                }
            }
            else
            {
                MessageBox.Show("No project is selected!");
            }
            return new AdditionalDeps
            {
                AdditionalProjectDependencies = new List<string>()
            };
        }
        public static void InstalDependencies(IEnumerable items)
        {
            var project = VSUtils.GetSelectedProject();
            if (project != null && items != null)
            {
                AdditionalDeps deps = DependencyUtil.GetInstalledDependencies();
                string filePath = project.Properties.Item("FullPath").Value.ToString();
                var depFile = filePath + "\\" + TEXT_DEPS_FILE;
                foreach (Doc item in items)
                {
                    if (!deps.AdditionalProjectDependencies.Where(d => d.ToLowerInvariant().StartsWith(item.id.ToLowerInvariant() + ":")).Any())
                    {
                        deps.AdditionalProjectDependencies.Add(item.id + ":" + item.latestVersion);
                    }
                }
                File.WriteAllText(depFile, JsonConvert.SerializeObject(deps, settings));
            }
            else
            {
                MessageBox.Show("No project is selected!");
            }
        }
        public static void UninstallDependency(IEnumerable items)
        {
            var project = VSUtils.GetSelectedProject();
            if (project != null && items != null)
            {
                AdditionalDeps deps = DependencyUtil.GetInstalledDependencies();
                string filePath = project.Properties.Item("FullPath").Value.ToString();
                var depFile = filePath + "\\" + TEXT_DEPS_FILE;

                foreach (String item in items)
                {
                    deps.AdditionalProjectDependencies.RemoveAll(d => d.ToLowerInvariant() == item.ToLowerInvariant());
                }
                File.WriteAllText(depFile, JsonConvert.SerializeObject(deps, settings));
            }
            else
            {
                MessageBox.Show("No project is selected!");
            }
        }
        public static void UpdateDependency(IEnumerable items)
        {
            var project = VSUtils.GetSelectedProject();
            if (project != null && items != null)
            {
                AdditionalDeps deps = DependencyUtil.GetInstalledDependencies();
                foreach (Doc item in items)
                {
                    deps.AdditionalProjectDependencies.RemoveAll(d => d.ToLowerInvariant().StartsWith((item.g + ":" + item.a + ":").ToLowerInvariant()));
                }

                foreach (Doc item in items)
                {
                    if (!deps.AdditionalProjectDependencies.Where(d => d.ToLowerInvariant().StartsWith(item.id.ToLowerInvariant())).Any())
                    {
                        deps.AdditionalProjectDependencies.Add(item.id);
                    }
                }
                string filePath = project.Properties.Item("FullPath").Value.ToString();
                var depFile = filePath + "\\" + TEXT_DEPS_FILE;
                File.WriteAllText(depFile, JsonConvert.SerializeObject(deps, settings));
            }
            else
            {
                MessageBox.Show("No project is selected!");
            }
        }
    }
}
