// <copyright file="ComboCover.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Linq;
using System.Windows.Controls;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Combo Cover.
    /// </summary>
    public class ComboCover : UserControl {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ComboCover"/> class.
        /// </summary>
        protected ComboCover() {
        }

        #endregion

        #region Events
        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        public event EventHandler SelectionChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the combo.
        /// </summary>
        /// <value>
        /// The combo.
        /// </value>
        public ComboBox Combo { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether Internal Change.
        /// </summary>
        protected bool InternalChange { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return @"ComboCover";
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Selects the item numeric key.
        /// </summary>
        /// <param name="key">The numeric key.</param>
        /// <param name="internally">If set to <c>true</c> [internally].</param>
        public void SelectItemNumericKey(int? key, bool internally) {
            if (internally) {
                this.InternalChange = true;
            }

            var m = key != null ? (from item in this.Combo.Items.Cast<KeyValuePair>()
                                            where item.NumericKey == (int)key
                                            select item).FirstOrDefault() : null;

            if (m != null) {
                this.Combo.SelectedItem = m;
            }
            else {
                this.Combo.Text = string.Empty;
            }

            this.InternalChange = false;
        }

        /// <summary>
        /// Combo selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        [UsedImplicitly]
        protected virtual void ComboSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.InternalChange) {
                return;
            }

            this.SelectionChanged?.Invoke(this, e);

            e.Handled = true;
        }

        /// <summary>
        /// Selects the item key.
        /// </summary>
        /// <param name="key">The given key.</param>
        protected void SelectItemKey(string key) {
            var m = this.Combo.Items.Cast<KeyValuePair>().FirstOrDefault(item => item.Key == key);

            if (m != null) {
                this.Combo.SelectedItem = m;
            }
        }
        #endregion
    }
}
