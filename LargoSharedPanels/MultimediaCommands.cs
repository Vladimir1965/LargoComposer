// <copyright file="MultimediaCommands.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using LargoSharedClasses.Music;
using LargoSharedClasses.Settings;

namespace LargoSharedPanels
{
    /// <summary>
    /// Manage Multimedia Commands.
    /// </summary>
    public class MultimediaCommands {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly MultimediaCommands InternalSingleton = new MultimediaCommands();
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the <see cref="MultimediaCommands"/> class from being created.
        /// </summary>
        private MultimediaCommands() {
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static MultimediaCommands Singleton {
            get {
                Contract.Ensures(Contract.Result<MultimediaCommands>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton ManageMultimediaCommands is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "Manager for Multimedia Commands";
        }
        #endregion

        #region Player (multimedia support]
    
        /// <summary>
        /// Music to note editor.
        /// </summary>
        /// <param name="notatorPath">The note editor path.</param>
        public void MusicToNotator(string notatorPath) {
            //// 2018/10 this.MusicStop(0);
            if (notatorPath == null) {
                return;
            }

            using (var p = new Process { StartInfo = new ProcessStartInfo() }) {
                p.StartInfo.FileName = notatorPath;
                var tempFolder = SettingsFolders.Singleton.GetFolder(MusicalFolder.Temporary);
                var fileName = $@"{DateTime.Now.Ticks}.mid";
                var filepath = Path.Combine(tempFolder, fileName);
                var musicalPlayer = MusicalPlayer.Singleton;
                if (!musicalPlayer.ExportToMidi(filepath)) {
                    return;
                }

                //// 2018/12 there are spaces in the path...
                p.StartInfo.Arguments = '"' + filepath + '"';
                p.Start();
            }
        }

        #endregion
    }
}
