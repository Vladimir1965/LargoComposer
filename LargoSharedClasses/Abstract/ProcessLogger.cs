// <copyright file="ProcessLogger.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;

namespace LargoSharedClasses.Abstract {
    /// <summary>
    /// Process Logger.
    /// </summary>
    public sealed class ProcessLogger {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly ProcessLogger InternalSingleton = new ProcessLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the ProcessLogger class from being created.
        /// </summary>
        private ProcessLogger() {
        }
        #endregion

        #region Events
        /// <summary>
        /// Define the event - Message to be displayed to user. 
        /// </summary>
        public event EventHandler<ProcessLoggerMessageEventArgs> MessageAppeared;

        /// <summary>
        /// Define the event - Text to be written to file.
        /// </summary>
        public event EventHandler<ProcessLoggerActionEventArgs> ActionAppeared;
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>/// 
        public static ProcessLogger Singleton {
            get {
                Contract.Ensures(Contract.Result<ProcessLogger>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Process Logger is null.");
                }

                return InternalSingleton; 
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Add a message, and log it.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">Message to display.</param>
        /// <param name="percentage">Percentage value.</param>
        public void SendMessageEvent(string title, string message, int percentage) {
            var command = this.MessageAppeared;
            command?.Invoke(this, new ProcessLoggerMessageEventArgs(title, message, percentage)); 
        }

        /// <summary>
        /// Add a message, and log it.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SendActionEvent(string text) {
            var command = this.ActionAppeared;
            command?.Invoke(this, new ProcessLoggerActionEventArgs(text));
        }
        #endregion
    }
}