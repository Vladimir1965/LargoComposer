using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;
using System.Diagnostics.Contracts;
using System.IO;

namespace LargoSharedClasses.Player
{
    public class PlayCentrum
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static PlayCentrum singleton = new PlayCentrum();

        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static PlayCentrum Singleton {
            get {
                Contract.Ensures(Contract.Result<PlayCentrum>() != null);
                return singleton;
            }

            private set => singleton = value;
        }

        public object PlayerSettings { get; private set; }
        #endregion

        public string UserMusicFolder { get; set; }

        public string PathToInternalConverter { get; set; }

        public string SoundFontName { get; set; }

        /// <summary>
        /// Music play MP3.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public void MusicPlayMp3(MusicalBlock givenBlock)
        {
            var midiBlock = new CompactMidiBlock(givenBlock);
            //// midiBlock.CollectMidiEvents();
            var fileName = givenBlock.Header.FullName;
            var sequence = midiBlock.Sequence(fileName + ".mid");
            //// MusicalPlayer.Singleton.PlayImmediately = false;
            MusicalPlayer.Singleton.TakeSequence(sequence, false);
            var path = this.UserMusicFolder;
            var midiFilePath = Path.Combine(path, sequence.InternalName);
            var mp3FilePath = Path.Combine(path, fileName + ".mp3");
            this.ConvertToMp3(midiFilePath, mp3FilePath);
            SystemProcesses.RunProcessSecure(mp3FilePath, string.Empty, false);
        }

        /// <summary>
        /// Plays the wave file (see BuildSoundFile)
        /// </summary>
        /// <param name="midiFilePath">The midi file path.</param>
        /// <param name="mp3FilePath">The MP3 file path.</param>
        public void ConvertToMp3(string midiFilePath, string mp3FilePath)
        {
            var convertPath = this.PathToInternalConverter;
            string midiName = @"music.mid";
            var midiMusicPath = Path.Combine(convertPath, midiName);
            File.Copy(midiFilePath, midiMusicPath, true);
            //// var fto = new FileInfo(fileNameTo); if (fto.Exists) { fto.Delete();  } 

            if (!File.Exists(midiMusicPath)) {
                return;
            }

            Directory.SetCurrentDirectory(convertPath);
            var command = Path.Combine(convertPath, "convert.bat");

            //// var arguments = string.Format(CultureInfo.InvariantCulture, "{0},{1}", midiName, soundFontName), false);
            SystemProcesses.RunProcessSecure(command, string.Empty, true);

            var resultFilePath = Path.Combine(convertPath, "music.mp3");
            File.Copy(resultFilePath, mp3FilePath, true);
        }

    }
}
