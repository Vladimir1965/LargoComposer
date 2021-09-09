// <copyright file="ProcessLoggerActionEventArgs.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Abstract
{
    /// <summary>
    /// Process Logger Action Event Args.
    /// </summary>
    public sealed class ProcessLoggerActionEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ProcessLoggerActionEventArgs class.
        /// </summary>
        /// <param name="text">The text.</param>
        public ProcessLoggerActionEventArgs(string text) {
            this.Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessLoggerActionEventArgs"/> class.
        /// </summary>
        [UsedImplicitly]
        public ProcessLoggerActionEventArgs() {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public string Text { get; }

        #endregion
    }
}
