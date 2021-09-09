// <copyright file="IAbstractElement.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System.Xml.Linq;

namespace LargoSharedClasses.Interfaces
{
    /// <summary>
    /// Abstract Element Interface.
    /// </summary>
    public interface IAbstractElement
    {
        /// <summary> Gets or sets the bar. </summary>
        /// <value> The bar. </value>
        IAbstractBar Bar { get; set; }

        /// <summary> Gets or sets the line. </summary>
        /// <value> The line. </value>
        IAbstractLine Line { get; set; }

        /// <summary> Gets or sets the display text. </summary>
        /// <value> The display text. </value>
        string DisplayText { get; set; }

        /// <summary> Gets the get x coordinate element. </summary>
        /// <value> The get x coordinate element. </value>
        XElement GetXElement { get; }

        /// <summary> Gets or sets a value indicating whether this object is composed. </summary>
        /// <value> True if this object is composed, false if not. </value>
        bool IsComposed { get; set; }

        /// <summary> Gets or sets a value indicating whether this object is finished. </summary>
        /// <value> True if this object is finished, false if not. </value>
        bool IsFinished { get; set; }

        /// <summary> Gets the point. </summary>
        /// <value> The point. </value>
        MusicalPoint Point { get; }

        /// <summary> Gets or sets the status. </summary>
        /// <value> The status. </value>
        LineStatus Status { get; set; }

        /// <summary> Gets or sets the tool tip. </summary>
        /// <value> The tool tip. </value>
        string ToolTip { get; set; }
    }
}