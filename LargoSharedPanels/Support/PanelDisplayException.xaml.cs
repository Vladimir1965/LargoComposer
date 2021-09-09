// <copyright file="PanelDisplayException.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Windows;
using LargoSharedClasses.Abstract;

namespace LargoSharedPanels.Support
{
    /// <summary>
    /// PanelMain Exception.
    /// </summary>
    public sealed partial class PanelDisplayException
    {
        #region Fields
        //// private readonly object thisLock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelDisplayException"/> class.
        /// </summary>
        public PanelDisplayException() {
            this.InitializeComponent();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the log file path.
        /// </summary>
        /// <value>
        /// The log file path.
        /// </value>
        public string LogFilePath { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// Loads data.
        /// </summary>
        /// <param name="exception">Given exception.</param>
        /// <param name="givenLogFilePath">The given log file path.</param>
        public void LoadData(Exception exception, string givenLogFilePath) {
            if (exception == null) {
                return;
            }

            this.LogFilePath = givenLogFilePath;
            this.TextBlockMessage.Text = exception.GetExceptionMessages();
            //// this.textBlockStackTrace.Text = exception.StackTrace;
            //// this.WriteExceptionToLogFile(this.textBlockMessage.Text, this.textBlockStackTrace.Text);
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Handles the Click event of the button1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ClickOk(object sender, RoutedEventArgs e) {
            this.Window.Close();
        }
        #endregion
    }
}