// <copyright file="DataInstaller.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Settings;
using System.IO;

namespace LargoManager
{
    /// <summary>
    /// Data Installer.
    /// </summary>
    public static class DataInstaller
    {
        #region Public methods - Install
        /// <summary>
        /// Checks the files.
        /// </summary>
        public static void InstallMissingFiles()
        {
            var pathFrom = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.FactoryData);
            var pathTo = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalData);
            DataInstaller.CopyFiles(pathFrom, pathTo);

            pathFrom = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.FactoryMusic);
            pathTo = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalMusic);
            DataInstaller.CopyFiles(pathFrom, pathTo);

            pathFrom = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.FactorySettings);
            pathTo = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalSettings);
            DataInstaller.CopyFiles(pathFrom, pathTo);

            pathFrom = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.FactoryTemplates);
            pathTo = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalTemplates);
            DataInstaller.CopyFiles(pathFrom, pathTo);
        }

        /// <summary>
        /// Copies the files.
        /// </summary>
        /// <param name="pathFrom">The path from.</param>
        /// <param name="pathTo">The path to.</param>
        public static void CopyFiles(string pathFrom, string pathTo)
        {
            if (!Directory.Exists(pathTo)) {
                Directory.CreateDirectory(pathTo);
            }

            var di = new DirectoryInfo(pathFrom);
            foreach (var file in di.EnumerateFiles()) {
                var fullNameTo = Path.Combine(pathTo, file.Name);
                if (!File.Exists(fullNameTo)) {
                    file.CopyTo(fullNameTo, false);
                }
            }

            foreach (var folder in di.EnumerateDirectories()) {
                var folderTo = Path.Combine(pathTo, folder.Name);
                if (!Directory.Exists(folderTo)) {
                    Directory.CreateDirectory(folderTo);
                }

                foreach (var file in folder.EnumerateFiles()) {
                    var fullNameTo = Path.Combine(folderTo, file.Name);
                    if (!File.Exists(fullNameTo)) {
                        file.CopyTo(fullNameTo, false);
                    }
                }
            }
        }
        #endregion
    }
}
