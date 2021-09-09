// <copyright file="CellType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace EditorPanels.Cells
{
    /// <summary>
    /// Cell Type.
    /// </summary>
    public enum CellType
    {
        /// <summary> Cell Type. </summary>
        None,

        /// <summary> Cell Type. </summary>
        LineCell,

        /// <summary> Cell Type. </summary>
        BarCell,

        /// <summary> Cell Type. </summary>
        GroupCell,

        /// <summary> Cell Type. </summary>
        [UsedImplicitly] CornerCell,

        /// <summary> Cell Type. </summary>
        ContentCell
    }
}
