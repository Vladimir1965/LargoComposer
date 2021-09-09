// <copyright file="InstrumentGroup.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary> Type of band. </summary>
    public enum InstrumentGroup {
        /// <summary> Type of voice. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Keyboards = 1,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Strings = 2,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Woodwind = 3,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Brass = 4,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Guitars = 5,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Vocal = 6,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] Synthetic = 7,

        /// <summary> Type of voice. </summary>
        [UsedImplicitly] MelodicDrums = 8,
                                
        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Drums = 9,      //// section 0  

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Cymbals = 10,   //// section 1

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Snare = 11,     //// section 2

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Toms = 12,      //// section 3

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Hihat = 13,     //// section 4

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Triangle = 14,  //// section 5

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Woodblock = 15, //// section 6

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Conga = 16,    //// section 7

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Bonga = 17,    //// section 8

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Guiro = 18,    //// section 9

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Cuica = 19,    //// section 10

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Timbale = 20,  //// section 11

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Agogo = 21,    //// section 12

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Taiko = 22,    //// section 13

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Metronome = 23, //// section 14

        /// <summary> Rhythmic section. </summary>
        [UsedImplicitly] Others = 24    //// section 15
    }
}
