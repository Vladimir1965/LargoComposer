// <copyright file="ControlMusicalInstrument.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedControls {
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using LargoSharedClasses.Abstract;
    using LargoSharedClasses.Melody;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Support;

    /// <summary>
    /// Control Instrument.
    /// </summary>
    public sealed partial class ControlMusicalInstrument {
        #region Fields
        /// <summary>
        /// Musical Line Type.
        /// </summary>
        private MusicalLineType lineType;

        //// private bool internalChange;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlMusicalInstrument"/> class.
        /// </summary>
        public ControlMusicalInstrument() {
            this.InitializeComponent();
            this.Combo = this.ComboBoxInstrument;
            //// this.LoadData();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of the line.
        /// </summary>
        /// <value>The type of the line.</value>
        public MusicalLineType? LineType {
            get => this.lineType;

            set {
                if (this.lineType == value) {
                    return;
                }

                this.InternalChange = true;
                this.lineType = value ?? MusicalLineType.None;
                if (this.lineType == MusicalLineType.Melodic) {
                    this.ComboBoxClass.ItemsSource = DataEnums.MelodicInstrumentGroups;
                }

                if (this.lineType == MusicalLineType.Rhythmic) {
                    this.ComboBoxClass.ItemsSource = DataEnums.RhythmicInstrumentGroups;
                }

                //// if (this.comboBoxClass.Items.Count > 0) { this.comboBoxClass.SelectedIndex = 0;  }
                this.InternalChange = false;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlMusicalInstrument"/> class.
        /// </summary>
        /// <param name="instrNum">The instrument number.</param>
        /// <param name="internally">If set to <c>true</c> [internally].</param>
        [UsedImplicitly]
        public void SelectInstrument(byte? instrNum, bool internally) {
            if (instrNum == null || instrNum == (byte)MidiMelodicInstrument.None) {
                this.SelectInstrumentGroup(null);
                this.SelectItemNumericKey(null, internally);
                return;
            }

            if (internally) {
                this.InternalChange = true;
            }

            switch (this.lineType) {
                case MusicalLineType.Melodic: {
                        var mg = PortInstruments.GetGroupOfMelodicInstrument((byte)instrNum);
                        this.SelectInstrumentGroup((int)mg);
                        this.LoadComboOfInstruments((InstrumentGroup)mg);
                        this.SelectItemNumericKey(instrNum, internally);
                    }

                    break;
                case MusicalLineType.Rhythmic: {
                        var rg = PortInstruments.GetGroupOfRhythmicInstrument((byte)instrNum);
                        this.SelectInstrumentGroup((int)rg);
                        this.LoadComboOfInstruments((InstrumentGroup)rg);
                        this.SelectItemNumericKey(instrNum, internally);
                    }

                    break;
                case MusicalLineType.None:
                    break;
                case MusicalLineType.Empty:
                    break;
                case MusicalLineType.Harmonic:
                    break;
            }

            this.InternalChange = false;
        }

        #region Private

        /// <summary>
        /// Selects the instrument group.
        /// </summary>
        /// <param name="key">The instrument key.</param>
        public void SelectInstrumentGroup(int? key)
        {
            this.InternalChange = true;
            var m = key != null ?
                (from item in this.ComboBoxClass.Items.Cast<KeyValuePair>()
                    where item.NumericKey == (int)key
                    select item).FirstOrDefault() : null;

            if (m != null) {
                this.ComboBoxClass.SelectedItem = m;
            }
            else {
                this.ComboBoxClass.Text = string.Empty;
            }

            this.InternalChange = false;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the comboBoxClass control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void ComboBoxClass_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.InternalChange) {
                return;
            }

            if (!(this.ComboBoxClass.SelectedItem is KeyValuePair m)) {
                return;
            }

            var instrumentClass = (InstrumentGroup)m.NumericKey;
            this.LoadComboOfInstruments(instrumentClass);
            this.ComboBoxInstrument.SelectedIndex = 0;
        }

        /// <summary>
        /// Loads the combo of instruments.
        /// </summary>
        /// <param name="instrumentClass">The instrument class.</param>
        private void LoadComboOfInstruments(InstrumentGroup instrumentClass) {
            IList<KeyValuePair> pairs = null;

            switch (this.lineType) {
                case MusicalLineType.Melodic:
                    pairs = PortInstruments.PrepareMelodicInstruments(instrumentClass);
                    break;
                case MusicalLineType.Rhythmic:
                    pairs = PortInstruments.PrepareRhythmicInstruments(instrumentClass);
                    break;
                case MusicalLineType.None:
                    break;
                case MusicalLineType.Empty:
                    break;
                case MusicalLineType.Harmonic:
                    break;
            }

            if (pairs == null || pairs.Count <= 0) {
                return;
            }

            this.InternalChange = true;
            this.ComboBoxInstrument.ItemsSource = pairs; ////DataEnums.MelodicInstruments;
            this.InternalChange = false;
        }
    }
        #endregion
}