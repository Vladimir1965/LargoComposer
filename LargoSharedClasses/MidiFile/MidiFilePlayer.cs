// <copyright file="MidiFilePlayer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>Stephen Toub</author>
// <email>stoub@microsoft.com</email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
// Class to play MIDI events/sequence/sequence/files.

using LargoSharedClasses.Midi;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>Plays MIDI files and messages.</summary>
    public static class MidiFilePlayer { //// sealed
        #region Midi Player
        /// <summary>
        /// Prepares the midi.
        /// </summary>
        public static void PrepareMidi() {
            if (MidiClock.Singleton.IsRunning) {
                MidiClock.Singleton.Stop();
            }

            MidiInternalDevices.PrepareMidi();
            MidiInternalMessages.PrepareMidi();
        }

        /// <summary>
        /// Opens the midi.
        /// </summary>
        public static void OpenMidi() {
            MidiInternalDevices.OpenMidi();
            //// if (!MidiInternalDevices.OpenMidi()) { }
        }

        /// <summary>
        /// Midi FileOpen.
        /// </summary>
        /// <param name="path">Midi file Path.</param>
        /// <param name="alias">Midi file Alias.</param>
        public static void MidiFileOpenAndPlay(string path, string alias) {
            PrepareMidi();
            MidiInternalMessages.MidiFileOpen(path, alias);
            MidiInternalMessages.MidiFilePlay();
        }

        /// <summary>
        /// Midi FileClose.
        /// </summary>
        public static void MidiFileClose() {
            MidiInternalMessages.MidiFileClose();
            //// MidiInternalDevices.CloseMidi();
        }
        #endregion
    }
}