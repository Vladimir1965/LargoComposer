// <copyright file="MelodicPlan.cs" company="J.K.R.">
// Copyright (c) 2012 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Xml.Serialization;

namespace LargoSharedClasses.Melody
{
    /// <summary> A melodic plan. </summary>
    public class MelodicPlan
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicPlan"/> class.
        /// </summary>
        public MelodicPlan() {
            this.PlannedTones = new ToneCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicPlan"/> class.
        /// </summary>
        /// <param name="givenTones"> The given tones. </param>
        public MelodicPlan(ToneCollection givenTones) {
            this.PlannedTones = givenTones.Clone(false);
        }

        /// <summary> Initializes a new instance of the <see cref="MelodicPlan"/> class. </summary>
        /// <param name="harmonicSystem"> Harmonic system. </param>
        public MelodicPlan(HarmonicSystem harmonicSystem) {
            int altitude = 80;
            int barNumber = 1;
            var p0 = new MusicalPitch(harmonicSystem, altitude);
            var t0 = new MusicalTone(p0, new BitRange(), MusicalLoudness.MeanLoudness, barNumber);
            var ms = new ToneCollection { t0 };

            /* Alternatives
                altitude += 2* element.EnterStep;
                for (int i = 0; i <= st.RhythmicStructure.ToneLevel; i++) {
                    var p2 = new MusicalPitch(header.System.HarmonicSystem, altitude);
                    var t2 = new MusicalTone(p2, new BitRange(), MusicalLoudness.MeanLoudness, st.BarNumber);
                    st.PlannedTones.Add(t2);
                    altitude += 2 * element.InnerStep;
                }
            */

            this.PlannedTones = ms;
        }
        #endregion

        /// <summary> Gets or sets complete list of all planed tones for musical part. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public ToneCollection PlannedTones { get; set; }

        #region Public methods - PlannedTones

        /// <summary>
        /// Project Into Planned Tones.
        /// </summary>
        /// <param name="harmonicSystem">Harmonic system.</param>
        /// <param name="harmonicModality">Harmonic Modality.</param>
        /// <param name="tones">Musical Tones.</param>
        /// <param name="melStruct">The mel structure.</param>
        /// <param name="modalityRestriction">if set to <c>true</c> Respect current modality of each bar.</param>
        /// <exception cref="ContractException">Thrown when a method Contract has been broken.</exception>
        [UsedImplicitly]
        public void ProjectIntoPlannedTones(HarmonicSystem harmonicSystem, BinarySchema harmonicModality, IEnumerable<IMusicalTone> tones, MelodicStructure melStruct, bool modalityRestriction) { 
            Contract.Requires(harmonicSystem != null);
            Contract.Requires(harmonicModality != null);
            Contract.Requires(tones != null);

            //// Respect current modality of each bar
            //// const bool modalityRestriction = true;
            if (this.PlannedTones == null) {
                this.PlannedTones = new ToneCollection();
            }

            //// MelodicStructure melStruct = this.Status.MelodicMotive.MelodicStructureInBar(tone.BarNumber);
            ////  Status.MelodicStructure
            if (melStruct == null || melStruct.Level <= 1) {
                return;
            }

            //// int ri = 0;
            int toneIdx = 0;
            foreach (var tone in tones) {
                var mtone = tone as MusicalTone;
                IMusicalTone pmt;
                if (modalityRestriction) {
                    if (mtone != null && mtone.Loudness > 0) {
                        int modalityIndex = ((toneIdx == 0) ? melStruct.Drift : 0) + melStruct.ElementList[toneIdx % melStruct.Order];
                        //// int modalityIndex = ((toneIdx == 0) ? melStruct.Drift : 0) + melStruct.DiffList[toneIdx % melStruct.Level];
                        //// int modalityIndex = ((toneIdx == 0) ? melStruct.Drift : 0) + ri++;
                        int altitude = melStruct.PlannedAltitude(harmonicSystem, harmonicModality, modalityIndex);
                        MusicalPitch mpi = harmonicSystem.GetPitch(altitude); //// new MusicalPitch(harmonicSystem, altitude);
                        pmt = new MusicalTone(mpi, mtone.BitRange, mtone.Loudness, mtone.BarNumber); //// (mtone.BarNumberFrom)
                    }
                    else {
                        pmt = mtone;
                    }
                }
                else {
                    //// ?!?
                    int altitude = melStruct.ElementList[toneIdx % melStruct.Order] * 2;
                    if (toneIdx > melStruct.Level) {
                        altitude += melStruct.Level * 2;
                    }

                    MusicalPitch mpi = harmonicSystem.GetPitch(altitude); //// new MusicalPitch(harmonicSystem, altitude);
                    pmt = new MusicalTone(mpi, mtone.BitRange, mtone.Loudness, mtone.BarNumber); //// (mtone.BarNumberFrom)
                }

                if (pmt != null) {
                    pmt.OrdinalIndex = toneIdx;
                    this.PlannedTones.Add(pmt);
                }

                toneIdx++;
            }
        }
        #endregion
    }
}
