// <copyright file="ControlNotator.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace PlayerControls {
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Support;

    /// <summary>
    /// Control note editor.
    /// </summary>
    public sealed partial class ControlNotator {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlNotator"/> class.
        /// </summary>
        public ControlNotator() {
            this.InitializeComponent();
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
        public void LoadData() {
            var list = PortCatalogs.Singleton.MusicalNotators;
            this.ComboObject.ItemsSource = list;
            //// if (list.Count > 0) { this.Combo.SelectedIndex = 0; }
        }

        /// <summary>
        /// Select item.
        /// </summary>
        /// <param name="name">Name of the sound font.</param>
        public void SelectItem(string name) {
            var m = (from item in this.Combo.Items
                        .Cast<MusicalNotator>()
                            where item.Name == name
                            select item).FirstOrDefault();
            if (m != null) {
                this.Combo.SelectedItem = m;
            } 
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