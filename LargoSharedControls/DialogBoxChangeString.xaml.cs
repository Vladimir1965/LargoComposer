// <copyright file="DialogBoxChangeString.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Windows;

namespace LargoSharedControls
{
    /// <summary>
    /// Dialog Box Change String.
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class DialogBoxChangeString : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogBoxChangeString"/> class.
        /// </summary>
        public DialogBoxChangeString() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogBoxChangeString"/> class.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="defaultAnswer">The default answer.</param>
        public DialogBoxChangeString(string question, string defaultAnswer = "") {
            this.InitializeComponent();
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
        }

        /// <summary>
        /// Gets the answer.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        public string Answer => txtAnswer.Text;

        /// <summary>
        /// Handles the Click event of the DialogOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DialogOk_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }

        /// <summary>
        /// Handles the ContentRendered event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_ContentRendered(object sender, EventArgs e) {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }
    }
}