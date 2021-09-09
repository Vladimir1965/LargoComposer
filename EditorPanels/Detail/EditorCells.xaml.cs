// <copyright file="EditorCells.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace EditorPanels.Detail
{
    /// <summary>
    /// Interact logic.
    /// </summary>
    public partial class EditorCells
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static EditorCells singleton;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorCells"/> class.
        /// </summary>
        public EditorCells()
        {
            this.InitializeComponent();
            singleton = this;
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the InspectorHeader Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static EditorCells Singleton
        {
            get
            {
                Contract.Ensures(Contract.Result<EditorInspector>() != null);
                if (singleton == null) {
                    throw new InvalidOperationException("Singleton EditorCells is null.");
                }

                return singleton;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>
        /// The units.
        /// </value>
        public List<MusicalElement> Elements { get; set; }
        #endregion

        /// <summary>
        /// Handles the SelectionChanged event of the dataGridStatus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataGridStatus_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        { 
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the dataGridStatus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DataGridStatus_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.DataGridStatus.SelectedItem is MusicalElement selectedElement) {
                //// var w = WindowManager.OpenWindow("LargoEditor", "LineMelodicEdit", null);
                //// w.LoadData();
            }
        }
    }
}
