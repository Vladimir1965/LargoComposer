// <copyright file="ControlSplitFile.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedControls {
    using LargoSharedClasses.Music;
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// Control Octave.
    /// </summary>
    public sealed partial class ControlSplitFile {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlSplitFile"/> class.
        /// </summary>
        public ControlSplitFile() {
            this.InitializeComponent();
            this.LoadData();
        }

        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Gets combo object.
        /// </summary>
        /// <value> Property description. </value>
        public ComboBox Combo => this.ComboObject;

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData() {
            this.ComboObject.ItemsSource = DataEnums.GetListFileSplit;
            this.ComboObject.SelectedIndex = 1;
        }

        /// <summary>
        /// Combo selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ComboSelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.SelectionChanged?.Invoke(this, e);

            e.Handled = true;
        }
    }
}