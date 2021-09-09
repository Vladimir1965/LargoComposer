// <copyright file="IMusicalTone.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Music;
using LargoSharedClasses.Notation;

namespace LargoSharedClasses.Interfaces
{
    /// <summary>
    /// IMusical Tone.
    /// </summary>
    public interface IMusicalTone : IMusicalLocation {
        #region Properties
        /// <summary> Gets or sets type of musical tone. </summary>
        /// <value> Property description. </value>
        MusicalToneType ToneType { get; set; }

        /// <summary> Gets or sets ordinal index of tone in musical line. </summary>
        /// <value> Property description. </value>
        int OrdinalIndex { get; set; }

        /// <summary> Gets or sets loudness. </summary>
        /// <value> Property description. </value>
        MusicalLoudness Loudness { get; set; }

        /// <summary> Gets or sets a value indicating whether property of musical tone. </summary>
        /// <value> Property description. </value>
        bool IsReady { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is pause.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is pause; otherwise, <c>false</c>.
        /// </value>
        bool IsPause { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is from previous bar.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance is from previous bar; otherwise, <c>false</c>.
        /// </value>
        bool IsFromPreviousBar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [continue to next bar].
        /// </summary>
        /// <value>
        /// <c>True</c> if [continue to next bar]; otherwise, <c>false</c>.
        /// </value>
        bool IsGoingToNextBar { get; set; }

        /// <summary>
        /// Gets or sets BitFrom.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        byte BitFrom { get; set; }

        /// <summary>
        /// Gets BitPosition.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        int BitPosition { get; }

        /// <summary>
        /// Gets BitTo.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        byte BitTo { get; }

        /// <summary>
        /// Gets or sets the bit range.
        /// </summary>
        /// <value>
        /// The bit range.
        /// </value>
        BitRange BitRange { get; set; }

        /// <summary>
        /// Gets or sets the length of the note.
        /// </summary>
        /// <value>
        /// The length of the note.
        /// </value>
        NoteLength NoteLength { get; set; }

        /// <summary>
        /// Gets or sets Duration.
        /// </summary>
        /// <value> Property description. </value>
        int Duration { get; [UsedImplicitly] set; }

        /// <summary>
        /// Gets RhythmicOrder.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        byte RhythmicOrder { get; }

        /// <summary>
        /// Gets the get x element.
        /// </summary>
        /// <value>
        /// The get x element.
        /// </value>
        XElement GetXElement { get; }

        /// <summary>
        /// Gets the melodic identifier.
        /// </summary>
        /// <value>
        /// The melodic identifier.
        /// </value>
        string MelodicIdentifier { get; }

        /// <summary>
        /// Gets the rhythmic identifier.
        /// </summary>
        /// <value>
        /// The rhythmic identifier.
        /// </value>
        string RhythmicIdentifier { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Clones the tone.
        /// </summary>
        /// <returns> Returns value. </returns>
        object CloneTone();

        /// <summary>
        /// Writes to.
        /// </summary>
        /// <param name="midiEvents">The midi events.</param>
        /// <param name="barDivision">The bar division.</param>
        [UsedImplicitly]
        void WriteTo(MidiEventCollection midiEvents, int barDivision);

        /// <summary>
        /// Gets or sets WriteTo.
        /// </summary>
        /// <param name="midiEvents">Midi events.</param>
        /// <param name="barDivision">Bar division.</param>
        /// <param name="bitDuration">Bit DeltaTime.</param>
        /// <param name="barDuration">Bar DeltaTime.</param>
        /// <param name="deltaTimeShift">Delta TimeShift.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        bool WriteTo(MidiEventCollection midiEvents, int barDivision, int bitDuration, int barDuration, int deltaTimeShift);
        #endregion
    }
}
