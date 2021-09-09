// <copyright file="FormatConversionOptions.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;

namespace LargoSharedClasses.Midi {
    /// <summary>Options used when performing a format conversion.</summary>
    [Flags]
    public enum FormatConversionOptions {
        /// <summary>No special formatting.</summary>
        None,

        /// <summary>
        /// Uses the number of the track as the channel for all events on that track.
        /// Only valid if the number of the track is a valid track number.
        /// </summary>
        CopyTrackToChannel
    }
}
