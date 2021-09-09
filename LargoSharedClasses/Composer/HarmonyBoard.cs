// <copyright file="HarmonyBoard.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Composer {
    using Music;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Harmony Board.
    /// </summary>
    public class HarmonyBoard
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly HarmonyBoard InternalSingleton = new HarmonyBoard();
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the HarmonyBoard class from being created.
        /// </summary>
        private HarmonyBoard() {
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>/// 
        public static HarmonyBoard Singleton {
            get {
                Contract.Ensures(Contract.Result<HarmonyBoard>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Harmony Board is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the harmonic modality.
        /// </summary>
        /// <value>
        /// The harmonic modality.
        /// </value>
        public HarmonicModality HarmonicModality { get; set; }

        /// <summary>
        /// Gets or sets the harmonic variety.
        /// </summary>
        /// <value>
        /// The harmonic variety.
        /// </value>
        [UsedImplicitly]
        public HarmonicStructuralVariety HarmonicVariety { get; set; }

        /// <summary>
        /// Gets or sets the harmonic structures.
        /// </summary>
        /// <value>
        /// The harmonic structures.
        /// </value>
        public List<HarmonicStructure> HarmonicStructures { get; set; }

        /// <summary>
        /// Gets or sets the selected structures.
        /// </summary>
        /// <value>
        /// The selected structures.
        /// </value>
        public List<HarmonicStructure> SelectedStructures { get; set; }

        /// <summary>
        /// Gets or sets the harmonic structure.
        /// </summary>
        /// <value>
        /// The harmonic structure.
        /// </value>
        public HarmonicStructure HarmonicStructure { get; set; }

        /// <summary>
        /// Gets the next harmonic structure.
        /// </summary>
        /// <param name="givenValue">The given value.</param>
        /// <param name="givenTone">The given tone.</param>
        /// <returns> Returns value. </returns>
        public HarmonicStructure GetNextHarmonicStructure(int givenValue, MusicalTone givenTone) {
            var request = this.GetRequest(100, givenValue, 50, 50);
            //// byte harIndex = (byte)(Math.Abs(this.CurrentHarmony) % this.HarmonicStructures.Count);
            //// var harStruct = this.HarmonicStructures[harIndex];

            this.HarmonicStructures = givenTone != null ? (from h in this.SelectedStructures
                                                           where h.BitPlaces.Contains(givenTone.Pitch.Element)
                                                           select h).ToList() : this.SelectedStructures;

            var harStruct = this.OptimalNextStructForRequest(request);
            if (harStruct != null) {
                foreach (var hs in this.SelectedStructures) {
                    hs.SetPreviousStruct(harStruct);
                }
            }

            return harStruct;
        }

        /// <summary>
        /// Gets the next harmonic structure.
        /// </summary>
        /// <param name="givenConsonance">The given consonance.</param>
        /// <param name="givenPotential">The given potential.</param>
        /// <param name="givenContinuity">The given continuity.</param>
        /// <param name="givenImpulse">The given impulse.</param>
        /// <returns>
        /// Returns harmonic structure.
        /// </returns>
        public HarmonicStructure GetNextHarmonicStructure(float? givenConsonance, float? givenPotential, float? givenContinuity, float? givenImpulse) {
            var request = this.GetRequest(givenConsonance, givenPotential, givenContinuity, givenImpulse);

            var harStruct = this.OptimalNextStructForRequest(request);
            if (harStruct != null) {
                foreach (var hs in this.SelectedStructures) {
                    hs.SetPreviousStruct(harStruct);
                }
            }

            return harStruct;
        }

        /// <summary>
        /// Optimal the next structure for request.
        /// </summary>
        /// <param name="givenRequest">The given request.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public HarmonicStructure OptimalNextStructForRequest(GeneralRequest givenRequest) {
            HarmonicStructure optimalStruct = null;
            var extremeTotal = -10000000f;
            foreach (var str in this.SelectedStructures) {
                str.WriteBehaviorToProperties();
                var total = str.SumForRequest(givenRequest);
                if (total <= extremeTotal) {
                    continue;
                }

                extremeTotal = total;
                optimalStruct = str;
            }

            if ((optimalStruct == null) && (this.SelectedStructures.Count > 0)) {
                optimalStruct = this.SelectedStructures[0];
            }

            return optimalStruct;
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <param name="givenConsonance">The given consonance.</param>
        /// <param name="givenPotential">The given potential.</param>
        /// <param name="givenContinuity">The given continuity.</param>
        /// <param name="givenImpulse">The given impulse.</param>
        /// <returns> Returns value. </returns>
        public GeneralRequest GetRequest(float? givenConsonance, float? givenPotential, float? givenContinuity, float? givenImpulse) {
            var request = new GeneralRequest();
            GeneralRequestItem item;

            if (givenConsonance != null) {
                item = new GeneralRequestItem(GenProperty.Consonance, 1.0f, givenConsonance);
                request.Items.Add(GenProperty.Consonance, item);
            }

            if (givenPotential != null) {
                item = new GeneralRequestItem(GenProperty.Potential, 1.0f, givenPotential);
                request.Items.Add(GenProperty.Potential, item);
            }

            if (givenContinuity != null) {
                item = new GeneralRequestItem(GenProperty.RelatedContinuity, 1.0f, givenContinuity);
                request.Items.Add(GenProperty.RelatedContinuity, item);
            }

            if (givenImpulse != null) {
                item = new GeneralRequestItem(GenProperty.RelatedImpulse, 1.0f, givenImpulse);
                request.Items.Add(GenProperty.RelatedImpulse, item);
            }

            return request;
        }
    }
}
