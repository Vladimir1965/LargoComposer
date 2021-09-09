// <copyright file="OrchestraStrip.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;

namespace LargoSharedClasses.Orchestra
{
    /// <summary>
    /// Orchestra Strip.
    /// </summary>
    public class OrchestraStrip {
        #region Fields
        /// <summary> Unique Identifier. </summary>
        private string uniqueIdentifier;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the OrchestraStrip class.
        /// </summary>
        public OrchestraStrip() {
            this.OrchestraVoices = new List<OrchestraVoice>();
        }

        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the musical voice.
        /// </summary>
        /// <value>
        /// The musical voice.
        /// </value>
        public IList<OrchestraVoice> OrchestraVoices { get; set; }

        /// <summary>
        /// Gets the melodic orchestra tracks.
        /// </summary>
        /// <value>
        /// The melodic orchestra tracks.
        /// </value>
        public IList<OrchestraVoice> MelodicOrchestraVoices =>
            (from t in this.OrchestraVoices
                where t.Instrument.Genus == InstrumentGenus.Melodical
                select t).ToList();

        /// <summary>
        /// Gets the rhythmic orchestra tracks.
        /// </summary>
        /// <value>
        /// The rhythmic orchestra tracks.
        /// </value>
        public IList<OrchestraVoice> RhythmicOrchestraVoices =>
            (from t in this.OrchestraVoices
                where t.Instrument.Genus == InstrumentGenus.Rhythmical
                select t).ToList();

        /// <summary> Gets Unique Identifier. </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string UniqueIdentifier {
            get {
                if (this.uniqueIdentifier != null) {
                    return this.uniqueIdentifier;
                }

                var ident = new StringBuilder();
                var tracks = (from mv in this.OrchestraVoices
                              orderby mv.Octave, mv.Instrument
                              select mv).Distinct();

                foreach (var track in tracks) {
                    ident.Append(track.UniqueIdentifier);
                }

                this.uniqueIdentifier = ident.ToString();

                return this.uniqueIdentifier;
            }
        }

        /// <summary>
        /// Gets or sets the instrument count.
        /// </summary>
        /// <value>
        /// The instrument count.
        /// </value>
        public int InstrumentCount { get; set; }

        /// <summary>
        /// Gets or sets the section count.
        /// </summary>
        /// <value>
        /// The section count.
        /// </value>
        public int SectionCount { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// THe optimal orchestra track.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenVoice">The given voice.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public OrchestraVoice OptimalOrchestraVoice(MusicalLine givenLine, IAbstractVoice givenVoice) {
            Contract.Requires(givenLine != null);
            if (givenLine.FirstStatus.IsMelodic) {
                return this.OptimalMelodicOrchestraVoice(givenLine, givenVoice);
            }

            if (givenLine.FirstStatus.IsRhythmic) {
                return this.OptimalRhythmicOrchestraVoice(givenLine, givenVoice);
            }

            return null;
        }

        /// <summary>
        /// Optimal the melodic orchestra line.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenVoice">The given voice.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public OrchestraVoice OptimalMelodicOrchestraVoice(MusicalLine givenLine, IAbstractVoice givenVoice) {
            Contract.Requires(givenLine != null);
            const byte fullness = 20; //// (30)
            //// const byte momentum = 20; //// (10)
            const byte similarity = 10; //// (30)
            var bestValue = 0;
            OrchestraVoice bestOrchestraVoice = null;
            //// int randomPartNumber = MathSupport.RandomNatural(voices.Count());
            
            //// var voiceOctave = givenLine.Tones.MeanOctave; //// mtrack.MusicalOctave
            var voiceOctave = givenVoice.Octave;
            foreach (var v in this.MelodicOrchestraVoices) {
                var value = 100 - (10 * Math.Abs((byte)v.Octave - (byte)voiceOctave));
                if (!v.IsUsed) {
                    value += fullness;
                }

                //// The same instruments prefered
                if (v.Instrument != null && givenVoice.Instrument != null && v.Instrument.Number == givenVoice.Instrument.Number) {
                    value += similarity;
                }
               
                //// if (givenLine.FirstStatus.CurrentOrchestraVoice != null && givenLine.FirstStatus.CurrentOrchestraVoice.InstrumentNumber == v.InstrumentNumber) {
                //// value += momentum; } 

                //// value += v.PartNumber % 3; //// MathSupport.RandomNatural(3);
                if (value <= bestValue) {
                    continue;
                }

                bestValue = value;
                bestOrchestraVoice = v;
            }

            return bestOrchestraVoice;
        }

        /// <summary>
        /// Optimal the rhythmic orchestra line.
        /// </summary>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenVoice">The given voice.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public OrchestraVoice OptimalRhythmicOrchestraVoice(MusicalLine givenLine, IAbstractVoice givenVoice) {
            Contract.Requires(givenLine != null);
            const byte fullness = 20; //// (30)
            const byte similarity = 10; //// (30)
            //// const byte momentum = 20; //// (10)
            var bestValue = 0;
            OrchestraVoice bestOrchestraVoice = null;
            foreach (var v in this.RhythmicOrchestraVoices) {
                var value = 100;
                if (!v.IsUsed) {
                    value += fullness;
                }

                //// The same instruments prefered
                if (v.Instrument != null && givenVoice.Instrument != null && v.Instrument.Number == givenVoice.Instrument.Number) {
                    value += similarity;
                }
                
                //// if (givenLine.FirstStatus.CurrentOrchestraVoice != null && givenLine.FirstStatus.CurrentOrchestraVoice.InstrumentNumber == v.InstrumentNumber) {
                //// value += momentum; } 

                //// value += v.PartNumber % 3; //// MathSupport.RandomNatural(3);
                if (value <= bestValue) {
                    continue;
                }

                bestValue = value;
                bestOrchestraVoice = v;
            }

            return bestOrchestraVoice;
        }

        /// <summary>
        /// Recompute the properties.
        /// </summary>
        public void RecomputeProperties()
        {
            foreach (var track in this.OrchestraVoices)
            {
                if (track.Instrument == null)
                {
                    continue;
                }

                if (track.Instrument.Genus == InstrumentGenus.Melodical)
                {
                    track.Instrument.Section = (byte)PortInstruments.GetGroupOfMelodicInstrument(track.Instrument.Number);
                }

                if (track.Instrument.Genus == InstrumentGenus.Rhythmical)
                {
                    track.Instrument.Section = (byte)PortInstruments.GetGroupOfRhythmicInstrument(track.Instrument.Number);
                }
            }

            var number = (from track in this.OrchestraVoices select track.InstrumentNumber).Distinct().Count();
            this.InstrumentCount = number;

            number = (from track in this.OrchestraVoices where track.Instrument != null select track.Instrument.Section).Distinct().Count();
            this.SectionCount = number;
        }

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            var tracks = (from mv in this.OrchestraVoices
                          orderby mv.Octave, mv.Instrument
                          select mv).Distinct();

            foreach (var track in tracks) {
                s.Append("Octave:" + track.Octave);  //// musicalVoice.MelodicInstrument
                s.Append("Instrument" + track.Instrument);
            }

            return s.ToString();
        }
        #endregion
    }
}
