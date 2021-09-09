// <copyright file="MelodicToneCollectionExtension.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// MusicalToneCollection Extension.
    /// </summary>
    public static class MelodicToneCollectionExtension {
        #region Determine
        /// <summary>
        /// For melodical sequence only.
        /// </summary>
        /// <param name="melodicCollection">Melodic-tone collection.</param>
        /// <param name="harmonicBar">Bar of harmonical motive.</param>
        /// <returns> Returns value. </returns>
        public static MelodicFunction DetermineMelodicType(this MusicalToneCollection melodicCollection, HarmonicBar harmonicBar) {  //// BinaryStructure harStruct
            Contract.Requires(melodicCollection != null);
            //// Contract.Requires(harmonicBar != null); 

            //// if (harmonicBar == null) { return MelodicFunction.None; }

            if (melodicCollection.Count == 0) {
                return MelodicFunction.None;
            }

            if (melodicCollection.IsSimpleStagnation) {
                return MelodicFunction.HarmonicFilling;
            }

            if (harmonicBar == null) {
                return MelodicFunction.MelodicMotion;
            }

            var bandType = melodicCollection.MeanBandType;
            //// HarmonicStructure harStruct = this.DetermineHarmonicStructure(harmonicModality);            
            var meanDuration = melodicCollection.MeanDuration;

            var mpt = AnalyzeMotionType(melodicCollection, harmonicBar, meanDuration);
            if (mpt == MelodicFunction.None) {
                mpt = MelodicFunction.HarmonicFilling;
            }

            if (bandType != MusicalBand.BassTones || meanDuration <= DefaultValue.HalfUnit) {
                return mpt;
            }

            if (mpt == MelodicFunction.HarmonicFilling) {
                mpt = MelodicFunction.HarmonicBass;
            }

            if (mpt == MelodicFunction.MelodicFilling) {
                mpt = MelodicFunction.MelodicBass;
            }

            return mpt;
        }

        /// <summary>
        /// Determine Melodic Structure.
        /// </summary>
        /// <param name="melodicCollection">Melodic Collection.</param>
        /// <param name="lastMelTone">Last Melodic Tone.</param>
        /// <param name="harmonicBar">Bar of harmonic motive.</param>
        /// <returns> Returns value. </returns>
        public static MelodicStructure DetermineMelodicStructure(this MusicalToneCollection melodicCollection, MusicalTone lastMelTone, HarmonicBar harmonicBar) {
            Contract.Requires(melodicCollection != null);
            Contract.Requires(harmonicBar != null);
            Contract.Requires(melodicCollection.Count > 0);

            const float diatonicQuotient = 1.5f; //// rounded approximate diatonic coefficient 12/7 = 1.7 (generally erroneous)
            if (melodicCollection.Count == 0) {
                return null;
            }

            var startTone = melodicCollection.FirstOrDefault();
            if (startTone?.Pitch == null)
            {
                return null;
            }

            var startAltitude = startTone.Pitch.SystemAltitude;
            var minAltitude = (from mt in melodicCollection where mt.Pitch != null select mt.Pitch.SystemAltitude).Min();
            const byte mapOrder = 24;
            var harmonicModality = new HarmonicModality(mapOrder, melodicCollection, minAltitude, false);
            var melodicSystem = new MelodicSystem(harmonicModality.Level, (byte)melodicCollection.Count);
            var mstruct = new MelodicStructure(melodicSystem, null);
            byte pos = 0;
            foreach (var elem in from mt in melodicCollection
                                  where mt != null && mt.IsTrueTone
                                  select (byte)((mt.Pitch.SystemAltitude - minAltitude) % mapOrder)
                                      into altitude
                                      select harmonicModality.LevelOfBit(altitude)) {
                mstruct.SetElement(pos++, elem);
            }

            //// string test = mstruct.ToneSchema;
            if (mstruct.ElementList.Count > 0) {
                mstruct.CompleteFromElements();
            }

            //// mstruct.ToString();

            if (lastMelTone?.Pitch != null) {
                var lastAltitude = lastMelTone.Pitch.SystemAltitude;

                mstruct.Drift = (int)Math.Round((startAltitude - lastAltitude) / diatonicQuotient);
            }
            else {
                mstruct.Drift = 0;
            }

            mstruct.Octave = melodicCollection.MeanOctave;
            mstruct.BandType = melodicCollection.MeanBandType;
            mstruct.MelodicFunction = melodicCollection.DetermineMelodicType(harmonicBar);
            //// if (mstruct.DecimalNumber > 1000000000) { mstruct.DecimalNumber = 0; } 

            return mstruct;
        }

        /// <summary>
        /// Analyzes the type of the motion.
        /// </summary>
        /// <param name="melodicCollection">The melodic collection.</param>
        /// <param name="harmonicBar">The harmonic motive bar.</param>
        /// <param name="meanDuration">Duration of the mean.</param>
        /// <returns> Returns value. </returns>
        private static MelodicFunction AnalyzeMotionType(MusicalToneCollection melodicCollection, HarmonicBar harmonicBar, float meanDuration) {
            Contract.Requires(melodicCollection != null);
            Contract.Requires(harmonicBar != null);

            const float durationLimit = 0.6f;  //// 0.3
            var mpt = melodicCollection.Count > 1 && meanDuration < durationLimit ? melodicCollection.Count > 2 && melodicCollection.ContainsHalftone ? MelodicFunction.MelodicMotion : MelodicFunction.HarmonicMotion : MelodicFunction.None;

            if (mpt == MelodicFunction.MelodicMotion) {
                return mpt;
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery 
            if (harmonicBar.HarmonicStructures.Any(structure => melodicCollection.HasAnyInconsistencyWithHarmony(structure, structure.BitRange)))
            {
                mpt = MelodicFunction.MelodicMotion;
            }

            return mpt;
        }
        #endregion
    }
}
