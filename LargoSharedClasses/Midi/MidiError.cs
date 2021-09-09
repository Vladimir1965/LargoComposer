// <copyright file="MidiError.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Midi
{
    /// <summary> Midi Error. </summary>
    public enum MidiError {
        /// <summary> Midi Error. </summary>
        [UsedImplicitly] SystemNoError = 0, //// None, SystemNoError, MidiSystemErrorBase

                /// <summary> Midi System Error. </summary>
        [UsedImplicitly] BadDeviceId = 2,

        /// <summary> Midi System Error. </summary>
        [UsedImplicitly] Allocated = 4,
        
        /// <summary> Midi System Error. </summary>
        [UsedImplicitly] InvalidHandle = 5,

        /// <summary> Midi System Error. </summary>
        [UsedImplicitly] NoMemory = 7,

        /// <summary> Midi System Error. </summary>
        [UsedImplicitly] BadErrorNumber = 9,

        /// <summary> Midi System Error. </summary>
        [UsedImplicitly] InvalidParameter = 11,

        /// <summary> Midi Error. Header not prepared. </summary>        
        [UsedImplicitly] MidiErrorUnprepared = 64, //// MidiErrorBaseNumber

        /// <summary> Midi Error. Still something playing. </summary>
        [UsedImplicitly] StillPlaying = 65,

        /// <summary> Midi Error. No Map. </summary>
        [UsedImplicitly] NoMap = 66,

        /// <summary> Midi Error. Not Ready. </summary>
        [UsedImplicitly] NotReady = 67,

        /// <summary> Midi Error. No Device. </summary>
        [UsedImplicitly] NoDevice = 68,

        /// <summary> Midi Error. Invalid Setup. </summary>
        [UsedImplicitly] InvalidSetup = 69,

        /// <summary> Midi Error. Bad Open Mode. </summary>
        [UsedImplicitly] BadOpenMode = 70,

        /// <summary> Midi Error. Do Not Continue. </summary>
        [UsedImplicitly] DoNotContinue = 71 //// MidiErrorLastError
    }
}
