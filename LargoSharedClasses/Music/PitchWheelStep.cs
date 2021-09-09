// <copyright file="PitchWheelStep.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary>Half and whole value steps for the pitch wheel.</summary>
    [Serializable]
    public enum PitchWheelStep {
        #region Defined Step Values
        /// <summary>A complete whole step up.</summary>
        [UsedImplicitly] WholeStepUp = 0x3FFF,

        /// <summary>3/4 steps up.</summary>
        [UsedImplicitly] ThreeQuarterStepUp = 0x3500,

        /// <summary>1/2 step up.</summary>
        [UsedImplicitly] HalfStepUp = 0x3000,

        /// <summary>1/4 step up.</summary>
        [UsedImplicitly] QuarterStepUp = 0x2500,

        /// <summary>No movement.</summary>
        [UsedImplicitly] NoStep = 0x2000,

        /// <summary>1/4 step down.</summary>
        [UsedImplicitly] QuarterStepDown = 0x1500,

        /// <summary>1/2 step down.</summary>
        [UsedImplicitly] HalfStepDown = 0x1000,

        /// <summary>3/4 steps down.</summary>
        [UsedImplicitly] ThreeQuarterStepDown = 0x500,

        /// <summary>A complete whole step down.</summary>
        [UsedImplicitly] WholeStepDown = 0x0
        #endregion
    }
}
