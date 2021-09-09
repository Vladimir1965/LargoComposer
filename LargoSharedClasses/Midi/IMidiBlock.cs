// <copyright file="IMidiBlock.cs" company="Traced-Ideas, Czech republic">
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
    /// I Midi Block.
    /// </summary>
    public interface IMidiBlock
    {
        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <value>
        /// The area.
        /// </value>
        MusicalSection Area { get; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the midi time from.
        /// </summary>
        /// <value>
        /// The midi time from.
        /// </value>
        long MidiTimeFrom { get; set; }

        /// <summary>
        /// Gets or sets the midi time to.
        /// </summary>
        /// <value>
        /// The midi time to.
        /// </value>
        long MidiTimeTo { get; set; }

        /// <summary>
        /// Gets the tempo.
        /// </summary>
        /// <value>
        /// The tempo.
        /// </value>
        int Tempo { get; }

        /// <summary>
        /// Gets the tonality genus.
        /// </summary>
        /// <value>
        /// The tonality genus.
        /// </value>
        TonalityGenus TonalityGenus { get; }

        /// <summary>
        /// Gets the tonality key.
        /// </summary>
        /// <value>
        /// The tonality key.
        /// </value>
        TonalityKey TonalityKey { get; }

        /// <summary>
        /// Checks the tempo.
        /// </summary>
        [UsedImplicitly]
        void CheckTempo();
    }
}