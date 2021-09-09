// <copyright file="RhythmicProvider.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Templates
{
    using LargoSharedClasses.Music;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Rhythmic Provider.
    /// </summary>
    public class RhythmicProvider {
        #region Fields
        /// <summary>
        /// The variety
        /// </summary>
        private readonly StructuralVariety<RhythmicStructure> variety;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicProvider" /> class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structures">The structures.</param>
        public RhythmicProvider(RhythmicSystem givenSystem, IList<RhythmicStructure> structures) {
            if (structures == null) {
                throw new InvalidOperationException("List structures is not defined.");
            }

            this.RhythmicSystem = givenSystem;
            this.variety = new StructuralVariety<RhythmicStructure>(this.RhythmicSystem) {
                LimitCount = 1000
            };

            this.variety.SetStructList(structures);
        }
        #endregion

        #region Private properties
        /// <summary>
        /// Gets the rhythmic system.
        /// </summary>
        /// <value>
        /// The rhythmic system.
        /// </value>
        private RhythmicSystem RhythmicSystem { get;  }

        #endregion

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <param name="rhythmicEnergyBar">The rhythmic energy bar.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static GeneralRequest GetRequest(RhythmicEnergyBar rhythmicEnergyBar)
        {
            var request = new GeneralRequest();
            GeneralRequestItem item = new GeneralRequestItem(GenProperty.ToneLevel, 100.0f, (float)rhythmicEnergyBar.ToneLevel);
            request.Items.Add(GenProperty.ToneLevel, item);

            item = new GeneralRequestItem(GenProperty.Level, 10.0f, (float)rhythmicEnergyBar.Level);
            request.Items.Add(GenProperty.Level, item);

            item = new GeneralRequestItem(GenProperty.FormalBalance, 10.0f, rhythmicEnergyBar.FormalBehavior.Balance);
            request.Items.Add(GenProperty.FormalBalance, item);

            item = new GeneralRequestItem(GenProperty.FormalFilling, 1.0f, rhythmicEnergyBar.RhythmicBehavior.Filling);
            request.Items.Add(GenProperty.FormalFilling, item);

            //// item = new GeneralRequestItem(GenProperty.FormalMobility, 1.0f, (float)rhythmicEnergyBar.RhythmicBehavior.Mobility);
            //// request.Items.Add(GenProperty.FormalMobility, item);

            //// item = new GeneralRequestItem(GenProperty.FormalVariance, 1.0f, (float)rhythmicEnergyBar.FormalBehavior.Variance);
            //// request.Items.Add(GenProperty.FormalVariance, item);

            return request;
        }

        #region Public methods
        /// <summary>
        /// Gets the rhythmic stream.
        /// </summary>
        /// <param name="energyStream">The energy stream.</param>
        /// <returns>Returns value.</returns>
        public RhythmicStream GetRhythmicStream(RhythmicEnergyStream energyStream) {
            var stream = new RhythmicStream();

            foreach (var ebar in energyStream.EnergyBars) {
                //// var structure = stream.StructureInBar(ebar.BarNumber);
                var request = RhythmicProvider.GetRequest(ebar);
                var rstruct = this.PrepareRhythmicStructure(request);
                //// hbar.BarNumber = ebar.BarNumber;
                stream.Structures.Add(rstruct);
            }

            return stream;
        }
        #endregion

        #region Private static methods

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            return "Rhythmic Provider";
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Prepares the rhythmic structure.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns value.</returns>
        public RhythmicStructure PrepareRhythmicStructure(GeneralRequest request) {
            var rhythmicStructure = this.variety.OptimalNextStructForRequest(request);
            foreach (var rstr in this.variety.StructList) {
                rstr.SetPreviousStruct(rhythmicStructure);
            }

            return rhythmicStructure;
        }
        #endregion
    }
}
