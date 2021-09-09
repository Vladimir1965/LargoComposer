// <copyright file="MusicalTempo.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical tempo.
    /// </summary>
    public enum MusicalTempo {
        /// <summary> No musical tempo. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Very slow tempo - Gravissimo. </summary>
        [UsedImplicitly] Tempo10 = 10,

        /// <summary> Very slow tempo - Larghissimo. </summary>
        [UsedImplicitly] Tempo20 = 20,

        /// <summary> Very slow tempo - Lentisimo. </summary>
        [UsedImplicitly] Tempo30 = 30,

        /// <summary> Very slow tempo - Grave. </summary>
        [UsedImplicitly] Tempo40 = 40,

        /// <summary> Very slow tempo - Largo. </summary>
        [UsedImplicitly] Tempo45 = 45,

        /// <summary> Very slow tempo - Lento. </summary>
        [UsedImplicitly] Tempo50 = 50,

        /// <summary> Very slow tempo - Adagio. </summary>
        [UsedImplicitly] Tempo55 = 55,

        /// <summary> Very slow tempo - Larghetto. </summary>
        [UsedImplicitly] Tempo60 = 60,

        /// <summary> Slow tempo - Andante. </summary>
        [UsedImplicitly] Tempo65 = 65,

        /// <summary> Slow tempo - Andantino. </summary>
        [UsedImplicitly] Tempo70 = 70,

        /// <summary> Slow tempo - Sostenuto. </summary>
        [UsedImplicitly] Tempo75 = 75,

        /// <summary> Slow tempo - Commodo. </summary>
        [UsedImplicitly] Tempo80 = 80,

        /// <summary> Slow tempo - Maestoso. </summary>
        [UsedImplicitly] Tempo85 = 85,

        /// <summary> Middle tempo - Moderato. </summary>
        [UsedImplicitly] Tempo90 = 90,

        /// <summary> Middle tempo - Allegretto. </summary>
        [UsedImplicitly] Tempo105 = 105,

        /// <summary> Middle tempo - Animato. </summary>
        [UsedImplicitly] Tempo110 = 110,

        /// <summary> Middle tempo - Allegro moderato. </summary>
        [UsedImplicitly] Tempo120 = 120,

        /// <summary> Middle tempo - Allegro ma non troppo. </summary>
        [UsedImplicitly] Tempo125 = 125,

        /// <summary> Fast tempo - Allegro. </summary>
        [UsedImplicitly] Tempo130 = 130,

        /// <summary> Fast tempo - Allegro assai. </summary>
        [UsedImplicitly] Tempo140 = 140,

        /// <summary> Fast tempo - Allegro vivace. </summary>
        [UsedImplicitly] Tempo150 = 150,

        /// <summary> Fast tempo - Vivace. </summary>
        [UsedImplicitly] Tempo160 = 160,

        /// <summary> Fast tempo - Molto vivace. </summary>
        [UsedImplicitly] Tempo175 = 175,

        /// <summary> Very fast tempo - Vivacissimo. </summary>
        [UsedImplicitly] Tempo190 = 190,

        /// <summary> Very fast tempo - Presto. </summary>
        [UsedImplicitly] Tempo210 = 210,

        /// <summary> Very fast tempo - Molto presto. </summary>
        [UsedImplicitly] Tempo230 = 230,

        /// <summary> Very fast tempo - Presto. </summary>
        [UsedImplicitly] Tempo250 = 250,

        /// <summary> Very fast tempo - Presto. </summary>
        [UsedImplicitly] Tempo270 = 270
    }
}
