// <copyright file="PanelHarmonicStructures.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;
using LargoSharedControls.Abstract;

namespace LargoSharedPanels.Detail {
    /// <summary>
    /// Panel Harmonic Structures.
    /// </summary>
    public sealed partial class PanelHarmonicStructures {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelHarmonicStructures"/> class.
        /// </summary>
        public PanelHarmonicStructures() {
            this.InitializeComponent();
        }
        #endregion

        #region Load data
        /// <summary>
        /// Loads data.
        /// </summary>
        public override void LoadData() {
            base.LoadData();
            var list = DataEnums.GetHarmonicSystems;
            this.ControlHarmonicSystem.LoadData(list);
            CultureMaster.Localize(this);
            this.LoadGrid(); //// temporary
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Loads the grid.
        /// </summary>
        private void LoadGrid() {
            if (!(this.ControlHarmonicSystem.Combo.SelectedItem is KeyValuePair)) {
                return;
            }

            var structures = PortCatalogs.Singleton.HarmonicEssence;
            this.DataGridStructure.ItemsSource = structures;
        }
        #endregion
    }
}