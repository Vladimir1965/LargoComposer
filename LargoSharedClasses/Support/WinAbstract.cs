// <copyright file="WinAbstract.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Annotations;
using LargoSharedClasses.Localization;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Win Abstract.
    /// </summary>
    public class WinAbstract : Window {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WinAbstract"/> class.
        /// </summary>
        public WinAbstract()
        {
            //// Thread.CurrentThread.CurrentUICulture = MusicalSettings.CultureInfo;
            WindowManager.Singleton.LoadPosition(this);
            //// this.ContextMenu = this.MainContextMenu;
            //// this.Panels = new Dictionary<FunctionalPanel, PanelAbstract>(); 
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets Main Context Menu.
        /// </summary>
        private ContextMenu MainContextMenu {
            get {
                var mainMenu = new ContextMenu();
                var itemPrint = new MenuItem {
                    Header = "Print window",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.Black,
                    Background = Brushes.Transparent
                };
                itemPrint.Click += this.PrintWindow;
                mainMenu.Items.Add(itemPrint);

                return mainMenu;
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("Window {0}", this.Name);

            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Loads the data.
        /// </summary>
        public virtual void LoadData() {
            Localize(this);
        }
        #endregion

        #region Protected static methods
        /// <summary>
        /// Localizes this instance.
        /// </summary>
        /// <param name="window">The window.</param>
        [UsedImplicitly]
        protected static void Localize(Window window) {
            //// if (MusicalSettings.CultureInfo == null) {  return;  }
            var sc = window.Title; 
            if (string.IsNullOrEmpty(sc)) {
                return;
            }

            var s = LocalizedControls.String(sc);
            if (!string.IsNullOrEmpty(s)) {
                window.Title = s;
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Windows the closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        [UsedImplicitly]
        protected void WindowClosing(object sender, CancelEventArgs e) {
            WindowManager.Singleton.SavePosition(this);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Prints the window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PrintWindow(object sender, RoutedEventArgs e) {
            var w = this as WinMainAbstract;

            w?.PrintVisual(this);
            #warning PrintVisual ?!??!?
            //// !!!!!!!! this.EditorWindow.PrintVisual(this);
        }
        #endregion
    }
}
