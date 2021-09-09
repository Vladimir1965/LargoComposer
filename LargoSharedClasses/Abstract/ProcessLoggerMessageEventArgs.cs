// <copyright file="ProcessLoggerMessageEventArgs.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Abstract {
    /// <summary>
    /// Process Logger Event Args.
    /// </summary>
    public sealed class ProcessLoggerMessageEventArgs : EventArgs {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ProcessLoggerMessageEventArgs class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">Passed message.</param>
        /// <param name="percentage">The percentage.</param>
        public ProcessLoggerMessageEventArgs(string title, string message, int percentage) {
            this.Title = title;
            this.Message = message;
            this.Percentage = percentage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessLoggerMessageEventArgs"/> class.
        /// </summary>
        [UsedImplicitly]
        public ProcessLoggerMessageEventArgs() {    
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public string Title { get; }

        /// <summary>
        /// Gets Message.
        /// </summary>
        /// <value> Property description. </value>/// 
        public string Message { get; }

        /// <summary>
        /// Gets the percentage.
        /// </summary>
        /// <value>
        /// The percentage.
        /// </value>
        public int Percentage { get; }
        #endregion
    }
}
