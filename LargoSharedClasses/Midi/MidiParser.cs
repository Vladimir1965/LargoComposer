// <copyright file="MidiParser.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Classes for parsing track data into actual MIDI track and event objects

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Midi {
    /// <summary>MIDI track parser.</summary>
    /// was internal, added sealed
    public sealed class MidiParser {
        /// <summary>
        /// SysEx Continue.
        /// </summary>
        private static bool sysExContinue;

        /// <summary>
        /// Data to parse.
        /// </summary>
        private readonly byte[] data;

        /// <summary>
        /// SysEx data to parse.
        /// </summary>
        private byte[] sysExData;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiParser"/> class.
        /// </summary>
        /// <param name="givenData">The given data.</param>
        /// <param name="givenSysExData">The given sys ex data.</param>
        public MidiParser(byte[] givenData, byte[] givenSysExData) {
            Contract.Requires(givenData != null);
            //// if (givenData == null) { throw new InvalidOperationException("Empty Midi data to parse"); }

            this.data = givenData;
            this.sysExData = givenSysExData;
        }

        #endregion

        /// <summary>
        /// Parses a byte array into a track's worth of events.
        /// </summary>
        /// <returns>
        /// The track containing the parsed events.
        /// </returns>
        public MidiTrack ParseToTrack() { //// cyclomatic complexity 10:15
            var pos = 0; // current position in data
            var status = 0; // the current status byte
            sysExContinue = false; // whether we're in a multi-segment system exclusive message
            try {
                // Create the new track
                var track = new MidiTrack();

                // Process all bytes, turning them into events
                while (pos >= 0 && pos < this.data.Length) {
                    //// // Read in the delta time
                    long deltaTime = ReadVariableLength(this.data, ref pos);
                    var running = this.GetRunningStatus(pos, ref status);
                    var tempEvent = this.ParseDataToEvent(deltaTime, ref pos, status, running);

                    if (tempEvent != null) {
                        //// Add the newly parsed event if we got one
                        track.Events.Add(tempEvent);
                    }
                }
                //// Return the newly populated track
                return track;
            }
            catch (OverflowException exc) { //// Wrap all other exceptions in MidiParserExceptions
                throw new MidiParserException("Failed to parse MIDI file (Overflow).", exc, pos);
            }
            catch (OutOfMemoryException exc) { //// Wrap all other exceptions in MidiParserExceptions
                throw new MidiParserException("Failed to parse MIDI file (Out of memory).", exc, pos);
            }
            catch (ArithmeticException exc) { //// Wrap all other exceptions in MidiParserExceptions
                throw new MidiParserException("Failed to parse MIDI file (Arithmetic exception).", exc, pos);
            }

            //// Let MidiParserExceptions through
            //// catch (MidiParserException) { throw; } 
        }

        /// <summary>
        /// Parse TextEvent.
        /// </summary>
        /// <param name="deltaTime">The previously parsed delta-time for this event.</param>
        /// <param name="eventType">The previously parsed type of message we're expecting to find.</param>
        /// <param name="data">The data stream from which to read the event information.</param>
        /// <param name="pos">The position of the start of the event information.</param>
        /// <returns> Returns value. </returns>
        private static MidiEvent ParseTextEvent(long deltaTime, MidiMetaType eventType, byte[] data, ref int pos) { //// cyclomatic complexity 10:12
            Contract.Requires(data != null);
            Contract.Requires(pos > (long)0);
            Contract.Requires(pos < data.Length);
            //// if (data == null) { return null; }

            MidiEvent tempEvent = null;
            if (!(eventType == MidiMetaType.TextEvent
                || eventType == MidiMetaType.CopyrightNotice
                || eventType == MidiMetaType.SoundtrackName
                || eventType == MidiMetaType.TrackInstrumentName
                || eventType == MidiMetaType.Lyric
                || eventType == MidiMetaType.Marker
                || eventType == MidiMetaType.CuePoint
                || eventType == MidiMetaType.ProgramName
                || eventType == MidiMetaType.DeviceName)) {
                return null;
            }

            var text = ReadAsciiText(data, ref pos);
            switch (eventType) {
                //// Text events (copyright, lyrics, etc)
                case MidiMetaType.TextEvent:
                    tempEvent = new MetaText(deltaTime, text);
                    break;
                case MidiMetaType.CopyrightNotice:
                    tempEvent = new MetaCopyright(deltaTime, text);
                    break;
                case MidiMetaType.SoundtrackName:
                    tempEvent = new MetaSequenceTrackName(deltaTime, text);
                    break;
                case MidiMetaType.TrackInstrumentName:
                    tempEvent = new MetaInstrument(deltaTime, text);
                    break;
                case MidiMetaType.Lyric:
                    tempEvent = new MetaLyric(deltaTime, text);
                    break;
                case MidiMetaType.Marker:
                    tempEvent = new MetaMarker(deltaTime, text);
                    break;
                case MidiMetaType.CuePoint:
                    tempEvent = new MetaCuePoint(deltaTime, text);
                    break;
                case MidiMetaType.ProgramName:
                    tempEvent = new MetaProgramName(deltaTime, text);
                    break;
                case MidiMetaType.DeviceName:
                    tempEvent = new MetaDeviceName(deltaTime, text);
                    break;
                case MidiMetaType.TrackSequenceNumber:
                    break;
                case MidiMetaType.MidiChannelPrefix:
                    break;
                case MidiMetaType.MidiPort:
                    break;
                case MidiMetaType.MetaEndOfTrack:
                    break;
                case MidiMetaType.SetTempo:
                    break;
                case MidiMetaType.TimeCodeOffset:
                    break;
                case MidiMetaType.TimeSignature:
                    break;
                case MidiMetaType.KeySignature:
                    break;
                case MidiMetaType.SequencerSpecific:
                    break;
                //// resharper default: break;
            }

            return tempEvent;
        }

        /// <summary>
        /// Parse UnknownEvent.
        /// </summary>
        /// <param name="deltaTime">The previously parsed delta-time for this event.</param>
        /// <param name="eventType">The previously parsed type of message we're expecting to find.</param>
        /// <param name="data">The data stream from which to read the event information.</param>
        /// <param name="pos">The position of the start of the event information.</param>
        /// <returns> Returns value. </returns>
        private static MidiEvent ParseUnknownEvent(long deltaTime, byte eventType, byte[] data, ref int pos) {
            Contract.Requires(data != null);
            if (data == null || pos < 0) {
                return null;
            }

            ////  Read in the variable length and that much data, then store it
            var length = ReadVariableLength(data, ref pos);
            if (length < 0 || pos < data.GetLowerBound(0) || pos + length > data.GetLowerBound(0) + data.Length) {
                return null;
            }

            var unknownData = new byte[length];
            Array.Copy(data, pos, unknownData, 0, length);
            MidiEvent tempEvent = new MetaUnknown(deltaTime, eventType, unknownData);
            pos += length;

            return tempEvent;
        }

        /// <summary>Parse a meta MIDI event from the data stream.</summary>
        /// <param name="deltaTime">The previously parsed delta-time for this event.</param>
        /// <param name="eventType">The previously parsed type of message we're expecting to find.</param>
        /// <param name="data">The data stream from which to read the event information.</param>
        /// <param name="pos">The position of the start of the event information.</param>
        /// <returns>The parsed meta MIDI event.</returns>
        private static MidiEvent ParseMetaEvent(long deltaTime, byte eventType, byte[] data, ref int pos) { //// cyclomatic complexity 10:16
            Contract.Requires(data != null);
            Contract.Requires(pos >= (long)0);
            Contract.Requires(pos + 1 < data.Length);
            //// if (data == null) { return null; }
            try {
                MidiEvent tempEvent = null;
                if (eventType >= (byte)MidiMetaType.TextEvent && eventType <= (byte)MidiMetaType.DeviceName) { //// 0x01 to 0x09
                    tempEvent = ParseTextEvent(deltaTime, (MidiMetaType)eventType, data, ref pos);
                    return tempEvent;
                }
                //// Create the correct meta event based on its meta event id/type
                switch ((MidiMetaType)eventType) {
                    //// Sequence number
                    case MidiMetaType.TrackSequenceNumber:
                        tempEvent = GetSequenceNumberEvent(deltaTime, data, ref pos);
                        break;

                    //// Channel prefix
                    case MidiMetaType.MidiChannelPrefix:
                        pos++; // skip 0x1
                        if (pos >= 0 && pos < data.Length) {
                            tempEvent = new MetaChannelPrefix(deltaTime, data[pos]);
                        }

                        pos++; // skip read value
                        break;

                    //// Port number
                    case MidiMetaType.MidiPort:
                        pos++; // skip 0x1
                        if (pos >= 0 && pos < data.Length) {
                            tempEvent = new MetaPort(deltaTime, data[pos]);
                        }

                        pos++; //// skip read value
                        break;

                    //// Key signature
                    case MidiMetaType.KeySignature:
                        pos++; // skip past 0x2
                        if (pos >= 0 && pos + 1 < data.Length) {
                            tempEvent = new MetaKeySignature(deltaTime, (TonalityKey)data[pos], (TonalityGenus)data[pos + 1]);
                        }

                        pos += 2;
                        break;
                    
                    //// End of track
                    case MidiMetaType.MetaEndOfTrack:
                        pos++; //// skip 0x0
                        tempEvent = new MetaEndOfTrack(deltaTime);
                        break;

                    //// Tempo
                    case MidiMetaType.SetTempo:
                        tempEvent = GetTempoEvent(deltaTime, data, ref pos);
                        break;

                    //// SMPTE offset
                    case MidiMetaType.TimeCodeOffset:
                        tempEvent = GetTimeCodeOffsetEvent(deltaTime, data, ref pos);
                        break;

                    //// Time signature
                    case MidiMetaType.TimeSignature:
                        tempEvent = GetTimeSignatureEvent(deltaTime, data, ref pos);
                        break;

                    //// Proprietary
                    case MidiMetaType.SequencerSpecific:
                        tempEvent = GetSequencerSpecificEvent(deltaTime, data, ref pos);

                        break;

                    //// An unknown meta event!
                    default:
                        tempEvent = ParseUnknownEvent(deltaTime, eventType, data, ref pos);
                        break;
                }

                return tempEvent;
            }
            catch (OverflowException exc) {
                throw new MidiParserException("Unable to parse meta MIDI event (Overflow).", exc, pos);
            }
            catch (OutOfMemoryException exc) {
                throw new MidiParserException("Unable to parse meta MIDI event (Out of memory).", exc, pos);
            }
            catch (ArithmeticException exc) {
                throw new MidiParserException("Unable to parse meta MIDI event (Arithmetic exception).", exc, pos);
            }
        }

        #region Reading Helpers
        /// <summary>Reads an ASCII text sequence from the data stream.</summary>
        /// <param name="data">The data stream from which to read the text.</param>
        /// <param name="pos">The position of the start of the sequence.</param>
        /// <returns>The text as a string.</returns>
        private static string ReadAsciiText(byte[] data, ref int pos) {
            Contract.Requires(data != null);
            Contract.Requires(pos >= (long)0);
            Contract.Requires(pos < data.Length);
            //// Read the length of the string, grab the following data as ASCII text, move ahead
            var length = ReadVariableLength(data, ref pos);
            //// 2013/03
            var text = data.Length >= pos + length ? Encoding.ASCII.GetString(data, pos, length) : string.Empty;
            pos += length;
            return text;
        }

        /// <summary>Reads a variable-length value from the data stream.</summary>
        /// <param name="data">The data to process.</param>
        /// <param name="pos">The position at which to start processing.</param>
        /// <returns>The value read; position is updated to reflect the new position.</returns>
        private static int ReadVariableLength(IList<byte> data, ref int pos) {
            Contract.Requires(data != null);
            if (pos < 0 || pos >= data.Count) {
                return 0;
            }
            //// Start with the first byte
            int length = data[pos];

            //// If the special "there's more data" marker isn't set, we're done
            if ((data[pos] & 0x80) != 0) {
                // Remove the special marker
                length &= 0x7f;
                do {
                    ////  Continually get all bytes, removing the marker, until no marker is found
                    pos++;
                    if (pos < data.Count) {
                        length = (length << 7) + (data[pos] & 0x7f);
                    }
                }
                while (pos < data.Count && ((data[pos] & 0x80) != 0));
            }

            // Advance past the last used byte and return the length
            pos++;
            return length;
        }
        #endregion

        #region Particular Meta Events
        /// <summary>
        /// Gets the tempo event.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="data">The given data.</param>
        /// <param name="pos">The position.</param>
        /// <returns> Returns value. </returns>
        private static MidiEvent GetTempoEvent(long deltaTime, IList<byte> data, ref int pos) {
            Contract.Requires(data != null);
            const byte eventLength = 3;
            pos++; // skip 0x3
            var tempo = pos >= 0 && pos + 2 < data.Count ? (data[pos] << 16) | data[pos + 1] << 8 | data[pos + 2] : 0;

            MidiEvent tempEvent = new MetaTempo(deltaTime, tempo);
            pos += eventLength;
            return tempEvent;
        }

        /// <summary>
        /// Gets the time code offset event.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="data">The given data.</param>
        /// <param name="pos">The position.</param>
        /// <returns> Returns value. </returns>
        private static MidiEvent GetTimeCodeOffsetEvent(long deltaTime, IList<byte> data, ref int pos) {
            Contract.Requires(data != null);
            const byte eventLength = 5;
            pos++; // skip 0x5
            MidiEvent tempEvent = pos >= 0 && pos + 4 < data.Count ? new MetaTimeCodeOffset(deltaTime, data[pos], data[pos + 1], data[pos + 2], data[pos + 3], data[pos + 4]) 
                : null;

            pos += eventLength;
            return tempEvent;
        }

        /// <summary>
        /// Gets the time signature event.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="data">The given data.</param>
        /// <param name="pos">The position.</param>
        /// <returns> Returns value. </returns>
        private static MidiEvent GetTimeSignatureEvent(long deltaTime, IList<byte> data, ref int pos) {
            Contract.Requires(data != null);
            const byte eventLength = 4;
            pos++; // skip past 0x4
            MidiEvent tempEvent = pos >= 0 && pos + 3 < data.Count ? new MetaTimeSignature(deltaTime, data[pos], data[pos + 1], data[pos + 2], data[pos + 3]) : null;

            pos += eventLength;
            return tempEvent;
        }

        /// <summary>
        /// Gets the sequencer specific event.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="data">The given data.</param>
        /// <param name="pos">The position.</param>
        /// <returns> Returns value. </returns>
        private static MidiEvent GetSequencerSpecificEvent(long deltaTime, byte[] data, ref int pos) {
            Contract.Requires(data != null);
            ////  Read in the variable length and that much data, then store it
            var length = ReadVariableLength(data, ref pos);
            if (length < 0 || pos < data.GetLowerBound(0) || pos + length > data.GetLowerBound(0) + data.Length)
            {
                return null;
            }

            var propData = new byte[length];
            Array.Copy(data, pos, propData, 0, length);
            MidiEvent tempEvent = new MetaProprietary(deltaTime, propData);
            pos += length;

            return tempEvent;
        }

        /// <summary>
        /// Gets the sequence number event.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="data">The given data.</param>
        /// <param name="pos">The position.</param>
        /// <returns> Returns value. </returns>
        private static MidiEvent GetSequenceNumberEvent(long deltaTime, IList<byte> data, ref int pos) {
            Contract.Requires(data != null);
            const byte eventLength = 2;
            MidiEvent tempEvent = null;
            pos++;     //// skip past the 0x02
            if (pos >= 0 && pos + 1 < data.Count) {
                var number = (data[pos] << 8) | data[pos + 1];
                tempEvent = new MetaSequenceNumber(deltaTime, number);
            }

            pos += eventLength;  //// skip read values
            return tempEvent;
        }
        
        /// <summary>Parse a voice event from the data stream.</summary>
        /// <param name="deltaTime">The previously parsed delta-time for this event.</param>
        /// <param name="messageType">The previously parsed type of message we're expecting to find.</param>
        /// <param name="channel">The previously parsed channel for this message.</param>
        /// <param name="data">The data stream from which to read the event information.</param>
        /// <param name="pos">The position of the start of the event information.</param>
        /// <returns>The parsed voice MIDI event.</returns>
        private static MidiEvent ParseVoiceEvent(
                            long deltaTime,
                            MidiVoiceMessageType messageType,
                            MidiChannel channel,
                            IList<byte> data,
                            ref int pos) {  //// cyclomatic complexity 10:16
            Contract.Requires(data != null);
            Contract.Requires(pos + 1 < data.Count);
            //// if (data == null) { return null; }

            try {
                MidiEvent tempEvent = null;
                //// Create the correct voice event based on its message id/type
                switch (messageType) {
                    case MidiVoiceMessageType.VoiceNoteOff:
                        if (pos >= 0 && pos + 1 < data.Count) {
                            tempEvent = new VoiceNoteOff(deltaTime, channel, data[pos], data[pos + 1]);
                        }

                        pos += 2;
                        break;

                    case MidiVoiceMessageType.VoiceNoteOn:
                        if (pos >= 0 && pos + 1 < data.Count) {
                            tempEvent = new VoiceNoteOn(deltaTime, channel, data[pos], data[pos + 1]);
                        }

                        pos += 2;
                        break;

                    case MidiVoiceMessageType.PolyphonicKeyPressure:
                        if (pos >= 0 && pos + 1 < data.Count) {
                            tempEvent = new VoiceAftertouch(deltaTime, channel, data[pos], data[pos + 1]);
                        }

                        pos += 2;
                        break;

                    case MidiVoiceMessageType.ControllerChange:
                        if (pos >= 0 && pos + 1 < data.Count) {
                            tempEvent = new VoiceController(deltaTime, channel, data[pos], data[pos + 1]);
                        }

                        pos += 2;
                        break;

                    case MidiVoiceMessageType.ProgramChange:
                        if (pos >= 0 && pos < data.Count) {
                            tempEvent = new VoiceProgramChange(deltaTime, channel, data[pos]);
                        }

                        pos += 1;
                        break;
                    case MidiVoiceMessageType.ChannelKeyPressure:
                        if (pos >= 0 && pos < data.Count) {
                            tempEvent = new VoiceChannelPressure(deltaTime, channel, data[pos]);
                        }

                        pos += 1;
                        break;

                    case MidiVoiceMessageType.PitchBend:
                        tempEvent = GetPitchBendEvent(deltaTime, channel, data, ref pos);
                        break;

                    //// UH OH!
                    default:
                        throw new ArgumentOutOfRangeException(nameof(messageType), messageType, "Not a voice message.");
                }
                //// Return the newly parsed event
                return tempEvent;
            }
            catch (OverflowException exc) {
                throw new MidiParserException("Unable to parse meta MIDI event (Overflow).", exc, pos);
            }
            catch (OutOfMemoryException exc) {
                throw new MidiParserException("Unable to parse meta MIDI event (Out of memory).", exc, pos);
            }
            catch (ArithmeticException exc) {
                throw new MidiParserException("Unable to parse meta MIDI event (Arithmetic exception).", exc, pos);
            }
        }

        /// <summary>
        /// Gets the pitch bend event.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="data">The given data.</param>
        /// <param name="pos">The position.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static MidiEvent GetPitchBendEvent(long deltaTime, MidiChannel channel, IList<byte> data, ref int pos) {
            Contract.Requires(data != null);
            const byte eventLength = 2;
            MidiEvent tempEvent = null;
            if (pos >= 0 && pos + 1 < data.Count) {
                var position = (data[pos] << 8) | data[pos + 1];
                MidiEvent.Split14BitsToBytes(position, out var upper, out var lower);
                tempEvent = new VoicePitchWheel(deltaTime, channel, upper, lower);
            }

            pos += eventLength;
            return tempEvent;
        }
        #endregion

        #region High-Level Parsing
        /// <summary>
        /// Gets the running status.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="status">The status.</param>
        /// <returns> Returns value. </returns>
        private bool GetRunningStatus(int pos, ref int status) {
            //// // Get the next character
            var nextValue = (byte)(pos >= 0 && pos < this.data.Length ? this.data[pos] : 0);
            //// // Are we continuing a sys ex? If so, the next value better be 0x7F
            if (sysExContinue && (nextValue != 0x7f)) {
                throw new MidiParserException("Expected to find a system exclusive continue byte.", pos);
            }
            //// // Are we in running status? Determine whether we're running and
            //// // what the current status byte is.
            bool running; // whether we're in running status
            if ((nextValue & 0x80) == 0) {
                ////  We're now in running status... if the last status was 0, uh oh!
                if (status == 0) {
                    throw new MidiParserException("Status byte required for running status.", pos);
                }
                //// // Keep the last iteration's status byte, and now we're in running mode
                running = true;
            }
            else {
                //// // Not running, so store the current status byte and mark running as false
                status = nextValue;
                running = false;
            }

            return running;
        }

        /// <summary>
        /// Parses the data to event.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <param name="pos">The position.</param>
        /// <param name="status">The status.</param>
        /// <param name="running">If set to <c>true</c> [running].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private MidiEvent ParseDataToEvent(long deltaTime, ref int pos, int status, bool running) {
            Contract.Requires(this.data != null);

            MidiEvent tempEvent = null;
            //// // Grab the 4-bit identifier
            var messageType = (byte)((status >> 4) & 0xF);
            //// // Handle voice events
            if (messageType >= (byte)MidiVoiceMessageType.VoiceNoteOff && messageType <= (byte)MidiVoiceMessageType.PitchBend) { //// 0x8 to 0xE
                if (!running) {
                    // if we're running, we don't advance; if we're not running, we do 
                    pos++;
                }

                var channel = (MidiChannel)(byte)(status & 0xF); // grab the channel from the status byte
                if (pos + 1 < this.data.Length) {
                    tempEvent = ParseVoiceEvent(deltaTime, (MidiVoiceMessageType)messageType, channel, this.data, ref pos);
                }
            }
            else {
                switch ((MidiCommandCode)status) {
                    case MidiCommandCode.MetaEvent: {
                            //// Handle meta events
                            pos++;
                            byte eventType = 0;
                            if (pos >= 0 && pos < this.data.Length) {
                                eventType = this.data[pos];
                            }

                            pos++;
                            tempEvent = ParseMetaEvent(deltaTime, eventType, this.data, ref pos);
                        }

                        break;
                    case MidiCommandCode.SystemExclusive:
                        tempEvent = this.ParseSysExEvent(deltaTime, ref pos);
                        break;
                    case MidiCommandCode.EndOfSystemExclusive:
                        tempEvent = this.ParseSysExData(deltaTime, ref pos);
                        break;
                    default:
                        throw new MidiParserException("Invalid status byte found.", pos);
                }
            }

            return tempEvent;
        }
        #endregion

        #region Event Parsing
        /// <summary>
        /// Parse SysExEvent.
        /// </summary>
        /// <param name="deltaTime">The previously parsed delta-time for this event.</param>
        /// <param name="pos">The position of the start of the event information.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private MidiEvent ParseSysExEvent(long deltaTime, ref int pos) {
            Contract.Requires(this.data != null);

            MidiEvent tempEvent = null;
            pos++;
            var length = pos >= 0 && pos < this.data.Length ? ReadVariableLength(this.data, ref pos) : 0;

            //// If this is single-segment message, process the whole thing
            if (pos + length - 1 >= 0 && pos + length - 1 < this.data.Length
                            && this.data[pos + length - 1] == (byte)MidiCommandCode.EndOfSystemExclusive
                            && length > 0 && pos >= this.data.GetLowerBound(0)) {
                this.sysExData = new byte[length - 1];
                Array.Copy(this.data, pos, this.sysExData, 0, length - 1);
                tempEvent = MidiEventSystemExclusive.NewMidiEventSystemExclusive(deltaTime, this.sysExData);
            }
            else { //// It's multi-segment, so add the new data to the previously acquired data
                //// Add to previously acquired sys ex data
                var oldLength = this.sysExData?.Length ?? 0;
                if (oldLength + length >= 0 && pos >= this.data.GetLowerBound(0)
                            && pos + length <= this.data.GetLowerBound(0) + this.data.Length) {
                    var newSysExData = new byte[oldLength + length];
                    this.sysExData?.CopyTo(newSysExData, 0);

                    if (length > 0) {
                        Array.Copy(this.data, pos, newSysExData, oldLength, length);
                    }

                    //// sysExData = newSysExData; Resharper
                    sysExContinue = true;
                }
            }

            pos += length;
            return tempEvent;
        }

        /// <summary>
        /// Parse SysEx Data.
        /// </summary>
        /// <param name="deltaTime">The previously parsed delta-time for this event.</param>
        /// <param name="pos">The position of the start of the event information.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private MidiEvent ParseSysExData(long deltaTime, ref int pos) {
            Contract.Requires(this.data != null);

            if (!sysExContinue) {
                this.sysExData = null;
            }

            //// Figure out how much data there is
            pos++;
            long length = pos >= 0 && pos < this.data.Length ? ReadVariableLength(this.data, ref pos) : 0;

            //// Add to previously acquired sys ex data
            var oldLength = this.sysExData?.Length ?? 0;
            if (oldLength + length >= 0) {
                var newSysExData = new byte[oldLength + length];
                this.sysExData?.CopyTo(newSysExData, 0);

                if (length >= 0) {
                    Array.Copy(this.data, pos, newSysExData, oldLength, (int)length);
                    this.sysExData = newSysExData;
                }
            }

            //// Make it a system message if necessary (i.e. if we find an end marker)
            if (pos + length - 1 < 0 || this.data[pos + length - 1] != (byte)MidiCommandCode.EndOfSystemExclusive) {
                return null;
            }

            MidiEvent tempEvent = MidiEventSystemExclusive.NewMidiEventSystemExclusive(deltaTime, this.sysExData);
            //// sysExData = null; Resharper
            sysExContinue = false;

            return tempEvent;
        }
        #endregion
    }
}