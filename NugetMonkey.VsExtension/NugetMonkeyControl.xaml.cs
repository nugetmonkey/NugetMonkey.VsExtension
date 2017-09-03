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
    /// Interaction logic for NugetMonkeyControl.xaml
    /// </summary>
    public partial class NugetMonkeyControl : UserControl
    {
        public NugetMonkeyControl()
        {
            InitializeComponent();
        }

        private void OnUpdatesTabSelected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab != null)
            {
                ctlUpdates.RefreshControl();
            }
        }
        private void OnInstalledTabSelected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab != null)
            {
                ctlInstalled.RefreshControl();
            }
        }

        private void ctlSearch_SelectedDependencyChanged(object sender, SelectionEventArgs e)
        {
            ctlDetails.SelectedDependency = e.Selection.FirstOrDefault();
        }
    }
}
