// <copyright file="FileAssociations.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// File Association
    /// Thanks, it is perfect! It works in Windows 10,this saved me a lot of time.
    /// works perfectly on windows 10. Great work! Really nice and works in Windows 10.
    /// </summary>
    [UsedImplicitly]
    public class FileAssociations
    {
        /// <summary>
        /// The association changed.
        /// </summary>
        private const int AssociationChanged = 0x8000000;

        /// <summary>
        /// The association flush.
        /// </summary>
        private const int AssociationFlush = 0x1000;

        /// <summary>
        /// Ensures the associations set.
        /// </summary>
        [UsedImplicitly]
        public static void EnsureAssociationsSet()
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
            {
                var filePath = processModule.FileName;
                EnsureAssociationsSet(
                    new FileAssociation
                    {
                        Extension = ".ucs",
                        ProgId = "UCS_Editor_File",
                        FileTypeDescription = "UCS File",
                        ExecutableFilePath = filePath
                    });
            }
        }

        /// <summary>
        /// Ensures the associations set.
        /// </summary>
        /// <param name="associations">The associations.</param>
        public static void EnsureAssociationsSet(params FileAssociation[] associations)
        {
            bool madeChanges = false;
            foreach (var association in associations)
            {
                madeChanges |= SetAssociation(
                    association.Extension,
                    association.ProgId,
                    association.FileTypeDescription,
                    association.ExecutableFilePath);
            }

            if (madeChanges)
            {
                SHChangeNotify(AssociationChanged, AssociationFlush, IntPtr.Zero, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Sets the association.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="programId">The program identifier.</param>
        /// <param name="fileTypeDescription">The file type description.</param>
        /// <param name="applicationFilePath">The application file path.</param>
        /// <returns> Returns value. </returns>
        public static bool SetAssociation(string extension, string programId, string fileTypeDescription, string applicationFilePath)
        {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, programId);
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + programId, fileTypeDescription);
            madeChanges |= SetKeyDefaultValue(
                                $@"Software\Classes\{programId}\shell\open\command",
                                "\"" + applicationFilePath + "\" \"%1\"");
            return madeChanges;
        }

        /// <summary>
        /// Sets the key default value.
        /// </summary>
        /// <param name="keyPath">The key path.</param>
        /// <param name="value">The value.</param>
        /// <returns> Returns value. </returns>
        private static bool SetKeyDefaultValue(string keyPath, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key != null && key.GetValue(null) as string != value)
                {
                    key.SetValue(null, value);
                    return true;
                }
            }

            return false;
        }

        // needed so that Explorer windows get refreshed after the registry is updated
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);
    }
}
