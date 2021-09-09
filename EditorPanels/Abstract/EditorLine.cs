// <copyright file="EditorLine.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorPanels.Abstract
{
    using Cells;
    using LargoSharedClasses.Interfaces;
    using System.Collections.Generic;

    /// <summary>
    /// Editor Line.
    /// </summary>
    public class EditorLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorLine"/> class.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        public EditorLine(IAbstractLine givenLine)
        {
            this.Line = givenLine;
            this.ContentCells = new List<ContentCell>();
            this.GroupCells = new List<GroupCell>();
        }

        /// <summary>
        /// Gets or sets the musical line.
        /// </summary>
        /// <value>
        /// The musical line.
        /// </value>
        public IAbstractLine Line { get; set; }

        /// <summary>
        /// Gets or sets the content cells.
        /// </summary>
        /// <value>
        /// The content cells.
        /// </value>
        public List<ContentCell> ContentCells { get; set; }

        /// <summary>
        /// Gets or sets the group cells.
        /// </summary>
        /// <value>
        /// The group cells.
        /// </value>
        public List<GroupCell> GroupCells { get; set; }
    }
}