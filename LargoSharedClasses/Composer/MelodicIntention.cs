// <copyright file="MelodicIntention.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Composer {
    /// <summary>
    /// Melodic Intention.
    /// </summary>
    public class MelodicIntention
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly MelodicIntention InternalSingleton = new MelodicIntention();
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the MelodicIntention class from being created.
        /// </summary>
        private MelodicIntention() {
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>/// 
        public static MelodicIntention Singleton {
            get {
                Contract.Ensures(Contract.Result<MelodicIntention>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Melodic Intention is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the minimum note.
        /// </summary>
        /// <value>
        /// The minimum note.
        /// </value>
        public byte MinNote { get; set; }

        /// <summary>
        /// Gets or sets the maximum note.
        /// </summary>
        /// <value>
        /// The maximum note.
        /// </value>
        public byte MaxNote { get; set; }

        /// <summary>
        /// Gets or sets the altitude.
        /// </summary>
        /// <value>
        /// The altitude.
        /// </value>
        public int Altitude { get; set; }

        /// <summary>
        /// Gets or sets the harmonic modality.
        /// </summary>
        /// <value>
        /// The harmonic modality.
        /// </value>
        public HarmonicModality HarmonicModality { get; set; }
        #endregion

        /// <summary>
        /// Gets the musical pitch.
        /// </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public MusicalPitch GetMusicalPitch() {
            this.Altitude += MathSupport.RandomNatural(7) - 4;
            var request = new MusicalPitch(this.HarmonicModality.HarmonicSystem, this.Altitude);
            request.MoveFromEdges(this.MinNote, this.MaxNote);
            var toneIndex = this.HarmonicModality.LevelContainingBit(request.Element);
            byte toneElement = this.HarmonicModality.PlaceAtLevel(toneIndex);
            var pitch = new MusicalPitch(this.HarmonicModality.HarmonicSystem, request.Octave, toneElement);
            return pitch;
        }

        /// <summary>
        /// Gets the musical pitch.
        /// </summary>
        /// <param name="givenOctave">The given octave.</param>
        /// <param name="levelAltitude">The level altitude.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public MusicalPitch GetMusicalPitch(MusicalOctave givenOctave, int levelAltitude) {
            byte toneIndex = (byte)(levelAltitude % this.HarmonicModality.Level);
            int octavePos = levelAltitude / this.HarmonicModality.Level;
            byte toneElement = this.HarmonicModality.PlaceAtLevel(toneIndex);
            var pitch = new MusicalPitch(this.HarmonicModality.HarmonicSystem, (short)(givenOctave + octavePos), toneElement);
            return pitch;
        }

        /// <summary>
        /// Changes the altitude.
        /// </summary>
        /// <param name="givenProgressValue">The given progress value.</param>
        [UsedImplicitly]
        public void ChangeAltitude(double givenProgressValue) {
            //// var signum = givenProgressValue / 10;
            //// var signum = givenProgressValue < -5 ? -1 : givenProgressValue > 5 ? +1 : 0;
            var extent = this.MaxNote - this.MinNote;
            Singleton.MaxNote = 96;

            this.Altitude = (int)(this.MinNote + (givenProgressValue / 100.0 * extent));

            if (this.Altitude > 127) {
                this.Altitude = 127;
            }

            if (this.Altitude < 0) {
                this.Altitude = 0;
            }

            //// ArtLog.Singleton.Log(string.Format("Progress value {0} Altitude {1}", (int)givenProgressValue, this.Altitude));
        }
    }
}
