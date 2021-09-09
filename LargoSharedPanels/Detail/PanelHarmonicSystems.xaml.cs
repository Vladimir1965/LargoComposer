// <copyright file="PanelHarmonicSystems.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedControls.Abstract;

namespace LargoSharedPanels.Detail
{
    /// <summary>
    /// Panel Harmonic System.
    /// </summary>
    [UsedImplicitly]
    public sealed partial class PanelHarmonicSystems
    {
        #region Fields
        /// <summary>
        /// Harmonic System.
        /// </summary>
        private HarmonicSystem harmonicSystem;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelHarmonicSystems"/> class.
        /// </summary>
        public PanelHarmonicSystems() {
            this.InitializeComponent();
            ////this.modelContext = DataLink.NewContext;
            this.ControlHarmonicSystem.Combo.SelectedIndex = 0;
            this.UCHarSystem_SelectionChanged(null, null);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Intervals.
        /// </summary>
        /// <value> Property description. </value>
        private IEnumerable<HarmonicInterval> Intervals => this.harmonicSystem.Intervals;

        #endregion

        #region Load data
        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
            var list = DataEnums.GetHarmonicSystems;
            this.ControlHarmonicSystem.LoadData(list);
            CultureMaster.Localize(this);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Harmonic System Selection Changed.
        /// </summary>
        /// <param name="sender">Object -Sender.</param>
        /// <param name="e">Event Arguments.</param>
        private void UCHarSystem_SelectionChanged(object sender, EventArgs e) {
            var vt = (KeyValuePair)this.ControlHarmonicSystem.Combo.SelectedItem;
            if (vt == null) {
                return;
            }

            var order = byte.Parse(vt.Value, CultureInfo.InvariantCulture);
            this.harmonicSystem = HarmonicSystem.GetHarmonicSystem(order);
            this.DataGridIntervals.ItemsSource = this.Intervals;
            this.TbTones.Text = this.harmonicSystem.StringOfSharpSymbols;
        }
        #endregion
    }
}