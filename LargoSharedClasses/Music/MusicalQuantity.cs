// <copyright file="MusicalQuantity.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music {
    /// <summary> Musical quantities. </summary>
    /// <remarks> Musical class. </remarks>
    public static class MusicalQuantity {
        #region Constants
        /// <summary> Value of effect. </summary>
        public const int HarmfulValue = -100;

        /// <summary> Value of effect. </summary>
        public const int PoorValue = -10;

        /// <summary> Value of effect. </summary>
        public const int NeutralValue = 0;

        /// <summary> Value of effect. </summary>
        public const int GoodValue = +1;    // 5.0        

        /// <summary> Value of effect. </summary>
        public const int NiceValue = +2;    // 5.0

        /// <summary> Value of effect. </summary>
        public const int VeryNiceValue = +10;    // 5.0

        /// <summary> Value of effect. </summary>
        public const int ForcedValue = +100;    // 5.0
        #endregion

        #region Public static methods
        //// <summary>
        //// Prevents a default instance of the <see cref="MusicalQuantity"/> class from being created.
        //// </summary>
        //// private MusicalQuantity() { }

        //// public const int VarietyLimitCount = 50;

        /// <summary> Value of property. of property in between 0-100. </summary>
        /// <param name="givenValue">Given value.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static float Value(MusicalValue givenValue) {
            return (float)((int)givenValue * 100.00 / 8);
        }
        #endregion
    }
}