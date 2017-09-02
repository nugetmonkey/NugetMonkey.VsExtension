using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NugetMonkey.VsExtension;
using System;
using System.Collections;
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

namespace NugetMonkeyVsExtension
{
    public class SelectionEventArgs : EventArgs
    {
        public List<Doc> Selection { get; set; }
    }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SearchGridUserControl : UserControl
    {
        #region constants
        private const string TEXT_SEARCH = "http://search.maven.org/solrsearch/select?q=\"{0}\"&rows=9999900&wt=json";
        #endregion
        #region fields
        public static readonly DependencyProperty IsSpinningProperty = DependencyProperty.Register("SearchMode", typeof(SearchMode), typeof(SearchGridUserControl));
        public event EventHandler<SelectionEventArgs> SelectedDependencyChanged;
        #endregion
        #region life
        public SearchGridUserControl()
        {
            InitializeComponent();
        }
        #endregion
        #region properties
        public SearchMode SearchMode
        {
            get { return (SearchMode)GetValue(IsSpinningProperty); }
            set { SetValue(IsSpinningProperty, value); }
        }
        #endregion
        #region utility
        public void RefreshControl()
        {
            ArrangeVisibility();
            switch (SearchMode)
            {
                case SearchMode.SEARCH:
                    break;
                case SearchMode.INSTALLED:
                    LoadInstalled();
                    break;
                case SearchMode.UPDATE:
                    LoadUpdates();
                    break;
            }
        }
        public void ArrangeVisibility()
        {
            switch (SearchMode)
            {
                case SearchMode.SEARCH:
                    btnInstall.Visibility = Visibility.Visible;
                    btnRefresh.Visibility = Visibility.Collapsed;
                    btnUnistall.Visibility = Visibility.Collapsed;
                    btnUpdate.Visibility = Visibility.Collapsed;
                    break;
                case SearchMode.INSTALLED:
                    btnInstall.Visibility = Visibility.Collapsed;
                    btnRefresh.Visibility = Visibility.Visible;
                    btnUnistall.Visibility = Visibility.Visible;
                    btnUpdate.Visibility = Visibility.Collapsed;
                    break;
                case SearchMode.UPDATE:
                    btnInstall.Visibility = Visibility.Collapsed;
                    btnRefresh.Visibility = Visibility.Visible;
                    btnUnistall.Visibility = Visibility.Collapsed;
                    btnUpdate.Visibility = Visibility.Visible;
                    break;
            }
        }
        public IEnumerable LoadDocs()
        { 
            var txt = txtSearch.Text;
            if (!string.IsNullOrWhiteSpace(txt))
            {
                switch (SearchMode)
                {
                    case SearchMode.SEARCH:
                        return MavenCentralUtil.GetReleases(String.Format(TEXT_SEARCH, txtSearch.Text)).response.docs;
                    case SearchMode.INSTALLED:
                        var deps = DependencyUtil.GetInstalledDependencies();
                        if (deps != null)
                        {
                            return deps.AdditionalProjectDependencies.Where(d => d.ToLowerInvariant().Contains(txt.ToLowerInvariant()));
                        }
                        return null;
                    case SearchMode.UPDATE:
                        return DependencyUtil.GetInstalledDependencies().AdditionalProjectDependencies.Where(d => d.ToLowerInvariant().Contains(txt.ToLowerInvariant()));
                }
            }
            else
            {
                MessageBox.Show("Please enter some texts");
            }
            return null;
        }
        private void LoadUpdates()
        {
            grdSearchResults.ItemsSource = MavenCentralUtil.GetUpdates();
        }
        private void LoadInstalled()
        {
            var deps = DependencyUtil.GetInstalledDependencies();
            if (deps != null)
            {
                grdSearchResults.ItemsSource = deps.AdditionalProjectDependencies.Select(d =>
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
        #endregion
        #region events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            grdSearchResults.ItemsSource = LoadDocs();
        }
        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            DependencyUtil.InstalDependencies(grdSearchResults.SelectedItems);
        }
        private void btnUninstall_Click(object sender, RoutedEventArgs e)
        {
            DependencyUtil.UninstallDependency(grdSearchResults.SelectedItems);
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            DependencyUtil.UpdateDependency(grdSearchResults.SelectedItems);
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshControl();
        }
        #endregion

        private void grdSearchResults_CurrentCellChanged(object sender, EventArgs e)
        {
            if (SelectedDependencyChanged != null)
            {
                var lst = new List<Doc>();
                var items = grdSearchResults.SelectedItems;
                if (  items!=null) { 
                    foreach (Doc item in items)
                    {
                        lst.Add(item);
                    }
                    
                }

                SelectedDependencyChanged.Invoke(this, new SelectionEventArgs() { Selection = lst });
            }
        }
    }
}
