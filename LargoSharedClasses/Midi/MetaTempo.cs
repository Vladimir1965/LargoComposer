// <copyright file="MetaTempo.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Midi
{
    using Abstract;
    using JetBrains.Annotations;
    using Music;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>A tempo meta event message.</summary>
    [Serializable]
    public sealed class MetaTempo : MetaEvent {
        /// <summary>
        /// Tempo Base Number.
        /// </summary>
        public const int MidiTempoBaseNumber = 60000000;

        /// <summary>
        /// Max Tempo Value.
        /// </summary>
        private const int MidiMaxTempoValue = 0xFFFFFF;
        
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x51;

        /// <summary>The tempo for the event.</summary>
        private int tempoValue;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaTempo class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="value">The tempo for the event.</param>
        public MetaTempo(long deltaTime, int value)
            : base(deltaTime, EventMetaId) {
            this.Value = value;
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets the tempo for the event.</summary>
        /// <value> General musical property.</value>
        public int Tempo {
            get
            {
                if (this.Value != 0) {
                    return (int)Math.Round(1.000f * MidiTempoBaseNumber / this.Value, 0);
                }

                return 0;
            }

            [UsedImplicitly]
            set => this.tempoValue = value > 0 ? MidiTempoBaseNumber / value : (int)MusicalTempo.Tempo120;
        }

        /// <summary>Gets or sets the tempo for the event.</summary>
        /// <value> General musical property.</value>
        private int Value {
            get => this.tempoValue;

            set {
                if (value < 0 || value > MidiMaxTempoValue) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The tempo must be in the range from 0x0 to 0xFFFFFF.");
                }

                this.tempoValue = value;
            }
        }
        #endregion

        #region To String
        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("\t");
            sb.Append(" Tempo=" + this.Tempo.ToString(CultureInfo.CurrentCulture.NumberFormat));
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

            //// Event data
            outputStream.WriteByte(0x03);
            outputStream.WriteByte((byte)((this.tempoValue & DefaultValue.MaskByte2Of4) >> 16)); //// 0xFF0000
            outputStream.WriteByte((byte)((this.tempoValue & DefaultValue.MaskByte3Of4) >> 8)); //// 0x00FF00
            outputStream.WriteByte((byte)(this.tempoValue & DefaultValue.MaskByte4Of4));        //// 0x0000FF
        }
        #endregion
    }
}
