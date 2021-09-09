// <copyright file="GenProperty.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    /// <summary> Outline of all musical properties used to evaluate. </summary>
    public enum GenProperty {
        #region Binding behavior
        /// <summary> Musical property. </summary>
        InnerContinuity,

        /// <summary> Musical property. </summary>
        InnerImpulse,

        /// <summary> Musical property. </summary>
        RelatedContinuity,

        /// <summary> Musical property. </summary>
        RelatedImpulse,

        /// <summary> Musical property. </summary>
        ModalContinuity,

        /// <summary> Musical property. </summary>
        ModalImpulse,

        /// <summary> Musical property. </summary>
        TonicContinuity,

        /// <summary> Musical property. </summary>
        TonicImpulse,

        /// <summary> Musical property. </summary>
        RealContinuity,

        /// <summary> Musical property. </summary>
        RealImpulse,
        #endregion

        #region Harmonic behavior
        /// <summary> Musical property. </summary>
        Consonance,

        /// <summary> Musical property. </summary>
        Genus,

        /// <summary> Musical property. </summary>
        Potential,

        /// <summary> Musical property. </summary>
        Tonicity,
        #endregion

        #region Formal Structural Properties

        /// <summary> Musical property. </summary>
        FormalPotentialInfluence,

        /// <summary> Musical property. </summary>
        FormalBalance,

        /// <summary> Musical property. </summary>
        FormalVariance,

        /// <summary> Musical property. </summary>
        FormalFilling,

        /// <summary> Musical property. </summary>
        FormalMobility,

        /// <summary> Musical property. </summary>
        FormalComplexity,

        /// <summary> Musical property. </summary>
        FormalEntropy,

        /// <summary> Musical property. </summary>
        FormalBeat,

        /// <summary> Musical property. </summary>
        Level,

        /// <summary> Musical property. </summary>
        ToneLevel

        #endregion
    }
}
