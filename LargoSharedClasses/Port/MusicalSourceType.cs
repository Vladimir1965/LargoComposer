// <copyright file="MusicalSourceType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Port
{
    /// <summary>
    /// Musical Source Type.
    /// </summary>
    public enum MusicalSourceType {
        /// <summary>
        /// Musical Source Type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Musical Source Type.
        /// </summary>
        MIDI = 1,

        /// <summary>
        /// Musical Source Type.
        /// </summary>
        MIFI = 2, //// MusicInterchange

        /// <summary>
        /// Musical Source Type.
        /// </summary>
        MusicXML = 3,

        /// <summary>
        /// Musical Source Type.
        /// </summary>
        MusicMXL = 4,
    }
}
