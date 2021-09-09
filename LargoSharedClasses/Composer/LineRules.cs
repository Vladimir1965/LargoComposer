// <copyright file="LineRules.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Composer
{
    /// <summary>
    /// Musical Engine.
    /// </summary>
    [Serializable]
    public sealed class LineRules : ILineRules
    {
        #region Public properties
        /// <summary>
        /// Gets RuleToneHarmonic.
        /// </summary>
        /// <value> Property description. </value>
        public float RuleToneHarmonic { get; private set; }

        /// <summary>
        /// Gets RuleMelodicFigural.
        /// </summary>
        /// <value> Property description. </value>
        public float RuleMelodicFigural { get; private set; }

        /// <summary>
        /// Gets RuleMelodicVariability.
        /// </summary>
        /// <value> General musical property.</value>
        public float RuleMelodicVariability { get; private set; }

        /// <summary>
        /// Gets RuleBreakingLine.
        /// </summary>
        /// <value> Property description. </value>
        public MelodicShape MelodicShape { get; private set; }

        /// <summary>
        /// Gets RuleIntervalEasySing.
        /// </summary>
        /// <value> General musical property.</value>
        public float RuleIntervalEasySing { get; private set; }

        #endregion

        #region Public static methods
        /// <summary>
        /// New Standard Line Rules.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static LineRules NewStandardLineRules(LineStatus status) {
            var lr = new LineRules();
            if (status == null) {
                return null;
            }

            lr.RuleIntervalEasySing = 1.0f;

            if (status.IsHarmonic) {
                lr.RuleToneHarmonic = 1.0f;

                if (status.IsHarmonicBass) {
                    //// lr.RuleToneRoot = 1.0f;
                }
            }

            //// if (status.HasMelodicMotive) {
            lr.RuleMelodicFigural = 1.0f;
            //// }

            lr.MelodicShape = status.MelodicShape;

            if (!status.IsFilling) {
                lr.RuleMelodicVariability = 1.0f;
            }

            return lr;
        }
        #endregion
    }
}