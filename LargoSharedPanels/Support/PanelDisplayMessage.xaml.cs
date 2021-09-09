// <copyright file="PanelDisplayMessage.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Windows.Threading;
using LargoSharedClasses.Abstract;

namespace LargoSharedPanels.Support
{
    /// <summary>
    /// PanelProcess Logger.
    /// </summary>
    public sealed partial class PanelDisplayMessage
    {
        #region Fields
        /// <summary>
        /// The void handler
        /// </summary>
        private static readonly VoidHandler Handler = () => { }; //// 2016/08
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelDisplayMessage"/> class. 
        /// </summary>
        public PanelDisplayMessage() {
            this.InitializeComponent();
            this.TextBoxMessage.Text = string.Empty; //// "Test";
            ProcessLogger.Singleton.MessageAppeared += this.MessageAppeared;
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Void Handler.
        /// </summary>
        private delegate void VoidHandler();
        #endregion

        #region Private static

        /// <summary>
        /// Does the void events.
        /// </summary>
        private static void DoVoidEvents() {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, Handler);
            //// Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new VoidHandler(() => { }));
        }

        #endregion

        /// <summary>
        /// Singleton Log.
        /// </summary>
        /// <param name="sender">Object -Sender.</param>
        /// <param name="args">Event arguments.</param>
        private void MessageAppeared(object sender, ProcessLoggerMessageEventArgs args) {
            var dispatcher = this.Dispatcher;
            dispatcher?.BeginInvoke(
                DispatcherPriority.Background,
                (Action)(() => this.LogMessage(args.Title, args.Message, args.Percentage)));

            DoVoidEvents();
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="percentage">The percentage.</param>
        private void LogMessage(string title, string message, int percentage) {
            const int constMaxDots = 45; //// Maximum number of dots in the field 
            if (!string.IsNullOrEmpty(title)) {
                this.TextBlockTitle.Text = title;
            }

            this.TextBoxMessage.Text = message;
            if (percentage > 0) {
                this.TextBoxNote.Text = string.Empty; //// DateTime.Now.ToLongTimeString();
                int numOfDots = constMaxDots * percentage / 100;
                for (int dotNumber = 0; dotNumber < numOfDots; dotNumber++) {
                    this.TextBoxNote.Text = this.TextBoxNote.Text + "●";
                }
            }
            else {
                if (this.TextBoxNote.Text.Length <= constMaxDots) {
                    this.TextBoxNote.Text = this.TextBoxNote.Text + "●";
                }
            }

            //// this.textBoxNote.Text = DateTime.Now.ToLongTimeString();
            DoVoidEvents();
            //// this.Window.Refresh();
        }
    }
}