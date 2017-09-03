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
    public partial class DependencyDetailsControl : UserControl
    {
        #region life
        public DependencyDetailsControl()
        {
            InitializeComponent();
        }
        #endregion
        #region fields
        public static readonly DependencyProperty selectedDependency = DependencyProperty.Register("SelectedDependency", typeof(Doc), typeof(DependencyDetailsControl));
        #endregion
        #region properties
        public Doc SelectedDependency
        {
            get { return (Doc)GetValue(selectedDependency); }
            set
            {
                SetValue(selectedDependency, value);
                RefreshControl();
            }
        }
        #endregion
        #region utility
        public void RefreshControl()
        {
            var dep = SelectedDependency;

            if (dep != null)
            {
                var docs = MavenCentralUtil.GetAllVersions(dep);
                cmbOtherVersions.ItemsSource = docs;
                cmbOtherVersions.SelectedItem = dep;
                txtInstalledVersion.Text = "";
                var splits = MavenCentralUtil.ParseVersion(dep.id);
                AdditionalDeps deps = DependencyUtil.GetInstalledDependencies();
                if (deps != null && deps.AdditionalProjectDependencies != null)
                {
                    var installed = deps.AdditionalProjectDependencies.Where(d =>
                    {

                        var curSplits = MavenCentralUtil.ParseVersion(d );
                        return curSplits[0] == splits[0] && curSplits[1] == splits[1];
                    });
                    if (installed.Any())
                    {
                        var curSplits = MavenCentralUtil.ParseVersion(installed.First());
                        txtInstalledVersion.Text = curSplits[2];
                    }
                }
            }
            else
            {
                cmbOtherVersions.ItemsSource = null;
                cmbOtherVersions.SelectedItem = null;
                txtInstalledVersion.Text = "";
            }
        }
        #endregion
    }
}
