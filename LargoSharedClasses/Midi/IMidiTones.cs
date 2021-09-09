// <copyright file="IMidiTones.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Midi
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Music;

    /// <summary>
    /// I Midi Tones.
    /// </summary>
    public interface IMidiTones
    {
        /// <summary>
        /// Gets the type of the band.
        /// </summary>
        /// <value>
        /// The type of the band.
        /// </value>
        MusicalBand BandType { get; }

        /// <summary>
        /// Gets the bar division.
        /// </summary>
        /// <value>
        /// The bar division.
        /// </value>
        int BarDivision { get; }

        /// <summary>
        /// Gets the first bar number.
        /// </summary>
        /// <value>
        /// The first bar number.
        /// </value>
        int FirstBarNumber { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is rhythmical.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is rhythmical; otherwise, <c>false</c>.
        /// </value>
        bool IsRhythmical { get; }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        IList<IMidiTone> List { get; }

        /// <summary>
        /// Gets or sets the metric.
        /// </summary>
        /// <value>
        /// The metric.
        /// </value>
        MusicalMetric Metric { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the octave.
        /// </summary>
        /// <value>
        /// The octave.
        /// </value>
        MusicalOctave Octave { get; }

        /// <summary>
        /// Checks the rests.
        /// </summary>
        [UsedImplicitly]
        void CheckRests();

        /// <summary>
        /// Checks the tones.
        /// </summary>
        /// <returns>Returns value.</returns>
        [UsedImplicitly]
        bool CheckTones();

        /// <summary>
        /// Sets the first bar number.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        [UsedImplicitly]
        void SetFirstBarNumber(int barNumber);

        /// <summary>
        /// Sets the tones.
        /// </summary>
        /// <param name="givenList">The given list.</param>
        [UsedImplicitly]
        void SetTones(IEnumerable<IMidiTone> givenList);
    }
}