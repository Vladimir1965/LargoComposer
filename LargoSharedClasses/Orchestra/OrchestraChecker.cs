// <copyright file="OrchestraChecker.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LargoSharedClasses.Orchestra
{
    /// <summary>
    /// Instrument Master.
    /// </summary>
    public sealed class OrchestraChecker {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly OrchestraChecker InternalSingleton = new OrchestraChecker();
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the <see cref="OrchestraChecker"/> class from being created.
        /// </summary>
        private OrchestraChecker()
        {
        }
        #endregion

        #region Static properties

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>
        /// Returns value.
        /// </value>
        public static OrchestraChecker Singleton {
            get {
                Contract.Ensures(Contract.Result<OrchestraChecker>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Orchestra Checker is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the melodic instruments.
        /// </summary>
        /// <value>
        /// The melodic instruments.
        /// </value>
        private List<MelodicInstrument> MelodicInstruments { get; set; }
        #endregion

        #region Public static methods
        #endregion

        #region Public methods
        /// <summary>
        /// Corrects the octave for instrument.
        /// </summary>
        /// <param name="givenTones">The given tones.</param>
        public void CorrectOctavesOfInstrumentedTones(ToneCollection givenTones) {
            if (this.MelodicInstruments == null) {
                return;
            }

            Contract.Requires(givenTones != null);
            const int defMinTone = 48;
            const int defMaxTone = 104;

            foreach (var mt in givenTones) {
                var mtone = mt as MusicalStrike;
                var melt = mt as MusicalTone;
                if (melt == null || melt.IsEmpty) {
                    continue;
                }

                var tmi = this.MelodicInstruments[mtone.InstrumentNumber];
                var tone = melt.Pitch.SystemAltitude;

                int minTone = Math.Max(tmi.MinTone, defMinTone);
                int maxTone = Math.Min(tmi.MaxTone, defMaxTone);

                while (tone < minTone) {
                    tone += DefaultValue.HarmonicOrder;  
                }

                while (tone > maxTone) { 
                    tone -= DefaultValue.HarmonicOrder;  
                } 

                melt.Pitch.SetAltitude(tone);
            }
        }

        /// <summary>
        /// Loads the melodic instruments.
        /// </summary>
        /// <param name="givenInstruments">The given instruments.</param>
        public void SetMelodicInstruments(IList<MelodicInstrument> givenInstruments) {
            this.MelodicInstruments = (List<MelodicInstrument>)givenInstruments;
        }
        #endregion
    }
}
