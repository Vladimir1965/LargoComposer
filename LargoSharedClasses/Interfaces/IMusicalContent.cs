// <copyright file="IMusicalContent.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using System.Collections.Generic;

namespace LargoSharedClasses.Interfaces {
    /// <summary>
    /// Musical Content Interface.
    /// </summary>
    public interface IMusicalContent
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets the content bars.
        /// </summary>
        /// <value>
        /// The content bars.
        /// </value>
        List<IAbstractBar> ContentBars { get; }

        /// <summary>
        /// Gets the content lines.
        /// </summary>
        /// <value>
        /// The content lines.
        /// </value>
        List<IAbstractLine> ContentLines { get; }

        /// <summary>
        /// Gets the content elements.
        /// </summary>
        /// <value>
        /// The content elements.
        /// </value>
        IList<MusicalElement> ContentElements { get; }

        /// <summary>
        /// Adds the content line.
        /// </summary>
        /// <param name="lineStatus">The line status.</param>
        /// <returns> Returns value. </returns>
        IAbstractLine AddContentLine(LineStatus lineStatus);
        }
    }