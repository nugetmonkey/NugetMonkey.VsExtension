//------------------------------------------------------------------------------
// <copyright file="FirstWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace NugetMonkeyVsExtension
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for FirstWindowControl.
    /// </summary>
    public partial class NugetMonkeyWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NugetMonkeyWindowControl"/> class.
        /// </summary>
        public NugetMonkeyWindowControl()
        {
            this.InitializeComponent();
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
            HttpClientRequestHandler c = new global::NugetMonkeyVsExtension.HttpClientRequestHandler();
            var r = c.GetReleases("http://search.maven.org/solrsearch/select?q=" + txtSearch.Text + "&rows=20&wt=json");
            grdSearchResults.ItemsSource = r.response.docs;
        }
    }
}