// <copyright file="MidiOutcaps.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Classes and structs that represent MIDI file and track headers.

using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi
{
    /// <summary>
    /// Midi out-caps.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MidiOutcaps {
        /// <summary>
        /// The midi devices maximum name length
        /// </summary>
        public const int MidiDevicesMaxNameLength = 32;

        /// <summary>
        /// Manufacturer identifier of the device driver for the Midi output 
        /// device. 
        /// </summary>
        private readonly short manufacturerIdentifier; //// Int16

        /// <summary>
        /// Product identifier of the Midi output device. 
        /// </summary>
        private readonly short productIdentifier; //// Int16

        /// <summary>
        /// Version number of the device driver for the Midi output device. The 
        /// high-order byte is the major version number, and the low-order byte 
        /// is the minor version number. 
        /// </summary>
        private readonly int driverVersion; 

        /// <summary>
        /// Product name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MidiDevicesMaxNameLength)] 
        private readonly string productName;  

        /// <summary>
        /// Flags describing the type of the Midi output device. 
        /// </summary>
        private readonly short technology; //// Int16

        /// <summary>
        /// Number of voices supported by an internal synthesizer device. If 
        /// the device is a port, this member is not meaningful and is set 
        /// to 0. 
        /// </summary>
        private readonly short voices; //// Int16

        /// <summary>
        /// Maximum number of simultaneous notes that can be played by an 
        /// internal synthesizer device. If the device is a port, this member 
        /// is not meaningful and is set to 0. 
        /// </summary>
        private readonly short notes; //// Int16

        /// <summary>
        /// Channels that an internal synthesizer device responds to, where the 
        /// least significant bit refers to channel 0 and the most significant 
        /// bit to channel 15. Port devices that transmit on all channels set 
        /// this member to 0xFFFF. 
        /// </summary>
        private readonly short channelMask; //// Int16

        /// <summary>
        /// Optional functionality supported by the device. 
        /// </summary>
        private readonly int support; 

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiOutcaps" /> struct.
        /// </summary>
        /// <param name="givenMidiId">The given midi id.</param>
        /// <param name="givenProcessId">The given process id.</param>
        /// <param name="givenDriverVersion">The given driver version.</param>
        /// <param name="givenName">The given name.</param>
        /// <param name="givenTechnology">The given technology.</param>
        /// <param name="givenVoices">The given voices.</param>
        /// <param name="givenNotes">The given notes.</param>
        /// <param name="givenChannelMask">The given channel mask.</param>
        /// <param name="givenSupport">The given support.</param>
        public MidiOutcaps(
                    short givenMidiId, 
                    short givenProcessId,
                    int givenDriverVersion,
                    string givenName,
                    short givenTechnology,
                    short givenVoices, 
                    short givenNotes, 
                    short givenChannelMask, 
                    int givenSupport)
        {
            this.manufacturerIdentifier = givenMidiId;
            this.productIdentifier = givenProcessId;
            this.driverVersion = givenDriverVersion;
            this.productName = givenName;
            this.technology = givenTechnology;
            this.voices = givenVoices;
            this.notes = givenNotes;
            this.channelMask = givenChannelMask;
            this.support = givenSupport;
        }

        /// <summary>
        /// Gets the midi id.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        internal short ManufacturerIdentifier => this.manufacturerIdentifier;

        /// <summary>
        /// Gets the process id.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        internal short ProductIdentifier => this.productIdentifier;

        /// <summary>
        /// Gets the driver version.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        internal int DriverVersion => this.driverVersion;

        /// <summary>
        /// Gets the name.
        /// </summary>
        internal string ProductName => this.productName;

        /// <summary>
        /// Gets the technology.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        internal short Technology => this.technology;

        /// <summary>
        /// Gets the voices.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        internal short Voices => this.voices;

        /// <summary>
        /// Gets the notes.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        internal short Notes => this.notes;

        /// <summary>
        /// Gets the channel mask.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        [UsedImplicitly]
        internal short ChannelMask => this.channelMask;

        /// <summary>
        /// Gets the support.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        internal int Support => this.support;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="outcaps1">The out-caps1.</param>
        /// <param name="outcaps2">The out-caps2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(MidiOutcaps outcaps1, MidiOutcaps outcaps2) {
            return object.Equals(outcaps1, outcaps2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="outcaps1">The out-caps1.</param>
        /// <param name="outcaps2">The out-caps2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(MidiOutcaps outcaps1, MidiOutcaps outcaps2) {
            return !object.Equals(outcaps1, outcaps2);
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
    }
}
