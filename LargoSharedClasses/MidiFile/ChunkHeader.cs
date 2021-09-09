// <copyright file="ChunkHeader.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Classes and structs that represent MIDI file and track headers.

using System;
using System.Diagnostics.Contracts;
using System.IO;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Midi;

namespace LargoSharedClasses.MidiFile
{
    #region File and Track Headers

    /// <summary>Chunk header to store base MIDI chunk information.</summary>
    [Serializable]
    public struct ChunkHeader { //// internal
        #region Fields
        /// <summary>The id representing this chunk header.</summary>
        private readonly byte[] id;

        /// <summary>The amount of data following the header.</summary>
        private long length;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the ChunkHeader struct.</summary>
        /// <param name="givenId">The 4-byte header identifier.</param>
        /// <param name="length">The amount of data to be stored under this header.</param>
        public ChunkHeader(byte[] givenId, long length) {
            // Verify the parameters
            if (givenId == null) {
                throw new ArgumentNullException(nameof(givenId));
            }

            if (givenId.Length != 4) {
                throw new MidiParserException("The MIDI header must be 4 bytes in length.", 0);
            }

            if (length < 0) {
                throw new MidiParserException("Invalid MIDI header.", 0);
            }

            // Store the data
            this.id = givenId;
            this.length = length;
        }
        #endregion

        #region Properties

        /// <summary>Gets or sets the amount of data following the header.</summary>
        /// <value> General musical property.</value>
        public long Length {
            get => this.length;
            //// private 
            set => this.length = value;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value> General musical property.</value>
        public byte[] Id => this.id;

        #endregion

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="header1">The header1.</param>
        /// <param name="header2">The header2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ChunkHeader header1, ChunkHeader header2) {
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
        public static bool operator !=(ChunkHeader header1, ChunkHeader header2) {
            return !object.Equals(header1, header2);
        }

        #region Reading
        /// <summary>Reads a chunk header from the input stream.</summary>
        /// <param name="inputStream">The stream from which to read.</param>
        /// <returns>The chunk header read from the stream.</returns>
        public static ChunkHeader Read(Stream inputStream) {
            // Validate input
            if (inputStream == null) {
                throw new ArgumentNullException(nameof(inputStream));
            }

            if (!inputStream.CanRead) {
                throw new MidiParserException("Can't read the MIDI file.", 0);
            }

            // Read the id
            var id = new byte[4];
            if (inputStream.Read(id, 0, id.Length) != id.Length) {
                throw new MidiParserException("The input MIDI file is invalid.", 0);
            }

            // Read the length
            long length = 0;
            for (var i = 0; i < 4; i++) {
                var val = inputStream.ReadByte();
                if (val < 0) {
                    throw new MidiParserException("The header of the MIDI file is invalid.", 0);
                }

                length <<= 8;
                length |= (byte)val;
            }

            // Create a new header with the read data
            return new ChunkHeader(id, length);
        }
        #endregion

        #region Output
        /// <summary>Writes the chunk header out to the stream.</summary>
        /// <param name="outputStream">The stream to which the header should be written.</param>
        public void Write(Stream outputStream) {
            Contract.Requires(this.Id != null);
            Contract.Requires(this.Id.Length > 3);
            //// Validate the stream
            if (outputStream == null) {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (!outputStream.CanWrite) {
                throw new MidiParserException("Enable to write data.", 0);
            }

            // Write out the id
            if (this.id != null) {
                outputStream.WriteByte(this.id[0]);
                outputStream.WriteByte(this.id[1]);
                outputStream.WriteByte(this.id[2]);
                outputStream.WriteByte(this.id[3]);
            }

            // Write out the length in big-endian
            outputStream.WriteByte((byte)((this.length & DefaultValue.MaskByte1Of4) >> DefaultValue.ThreeBytes));
            outputStream.WriteByte((byte)((this.length & DefaultValue.MaskByte2Of4) >> DefaultValue.TwoBytes));
            outputStream.WriteByte((byte)((this.length & DefaultValue.MaskByte3Of4) >> DefaultValue.OneByte));
            outputStream.WriteByte((byte)(this.length & DefaultValue.MaskByte4Of4));
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
    }
    #endregion
}
