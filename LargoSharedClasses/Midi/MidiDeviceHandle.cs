// <copyright file="MidiDeviceHandle.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Classes for interop with Win32 MCI and low-level MIDI API

using System;

namespace LargoSharedClasses.Midi
{
    /// <summary>Represents a safe handle to a MIDI device.</summary>
    public sealed class MidiDeviceHandle : IDisposable {
        #region Fields

        /// <summary>Whether the handle has been disposed.</summary>
        private bool isDisposed; //// = false;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the MidiDeviceHandle class.</summary>
        /// <param name="handle">The handle to the MIDI device.</param>
        public MidiDeviceHandle(int handle) {
            // Store the handle
            this.Handle = handle;
        }

        /// <summary>
        /// Finalizes an instance of the MidiDeviceHandle class.
        /// </summary>
        ~MidiDeviceHandle() {
            this.DisposeDevice(); //// false
        }
        #endregion

        #region Properties

        /// <summary>Gets the underlying handle.</summary>
        /// <value> General musical property.</value>
        public int Handle { get; private set; }

        /// <summary>Gets a value indicating whether the handle is active and open.</summary>
        /// <value> General musical property.</value>
        public bool IsOpen => this.Handle != 0;

        #endregion

        /// <summary>Dispose of the handle.</summary>
        public void Dispose() {
            this.DisposeDevice(); //// true
            //// FxCop
            GC.SuppressFinalize(this); 
        }

        #region Closing the Handle
        /// <summary>Closes the handle.</summary>
        private void Close() {
            // If the handle is open, close it and mark it as such.
            if (!this.IsOpen) {
                return;
            }

            MidiInternalDevices.CloseMidiOut(this.Handle);
            this.Handle = 0;
        }

        #endregion

        /// <summary>Dispose of the handle.</summary>
        private void DisposeDevice() { //// bool disposing
            // If not yet disposed
            if (this.isDisposed)
            {
                return;
            }

            // Close the handle and mark us as having been disposed
            this.Close();
            this.isDisposed = true;
            //// FxCop:   if (disposing) { GC.SuppressFinalize(this);  }
        }
    }
}
