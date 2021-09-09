// <copyright file="MidiTrackChunkHeader.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Midi;
using System;
using System.IO;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>Header for writing out tracks.</summary>
    /// <remarks>MidiTrackChunkHeader is a bit of a misnomer as it includes all of the data for the track, as well, in byte form.</remarks>
    [Serializable]
    public struct MidiTrackChunkHeader { //// internal
        #region Fields

        /// <summary>Data for which this is a header.</summary>
        private readonly byte[] data;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MidiTrackChunkHeader struct.</summary>
        /// <param name="givenData">The track data for which this is a header.</param>
        public MidiTrackChunkHeader(byte[] givenData)
            : this() {
            this.Header = new ChunkHeader(
            MTrkId, // 0x4d54726b = "MTrk"
            givenData?.Length ?? 0);
            this.data = givenData;
        }
        #endregion

        #region Properties
        /// <summary>Gets the track header id.</summary>
        private static byte[] MTrkId {
            get {
                const byte m = 0x4d;
                const byte T = 0x54;
                const byte r = 0x72;
                const byte k = 0x6b;
                return new[] { m, T, r, k };
            }
        }

        /// <summary>Gets additional chunk header data.</summary>
        /// <value> General musical property.</value>
        private ChunkHeader Header { get; }
        #endregion

        #region Public static
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="header1">The header 1.</param>
        /// <param name="header2">The header 2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(MidiTrackChunkHeader header1, MidiTrackChunkHeader header2) {
            return object.Equals(header1, header2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="header1">The header 1.</param>
        /// <param name="header2">The header 2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(MidiTrackChunkHeader header1, MidiTrackChunkHeader header2) {
            return !object.Equals(header1, header2);
        }
        #endregion

        #region Reading
        /// <summary>Read in an midi track chunk from the stream.</summary>
        /// <param name="inputStream">The stream from which to read the midi track chunk.</param>
        /// <returns>The midi track chunk read.</returns>
        public static MidiTrackChunkHeader Read(Stream inputStream) {
            // Validate input
            if (inputStream == null) {
                throw new ArgumentNullException(nameof(inputStream));
            }

            if (!inputStream.CanRead) {
                throw new MidiParserException("Stream must be readable.", 0);
            }

            // Read in a header from the stream and validate it
            var header = ChunkHeader.Read(inputStream);
            ValidateHeader(header);
            if (header.Length < 0) {
                throw new MidiParserException("Header length cannot be negative.", 0);
            }
            //// Read in the data (amount specified in the header)
            var data = new byte[header.Length];
            long realLength = inputStream.Read(data, 0, data.Length);
            if (realLength != data.Length) {
                //// [VL] SQL Compact, I will read so much it is possible 
                //// throw new InvalidOperationException("Not enough data in stream to read MIDI Track chunk.");
                header.Length = realLength;
            }

            // Return the new chunk
            return new MidiTrackChunkHeader(data);
        }
        #endregion

        #region Writing
        /// <summary>Writes the track header out to the stream.</summary>
        /// <param name="outputStream">The stream to which the header should be written.</param>
        public void Write(Stream outputStream) {
            // Validate the stream
            if (outputStream == null) {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (!outputStream.CanWrite) {
                throw new MidiParserException("Cannot write to stream.", 0);
            }

            if (this.Header.Id == null) {
                throw new MidiParserException("Header.ID is null.", 0);
            }

            //// Write out the main header followed by all of the data
            this.Header.Write(outputStream);
            if (this.data != null) {
                outputStream.Write(this.data, 0, this.data.Length);
            }
        }
        #endregion

        #region Other public methods
        /// <summary>
        /// Gets the data for which this is a header.
        /// </summary>
        /// <returns> Returns value. </returns>
        /// <value> General musical property.</value>
        public byte[] GetData() {
            return this.data;
        }

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
        #endregion

        #region Private Validation
        /// <summary>Validates that a header is correct as header.</summary>
        /// <param name="header">The header to be validated.</param>
        private static void ValidateHeader(ChunkHeader header) {
            //// if (header.Length < 0) { throw new InvalidOperationException("Header length cannot be negative."); } 

            if (header.Id == null) {
                throw new MidiParserException("Header.ID is null.", 0);
            }

            var validHeader = MTrkId;
            if (header.Id.Length < 4 || validHeader.Length < 4) {
                throw new MidiParserException("Incomplete header.Id.", 0);
            }

            for (var i = 0; i < 4; i++) {
                if (header.Id[i] != validHeader[i]) {
                    throw new MidiParserException("Invalid MIDI header.", 0);
                }
            }

            if (header.Length <= 0) {
                throw new MidiParserException("The length of the MIDI header is incorrect.", 0);
            }
        }
        #endregion
    }
}
