// <copyright file="OrchestraGroup.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Orchestra
{
    /// <summary> Orchestra Group. </summary>
    public enum OrchestraGroup {
        /// <summary> Orchestra Group. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Orchestra Group. </summary>
        Solo = 1,

        /// <summary> Orchestra Group. </summary>
        Uniform = 2,

        /// <summary> Orchestra Group. </summary>
        ChamberMusic = 3,

        /// <summary> Orchestra Group. </summary>
        ClassicOrchestra = 4,

        /// <summary> Orchestra Group. </summary>
        ModernGroup = 5,

        /// <summary> Orchestra Group. </summary>
        ModernOrchestra = 6,

        /// <summary> Orchestra Group. </summary>
        Vocal = 7,

        /// <summary> Orchestra Group. </summary>
        Choral = 8
    }
}
