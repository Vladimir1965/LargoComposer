// <copyright file="MusicalPlayer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Musical Director.
    /// </summary>
    public sealed class MusicalPlayer {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly MusicalPlayer InternalSingleton = new MusicalPlayer();

        /// <summary>
        /// Local MidiFileName.
        /// </summary>
        private string localMidiFilePath;

        /// <summary>
        /// Local Alias.
        /// </summary>
        private string localAlias;
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the MusicalPlayer class from being created.
        /// </summary>
        private MusicalPlayer() {
            this.WriteMidiToDisk = true;
            //// this.PlayImmediately = true;
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static MusicalPlayer Singleton {
            get {
                Contract.Ensures(Contract.Result<MusicalPlayer>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton MusicalPlayer is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets MidiTrack Collection.
        /// </summary>
        public CompactMidiStrip Sequence { get; set; }

        #endregion

        #region Private Properties
        /// <summary>
        /// Gets or sets a value indicating whether [write midi to disk].
        /// </summary>
        /// <value>
        ///   <c>True</c> if [write midi to disk]; otherwise, <c>false</c>.
        /// </value>
        public bool WriteMidiToDisk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is Playing.
        /// </summary>
        /// <value> Property description. </value>
        private bool IsPlaying { get; set; }

        #endregion

        #region Public Static
        /// <summary>
        /// Plays the specified given block.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        /// <param name="playOnline">if set to <c>true</c> [play online].</param>
        public static void Play(MusicalBlock givenBlock, bool playOnline) {
            //// givenBlock.LoadFirstStatusToTracks(); //// 2019/10
            CompactMidiBlock midiBlock = new CompactMidiBlock(givenBlock);

            if (playOnline)
            {
                midiBlock.CollectMidiEvents();
                MidiFilePlayer.PrepareMidi();
                MidiFilePlayer.OpenMidi();
                TimedPlayer.Singleton.PlayBlock(midiBlock);
            }
            else
            {
                var sequence = midiBlock.Sequence(string.Empty);
                Singleton.TakeSequence(sequence, true);
            }
        }
        #endregion

        #region Public methods - Set music

        /// <summary>
        /// Sets the sequence.
        /// </summary>
        /// <param name="givenSequence">The given sequence.</param>
        /// <param name="play">if set to <c>true</c> [play].</param>
        public void TakeSequence(CompactMidiStrip givenSequence, bool play) {
            if (givenSequence == null || givenSequence.Count == 0) {
                return;
            }

            this.Sequence = givenSequence;
            //// this.WriteTime = DateTime.Now;
            //// this.InnerName = CommonSupport.DateTimeIdentifier;
            //// if (this.PlayImmediately) {
            if (play) {
                this.StopPlaying();
                this.Play();
            }
            //// }

            //// Export to user folder - not needed for playing!
            if (this.WriteMidiToDisk) {
                var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
                if (path == null) {
                    return;
                }

                path = path.Trim();
                var filePath = Path.Combine(path, givenSequence.InternalName ?? givenSequence.Header.Specification); //// + ".mid"
                this.ExportToMidi(filePath);
            }
        }
        #endregion

        #region Public methods - Stop Playing
        /// <summary>
        /// Stop Playing.
        /// </summary>
        public void StopPlaying() {
            if (!this.IsPlaying) {
                return;
            }

            MidiFilePlayer.MidiFileClose();
            this.IsPlaying = false;
            //// Remove the local file
            //// DoNotCatchGeneralExceptionTypes
            if (string.IsNullOrEmpty(this.localMidiFilePath)) {
                return;
            }

            File.Delete(this.localMidiFilePath);
        }
        #endregion

        #region Public methods - Export To Files
        /// <summary>
        /// Export To Midi.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public bool ExportToMidi(string filePath) {
            if (filePath == null) {
                return false;
            }

            if (string.Compare(Path.GetExtension(filePath).ToLower(CultureInfo.CurrentCulture), ".mid", StringComparison.OrdinalIgnoreCase) != 0) {
                filePath += ".mid";
            }

            var file = new MidiFile.MidiFile(filePath, this.Sequence);
            if (!file.HasValidSequence) {
                return false;
            }

            file.Save();

            return true;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Play music.
        /// </summary>
        private void Play() {
            string tempFolder = SettingsFolders.Singleton.GetFolder(MusicalFolder.Temporary);
            string fileName = $@"{DateTime.Now.Ticks}.mid";
            this.localMidiFilePath = Path.Combine(tempFolder, fileName);
            this.ExportToMidi(this.localMidiFilePath);
            this.localAlias = Guid.NewGuid().ToString("N", CultureInfo.CurrentCulture); // randomly generated alias to avoid collisions
            //// Play it from the local file
            MidiFilePlayer.MidiFileOpenAndPlay(this.localMidiFilePath, this.localAlias);
            this.IsPlaying = true;
        }

        #endregion
    }
}