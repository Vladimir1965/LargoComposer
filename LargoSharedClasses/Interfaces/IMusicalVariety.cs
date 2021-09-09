// <copyright file="IMusicalVariety.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Interfaces
{
    /// <summary>
    /// I Musical Variety.
    /// </summary>
    public interface IMusicalVariety
    {
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        MusicalElement Element { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has trace values.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has trace values; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        bool HasTraceValues { get; set; }

        /// <summary>
        /// Gets the line rules.
        /// </summary>
        /// <value>
        /// The line rules.
        /// </value>
        [UsedImplicitly]
        ILineRules LineRules { get; }

        /// <summary>
        /// Generates the possibilities.
        /// </summary>
        /// <param name="givenTone">The given tone.</param>
        /// <param name="harmonicModality">The harmonic modality.</param>
        /// <param name="harmonicStruct">The harmonic structure.</param>
        /// <param name="harmonyOnly">if set to <c>true</c> [harmony only].</param>
        /// <returns>Returns value.</returns>
        bool GeneratePossibilities(MusicalTone givenTone, BinarySchema harmonicModality, BinaryStructure harmonicStruct, bool harmonyOnly);

        /// <summary>
        /// Optimal next melodic tone.
        /// </summary>
        /// <param name="givenTone">The given tone.</param>
        /// <returns>Returns value.</returns>
        MusicalTone OptimalNextMelTone(MusicalTone givenTone);

        /// <summary>
        /// Prepares the variety.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenRules">The given rules.</param>
        void PrepareVariety(MusicalElement givenLine, MusicalBar givenBar, ILineRules givenRules);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        string ToString();
    }
}