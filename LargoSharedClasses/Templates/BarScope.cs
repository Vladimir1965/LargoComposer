// <copyright file="BarScope.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Templates
{
    /// <summary>
    /// Bar scope.
    /// </summary>
    public enum BarScope
    {
        /// <summary>
        /// Bar scope.
        /// </summary>
        [UsedImplicitly] None = 0,

        /// <summary>
        /// Bar scope.
        /// </summary>
        OddBars = 1,

        /// <summary>
        /// Bar scope.
        /// </summary>
        EvenBars = 2,

        /// <summary>
        /// Bar scope.
        /// </summary>
        AllBars = 3
    }
}