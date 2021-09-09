// <copyright file="PanelAbstract.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Annotations;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Panel Abstract.
    /// </summary>
    public class PanelAbstract : UserControl {
        #region Fields
        /// <summary>
        /// Abstract window.
        /// </summary>
        private Window window;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelAbstract"/> class.
        /// </summary>
        [UsedImplicitly]
        public PanelAbstract() {
            ////2016 Thread.CurrentThread.CurrentUICulture = MusicalSettings.CultureInfo;
            this.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x0A, 0x46, 0x85)); //// #FF0A4685
            this.Padding = new Thickness(1);
            this.BorderThickness = new Thickness(1);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Window.
        /// </summary>
        /// <value> Property description. </value>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed.")]
        public Window Window {
             get {
                if (this.window != null) {
                    return this.window;
                }

                object obj = this.Parent;
                if (obj is Window win) {
                    return win;
                }

                if (obj is Grid grid) {
                    while (grid.Parent is Grid parent) {
                        grid = parent;
                    }

                    obj = grid.Parent;
                }

                if (obj is TabItem tabItem)
                {
                    obj = ((TabControl)tabItem.Parent).Parent;
                }

                grid = obj as Grid;
                if (grid != null) {
                    while (grid.Parent is Grid parent) {
                        grid = parent;
                    }

                    obj = grid.Parent;
                }

                win = obj as Window;
                return win;
            }

            set => this.window = value;
        }

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Panel {0}", this.Window.Name);

            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Loads the data.
        /// </summary>
        public virtual void LoadData() {
        }
        #endregion
      }
}
