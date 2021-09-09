// <copyright file="ObjectOperation.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Support
{
    /// <summary> Type of operation. </summary>
    public enum ObjectOperation {
        /// <summary> Type of part. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Type of operation. </summary>
        ObjectLoaded = 2
    }
}
