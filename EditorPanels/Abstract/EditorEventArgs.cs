// <copyright file="EditorEventArgs.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorPanels.Abstract
{
    using System;
    using JetBrains.Annotations;
    using LargoSharedClasses.Interfaces;
    using LargoSharedClasses.Music;

    /// <summary>
    /// Process Logger Event Args.
    /// </summary>
    public sealed class EditorEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorEventArgs"/> class.
        /// </summary>
        /// <param name="givenElement">The given element.</param>
        public EditorEventArgs(MusicalElement givenElement)
        {
            this.Element = givenElement;
            this.Bar = givenElement.Bar;
            this.Line = givenElement.Line;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorEventArgs"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        public EditorEventArgs(IAbstractBar givenBar)
        {
            this.Bar = givenBar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorEventArgs"/> class.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        public EditorEventArgs(IAbstractLine givenLine)
        {
            this.Line = givenLine;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorEventArgs"/> class.
        /// </summary>
        [UsedImplicitly]
        public EditorEventArgs() {    
        }

        #region Properties
        /// <summary>
        /// Gets the musical editor element.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalElement Element { get; }

        /// <summary>
        /// Gets the musical editor line.
        /// </summary>
        /// <value> Property description. </value>
        public IAbstractLine Line { get; }

        /// <summary>
        /// Gets the musical editor bar.
        /// </summary>
        /// <value> Property description. </value>
        public IAbstractBar Bar { get; }

        #endregion
    }
}
