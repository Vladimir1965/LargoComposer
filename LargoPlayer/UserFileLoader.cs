// <copyright file="UserFileLoader.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.MidiFile;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using LargoSharedWindows;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;

namespace LargoPlayer
{
    /// <summary>User File Loader</summary>
    public class UserFileLoader
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static UserFileLoader singleton = new UserFileLoader();

        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static UserFileLoader Singleton {
            get {
                Contract.Ensures(Contract.Result<UserFileLoader>() != null);
                return singleton;
            }

            private set => singleton = value;
        }
        #endregion

        #region Public methods
        /// <summary> Adds resource dictionary. </summary>
        /// <param name="directories">  The directories. </param>
        /// <param name="resourcePath"> Full pathname of the resource file. </param>
        public void AddResourceDictionary(Collection<ResourceDictionary> directories, string resourcePath) {
            directories.Add(new ResourceDictionary() { Source = new Uri(resourcePath, UriKind.Absolute) });
            //// Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(src, UriKind.Relative) });
        }

        /// <summary> Loads a theme. </summary>
        /// <param name="directories">  The directories. </param>
        public void LoadTheme(Collection<ResourceDictionary> directories) {
            //// string src = "pack://application:,,,/LargoSharedWindows;component/BlueTheme/Office2010Blue.MSControls.Core.Implicit.xaml";
            //// this.AddResourceDictionary(dirs, src);
            string resource = "pack://application:,,,/LargoSharedControls;component/SharedTheme.xaml";
            this.AddResourceDictionary(directories, resource);
        }

        /// <summary>
        /// Loads the window manager.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="mainClassName">Name of the main class.</param>
        /// <param name="mainObjType">Type of the main object.</param>
        public void LoadWindowManager(string moduleName, string mainClassName, Type mainObjType)
        {
            string folder = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalSettings);
            //// folder = Path.Combine(folder, moduleName);
            string filepath = Path.Combine(folder, moduleName + @"Windows.xml");
            var winManagerStatus = WindowManager.LoadWindowManager(filepath);
            if (winManagerStatus == null) {
                MessageBox.Show(string.Format("Window Manager failed to load file: {0}", filepath), SettingsApplication.ApplicationName);
                return;
            }

            WindowManager.Singleton.ManagerName = moduleName;
            WindowManager.Singleton.Status = winManagerStatus;

            //// 2019/05
            var objType = typeof(InherentException);
            if (objType.AssemblyQualifiedName != null) {
                var assemblyQualifiedName = objType.AssemblyQualifiedName.Replace("InherentException", "#WCLASSNAME");
                WindowManager.LargoSharedAssemblyName = assemblyQualifiedName;
            } 

            if (mainObjType.AssemblyQualifiedName != null) {
                var assemblyQualifiedName = mainObjType.AssemblyQualifiedName.Replace(mainClassName, "#WCLASSNAME");
                WindowManager.QualifiedAssemblyName = assemblyQualifiedName;
            }
        }

        /// <summary>
        /// Music play MP3.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public void MusicPlayMp3(MusicalBlock givenBlock) {
            var midiBlock = new CompactMidiBlock(givenBlock);
            //// midiBlock.CollectMidiEvents();
            var fileName = givenBlock.Header.FullName;
            var sequence = midiBlock.Sequence(fileName + ".mid");
            //// MusicalPlayer.Singleton.PlayImmediately = false;
            MusicalPlayer.Singleton.TakeSequence(sequence, false);
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            var midiFilePath = Path.Combine(path, sequence.InternalName);
            var mp3FilePath = Path.Combine(path, fileName + ".mp3");
            this.ConvertToMp3(midiFilePath, mp3FilePath, string.Empty);
            SystemProcesses.RunProcessSecure(mp3FilePath, string.Empty, false);
        }

        #endregion

        /// <summary>
        /// Plays the wave file (see BuildSoundFile)
        /// </summary>
        /// <param name="midiFilePath">The midi file path.</param>
        /// <param name="mp3FilePath">The MP3 file path.</param>
        /// <param name="soundFontName">Name of the sound font.</param>
        private void ConvertToMp3(string midiFilePath, string mp3FilePath, string soundFontName) {
            var convertPath = PlayerSettings.Singleton.PathToInternalConverter;  //// MusicalFolder.InternalConverter
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