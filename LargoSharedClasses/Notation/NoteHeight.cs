// <copyright file="NoteHeight.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Notation
{
    /// <summary>
    /// Note Height.
    /// </summary>
    public sealed class NoteHeight {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="NoteHeight"/> class.
        /// </summary>
        /// <param name="givenStep">The given step.</param>
        /// <param name="givenAlter">The given alter.</param>
        /// <param name="givenOctave">The given octave.</param>
        public NoteHeight(string givenStep, int givenAlter, int givenOctave) {
            this.Step = givenStep;
            this.Alter = givenAlter;
            this.Octave = givenOctave;
            this.MidiPitch = this.ToMidiPitch;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteHeight"/> class.
        /// </summary>
        /// <param name="givenMidiPitch">The given midi pitch.</param>
        [JetBrains.Annotations.UsedImplicitlyAttribute]
        public NoteHeight(int givenMidiPitch) {
            this.MidiPitch = givenMidiPitch;

            var midiPitchInLowestOctave = this.MidiPitch;
            while (midiPitchInLowestOctave > 32) {
                midiPitchInLowestOctave -= DefaultValue.HarmonicOrder;
            }

            switch (midiPitchInLowestOctave)
            {
                case 21:
                    this.Step = "A";
                    this.Alter = 0;
                    break;
                case 22:
                    this.Step = "B";
                    this.Alter = -1;
                    break;
                case 23:
                    this.Step = "B";
                    this.Alter = 0;
                    break;
                case 24:
                    this.Step = "C";
                    this.Alter = 0;
                    break;
                case 25:
                    this.Step = "C";
                    this.Alter = 1;
                    break;
                case 26:
                    this.Step = "D";
                    this.Alter = 0;
                    break;
                case 27:
                    this.Step = "E";
                    this.Alter = -1;
                    break;
                case 28:
                    this.Step = "E";
                    this.Alter = 0;
                    break;
                case 29:
                    this.Step = "F";
                    this.Alter = 0;
                    break;
                case 30:
                    this.Step = "F";
                    this.Alter = 1;
                    break;
                case 31:
                    this.Step = "G";
                    this.Alter = 0;
                    break;
                case 32:
                    this.Step = "G";
                    this.Alter = 1;
                    break;
            }

            if (this.MidiPitch < 24) {
                this.Octave = 0;
            }
            else if (this.MidiPitch < 36) {
                this.Octave = 1;
            }
            else if (this.MidiPitch < 48) {
                this.Octave = 2;
            }
            else if (this.MidiPitch < 60) {
                this.Octave = 3;
            }
            else if (this.MidiPitch < 72) {
                this.Octave = 4;
            }
            else if (this.MidiPitch < 84) {
                this.Octave = 5;
            }
            else if (this.MidiPitch < 96) {
                this.Octave = 6;
            }
            else if (this.MidiPitch < 108) {
                this.Octave = 7;
            }
            else if (this.MidiPitch < 120) {
                this.Octave = 8;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Note Step.
        /// </summary>
        /// <value> Property description. </value>
        public string Step { get; }

        /// <summary>
        /// Gets Note Alter.
        /// </summary>
        /// <value> Property description. </value>
        public int Alter { get; private set; }

        /// <summary>
        /// Gets Note Octave.
        /// </summary>
        /// <value> Property description. </value>
        public int Octave { get; }

        /// <summary>
        /// Gets Note Midi Pitch.
        /// </summary>
        /// <value> Property description. </value>
        public int MidiPitch { get; }

        /// <summary>
        /// Gets Toes the midi pitch.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value> 
        /// <returns> Returns value. </returns>
        private int ToMidiPitch {
            get {
                int pitch;
                switch (this.Step)
                {
                    case "A":
                        pitch = 21;
                        break;
                    case "B":
                        pitch = 23;
                        break;
                    case "C":
                        pitch = 24;
                        break;
                    case "D":
                        pitch = 26;
                        break;
                    case "E":
                        pitch = 28;
                        break;
                    case "F":
                        pitch = 29;
                        break;
                    case "G":
                        pitch = 31;
                        break;
                    default:
                        return 0;
                }

                //// Notes A0 and B0 are in octave 0 in MIDI standard:
                if ((this.Step == "A") || (this.Step == "B")) {
                    pitch = pitch + (this.Octave * DefaultValue.HarmonicOrder);
                }
                else {  //// The other are in octave 1 
                    pitch = pitch + ((this.Octave - 1) * DefaultValue.HarmonicOrder);
                }

                pitch = pitch + this.Alter;
                return pitch;
            }
        }
        #endregion

        #region String representation
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Step);
            while (this.Alter > 0) {
                sb.Append('#');
                this.Alter--;
            }

            while (this.Alter < 0) {
                sb.Append('b');
                this.Alter++;
            }

            sb.Append(this.Octave);

            return sb.ToString();
        }
        #endregion
    }
}
