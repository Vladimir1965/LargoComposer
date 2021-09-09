// <copyright file="MidiFileChunkHeader.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.IO;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Midi;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>Header for writing out MIDI files.</summary>
    [Serializable]
    public struct MidiFileChunkHeader { //// internal
        #region Fields
        /// <summary>The format for the MIDI file (0, 1, or 2).</summary>
        private readonly int format;

        /// <summary>Specifies the meaning of the delta-times.</summary>
        private readonly int division;

        /// <summary>The number of sequence in the MIDI sequence.</summary>
        private readonly int numTracks;

        /// <summary>Additional chunk header data.</summary>
        private readonly ChunkHeader header;

        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MidiFileChunkHeader struct.</summary>
        /// <param name="givenFormat">
        /// The format for the MIDI file (0, 1, or 2).
        /// 0 - a single multi-channel track
        /// 1 - one or more simultaneous sequence
        /// 2 - one or more sequentially independent single-track patterns.
        /// </param>
        /// <param name="numberTracks">The number of sequence in the MIDI file.</param>
        /// <param name="givenDivision">
        /// The meaning of the delta-times in the file.
        /// If the number is zero or positive, then bits 14 thru 0 represent the number of delta-time
        /// ticks which make up a quarter-note. If number is negative, then bits 14 through 0 represent
        /// subdivisions of a second, in a way consistent with SMPTE and MIDI time code.
        /// </param>
        public MidiFileChunkHeader(int givenFormat, int numberTracks, int givenDivision) {
            //// Verify the parameters
            if (givenFormat < 0 || givenFormat > 2) {
                throw new ArgumentOutOfRangeException(nameof(givenFormat), givenFormat, "Format must be 0, 1, or 2.");
            }

            if (numberTracks < 1) {
                throw new ArgumentOutOfRangeException(nameof(numberTracks), numberTracks, "There must be at least 1 track.");
            }

            if (givenDivision < 1) {
                throw new ArgumentOutOfRangeException(nameof(givenDivision), givenDivision, "Division must be at least 1.");
            }

            this.header = new ChunkHeader(
                                    MThdId,    // 0x4d546864 = "MThd"
                                    6);        // 2 bytes for each of the format, num sequence, and division == 6
            this.format = givenFormat;
            this.numTracks = numberTracks;
            this.division = givenDivision;
        }
        #endregion

        #region Public Properties
        /// <summary>Gets the format for the MIDI file (0, 1, or 2).</summary>
        /// <value> General musical property.</value>/// 
        public int Format => this.format;

        /// <summary>Gets the number of sequence in the MIDI sequence.</summary>
        /// <value> General musical property.</value>/// 
        public int NumberOfLines => this.numTracks;

        /// <summary>Gets the meaning of the delta-times.</summary>
        /// <value> General musical property.</value>/// 
        public int Division => this.division;

        #endregion

        #region Private Properties
        /// <summary>Gets the id for header.</summary>
        /// <value> General musical property.</value>
        private static byte[] MThdId {
            get {
                const byte m = 0x4d;
                const byte t = 0x54;
                const byte h = 0x68;
                const byte d = 0x64;
                return new[] { m, t, h, d };
            }
        }

        /// <summary>Gets additional chunk header data.</summary>
        /// <value> General musical property.</value>
        private ChunkHeader Header {
            get {
                Contract.Requires(this.header.Length >= 3);
                Contract.Ensures(Contract.Result<ChunkHeader>() != null);
                Contract.Ensures(Contract.Result<ChunkHeader>().Length >= 3);
                //// Contract.Ensures(Contract.Result<ChunkHeader>().GetId() != null);

                //// Resharper - ?Impure method is called for readonly field of value type
                var data = this.header.Id; //// GetId();
                if (data == null) {
                    throw new MidiParserException("Midi header can not be empty.", 0);
                }

                if (data.Length < 3) {
                    throw new MidiParserException("Incorrect MIDI header.", 0);
                }

                return this.header;
            }
        }
        #endregion

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="header1">The header1.</param>
        /// <param name="header2">The header2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(MidiFileChunkHeader header1, MidiFileChunkHeader header2) {
            return object.Equals(header1, header2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="header1">The header1.</param>
        /// <param name="header2">The header2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(MidiFileChunkHeader header1, MidiFileChunkHeader header2) {
            return !object.Equals(header1, header2);
        }

        #region Reading
        /// <summary>Read in an header chunk from the stream.</summary>
        /// <param name="inputStream">The stream from which to read the header chunk.</param>
        /// <returns>The header chunk read.</returns>
        public static MidiFileChunkHeader Read(Stream inputStream) {
            // Validate input
            if (inputStream == null) {
                throw new ArgumentNullException(nameof(inputStream));
            }

            if (!inputStream.CanRead) {
                throw new MidiParserException("Can't read MIDI file.", 0);
            }

            // Read in a header from the stream and validate it
            var header = ChunkHeader.Read(inputStream);
            if (header.Id == null) {
                throw new MidiParserException("Invalid MIDI header.", 0);
            }

            ValidateHeader(header);
            var format = ReadFormat(inputStream);
            var numTracks = ReadNumberOfSequence(inputStream);
            var division = ReadDivision(inputStream);

            // Create a new MThd header and return it
            return new MidiFileChunkHeader(format, numTracks, division);
        }

        #endregion

        #region Writing
        /// <summary>Writes the header out to the stream.</summary>
        /// <param name="outputStream">The stream to which the header should be written.</param>
        public void Write(Stream outputStream) {
            // Validate the stream
            if (outputStream == null) {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (!outputStream.CanWrite) {
                throw new MidiParserException("Can't write the MIDI file.", 0);
            }

            // Write out the main header
            this.Header.Write(outputStream);

            // Add format
            outputStream.WriteByte((byte)((this.format & DefaultValue.MaskFirstByte) >> 8));
            outputStream.WriteByte((byte)(this.format & DefaultValue.MaskSecondByte));

            // Add numTracks
            outputStream.WriteByte((byte)((this.numTracks & DefaultValue.MaskFirstByte) >> 8));
            outputStream.WriteByte((byte)(this.numTracks & DefaultValue.MaskSecondByte));

            // Add division
            outputStream.WriteByte((byte)((this.division & DefaultValue.MaskFirstByte) >> 8));
            outputStream.WriteByte((byte)(this.division & DefaultValue.MaskSecondByte));
        }
        #endregion

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() {
            return 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>True</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) {
            return false;
        }

        /// <summary>
        /// Reads the division.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns> Returns value. </returns>
        private static int ReadDivision(Stream inputStream) {
            Contract.Requires(inputStream != null);

            // Read in the division
            var division = 0;
            for (var i = 0; i < 2; i++) {
                var val = inputStream.ReadByte();
                if (val < 0) {
                    throw new MidiParserException("The MIDI file is invalid.", 0);
                }

                division <<= 8;
                division |= val;
            }

            return division;
        }

        /// <summary>
        /// Reads the number of sequence.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns> Returns value. </returns>
        private static int ReadNumberOfSequence(Stream inputStream) {
            Contract.Requires(inputStream != null);

            // Read in the number of sequence
            var numTracks = 0;
            for (var i = 0; i < 2; i++) {
                var val = inputStream.ReadByte();
                if (val < 0) {
                    throw new MidiParserException("The MIDI file is invalid.", 0);
                }

                numTracks <<= 8;
                numTracks |= val;
            }

            return numTracks;
        }

        /// <summary>
        /// Reads the format.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns> Returns value. </returns>
        private static int ReadFormat(Stream inputStream) {
            Contract.Requires(inputStream != null);

            // Read in the format
            var format = 0;
            for (var i = 0; i < 2; i++) {
                var val = inputStream.ReadByte();
                if (val < 0) {
                    throw new MidiParserException("The MIDI file is invalid.", 0);
                }

                format <<= 8;
                format |= val;
            }

            return format;
        }

        #region Validation

        /// <summary>Validates that a header is correct as header.</summary>
        /// <param name="header">The header to be validated.</param>
        private static void ValidateHeader(ChunkHeader header) {
            Contract.Requires(header.Id != null);
            const byte headerLength = 6;
            var validHeader = MThdId;
            for (var i = 0; i < 4; i++) {
                if (i >= header.Id.Length || i >= validHeader.Length) {
                    continue;
                }

                if (header.Id[i] != validHeader[i]) {
                    throw new MidiParserException("Invalid MIDI header.", 0);
                }
            }

            if (header.Length != headerLength) {
                throw new MidiParserException("The length of the MIDI header is incorrect.", 0);
            }
        }
        #endregion
    }
}
