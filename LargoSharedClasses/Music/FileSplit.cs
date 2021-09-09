// <copyright file="FileSplit.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Model transformation type.
    /// </summary>
    /// <summary>
    /// File Split.
    /// </summary>
    public enum FileSplit {
        /// <summary>
        ///  File Split.
        /// </summary>
        [UsedImplicitly] None = 0,
        
        /// <summary>
        ///  File Split.
        /// </summary>
        Automatic = 1,
        
        /// <summary>
        ///  File Split.
        /// </summary>
        Total = 2
    }
}
