// <copyright file="IAbstractVoice.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Xml.Linq;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Interfaces
{
    /// <summary> Interface for abstract voice. </summary>
    public interface IAbstractVoice
    {
        /// <summary> Gets the get x coordinate element. </summary>
        /// <value> The get x coordinate element. </value>
        XElement GetXElement { get; }

        /// <summary> Gets or sets the channel. </summary>
        /// <value> The channel. </value>
        MidiChannel Channel { get; set; }

        /// <summary> Gets or sets the instrument. </summary>
        /// <value> The instrument. </value>
        MusicalInstrument Instrument { get; set; }

        /// <summary> Gets or sets the line. </summary>
        /// <value> The line. </value>
        IAbstractLine Line { get; set; }

        /// <summary> Gets or sets the loudness. </summary>
        /// <value> The loudness. </value>
        MusicalLoudness Loudness { get; set; }

        /// <summary> Gets or sets the octave. </summary>
        /// <value> The octave. </value>
        MusicalOctave Octave { get; set; }
    }
}