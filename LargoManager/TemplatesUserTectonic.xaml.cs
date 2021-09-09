// <copyright file="TemplatesUserTectonic.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using LargoSharedClasses.Templates;

namespace LargoManager
{
    /// <summary>
    /// Templates Window.
    /// </summary>
    public partial class TemplatesUserTectonic : WinAbstract
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatesUserTectonic"/> class.
        /// </summary>
        public TemplatesUserTectonic() {
            this.InitializeComponent();
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalTemplates);
            var blockTemps = TemplateBlock.ReadTemplates(path, "TectonicTemplates.xml"); //// UserFileLoader.Singleton.LoadBlockTemplates();
            this.GridTemplates.ItemsSource = blockTemps;
            if (blockTemps.Count > 0) {
                this.GridTemplates.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the GridTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GridTemplates_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (!(this.GridTemplates.SelectedItem is TemplateBlock block)) {
                return;
            }

            TemplateBlock block1 = block;
            this.DataContext = block1;
            this.GridLines.ItemsSource = block.Lines;
        }
    }
}
