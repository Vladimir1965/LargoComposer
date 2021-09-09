// <copyright file="MusicalFolder.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// Musical Folder.
    /// </summary>
    public enum MusicalFolder {
        /// <summary>
        /// Musical folder.
        /// </summary>
        None = 0,

        /// <summary>
        /// The program folder.
        /// </summary>
        BinaryFolder,
        
        /// <summary>
        /// Musical folder.
        /// </summary>
        Licenses,

        /// <summary>
        /// Factory data files.
        /// </summary>
        FactoryData,

        /// <summary>
        /// Musical folder.
        /// </summary>
        FactorySettings,

        /// <summary>
        /// Musical folder.
        /// </summary>
        FactoryMusic,

        /// <summary>
        /// Musical folder.
        /// </summary>
        FactoryTemplates,
        
        //// ----------------------------------------------------------
        
        /// <summary>
        /// Musical folder.
        /// </summary>
        InternalData,

        /// <summary>
        /// Musical folder
        /// </summary>
        InternalTemplates,

        /// <summary>
        /// Largo Conductor - Internal Stream.
        /// </summary>
        InternalStream,

        //// ----------------------------------------------------------
        
        /// <summary>
        /// Musical folder.
        /// </summary>
        InternalMusic,

        /// <summary>
        /// Musical folder.
        /// </summary>
        InternalSettings,
        //// ----------------------------------------------------------

        /// <summary>
        /// Musical folder.
        /// </summary>
        MusicImport,

        /// <summary>
        /// Musical folder.
        /// </summary>
        UserMusic,

        /// <summary>
        /// Musical folder.
        /// </summary>
        Temporary,

        /// <summary>
        /// Musical folder.
        /// </summary>
        Errors,

        /// <summary>
        /// Musical folder.
        /// </summary>
        EndOfFolders
    }
}
