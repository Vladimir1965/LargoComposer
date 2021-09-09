// <copyright file="StructuralVarietyType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    //// see Enum.GetNames ,...

    /// <summary> Variety type. </summary>
    public enum StructuralVarietyType {
        /// <summary> Type of variety. </summary>
        None = 0,

        /// <summary> Type of variety. </summary>
        Instances = 1,

        /// <summary> Type of variety. </summary>
        BinaryClasses = 2,

        /// <summary> Type of variety. </summary>
        BinarySubstructuresOfModality = 3,

        /// <summary> Type of variety. </summary>
        FiguralSubstructuresOfModality = 4,

        /// <summary> Type of variety. </summary>
        MelodicStructuresOfModality = 5,

        /// <summary> Type of variety. </summary>
        RhythmicModalityClasses = 6,

        /// <summary> Type of variety. </summary>
        RhythmicMetricClasses = 7,

        /// <summary> Type of variety. </summary>
        Classes = 8
    }
}
