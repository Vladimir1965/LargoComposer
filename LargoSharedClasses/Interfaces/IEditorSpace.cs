// <copyright file="IEditorSpace.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System.Windows.Media;

namespace LargoSharedClasses.Interfaces {
    /// <summary>
    /// Editor Space Interface.
    /// </summary>
    public interface IEditorSpace
    {
        /// <summary>
        /// Gets or sets the drawing visual.
        /// </summary>
        /// <value>
        /// The drawing visual.
        /// </value>
        DrawingVisual DrawingVisual { get; set; }

        /// <summary>
        /// Gets the get musical header.
        /// </summary>
        /// <value>
        /// The get musical header.
        /// </value>
        MusicalHeader GetMusicalHeader { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is music editor.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is music editor; otherwise, <c>false</c>.
        /// </value>
        bool IsMusicEditor { get; set; }

        /// <summary>
        /// Copies the paste.
        /// </summary>
        /// <param name="code">The code.</param>
        void CopyPaste(char code);
    }
}