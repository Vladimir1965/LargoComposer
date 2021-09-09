// <copyright file="MidiEvent.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Contains Classes to represent MIDI events (voice, meta, system, etc).

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Midi
{
    /// <summary>A MIDI event, serving as the base class for all types of MIDI events.</summary>
    /// was abstract
    [Serializable]
    public class MidiEvent : ICloneable, IMidiEvent
    {
        #region Fields
        /// <summary>The amount of time before this event.</summary>
        private long deltaTime;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MidiEvent class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        public MidiEvent(long deltaTime) { //// protected
            // Store the data
            this.DeltaTime = deltaTime;
            this.StartTime = deltaTime;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        public string EventType {
            get {
                var eventType = this.GetType().ToString();
                var dotPosition = eventType.LastIndexOf('.');
                var pureType = eventType.Substring(dotPosition + 1);
                //// string dotPureType = Path.GetExtension(eventType);
                return pureType;
            }
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public long StartTime { get; set; }

        /// <summary>Gets or sets the amount of time before this event.</summary>
        /// <value> General musical property.</value>
        public long DeltaTime { //// virtual (11/2010)
            get => this.deltaTime;

            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Delta times must be non-negative.");
                }

                this.deltaTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the bar number.
        /// </summary>
        /// <value>
        /// The bar number.
        /// </value>
        public int BarNumber { get; set; }

        /// <summary>
        /// Gets a value indicating whether IsVoiceNoteEvent.
        /// </summary>
        /// <value> General musical property.</value>
        public bool IsVoiceNoteEvent => this is VoiceAbstractNote voiceEvent && (voiceEvent.Channel != MidiChannel.DrumChannel);

        /// <summary>
        /// Gets a value indicating whether IsMetaEvent.
        /// </summary>
        /// <value> General musical property.</value>
        public bool IsMetaEvent {
            get {
                var eventType = this.EventType;
                switch (eventType) {
                    case "MetaText":
                        break;
                    case "MetaCopyright":
                        break;
                    case "MetaSequenceTrackName":
                        break;
                    case "MetaInstrument":
                        break;
                    case "MetaLyric":
                        break;
                    case "MetaMarker":
                        break;
                    case "MetaCuePoint":
                        break;
                    case "MetaProgramName":
                        break;
                    case "MetaDeviceName":
                        break;
                    default:
                        return false;
                }

                return true;
            }
        }

        #endregion

        #region Static methods
        /// <summary>Splits a 14-bit value into two bytes each with 7 of the bits.</summary>
        /// <param name="bits">The value to be split.</param>
        /// <param name="upperBits">The upper 7 bits.</param>
        /// <param name="lowerBits">The lower 7 bits.</param>
        //// internal 
        public static void Split14BitsToBytes(int bits, out byte upperBits, out byte lowerBits) {
            lowerBits = (byte)(bits & 0x7F);
            bits >>= 7;
            upperBits = (byte)(bits & 0x7F);
        }

        #endregion

        #region Public methods
        /// <summary>Write the event to the output stream.</summary>
        /// <param name="outputStream">The stream to which the event should be written.</param>
        public virtual void Write(Stream outputStream) {
            Contract.Requires(outputStream != null);
            //// Write out the delta time
            WriteVariableLength(outputStream, this.deltaTime);
        }

        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(this.GetType().Name.PadRight(12));
            sb.Append("\t");
            var startString = " StartTime =" + this.StartTime.ToString(CultureInfo.CurrentCulture.NumberFormat);
            sb.Append(startString.PadRight(12));
            var deltaString = " DeltaTime =" + this.DeltaTime.ToString(CultureInfo.CurrentCulture.NumberFormat);
            sb.Append(deltaString.PadRight(12));
            return sb.ToString();
        }
        #endregion

        #region Public Implementation of ICloneable
        /// <summary>Creates a shallow copy of the MIDI event.</summary>
        /// <returns>A shallow-clone of the MIDI event.</returns>
        public IMidiEvent Clone() {
            return (MidiEvent)this.MemberwiseClone();
        }
        #endregion

        #region Implementation of ICloneable

        /// <summary>Creates a shallow-copy of the object.</summary>
        /// <returns>A shallow-clone of the MIDI event.</returns>
        object ICloneable.Clone() {
            return this.Clone();
        }
        #endregion

        #region Internal methods

        /// <summary>Combines two 7-bit values into a single 14-bit value.</summary>
        /// <param name="upper">The upper 7-bits.</param>
        /// <param name="lower">The lower 7-bits.</param>
        /// <returns>A 14-bit value stored in an integer.</returns>
        internal static int CombineBytesTo14Bits(byte upper, byte lower) {
            // Turn the two bytes into a 14 bit value
            int fourteenBits = upper;
            fourteenBits <<= 7;
            fourteenBits |= lower;
            return fourteenBits;
        }
        #endregion

        #region Protected methods
        /// <summary>Converts an array of bytes into human-readable form.</summary>
        /// <param name="givenData">The array to convert.</param>
        /// <returns>The string containing the bytes.</returns>
        protected static string DataToString(byte[] givenData) {
            if (givenData == null) {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.Append("[");
            for (var i = 0; i < givenData.Length; i++) {
                ////  If we're not the first byte, output a comma as a separator
                if (i > 0) {
                    sb.Append(",");
                }

                ////  Spit out the byte itself
                sb.Append("0x");
                sb.Append(givenData[i].ToString("X2", CultureInfo.CurrentCulture.NumberFormat));
            }

            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>Writes bytes for a long value in the special 7-bit form.</summary>
        /// <param name="outputStream">The stream to which the length should be written.</param>
        /// <param name="value">The value to be converted and written.</param>
        protected static void WriteVariableLength(Stream outputStream, long value) {
            Contract.Requires(outputStream != null);
            if (outputStream == null) {
                return;
            }

            // TODO: Clean this up!

            // Parse the value into bytes containing each set of 7-bits and a 1-bit marker
            // for whether there are more bytes in the length
            var buffer = value & 0x7f;
            while ((value >>= 7) > 0) {
                buffer <<= 8;
                buffer |= 0x80;
                buffer += value & 0x7f;
            }

            // Get all of the bytes in correct order
            while (true) {
                outputStream.WriteByte((byte)(buffer & 0xFF));
                if ((buffer & 0x80) == 0) {
                    break;
                } // if the marker bit is not set, we're done
                buffer >>= 8;
            }
        }
        #endregion
    }
}
