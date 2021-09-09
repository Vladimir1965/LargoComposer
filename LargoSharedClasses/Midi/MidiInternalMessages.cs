// <copyright file="MidiInternalMessages.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using JetBrains.Annotations;

namespace LargoSharedClasses.Midi
{
    /// <summary>
    /// Midi Internal.
    /// </summary>
    internal static class MidiInternalMessages {
        /// <summary>
        /// Synchronization Lock.
        /// </summary>
        private static readonly object ThisLock = new object();

        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value>
        ///   <c>True</c> if this instance is open; otherwise, <c>false</c>.
        /// </value>
        private static bool IsOpen => !string.IsNullOrEmpty(PlayingAlias);

        /// <summary>
        /// Gets or sets the playing alias.
        /// </summary>
        /// <value>
        /// The playing alias.
        /// </value>
        private static string PlayingAlias { get; set; }

        #region Midi Output Messages - sending

        /// <summary>
        /// Send Midi Message.
        /// </summary>
        /// <param name="givenType">Midi Command Code.</param>
        /// <param name="channel">Midi Channel.</param>
        /// <param name="dataValue1">Data Value1.</param>
        /// <param name="dataValue2">Data Value2.</param>
        public static void SendMidiMessage(MidiCommandCode givenType, int channel, int dataValue1, int dataValue2 = 0) {
            lock (ThisLock) {
                if (MidiInternalDevices.MidiDeviceHandle == null) {
                    MidiInternalDevices.InternalOpenMidi();
                }

                if (MidiInternalDevices.MidiDeviceHandle != null) {
                    SendMidiMessage(MidiInternalDevices.MidiDeviceHandle, ((int)givenType | channel) | (dataValue1 << 8) | (dataValue2 << 16));
                }
            }
        }
        #endregion

        #region SendString
        /// <summary>
        /// Prepares the midi.
        /// </summary>
        public static void PrepareMidi() {
            //// We can't play using MCI if we already have an open handle to the default
            //// MIDI device. As such, we'll temporarily close it if its open and then
            //// when we're done reopen it if it was open.
            if (IsOpen) {
                MidiFileClose();
            }
        }

        /// <summary>
        /// Midi FileOpen.
        /// </summary>
        /// <param name="path">Midi file Path.</param>
        /// <param name="alias">Midi file Alias.</param>
        public static void MidiFileOpen(string path, string alias) {
            CheckPath(path);
            PlayingAlias = alias;
            //// MidiInternalDevices.PrepareMidi();
            //// Open the file, play it, close it
            MciSendString(string.Format(CultureInfo.InvariantCulture, "open \"{0}\" type mpegvideo alias {1}", path, alias));
        }

        /// <summary>
        /// Midi FilePlay.
        /// </summary>
        public static void MidiFilePlay() {
            lock (ThisLock) {
                //// MidiInternalDevices.MciSendString("play " + alias + " wait");
                if (IsOpen) {
                    MciSendString("play " + PlayingAlias);
                }
            }
        }

        /// <summary>
        /// Midi FileClose.
        /// </summary>
        public static void MidiFileClose() {
            if (!IsOpen) {
                return;
            }

            MciSendString("close " + PlayingAlias);
            PlayingAlias = null;

            //// Reopen the MIDI device if it was previously open
            //// if (MidiInternalDevices.IsOpen) { MidiInternalDevices.InternalOpenMidi(); }
        }

        /// <summary>Plays the specified MIDI file using Media Control Interface (MCI).</summary>
        /// <param name="path">The MIDI file to be played.</param>
        [UsedImplicitly]
        public static void PlayFile(string path) {
            CheckPath(path);
            //// Play the file using interop calls: open the file, play it (wait for it to finish), close it
            var alias = Guid.NewGuid().ToString("N", CultureInfo.CurrentCulture); //// randomly generated alias to avoid collisions
            lock (ThisLock) {
                //// We can't play using MCI if we already have an open handle to the default
                //// MIDI device. As such, we'll temporarily close it if its open and then
                //// when we're done reopen it if it was open.
                if (MidiInternalDevices.IsOpen) {
                    MidiInternalDevices.InternalCloseMidi();
                }
                //// Open the file, play it, close it
                MciSendString(string.Format(CultureInfo.InvariantCulture, "open \"{0}\" type mpegvideo alias {1}", path, alias));
                MciSendString(string.Format(CultureInfo.InvariantCulture, "play {0} wait", alias));
                MciSendString("close " + alias);

                //// Reopen the MIDI device if it was previously open
                //// if (MidiInternalDevices.IsOpen) { MidiInternalDevices.InternalOpenMidi(); }
            }
        }

        #endregion

        #region MCI Commands
        /// <summary>Sends an MCI command.</summary>
        /// <param name="command">The command to be sent.</param>
        public static void MciSendString(string command) {
            // Make sure we got a command
            if (command == null) {
                throw new ArgumentNullException(nameof(command));
            }

            // Send the command.
            var rv = NativeMethods.MciSendString(command, null, 0, IntPtr.Zero);
            if (rv != 0) {
                MidiInternalDevices.ThrowMciError(rv, string.Format(CultureInfo.InvariantCulture, "Could not execute command '{0}'.", command));
            }
        }

        #endregion

        #region
        /// <summary>
        /// Check Path.
        /// </summary>
        /// <param name="path">Midi File Path.</param>
        private static void CheckPath(string path) {
            // Validate the parameter; make sure the file actually exists
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path)) {
                throw new FileNotFoundException("The MIDI file was not found.", path);
            }
        }
        #endregion

        /// <summary>Sends the message to as a short MIDI message to the MIDI output device.</summary>
        /// <param name="handle">Handle to the MIDI output device.</param>
        /// <param name="message">The message to be sent.</param>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private static void SendMidiMessage(MidiDeviceHandle handle, int message) {
            Contract.Requires(handle != null);
            lock (ThisLock) {
                if (handle == null) {
                    MidiInternalDevices.InternalOpenMidi();
                }

                //// if (handle == null) { 
                //// throw new ArgumentNullException("handle", "The handle does not exist. Make sure the MIDI device has been opened."); }
                //// if (handle == null) { return; }

                //// 2013/02
                if (handle != null)
                {
                    var h = handle.Handle;
                    if (h == 0) { //// !handle.IsOpen
                        return;
                    }

                    //// An unhandled exception of type 'System.AccessViolationException' occurred in LargoBase.dll
                    //// int result = 0;
                    //// try {
                    //// Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
                    //// When handle.Handle > 2 mil !?!
                    //// result = 
                    NativeMethods.MidiOutShortMessage(h, message); //// MidiError result
                }
                //// }
                //// catch (AccessViolationException) {  return; } 
                //// 2013/02 The handler is used by another process, such as a callback.
                //// if (result != 0) {  //// MidiError.MidiSystemErrorNOERROR
                //// MidiInternalDevices.ThrowMciError(result, string.Format(CultureInfo.InvariantCulture, "Could not execute message '{0}'.", message));
                //// throw new Exception("Could not send MIDI message: " + Result.ToString(CultureInfo.CurrentCulture)); }
            }
        }

        /// <summary>
        /// Native Methods.
        /// </summary>
        private static class NativeMethods {
            #region Native Methods - Low-Level MIDI API - Messages
            /// <summary>The function sends a short MIDI message to the specified MIDI output device.</summary>
            /// <param name="hMidiOut">Handle to the MIDI output device.</param>
            /// <param name="dwMsg">MIDI message.</param>
            /// <returns>Returns SystemNoError if successful or an error otherwise.</returns>
            [DllImport("winmm.dll", EntryPoint = "midiOutShortMsg", CharSet = CharSet.Ansi)]   //// CharSet.Ansi
            [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "No other result found.")]
            public static extern int MidiOutShortMessage(int hMidiOut, int dwMsg);

            //// [DllImport("winmm.dll")]
            //// protected static extern int midiOutShortMsg(int handle, int message);

            //// Commented out as Unused
            //// [DllImport("winmm.dll")]
            //// protected static extern int midiOutPrepareHeader(int handle, IntPtr headerPtr, int sizeOfMidiHeader);

            //// Commented out as Unused
            //// [DllImport("winmm.dll")]
            //// protected static extern int midiOutUnprepareHeader(int handle, IntPtr headerPtr, int sizeOfMidiHeader);

            //// Commented out as Unused
            //// [DllImport("winmm.dll")]
            //// protected static extern int midiOutLongMsg(int handle, IntPtr headerPtr, int sizeOfMidiHeader);

            #endregion

            #region DllImports for MCI
            /// <summary>
            /// The mciSendString function sends a command string to an MCI device. The device that the
            /// command is sent to is specified in the command string.
            /// </summary>
            /// <param name="lpszCommand">Pointer to a null-terminated string that specifies an MCI command string.</param>
            /// <param name="lpszReturn">Pointer to a buffer that receives return information. If no return information is needed, this parameter can be null.</param>
            /// <param name="cchReturn">Size, in characters, of the return buffer specified by the previous parameter.</param>
            /// <param name="callbackHandle">Handle to a callback window if the "notify" flag was specified in the command string.</param>
            /// <returns>
            /// Returns zero if successful or an error otherwise. The low-order word of the returned
            /// DWORD value contains the error return value. If the error is device-specific, the
            /// high-order word of the return value is the driver identifier; otherwise, the high-order
            /// word is zero.
            /// </returns>
            //// Use string (or System.String) for a const char*, but StringBuilder for a char*.
            [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi)]   //// CharSet.Ansi //// 11/2010 - MarshalAs
            public static extern int MciSendString(string lpszCommand, StringBuilder lpszReturn, int cchReturn, IntPtr callbackHandle);
            //// [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
            //// public static extern int MciSendString([MarshalAs(UnmanagedType.LPWStr)]string lpszCommand, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpszReturnString, int cchReturn, IntPtr callbackHandle);
            //// public static extern int MciSendString([MarshalAs(UnmanagedType.LPWStr)]string lpszCommand, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpszReturnString, int cchReturn, IntPtr callbackHandle);
            //// [DllImport("winmm.dll", CharSet = CharSet.Ansi, BestFitMapping = true, ThrowOnUnmappableChar = true)]
            //// [return: MarshalAs(UnmanagedType.U4)]
            //// public static extern uint mciSendCommand(uint mciId, uint uMsg, uint dwParam1, IntPtr dwParam2);

            //// <summary>
            //// Sends command string.
            //// </summary>
            //// <param name="commandString">Command string.</param>
            //// <param name="returnString">Return string.</param>
            //// <param name="iReturnLength">Return Length.</param>
            //// <param name="callbackHandle">Callback Handle.</param>
            //// <returns>Returns value.</returns>
            //// [DllImport("winmm.dll")]
            //// Use string (or System.String) for a const char*, but StringBuilder for a char*.
            //// private static extern long mciSendString([MarshalAs(UnmanagedType.LPWStr)]string commandString, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder returnString, int iReturnLength, IntPtr callbackHandle);
            //// public static extern long mciSendString(string commandString, StringBuilder returnString, int iReturnLength, IntPtr callbackHandle);

            #endregion
        }
    }
}
