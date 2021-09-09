// <copyright file="MidiInternalDevices.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub, Leslie Sanford</author>
// <email>stoub@microsoft.com, jabberdabber@hotmail.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Classes for interop with Win32 MCI and low-level MIDI API

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi
{
    /// <summary>Provides access to the Media Control Interface and other MIDI functionality.</summary>
    //// internal 
    public static class MidiInternalDevices {
        //// Given in OutputDeviceBase
        //// private const int MOM_OPEN = 0x3C7; private const int MOM_CLOSE = 0x3C8;  private const int MOM_DONE = 0x3C9;

        #region Fields
        //// internal const int CallbackFunction = 0x00030000;

        /// <summary>The default output MIDI device id. Midi Handle. </summary>
        private const int MidiMapper = -1;
        #endregion

        /// <summary>
        /// Synchronization Lock.
        /// </summary>
        private static readonly object ThisLock = new object();
        //// <summary>Used for synchronization of all MIDI-related operations.</summary>
        //// private static readonly object midiLock = new object();

        #region Fields
        /// <summary>The number of references to the currently open MIDI device.</summary>
        private static int numberReferences; //// = 0;

        #endregion

        #region Properties
        /// <summary>
        /// Gets Device Count (unused).
        /// </summary>
        [UsedImplicitly]
        public static int CountDevices => NativeMethods.MidiOutGetNumDevs();

        /// <summary>
        /// Gets Handle of the midi device.
        /// </summary>
        public static MidiDeviceHandle MidiDeviceHandle { get; private set; }

        /// <summary>
        /// Gets a value indicating whether player was open.
        /// </summary>
        public static bool IsOpen => MidiDeviceHandle != null && MidiDeviceHandle.IsOpen;

        #endregion

        #region Midi Output device - open, reset (Win32 Midi Output Functions)
        /// <summary>
        /// Open the default MIDI device.
        /// </summary>
        /// <remarks>
        /// This is necessary only when playing individual events.
        /// </remarks>
        public static void OpenMidi()
        {
            lock (ThisLock) {
                // Open the MIDI device if it hasn't already been opened.
                if (numberReferences == 0) {
                    InternalOpenMidi();
                }

                numberReferences++;
            }
        }

        /// <summary>Opens the MIDI device without regard for whether it has already been opened.</summary>
        public static void InternalOpenMidi() {
            // Open the default MIDI device -  Open the MIDI_MAPPER device (default).
            OpenMidiOut(MidiMapper);
        }

        /* Unfinished management of Midi devices
        /// <summary>
        /// Reset Midi Out (unused).
        /// </summary>
        [UsedImplicitly]
        public static void ResetMidiOut() {
            lock (thisLock) {
                // Reset the OutputDevice.
                int result = NativeMethods.midiOutReset(MidiDeviceHandle.Handle);

                if (result == MidiDeviceException.SystemNoError) {
                    //// while (bufferCount > 0) { Monitor.Wait(thisLock); }
                }
                else {
                    // Throw an exception.
                    throw new OutputDeviceException(result);
                }
            }
        } */
        #endregion

        #region Midi Output device - close (Win32 Midi Output Functions)
        /* Unfinished management of Midi devices
        /// <summary>Close the default MIDI device.</summary>
        [UsedImplicitly]
        public static void CloseMidi() {
            lock (thisLock) {
                // Close the MIDI device if no one else is using it
                if (numberReferences == 0) {
                    return;
                }

                numberReferences--;
                if (numberReferences == 0) {
                    InternalCloseMidi();
                }
            }
        }*/

        /// <summary>
        /// Prepares the midi.
        /// </summary>
        public static void PrepareMidi() {
            //// We can't play using MCI if we already have an open handle to the default
            //// MIDI device. As such, we'll temporarily close it if its open and then
            //// when we're done reopen it if it was open.
            if (IsOpen) {
                InternalCloseMidi();
            }
        }

        /// <summary>Closes the MIDI device without regard for the reference count.</summary>
        public static void InternalCloseMidi() {
            // Close the MIDI device if it is open
            if (MidiDeviceHandle == null) {
                return;
            }

            MidiDeviceHandle.Dispose();
            MidiDeviceHandle = null;
            numberReferences = 0;
        }

        /// <summary>Close the specified MIDI output device.</summary>
        /// <param name="handle">Handle of the MIDI output device.</param>
        public static void CloseMidiOut(int handle) {
            var result = NativeMethods.MidiOutClose(handle); //// MidiError ... this.midiHandle
            if (result != 0) { //// MidiSystemErrorNOERROR
                ThrowMciError(result, "Could not close MIDI out.");
                //// throw new Exception("Closing MIDI device failed with error " + result.ToString(CultureInfo.CurrentCulture));
            }
            //// if (MidiDeviceHandle != null) { MidiDeviceHandle.Close();  }
        }
        #endregion

        #region Midi Output devices - number, outcaps

        /// <summary>
        /// Midi GetDeviceCaps.
        /// </summary>
        /// <param name="deviceId">Device Id.</param>
        /// <param name="deviceLpCaps">Device Midi out Caps.</param>
        /// <param name="size">Given size.</param>
        /// <returns> Returns value. </returns>
        public static MidiError GetDeviceCaps(int deviceId, ref MidiOutcaps deviceLpCaps, int size) {
            return (MidiError)NativeMethods.MidiOutGetDevCaps(deviceId, ref deviceLpCaps, size);
        }

        /// <summary>
        /// Get Device Capabilities.
        /// </summary>
        /// <param name="deviceId">Device Id.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static MidiOutcaps GetDeviceCapabilities(int deviceId) {
            var caps = new MidiOutcaps(0, 0, 0, string.Empty, 0, 0, 0, 0, 0);

            // Get the device's capabilities.
            var result = NativeMethods.MidiOutGetDevCaps(deviceId, ref caps, Marshal.SizeOf(caps));

            // If the capabilities could not be retrieved.
            if (result != (int)MidiError.SystemNoError) {
                // Throw an exception.
                throw new OutputDeviceException(result);
            }

            return caps;
        }
        #endregion

        #region MCI error
        /// <summary>Throws an exception based on the MCI error number.</summary>
        /// <param name="rv">The MCI error number.</param>
        /// <param name="optionalMessage">The message to throw if an MCI message can't be retrieved.</param>
        public static void ThrowMciError(int rv, string optionalMessage) { //// [VL] was private
            //// If there is an error, throw an exception with the best error description we can get.
            //// var error = GetMciError(rv) ?? "Could not close MIDI out.";
            //// error += optionalMessage; //// [VL] optionalMessage was unused
            //// Could not open MIDIOut (Device is already used...)
            #warning What here?
            //// throw new InvalidOperationException(error);
        }
        #endregion

        /// <summary>
        /// Midi GetNumberOfDevices.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static short GetNumberOfDevices() { //// Int16
            return NativeMethods.MidiOutGetNumDevs();
        }

        #region Private Support
        /// <summary>Opens the specified MIDI output device.</summary>
        /// <param name="deviceId">The ID of the MIDI output device to be opened.</param>
        private static void OpenMidiOut(int deviceId) {
            var handle = 0;
            //// RunningStatusEnabled = false;
            var result = NativeMethods.MidiOutOpen(ref handle, deviceId, IntPtr.Zero, 0, 0); //// MidiError ... ref this.midiHandle, -1
            //// midiOutProc = HandleMessage;
            //// int result = NativeMethods.MidiOutOpen(ref handle, deviceId, midiOutProc, 0, CallbackFunction);
            if (result != 0) { //// MidiDeviceException.SystemNoError
                ThrowMciError(result, "Could not open MIDI out.");
                //// throw new OutputDeviceException(result);
                //// throw new Exception("Opening MIDI device failed with error " + result.ToString(CultureInfo.CurrentCulture));
            }

            MidiDeviceHandle = new MidiDeviceHandle(handle);
        }
        #endregion

        /* Unused
        #region Private Errors
        /// <summary>Gets the description for the given MCI error code.</summary>
        /// <param name="errorCode">The error code for which we need an error description.</param>
        /// <returns>The error description (or null if none exists).</returns>
        private static string GetMciError(int errorCode) {
            var buffer = new StringBuilder(255); // max string should be 128, so 255 just to be safe
            return NativeMethods.MciGetErrorString(errorCode, buffer, buffer.Capacity) == 0 ? null : buffer.ToString();
        }
        #endregion
        */

        //// Handles Windows messages.
        //// private static void HandleMessage(int handle, int msg, int instance, int param1, int param2) {  }

        /// <summary>
        /// Native Methods.
        /// </summary>
        private static class NativeMethods {
            //// <summary>
            //// Prevents a default instance of the NativeMethods class from being created.
            //// </summary>
            //// private NativeMethods() {}

            #region Native Methods - Low-Level MIDI API - MidiOut Open, Reset (Win32 Midi Output Functions)
            /// <summary>The midiOutOpen function opens a MIDI output device for playback.</summary>
            /// <param name="lphMidiOut">Pointer to an HMIDIOUT handle.</param>
            /// <param name="uDeviceId">Identifier of the MIDI output device that is to be opened.</param>
            /// <param name="dwCallback">Pointer to a callback function, an event handle, a thread identifier, or a handle of a window or thread called during MIDI playback to process messages related to the progress of the playback.</param>
            /// <param name="dwInstance">User instance data passed to the callback.</param>
            /// <param name="dwCallbackFlag">Callback flag for opening the device.</param>
            /// <returns>Returns SystemNoError if successful or an error otherwise.</returns>
            [DllImport("winmm.dll", EntryPoint = "midiOutOpen", CharSet = CharSet.Ansi)]   //// CharSet.Ansi
            [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "No other result found.")]
            public static extern int MidiOutOpen(ref int lphMidiOut, int uDeviceId, IntPtr dwCallback, int dwInstance, int dwCallbackFlag); //// dwFlags

            //// [DllImport("winmm.dll")]
            //// private static extern int midiOutOpen(ref int handle, int deviceID, MidiOutProc proc, int instance, int flags);            

            /// <summary>
            /// Midis the out reset.
            /// </summary>
            /// <param name="handle">The handle.</param>
            /// <returns> Returns value. </returns>
            [DllImport("winmm.dll", EntryPoint = "midiOutReset"), UsedImplicitly]
            public static extern int MidiOutReset(int handle);
            #endregion

            #region Native Methods - Low-Level MIDI API - MidiOut Close (Win32 Midi Output Functions)
            /// <summary>The midiOutClose function closes the specified MIDI output device.</summary>
            /// <param name="hMidiOut">Handle to the MIDI output device.</param>
            /// <returns>Returns SystemNoError if successful or an error otherwise.</returns>
            [DllImport("winmm.dll", EntryPoint = "midiOutClose", CharSet = CharSet.Ansi)]   //// CharSet.Ansi
            [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "No other result found.")]
            public static extern int MidiOutClose(int hMidiOut);

            //// [DllImport("winmm.dll")]
            //// private static extern int midiOutClose(int handle);
            #endregion

            #region NativeMethods - Devices
            /// <summary>
            /// MidiOut Get Number of Devices.
            /// </summary>
            /// <returns> Returns value. </returns>
            [DllImport("winmm.dll", EntryPoint = "midiOutGetNumDevs", CharSet = CharSet.Ansi)]
            [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "No other result found.")]
            public static extern short MidiOutGetNumDevs();  //// Int16

            /// <summary>
            /// MidiOut Get Device Caps.
            /// </summary>
            /// <param name="uDeviceId">Device Id.</param>
            /// <param name="lpCaps">Midi out Caps.</param>
            /// <param name="uSize">Given size.</param>
            /// <returns>
            /// Returns value.
            /// </returns>
            [DllImport("winmm.dll", EntryPoint = "midiOutGetDevCapsA")]
            [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "No other result found.")]
            public static extern int MidiOutGetDevCaps(int uDeviceId, ref MidiOutcaps lpCaps, int uSize);

            //// [DllImport("winmm.dll")]
            //// protected static extern int midiOutGetDevCaps(int deviceID,
            ////     ref MidiOutcaps caps, int sizeOfMidiOutCaps);
            #endregion

            /// <summary>The mciGetErrorString function retrieves a string that describes the specified MCI error code.</summary>
            /// <param name="fdwError">Error code returned by the mciSendCommand or mciSendString function.</param>
            /// <param name="lpszErrorText">Pointer to a buffer that receives a null-terminated string describing the specified error.</param>
            /// <param name="cchErrorText">Length of the buffer, in characters, pointed to by the previous parameter.</param>
            /// <returns>Returns non-zero if successful or 0 if the error code is not known.</returns>
            /// <remarks>Each string that MCI returns, whether data or an error description, can be a maximum of 128 characters.</remarks>
            //// Use string (or System.String) for a const char*, but StringBuilder for a char*.
            [DllImport("winmm.dll", EntryPoint = "mciGetErrorStringA", CharSet = CharSet.Ansi)]
            [UsedImplicitly]
//// CharSet.Ansi  //// 11/2010 - MarshalAs
            public static extern int MciGetErrorString(int fdwError, StringBuilder lpszErrorText, int cchErrorText);
            //// public static extern int MciGetErrorString(int fdwError, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpszErrorText, int cchErrorText);
            //// BestFitMapping: Turns on or off some optimization behavior when converting from Unicode to ANSI
            ////[DllImport("winmm.dll", CharSet = CharSet.Ansi, BestFitMapping = true, ThrowOnUnmappableChar = true)]
            ////[return: MarshalAs(UnmanagedType.Bool)]
            ////public static extern bool mciGetErrorString(uint mciError, [MarshalAs(UnmanagedType.LPStr)] System.Text.StringBuilder pszText, uint cchText);
        }
    }
}