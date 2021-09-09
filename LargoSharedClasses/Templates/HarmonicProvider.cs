// <copyright file="HarmonicProvider.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Templates
{
    using LargoSharedClasses.Music;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Harmonic Provider.
    /// </summary>
    public class HarmonicProvider {
        #region Fields
        /// <summary>
        /// The variety
        /// </summary>
        private readonly StructuralVariety<HarmonicStructure> variety;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicProvider" /> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="modality">The modality.</param>
        public HarmonicProvider(MusicalHeader givenHeader, HarmonicModality modality) {
            Contract.Requires(modality != null);
            this.Header = givenHeader;

            this.variety = new StructuralVariety<HarmonicStructure>(this.Header.System.HarmonicSystem) {
                VarType = StructuralVarietyType.BinarySubstructuresOfModality,
                Modality = modality,
                LimitCount = 1000
            };

            this.variety.Generate();
            var structures = (from v in this.variety.StructList where v.Level == 3 select v).ToList();
            foreach (var s in structures) {
                s.Modality = modality;
                s.DetermineBehaviorInModality();
            }

            this.variety.SetStructList(structures);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "Harmonic Provider";
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the harmonic stream.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="rhythmicStream">The rhythmic stream.</param>
        /// <param name="energyStream">The energy stream.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public HarmonicStream GetHarmonicStream(MusicalHeader givenHeader, RhythmicStream rhythmicStream, HarmonicEnergyStream energyStream) {
            if (givenHeader.Clone() is MusicalHeader header)
            {
                //// header.Name = SupportCommon.DateTimeIdentifier;
                header.FileName = "Generated";
            }

            var stream = new HarmonicStream(givenHeader);
            
            foreach (var ebar in energyStream.EnergyBars) {
                var structure = rhythmicStream.StructureInBar(ebar.BarNumber);
                if (structure == null) {
                    continue;
                }

                var request = HarmonicProvider.GetRequest(ebar); //// ebar.HarmonicFlow, ebar.IsConsonant
                var hbar = this.PrepareHarmonicBar(structure, request);
                if (hbar == null) {
                    continue;
                }

                hbar.BarNumber = ebar.BarNumber;
                stream.HarmonicBars.Add(hbar);
            }

            return stream;
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <param name="energyBar">The energy bar.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static GeneralRequest GetRequest(HarmonicEnergyBar energyBar) { 
            var request = new GeneralRequest();
            var item = new GeneralRequestItem(GenProperty.Consonance, 1.0f, (float)energyBar.HarmonicConsonance);
            request.Items.Add(GenProperty.Consonance, item);

            item = new GeneralRequestItem(GenProperty.Potential, 1.0f, (float)energyBar.HarmonicPotential);
            request.Items.Add(GenProperty.Potential, item);

            item = new GeneralRequestItem(GenProperty.RelatedContinuity, 1.0f, 100);
            request.Items.Add(GenProperty.RelatedContinuity, item);

            return request;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Prepares the harmonic bar.
        /// </summary>
        /// <param name="structure">The structure.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private HarmonicBar PrepareHarmonicBar(RhythmicStructure structure, GeneralRequest request) {
            if (structure == null) {
                return null;        
            }

            if (this.variety == null || !this.variety.StructList.Any()) {
                return null;        
            }
            
            var rs = structure.RhythmicSystem;
            var shape = new RhythmicShape(rs.Order, structure);
            var harmonicBar = new HarmonicBar(1, 0) {
                RhythmicStructure = structure
            };

            for (byte level = 0; level < shape.Level; level++) {
                var place = shape.Places[level];
                var distance = shape.DistanceAtLevel(level);
                //// var request = HarmonicStream.GetRequest(rs.Order, distance);

                var hs = this.variety.OptimalNextStructForRequest(request);
                if (hs == null) {
                    continue;
                }

                foreach (var hstr in this.variety.StructList) {
                    hstr.SetPreviousStruct(hs);
                }

                ///// barMetric.On(tick);
                hs.BitFrom = place;
                hs.Length = distance;
                hs.HarmonicModality = (HarmonicModality)this.variety.Modality;
                harmonicBar.AddStructure(hs);
            }

            harmonicBar.HarmonicModality = (HarmonicModality)this.variety.Modality;

            //// barMetric.DetermineLevel();
            //// string barMetricCode = barMetric.GetStructuralCode();
            shape.DetermineLevel();
            var barMetricCode = shape.GetStructuralCode;
            harmonicBar.SetBarMetricCode(barMetricCode);
            return harmonicBar;
        }
        #endregion
    }
}