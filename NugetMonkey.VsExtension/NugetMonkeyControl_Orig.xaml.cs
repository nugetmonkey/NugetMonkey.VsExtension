using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NugetMonkey.VsExtension;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NugetMonkey.VsExtension
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NugetMonkeyControl_Orig : UserControl
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        public NugetMonkeyControl_Orig()
        {
            InitializeComponent(); 
        }

        private void LoadUpdates()
        {
            var project = VSUtils.GetSelectedProject();
            if (project != null)
            {
                string filePath = project.Properties.Item("FullPath").Value.ToString();
                var depFile = filePath + "\\" + TEXT_DEPS_FILE;
                AdditionalDeps deps;
                if (File.Exists(depFile))
                {
                    deps = JsonConvert.DeserializeObject<AdditionalDeps>(File.ReadAllText(depFile), settings);
                    if (deps.AdditionalProjectDependencies != null)
                    {
                        List<Doc> lstNew = new List<Doc>();
                        foreach (string item in deps.AdditionalProjectDependencies)
                        {
                            var splits = item.Split(":".ToCharArray()); 
                            var r = MavenCentralUtil.GetReleases(String.Format(TEXT_SEARCH_LATEST_VERSION, splits[0], splits[1]));
                            var docs = r.response.docs;
                            if (docs.Count>0)
                            {
                                var thereSplits = docs[0].id.Split(":".ToCharArray());
                                if (thereSplits[2].ToLowerInvariant() != splits[2].ToLowerInvariant()) {
                                    lstNew.Add(docs[0]);
                                }
                            } 
                        }
                        grdUpdatesSearchResults.ItemsSource = lstNew;
                    }
                } 
            }
            else
            {
                MessageBox.Show("No project is selected!");
            } 
        }
        private void OnUpdatesTabSelected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab != null)
            {
                LoadUpdates();
            }
        }
        private void OnInstalledTabSelected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab != null)
            {
                LoadInstalled();
            }
        }

        private void LoadInstalled()
        {
            var deps = GetInstalledDeps();
            if (deps != null)
            {
                grdInstalledSearchResults.ItemsSource = deps.AdditionalProjectDependencies.Select(d =>
                {
                    var splits = d.Split(":".ToCharArray());
                    return new
                    {
                        id = splits[0] + ":" + splits[1],
                        a = splits[1],
                        g = splits[0],
                        v = splits[2]
                    };
                });
            } 
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "NugetMonkey");
        }
        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var txt = txtSearch.Text;
            if (!string.IsNullOrWhiteSpace(txt))
            { 
                var r = MavenCentralUtil.GetReleases(String.Format(TEXT_SEARCH,txtSearch.Text));
                grdSearchResults.ItemsSource = r.response.docs;
            }else
            {
                MessageBox.Show("Please enter some texts");
            }
        }
        private const string TEXT_DEPS_FILE = "AdditionalJavaDependencies.json";
        private const string TEXT_SEARCH_LATEST_VERSION = "http://search.maven.org/solrsearch/select?q=g:\"{0}\"+AND+a:\"{1}\"+AND+v:\"\"&rows=1&wt=json";
        private const string TEXT_SEARCH = "http://search.maven.org/solrsearch/select?q=\"{0}\"&rows=9999900&wt=json";
        public static AdditionalDeps GetInstalledDeps()
        {
            var project = VSUtils.GetSelectedProject();
            if (project != null)
            {
                string filePath = project.Properties.Item("FullPath").Value.ToString();
                var depFile = filePath + "\\" + TEXT_DEPS_FILE;
                AdditionalDeps deps;
                if (File.Exists(depFile))
                {
                    deps = JsonConvert.DeserializeObject<AdditionalDeps>(File.ReadAllText(depFile), settings);
                    if (deps.AdditionalProjectDependencies == null)
                    {
                        deps.AdditionalProjectDependencies = new List<string>();
                    }
                }
                else
                {
                    deps = new AdditionalDeps() { AdditionalProjectDependencies = new List<string>() };
                }
                return deps;
            }
            else
            {
                MessageBox.Show("No project is selected!");
            }
            return null;
        }
        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            var project = VSUtils.GetSelectedProject();
            if (project!=null)
            {
                AdditionalDeps deps = GetInstalledDeps();
                string filePath = project.Properties.Item("FullPath").Value.ToString();
                var depFile = filePath + "\\" + TEXT_DEPS_FILE;

                var items = grdSearchResults.SelectedItems;
                if (items != null)
                {
                    foreach (Doc item in items)
                    {
                        if (!deps.AdditionalProjectDependencies.Where(d => d.ToLowerInvariant().StartsWith(item.id.ToLowerInvariant() + ":")).Any())
                        {
                            deps.AdditionalProjectDependencies.Add(item.id + ":" + item.latestVersion);
                        }
                    }
                    File.WriteAllText(depFile, JsonConvert.SerializeObject(deps, settings));
                }
            }
            else
            {
                MessageBox.Show("No project is selected!");
            }
        }
        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void btnInstalledSearch_Click(object sender, RoutedEventArgs e)
        {
            var txt = txtInstalledSearch.Text;
            var deps = GetInstalledDeps();
            if (!string.IsNullOrWhiteSpace(txt) && deps!=null)
            {
                grdInstalledSearchResults.ItemsSource = deps.AdditionalProjectDependencies.Where(d=>d.ToLowerInvariant().Contains(txt.ToLowerInvariant()));
            }else
            {
                MessageBox.Show("Please enter some texts");
            }
        }
        
        private void btnInstalledUninstall_Click(object sender, RoutedEventArgs e)
        {
            var project = VSUtils.GetSelectedProject();
            if (project != null)
            {
                AdditionalDeps deps = GetInstalledDeps();
                string filePath = project.Properties.Item("FullPath").Value.ToString();
                var depFile = filePath + "\\" + TEXT_DEPS_FILE;

                var items = grdInstalledSearchResults.SelectedItems;
                if (items != null)
                {
                    foreach (String item in items)
                    {
                        deps.AdditionalProjectDependencies.RemoveAll(d => d.ToLowerInvariant() == item.ToLowerInvariant());
                    }
                    File.WriteAllText(depFile, JsonConvert.SerializeObject(deps, settings));
                }
            }
            else
            {
                MessageBox.Show("No project is selected!");
            }
        }
        private void btnUpdatesUpdate_Click(object sender, RoutedEventArgs e)
        {
            var project = VSUtils.GetSelectedProject();
            if (project != null)
            {
                AdditionalDeps deps = GetInstalledDeps();
                string filePath = project.Properties.Item("FullPath").Value.ToString();
                var depFile = filePath + "\\" + TEXT_DEPS_FILE;

                var items = grdUpdatesSearchResults.SelectedItems;
                if (items != null)
                {
                    foreach (Doc item in items)
                    {
                        deps.AdditionalProjectDependencies.RemoveAll(d => d.ToLowerInvariant().StartsWith((item.g + ":" + item.a + ":").ToLowerInvariant()  ));
                    }

                    foreach (Doc item in items)
                    {
                        if (!deps.AdditionalProjectDependencies.Where(d => d.ToLowerInvariant().StartsWith(item.id.ToLowerInvariant()  )).Any())
                        {
                            deps.AdditionalProjectDependencies.Add(item.id );
                        }
                    }
                    File.WriteAllText(depFile, JsonConvert.SerializeObject(deps, settings));
                }
            }
            else
            {
                MessageBox.Show("No project is selected!");
            }
        }
        private void btnUpdatesRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUpdates();
        }
        private void btnInstalledRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadInstalled();
        }
        private void btnUpdatesSearch_Click(object sender, RoutedEventArgs e)
        {
            var txt = txtUpdatesSearch.Text;
            if (!string.IsNullOrWhiteSpace(txt))
            {
                grdUpdatesSearchResults.ItemsSource = GetInstalledDeps().AdditionalProjectDependencies.Where(d => d.ToLowerInvariant().Contains(txt.ToLowerInvariant()));
            }
            else
            {
                MessageBox.Show("Please enter some texts");
            }
        }
    }
}
