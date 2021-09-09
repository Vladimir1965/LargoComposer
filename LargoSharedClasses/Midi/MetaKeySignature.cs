// <copyright file="MetaKeySignature.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.IO;
using System.Text;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Midi
{
    /// <summary>A key signature meta event message.</summary>
    [Serializable]
    public sealed class MetaKeySignature : MetaEvent
    {
        #region Fields
        /// <summary>The meta id for this event.</summary>
        private const byte EventMetaId = 0x59;

        /// <summary>Number of sharps or flats in the signature.</summary>
        private TonalityKey key;

        /// <summary>TonalityGenus genus of the signature.</summary>
        private TonalityGenus tonalityGenus;

        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MetaKeySignature class.</summary>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="key">Key of the signature.</param>
        /// <param name="tonalGenus">Tonality genus of the signature.</param>
        public MetaKeySignature(long deltaTime, TonalityKey key, TonalityGenus tonalGenus) :
            base(deltaTime, EventMetaId) {
            this.Key = key;
            this.TonalityGenus = tonalGenus;
        }
        #endregion

        #region Properties
        /// <summary>Gets the numerator for the event.</summary>
        /// <value> General musical property.</value>
        public TonalityKey Key {
            get => this.key;

            private set {
                var k = (TonalityKey)(sbyte)value;
                if (!Enum.IsDefined(typeof(TonalityKey), k)) {
                    return;
                    //// throw new ArgumentOutOfRangeException(nameof(value), value, "Not a valid key.");
                }

                this.key = k;
            }
        }

        /// <summary>Gets or sets the denominator for the event.</summary>
        /// <value> General musical property.</value>
        private TonalityGenus TonalityGenus {
            get => this.tonalityGenus;

            set {
                if (!Enum.IsDefined(typeof(TonalityGenus), value)) {
                    this.tonalityGenus = TonalityGenus.Major;
                    //// throw new ArgumentOutOfRangeException("value", value, "Not a valid tonality.");
                }

                this.tonalityGenus = value;
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
            sb.Append(this.key);
            sb.Append("\t");
            sb.Append(this.TonalityGenus);
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
            outputStream.WriteByte(0x02);
            outputStream.WriteByte((byte)this.key);
            outputStream.WriteByte((byte)this.tonalityGenus);
        }
        #endregion
    }
}
