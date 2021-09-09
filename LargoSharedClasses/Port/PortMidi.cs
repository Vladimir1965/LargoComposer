// <copyright file="PortMidi.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Midi;
using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace LargoSharedClasses.Port
{
    /// <summary>
    /// Port Midi.
    /// </summary>
    /// <seealso cref="LargoSharedClasses.Port.PortAbstract" />
    public class PortMidi : PortAbstract
    {
        /// <summary>
        /// Write To MidiFile.
        /// </summary>
        /// <param name="musicalBlock">The musical block.</param>
        /// <param name="path">File path.</param>
        public static void WriteBlockToMidi(MusicalBlock musicalBlock, string path) {
            Contract.Requires(musicalBlock != null);
            Contract.Requires(!string.IsNullOrEmpty(path));

            if (musicalBlock.Header.Tempo == 0) {
                //// MessageBox.Show("Unknown tempo!", "Composed file.", MessageBoxButton.OK, MessageBoxImage.Information);
                musicalBlock.Header.Tempo = DefaultValue.DefaultTempo;
            }

            //// 2019/11 Error - solving ...
            bool newCode = false;
            CompactMidiStrip sequence;
            if (newCode) {
                var midiBlock = new CompactMidiBlock(musicalBlock);
                //// midiBlock.CollectMidiEvents();
                sequence = midiBlock.Sequence(musicalBlock.Header.FileName);
            }
            else {
                sequence = new CompactMidiStrip(musicalBlock, false) {
                    Header = musicalBlock.Header
                }; //// staff grouping
                //// 2021/01
            }

            var file = new MidiFile.MidiFile(path, sequence);
            file.Save();
        }

        /// <summary>
        /// Writes the meta harmony.
        /// </summary>
        /// <param name="midiEvents">The midi events.</param>
        /// <param name="bitDuration">Duration of the bit.</param>
        /// <param name="barDeltaTime">The bar delta time.</param>
        /// <param name="musicalBar">The musical bar.</param>
        public static void WriteMetaHarmony(MidiEventCollection midiEvents, int bitDuration, int barDeltaTime, MusicalBar musicalBar) {
            Contract.Requires(midiEvents != null);
            Contract.Requires(musicalBar != null);

            var rlevel = (byte)musicalBar.HarmonicBar.HarmonicStructures.Count;
            var rshape = musicalBar.HarmonicBar.RhythmicShape;
            for (byte rl = 0; rl < rlevel; rl++) {
                var harStrTick = (byte)(rshape != null && rl < rshape.BitPlaces.Count ? rshape.BitPlaces[rl] : 0);

                var chord = musicalBar.HarmonicBar.HarmonicStructureAtRhythmicLevel(rl);
                if (chord == null) {
                    continue;
                }

                //// long hsnumber = GeneralSystem.ConvertStruct(chord.Number, chord.GSystem.Order, DefaultValue.HarmonicOrder); //// 2
                //// var system = HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder);
                //// HarmonicModality modality = musicalBar.HarmonicBar.HarmonicModalityFromStructures(MusicalSetup.Singleton.MinimalModalityLevel);
                //// HarmonicModality modality = chord.HarmonicModality;
                //// var modality = musicalBar.HarmonicBar.HarmonicBar.HarmonicModality;

                ////201508
                //// var harStr = DataLink.BridgeHarmony.CompleteHarmonicStructure(system, modality, chord.GetStructuralCode()); //// hsnumber

                //// HarmonicStructure harStr = null;
                //// if (harStr != null) {
                var shortcut = chord.Shortcut; ////  .ToneSchema;
                var ptext = shortcut + " "; //// string.Format(CultureInfo.CurrentCulture, "{0}({1})", shortcut, hstr.ToneSchema);
                midiEvents.PutMetaText(1 + barDeltaTime + (bitDuration * harStrTick), ptext);
                //// }
            }
        }

        #region Public methods
        /// <summary>
        /// Load From MidiFile.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public override void LoadFromFiles(string givenPath) {
            //// MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.MusicImport)
            var files = FileDialogs.OpenSelectedMidiFiles(givenPath);
            if (files == null) {
                return;
            }

            var logger = WindowManager.OpenWindow("LargoSharedWindows", "InherentLogger", null);
            Array.ForEach(files, filepath => PortAbstract.LoadFromSourceFile(filepath, MusicalSourceType.MIDI, false));
            logger?.Close();

            FileInfo fi = new FileInfo(files[0]);
            MusicalSettings.Singleton.Folders.SetFolder(MusicalFolder.MusicImport, fi.DirectoryName);
        }

        /// <summary>
        /// Load MIDI File.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="internalName">Name of the internal.</param>
        /// <returns> Returns value. </returns>
        public override MusicalBundle ReadMusicFile(string filePath, string internalName) {
            Contract.Requires(filePath != null);

            if (string.IsNullOrEmpty(filePath)) {
                return null;
            }

            ProcessLogger.Singleton.SendMessageEvent(Path.GetFileName(filePath), LocalizedMusic.String("Reading Midi file ... "), 0);

            var midiFile = new MidiFile.MidiFile(filePath);

            var sequence = midiFile.Sequence;
            sequence.InternalName = internalName;
            return MusicalBundle.GetMusicalBundle(sequence, PortAbstract.SettingsImport);
        }

        /// <summary>
        /// Write To MidiFile.
        /// </summary>
        /// <param name="musicalBundle">Musical file.</param>
        /// <param name="path">File path.</param>
        /// <returns> Returns value. </returns>
        public override bool WriteMusicFile(MusicalBundle musicalBundle, string path) {
            Contract.Requires(musicalBundle != null);
            Contract.Requires(!string.IsNullOrEmpty(path));

            foreach (var musicalBlock in musicalBundle.Blocks) {
                WriteBlockToMidi(musicalBlock, path); //// bad - rewrites the sam midi name
            }

            return true;
        }

        /// <summary>
        /// Saves a bundle.
        /// </summary>
        /// <param name="musicalBundle">The musical bundle.</param>
        /// <returns> Returns value. </returns>
        public override bool SaveBundle(MusicalBundle musicalBundle) {
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            var defaultFilePath = Path.Combine(path, musicalBundle.FileName + ".mid");
            this.DestinationFilePath = FileDialogs.SelectMidiFileToSave(defaultFilePath);

            //// 2018/09, 2018/10
            if (string.IsNullOrEmpty(this.DestinationFilePath)) {
                return false;
            }

            return this.WriteMusicFile(musicalBundle, this.DestinationFilePath);
        }
        #endregion
    }
}
