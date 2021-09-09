// <copyright file="ControlRhythmicModality.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedControls
{
    using JetBrains.Annotations;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Rhythm;
    using LargoSharedClasses.Support;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// ControlRhythmic Modality.
    /// </summary>
    public partial class ControlRhythmicModality {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlRhythmicModality"/> class.
        /// </summary>
        public ControlRhythmicModality() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        [UsedImplicitly]
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Gets combo object.
        /// </summary>
        /// <value> Property description. </value>
        public ComboBox Combo => this.ComboModality;

        /// <summary>
        /// Gets or sets the Rhythmic system.
        /// </summary>
        /// <value>
        /// The Rhythmic system.
        /// </value>
        public RhythmicSystem RhythmicSystem { get; set; }

        /// <summary>
        /// Gets or sets the tone structures.
        /// </summary>
        /// <value>
        /// The tone structures.
        /// </value>
        public List<RhythmicModality> SortedList { get; set; }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="givenRhythmicOrder">The given rhythmic order.</param>
        /// <param name="levelFrom">The level from.</param>
        /// <param name="levelTo">The level to.</param>
        public void LoadData(byte givenRhythmicOrder, byte levelFrom, byte levelTo) {
            this.RhythmicSystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, givenRhythmicOrder);
            var list = PortCatalogs.Singleton.RhythmicEssence;
            if (list == null) {
                return;
            }

            var filteredList = (from ts in list
                                where ts.Order == givenRhythmicOrder && ts.Complexity < 10
                                      && ts.Level >= levelFrom && ts.Level <= levelTo
                                select ts).ToList();
            this.SortedList = (from st in filteredList orderby st.ElementSchema select st).Distinct().ToList();

            this.ComboModality.ItemsSource = this.SortedList;
            if (this.SortedList.Count > 0) {
                this.ComboModality.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="givenIndex">Index of the given.</param>
        public void SelectItem(int givenIndex) {
            this.ComboModality.SelectedIndex = givenIndex;
        }

        /// <summary>
        /// Selects the modality.
        /// </summary>
        /// <param name="givenModality">The given modality.</param>
        public void SelectModality(RhythmicModality givenModality) {
            var list = this.ComboModality.ItemsSource as List<RhythmicModality>;
            var structure = from r in list where r.Number == givenModality.Number select r;
            this.ComboModality.SelectedItem = structure;
        }

        /* Unused yet.
        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SelectItem(long key) {  //// decimal structuralNumber
            var list = CatalogsPorter.Singleton.HarmonicEssence;
            var toneStruct = (from st in list where st.Number == key select st).FirstOrDefault();
            if (toneStruct == null) {
                return;
            }

            this.ComboModalClass.SelectedValue = toneStruct.ModalityName;
            this.ModalClassSelectionChanged(null, null);
            var rs = (from item in this.ComboModality.Items
                             .Cast<KeyValuePair>()
                      where item.NumericKey == key
                      select item).FirstOrDefault();
            if (rs != null) {
                this.ComboModality.SelectedItem = rs;
            }
        } */

        /// <summary>
        /// Selected modality.
        /// </summary>
        /// <returns> Returns value. </returns>
        public RhythmicModality SelectedModality()
        {
            var item = this.ComboModality.SelectedItem as RhythmicModality;

            return item;
        }

        /// <summary>
        /// Modals the class selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ModalitySelectionChanged(object sender, SelectionChangedEventArgs e) {
              this.SelectionChanged?.Invoke(this, e);
        }
    }
}