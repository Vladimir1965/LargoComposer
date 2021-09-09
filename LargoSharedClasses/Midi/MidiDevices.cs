// <copyright file="MidiDevices.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Classes for interop with Win32 MCI and low-level MIDI API

using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi {
    /// <summary> Midi Devices. </summary>
    [UsedImplicitly]
    public sealed class MidiDevices {
        /// <summary>
        /// The maximum name length
        /// </summary>
        [UsedImplicitly] public const int MaxNameLength = 32;

        #region Constructors

        /// <summary> Initializes a new instance of the MidiDevices class. </summary>
        public MidiDevices() {
            int deviceCount = MidiInternalDevices.GetNumberOfDevices();
            if (deviceCount < 0) {
                throw new InvalidOperationException("Device-count cannot be negative.");
            }
            //// this.Devices = new MidiOutcaps[this.deviceCount];
            this.Items = new Collection<MidiDeviceItem>();
            for (var deviceId = 0; deviceId < deviceCount; ++deviceId) {
                var md = new MidiDeviceItem(deviceId);
                this.Items.Add(md);
            }
        }
        #endregion

        /// <summary>
        /// Gets Midi Device Items.
        /// </summary>
        /// <value> General property.</value>
        public Collection<MidiDeviceItem> Items { get; }
    }
}
