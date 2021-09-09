// <copyright file="ControlHarmonicModality.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedControls
{
    using JetBrains.Annotations;
    using LargoSharedClasses.Abstract;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Support;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// ControlHarmonic Modality.
    /// </summary>
    public partial class ControlHarmonicModality {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlHarmonicModality"/> class.
        /// </summary>
        public ControlHarmonicModality() {
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
        /// Gets or sets the harmonic system.
        /// </summary>
        /// <value>
        /// The harmonic system.
        /// </value>
        public HarmonicSystem HarmonicSystem { get; set; }

        /// <summary>
        /// Gets or sets the tone structures.
        /// </summary>
        /// <value>
        /// The tone structures.
        /// </value>
        private List<ToneStructure> FilteredList { get; set; }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="harmonicOrder">The harmonic order.</param>
        /// <param name="levelFrom">The level from.</param>
        /// <param name="levelTo">The level to.</param>
        public void LoadData(byte harmonicOrder, byte levelFrom, byte levelTo)
        {
            this.HarmonicSystem = HarmonicSystem.GetHarmonicSystem(harmonicOrder);
            var list = PortCatalogs.Singleton.HarmonicEssence;
            if (list == null) {
                return;
            }

            //// 145 major, 137 minor, 73 diminished, 137 augmented
            //// var collection = new Collection<long>(){145, 137, 73, 273};
            this.FilteredList = 
                          (from ts in list
                          where ts.Level >= levelFrom && ts.Level <= levelTo
                              && !string.IsNullOrEmpty(ts.ModalityName)  //// && ts.Number == ts.ClassNumber //// && collection.Contains(ts.ClassNumber)
                                 select ts).ToList();
            var classList = (from st in this.FilteredList orderby st.ModalityName select st.ModalityName).Distinct().ToList();
            
            this.ComboModalClass.ItemsSource = classList;
            if (classList.Count > 0) {
                this.ComboModalClass.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SelectItem(long key) {  //// decimal structuralNumber
            var list = PortCatalogs.Singleton.HarmonicEssence;
            if (list == null) {
                return;
            }

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
        }

        /// <summary>
        /// Selects the modality.
        /// </summary>
        /// <param name="givenModality">The given modality.</param>
        public void SelectModality(HarmonicModality givenModality) {
            var list = this.ComboModality.ItemsSource as List<KeyValuePair>;
            var structure = from r in list where r.Key == givenModality.Number.ToString() select r;
            this.ComboModality.SelectedItem = structure;
        }

        /// <summary>
        /// Selected modality.
        /// </summary>
        /// <returns> Returns value. </returns>
        public HarmonicModality SelectedModality()
        {
            var item = this.ComboModality.SelectedItem as KeyValuePair;
            if (item == null) {
                return null;
            }

            var key = item.NumericKey;
            return new HarmonicModality(this.HarmonicSystem, key);
        }

        /// <summary>
        /// Modals the class selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ModalClassSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!(this.ComboModalClass.SelectedValue is string modalClassName)) {
                return;
            }

            var selList = (from st in this.FilteredList where st.ModalityName == modalClassName select st).ToList();
            int maxTonesLength = (from st in selList select st.Tones.Length).Max();
            var modList = new List<KeyValuePair>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            var spaces = new string(' ', 24);
            foreach (var ts in selList) {
                //// if (ts.Number == null) { continue; }
                var keyValuePair = new KeyValuePair(ts.Number, (ts.Tones + spaces).Left(maxTonesLength) + " (" + ts.StructuralCode + ")");
                modList.Add(keyValuePair);
            }

            //// modList.Take(20);
            this.ComboModality.ItemsSource = null;
            this.ComboModality.ItemsSource = modList;
            if (modList.Count > 0) {
                this.ComboModality.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Modalities the selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ModalitySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectionChanged?.Invoke(this, e);

            //// e.Handled = true;
        }
    }
}