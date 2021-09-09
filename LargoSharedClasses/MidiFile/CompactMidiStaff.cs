// <copyright file="CompactMidiStaff.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;
using System.Text;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Compact Midi Staff.
    /// </summary>
    public class CompactMidiStaff
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompactMidiStaff" /> class.
        /// </summary>
        /// <param name="givenNumber">The given number.</param>
        /// <param name="givenLineType">Type of the given line.</param>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenVoice">The given voice.</param>
        /// <param name="givenUnit">The given unit.</param>
        public CompactMidiStaff(byte givenNumber, MusicalLineType givenLineType, MusicalLine givenLine, IAbstractVoice givenVoice, OrchestraUnit givenUnit) {
            this.Number = givenNumber;
            this.LineType = givenLineType;
            this.Line = givenLine;
            this.Voice = givenVoice;
            this.OrchestraUnit = givenUnit;
        }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public byte Number { get; set; }

        /// <summary>
        /// Gets or sets the type of the line.
        /// </summary>
        /// <value>
        /// The type of the line.
        /// </value>
        public MusicalLineType LineType { get; set; }

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public OrchestraUnit OrchestraUnit { get; set; }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public MusicalLine Line { get; set; }

        /// <summary>
        /// Gets or sets the voice.
        /// </summary>
        /// <value>
        /// The voice.
        /// </value>
        public IAbstractVoice Voice { get; set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public MidiChannel Channel { get; set; }

        /// <summary>
        /// Gets or sets the midi event collection.
        /// </summary>
        /// <value>
        /// The midi event collection.
        /// </value>
        public MidiEventCollection MidiEventCollection { get; set; }

    #region String representation
    /// <summary> String representation of the object. </summary>
    /// <returns> Returns value. </returns>
    public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("{0}/ {1} ({2}) {3}", this.Number, this.Voice.Instrument.ToString(), this.OrchestraUnit?.Name, this.Channel);
            return s.ToString();
        }
        #endregion
    }
}
