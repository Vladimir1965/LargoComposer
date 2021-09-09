// <copyright file="ILineRules.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Melody;

namespace LargoSharedClasses.Interfaces
{
    /// <summary>
    /// I Line Rules.
    /// </summary>
    public interface ILineRules
    {
        /// <summary>
        /// Gets the melodic shape.
        /// </summary>
        /// <value>
        /// The melodic shape.
        /// </value>
        MelodicShape MelodicShape { get; }

        /// <summary>
        /// Gets the rule interval easy sing.
        /// </summary>
        /// <value>
        /// The rule interval easy sing.
        /// </value>
        float RuleIntervalEasySing { get; }

        /// <summary>
        /// Gets the rule melodic figural.
        /// </summary>
        /// <value>
        /// The rule melodic figural.
        /// </value>
        float RuleMelodicFigural { get; }

        /// <summary>
        /// Gets the rule melodic variability.
        /// </summary>
        /// <value>
        /// The rule melodic variability.
        /// </value>
        float RuleMelodicVariability { get; }

        /// <summary>
        /// Gets the rule tone harmonic.
        /// </summary>
        /// <value>
        /// The rule tone harmonic.
        /// </value>
        float RuleToneHarmonic { get; }
    }
}