// <copyright file="MidiDeviceItem.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 2011 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Globalization;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi
{
    /// <summary>
    /// Initializes a new instance of the MidiDevice class.
    /// </summary>
    public sealed class MidiDeviceItem {
        /// <summary>
        /// Initializes a new instance of the MidiDeviceItem class.
        /// </summary>
        /// <param name="deviceId">Device Id.</param>
        public MidiDeviceItem(int deviceId) {
            var moc = new MidiOutcaps(0, 0, 0, string.Empty, 0, 0, 0, 0, 0);
            MidiInternalDevices.GetDeviceCaps(deviceId, ref moc, Marshal.SizeOf(moc));
            this.MidiOutcaps = moc;
        }

        #region Public properties
        /// <summary>
        /// Gets Device Name.
        /// </summary>
        /// <value> General musical property.</value>
        public string Name => this.MidiOutcaps.ProductName;

        /// <summary>
        /// Gets Device Driver.
        /// </summary>
        /// <value> General musical property.</value>
        [UsedImplicitly]
        public string Driver => string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.MidiOutcaps.Technology, this.MidiOutcaps.DriverVersion);

        /* MidiOutcaps - Channels ?!
        /// <summary>
        /// Gets Device Channels.
        /// </summary>
        /// <value> General musical property.</value>
        public string Channels => this.MidiOutcaps.ChannelMask.ToString(CultureInfo.InvariantCulture);
        */

        /// <summary>
        /// Gets Device Notes.
        /// </summary>
        /// <value> General musical property.</value>
        public string Notes => this.MidiOutcaps.Notes.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets Device Voices.
        /// </summary>
        /// <value> General musical property.</value>
        public string Voices => this.MidiOutcaps.Voices.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets Device functions.
        /// </summary>
        /// <value> General musical property.</value>
        public string Functions => string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", this.MidiOutcaps.ManufacturerIdentifier, this.MidiOutcaps.ProductIdentifier, this.MidiOutcaps.Support);

        #endregion

        #region Private properties
        /// <summary>
        /// Gets Midi  Out capabilities.
        /// </summary>
        /// <value> General musical property.</value>
        private MidiOutcaps MidiOutcaps { get; }
        #endregion
    }
}
