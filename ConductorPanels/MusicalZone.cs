// <copyright file="MusicalZone.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;
using System.Collections.Generic;
using System.Text;

namespace ConductorPanels
{
    /// <summary>
    /// Musical Zone.
    /// </summary>
    public class MusicalZone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalZone" /> class.
        /// </summary>
        /// <param name="systemLength">The system length.</param>
        /// <param name="mobility">The mobility.</param>
        /// <param name="bars">The bars.</param>
        /// <param name="lines">The lines.</param>
        /// <param name="variability">The variability.</param>
        /// <param name="name">The name.</param>
        /// <param name="orchestra">The orchestra.</param>
        public MusicalZone(byte systemLength, int mobility, int bars, int lines, int variability, string name, OrchestraUnit orchestra) {
            this.Length = systemLength;
            this.Name = name;
            this.Mobility = mobility;
            this.Bars = bars;
            this.Lines = lines;
            this.Varia = variability;
            this.Orchestra = orchestra;
        }
        
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public byte Length { get; set; }

        /// <summary>
        /// Gets or sets the mobility.
        /// </summary>
        /// <value>
        /// The mobility.
        /// </value>
        public int Mobility { get; set; }

        /// <summary>
        /// Gets or sets the bars.
        /// </summary>
        /// <value>
        /// The bars.
        /// </value>
        public int Bars { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        public int Lines { get; set; }

        /// <summary>
        /// Gets or sets the variability.
        /// </summary>
        /// <value>
        /// The variability.
        /// </value>
        public int Varia { get; set; }

        /// <summary>
        /// Gets or sets the loudness.
        /// </summary>
        /// <value>
        /// The loudness.
        /// </value>
        public MusicalLoudness Loudness { get; set; }

        /// <summary>
        /// Gets or sets the orchestra.
        /// </summary>
        /// <value>
        /// The orchestra.
        /// </value>
        public OrchestraUnit Orchestra { get; set; }

        /// <summary>
        /// Gets or sets the harmonic modality.
        /// </summary>
        /// <value>
        /// The harmonic modality.
        /// </value>
        public HarmonicModality HarmonicModality { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic modality.
        /// </summary>
        /// <value>
        /// The rhythmic modality.
        /// </value>
        public RhythmicModality RhythmicModality { get; set; }
        
        /// <summary>
        /// Gets or sets the rhythmic structure.
        /// </summary>
        /// <value>
        /// The rhythmic structure.
        /// </value>
        public RhythmicStructure RhythmicStructure { get; set; }

        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>
        /// The elements.
        /// </value>
        public List<MelodicElement> Elements { get; set; }

        /// <summary>
        /// Gets or sets the plan function.
        /// </summary>
        /// <value>
        /// The plan function.
        /// </value>
        public PlanFunction MobilityPlanFunction { get; set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public LineStatus Status {
            get {
                var status = new LineStatus {
                    LineType = MusicalLineType.Melodic,
                    MelodicFunction = MelodicFunction.MelodicMotion,
                    MelodicShape = MelodicShape.Scales,
                    LocalPurpose = LinePurpose.Composed,
                    IsMelodicOriginal = true
                };
                status.MelodicPlan.PlannedTones = new ToneCollection();

                return status;
            }
        }

        /// <summary>
        /// Mobility for bar.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        public void DetermineMobilityForBar(int barNumber) {
            var planMobility = this.MobilityPlanFunction;
            //// !?!?!?
            var mobility = planMobility.ValueForBar(barNumber) / 4;
            this.Mobility = mobility;
        }

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(this.Name);
            s.AppendFormat("{0,4},{1,4},{2,4}", this.Mobility, this.Lines, this.Varia);

            if (this.Elements != null) {
                foreach (var e in this.Elements) {
                    s.Append("=>" + e.ToString());
                }
            }

            return s.ToString();
        }
        #endregion
    }
}
