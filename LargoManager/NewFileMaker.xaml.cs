// <copyright file="NewFileMaker.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.IO;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;
using LargoSharedClasses.Templates;
using LargoSharedControls.Abstract;
using ManagerPanels;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace LargoManager
{
    /// <summary>
    /// New File Maker.
    /// </summary>
    public partial class NewFileMaker 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewFileMaker"/> class.
        /// </summary>
        public NewFileMaker()
        {
            InitializeComponent();

            var settings = ManagerSettings.Singleton;
            var blockTemps = TemplateBlock.ReadTemplates(settings.PathToInternalTemplates, "TectonicTemplates.xml"); //// UserFileLoader.Singleton.LoadBlockTemplates();
            this.GridTemplates.ItemsSource = blockTemps;

            var streams = HarmonicStream.ReadStreams(settings.PathToInternalTemplates, "HarmonicTemplates.xml"); //// UserFileLoader.Singleton.LoadBlockTemplates();
            this.GridStreams.ItemsSource = streams;
        }

        /// <summary>
        /// Creates a new file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        public void NewFile(object sender, RoutedEventArgs e)
        {
            string filePath = string.Empty; //// @"c:\Temp\Test.mif";
            var internalPath = ManagerSettings.Singleton.PathToInternalTemplates;
            string tectonicTemplatePath = Path.Combine(internalPath, @"SelectedTectonicTemplate.xml");
            string harmonicTemplatePath = Path.Combine(internalPath, @"SelectedHarmonicTemplate.xml");

            var bt = this.GridTemplates.SelectedItem as TemplateBlock;
            if (bt == null) {
                MessageBox.Show("Select a tectonic template!");
                return;
            }

            this.SaveSelectedTectonicTemplate(tectonicTemplatePath);

            var st = this.GridStreams.SelectedItem as HarmonicStream;
            if (st == null) {
                MessageBox.Show("Select a harmonic template!");
                return;
            }

            this.SaveSelectedHarmonicTemplate(harmonicTemplatePath);

            CommonActions.Singleton.EditFile(filePath, tectonicTemplatePath, harmonicTemplatePath);
        }

        /// <summary>
        /// Saves the selected template.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <returns> Returns value. </returns>
        private string SaveSelectedTectonicTemplate(string templatePath)
        {
            var bt = this.GridTemplates.SelectedItem as TemplateBlock;
            var xtemplate = bt.GetXElement;
            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xtemplate);
            xdoc.Save(templatePath);
            return templatePath;
        }

        /// <summary>
        /// Saves the selected template.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <returns> Returns value. </returns>
        private string SaveSelectedHarmonicTemplate(string templatePath)
        {
            var bt = this.GridStreams.SelectedItem as HarmonicStream;
            var xtemplate = bt.GetXElement;
            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xtemplate);
            xdoc.Save(templatePath);
            return templatePath;
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the GridTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void GridTemplates_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Handles the SelectionChanged event of the GridTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the UnloadingRow event of the GridTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridRowEventArgs"/> instance containing the event data.</param>
        private void GridTemplates_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the GridStreams control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private void GridStreams_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Handles the SelectionChanged event of the GridStreams control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridStreams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the UnloadingRow event of the GridStreams control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridRowEventArgs"/> instance containing the event data.</param>
        private void GridStreams_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
        }

        /// <summary>
        /// Templates window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void TectonicTemplates(object sender, RoutedEventArgs e)
        {
            WindowManager.OpenWindow("LargoManager", "TemplatesUserTectonic", null);
        }

        /// <summary>
        /// Harmonics the template.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void TemplatesUserHarmonic(object sender, RoutedEventArgs e)
        {
            WindowManager.OpenWindow("LargoManager", "TemplatesUserHarmonic", null);
        }
    }
}
