// <copyright file="IMidiTone.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Midi
{
    /// <summary>
    /// I Midi Tone.
    /// </summary>
    public interface IMidiTone
    {
        /// <summary>
        /// Gets or sets the bar number from.
        /// </summary>
        /// <value>
        /// The bar number from.
        /// </value>
        int BarNumberFrom { get; set; }

        /// <summary>
        /// Gets or sets the bar number to.
        /// </summary>
        /// <value>
        /// The bar number to.
        /// </value>
        int BarNumberTo { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        long Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [first in bar].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [first in bar]; otherwise, <c>false</c>.
        /// </value>
        [UsedImplicitly]
        bool FirstInBar { get; set; }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        MidiChannel Channel { get; }

        /// <summary>
        /// Gets the instrument.
        /// </summary>
        /// <value>
        /// The instrument.
        /// </value>
        byte InstrumentNumber { get; }

        /// <summary>
        /// Gets the loudness.
        /// </summary>
        /// <value>
        /// The loudness.
        /// </value>
        MusicalLoudness Loudness { get; }

        /// <summary>
        /// Gets the note number.
        /// </summary>
        /// <value>
        /// The note number.
        /// </value>
        byte NoteNumber { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [sound through].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sound through]; otherwise, <c>false</c>.
        /// </value>
        bool SoundThrough { get; set; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        long StartTime { get; }

        /// <summary>
        /// Completes the duration.
        /// </summary>
        /// <param name="endDeltaTime">The end delta time.</param>
        /// <param name="fullLength">if set to <c>true</c> [full length].</param>
        /// <param name="givenDivision">The given division.</param>
        void CompleteDuration(long endDeltaTime, bool fullLength, int givenDivision);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        string ToString();
    }
}