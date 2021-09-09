// <copyright file="VoiceAbstractNote.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Midi
{
    /// <summary>Represents a voice category message that deals with notes.</summary>
    [Serializable]
    public abstract class VoiceAbstractNote : VoiceEvent
    {
        #region Fields
        /// <summary>The MIDI note to modify (0x0 to 0x7F).</summary>
        private byte note;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the VoiceAbstractNote class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenCategory">The category identifier (0x0 through 0xF) for this voice event.</param>
        /// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
        /// <param name="note">The MIDI note to modify (0x0 to 0x7F).</param>
        protected VoiceAbstractNote(long deltaTime, byte givenCategory, MidiChannel channel, byte note) :
            base(deltaTime, givenCategory, channel) {
            this.Note = note;
        }

        /// <summary>Initializes a new instance of the VoiceAbstractNote class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="givenCategory">The category identifier (0x0 through 0xF) for this voice event.</param>
        /// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
        [UsedImplicitly]
        protected VoiceAbstractNote(long deltaTime, byte givenCategory, MidiChannel channel)
            : base(deltaTime, givenCategory, channel) {
        }

        #endregion

        #region Properties
        /// <summary>Gets or sets the MIDI note (0x0 to 0x7F).</summary>
        /// <value> General musical property.</value>
        public byte Note {
            get => this.note;

            set {
                if (value > 127) {
                    while (value > 127) {
                        value -= DefaultValue.HarmonicOrder;
                    }
                    //// // throw new ArgumentOutOfRangeException("Note", value, "The note must be in the range from 0 to 127.");
                }

                this.note = value;
            }
        }
        #endregion

        #region Protected Properties
        /// <summary>The first parameter as sent in the MIDI message.</summary>
        /// <value> General musical property.</value>
        public sealed override byte Parameter1 => this.note;

        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            if (this.Channel == MidiChannel.DrumChannel &&
                Enum.IsDefined(typeof(MidiRhythmicInstrument), this.note)) {
                sb.Append((MidiRhythmicInstrument)this.note); // print out percussion name
            }
            else {
                var note1 = MusicalProperties.GetNoteNameAndOctave(this.note, DefaultValue.HarmonicOrder);
                sb.Append(note1); // print out note name
            }

            sb.Append(" \t" + base.ToString());
            sb.Append(" \t");
            return sb.ToString();
        }
        #endregion

        #region Methods
        /// <summary>Write the event to the output stream.</summary>
        /// <param name="outputStream">The stream to which the event should be written.</param>
        public override void Write(Stream outputStream) {
            if (outputStream == null) {
                return;
            }
            //// Write out the base event information
            base.Write(outputStream);

            //// Write out the data
            outputStream.WriteByte(this.note);
        }
        #endregion
    }
}