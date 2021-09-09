// <copyright file="DirectPlayer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>
//// Windows directly support only: SystemSounds.Asterisk.Play();
//// Asterisk, Beep, Exclamation, Hand, Question

using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using LargoSharedClasses.Midi;

namespace LargoSharedClasses.MidiFile
{
    /// <summary>
    /// Direct Midi Player.
    /// </summary>
    public sealed class DirectPlayer
    {
        /// <summary>
        /// The was used
        /// </summary>
        private static bool wasUsed;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DirectPlayer class.
        /// </summary>
        public DirectPlayer() {
            const byte defaultSpaceDuration = 5;
            //// this.BarDuration = DefaultValue.LargeNumber; 
            this.SpacingDuration = defaultSpaceDuration;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the list events.
        /// </summary>
        /// <value>
        /// The list events.
        /// </value>
        public static List<VoiceEvent> ListEvents { get; set; }

        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        public static long CurrentTime { get; set; }

        /// <summary>
        /// Gets or sets Player.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public static DirectPlayer Player { get; set; }

        /// <summary> Gets SpacingDuration. </summary> 
        /// <value> Property description. </value>
        private int SpacingDuration { get; }
        #endregion

        #region Static methods
        /// <summary>Plays an individual MIDI event.</summary>
        /// <param name="midiEvent">The event to be played.</param>
        /// <remarks>
        /// Only VoiceEvent's are actually sent to the MIDI device.
        /// Delta-times are ignored.
        /// OpenMidi must be called before calling Play. CloseMidi should
        /// be called once all events have been played to free up the device.
        /// </remarks>
        public static void PlayEvent(IMidiEvent midiEvent) {
            wasUsed = true;
            ////lock (thisLock) {
            // Only send voice messages
            if (midiEvent is VoiceEvent vev && MidiInternalDevices.MidiDeviceHandle != null) {
                ////  Send the MIDI event to the MIDI device
                //// MidiInternalMessages.SendMidiMessage(MidiInternalDevices.MidiDeviceHandle, vev.Message);
                //// MidiInternalMessages.SendMidiMessage(MidiInternalDevices.MidiDeviceHandle, ((int)vev.Status | vev.Channel) | (vev.Parameter1 << 8) | (vev.Parameter2 << 16));
                MidiInternalMessages.SendMidiMessage((MidiCommandCode)vev.Status, (byte)vev.Channel, vev.Parameter1, vev.Parameter2);
                ListEvents?.Add(vev);
            }

            midiEvent.StartTime = CurrentTime;
            //// midiEvent.DeltaTime = DirectPlayer.CurrentTime;
            ////}
        }

        /// <summary>
        /// Stops the playing.
        /// </summary>
        public static void StopPlaying() {
            const int maxNumberOfChannels = 16; //// 32 //// It crashes for channel = 30
            if (!wasUsed) {
                return;
            }

            for (byte channel = 0; channel < maxNumberOfChannels; channel++) {
                //// for (int i = 0; i < 128; i++) {
                //// MidiInternalMessages.SendMidiMessage(MidiCommandCode.VoiceNoteOff, channel, i); }
                MidiInternalMessages.SendMidiMessage(MidiCommandCode.ControlChange, channel, (int)MidiController.AllNotesOff);
                //// Commented Out - Sometimes memory error appears.
                //// MidiInternalMessages.SendMidiMessage(MidiCommandCode.ControlChange, channel, (int)MidiController.AllSoundOff);
            }
        }

        #endregion

        #region Play tones
        /// <summary>
        /// Play Tone.
        /// </summary>
        /// <param name="givenMidiNote">The given midi note.</param>
        /// <param name="givenDelay">Midi delay.</param>
        /// Don't use dangerous threading methods (NDepend)
        [UsedImplicitly]
        public void PlayNote(int givenMidiNote, int givenDelay) {
            MidiInternalMessages.SendMidiMessage(MidiCommandCode.VoiceNoteOff, 0, givenMidiNote);
            if (givenDelay <= this.SpacingDuration) {
                return;
            }

            MidiInternalMessages.SendMidiMessage(MidiCommandCode.VoiceNoteOn, 0, givenMidiNote, 127);
            Thread.Sleep(givenDelay - this.SpacingDuration);
            MidiInternalMessages.SendMidiMessage(MidiCommandCode.VoiceNoteOff, 0, givenMidiNote);
            if (this.SpacingDuration > 0) {
                Thread.Sleep(this.SpacingDuration);
            }
        }

        #endregion
    }
}
