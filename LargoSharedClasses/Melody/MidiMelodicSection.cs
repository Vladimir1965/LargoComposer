// <copyright file="MidiMelodicSection.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Melody
{
    /// <summary> Melodic sections. </summary>
    [Serializable]
    public enum MidiMelodicSection {
        /// <summary> Melodic section. 
        /// </summary>
        [UsedImplicitly] Piano = 0,

        /// <summary> Melodic section. 
        /// </summary>
        [UsedImplicitly] ChromaticPercussion = 1,

        /// <summary> Melodic section. 
        /// </summary>
        [UsedImplicitly] Organ = 2,

        /// <summary> Melodic section. 
        /// </summary>
        [UsedImplicitly] Guitar = 3,

        /// <summary> Melodic section. 
        /// </summary>
        [UsedImplicitly] Bass = 4,

        /// <summary> Melodic section. 
        /// </summary>
        [UsedImplicitly] Strings = 5,

        /// <summary> Melodic section. 
        /// </summary>
        [UsedImplicitly] Ensemble = 6,

        /// <summary> Melodic section. 
        /// </summary>
        [UsedImplicitly] Brass = 7,

        /// <summary> Melodic section. 
        /// </summary>
        [UsedImplicitly] Reed = 8,

        /// <summary> Melodic section. </summary>
        [UsedImplicitly] Pipe = 9,

        /// <summary> Melodic section. </summary>
        [UsedImplicitly] SynthLead = 10,

        /// <summary> Melodic section. </summary>
        [UsedImplicitly] SynthPad = 11,

        /// <summary> Melodic section. </summary>
        [UsedImplicitly] SynthFx = 12,

        /// <summary> Melodic section. </summary>
        [UsedImplicitly] Ethnic = 13,

        /// <summary> Melodic section. </summary>
        [UsedImplicitly] Percussive = 14,

        /// <summary> Melodic section. </summary>
        [UsedImplicitly] SoundFx = 15,

        /// <summary> Melodic section. </summary>
        [UsedImplicitly] None = 255
    }
}
