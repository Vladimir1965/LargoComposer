// <copyright file="RhythmicMotive.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Harmonic Motive.
    /// </summary>
    [Serializable]
    public sealed class RhythmicMotive
    {
        #region Fields
        /// <summary>
        /// Element schema.
        /// </summary>
        private string elementSchema;

        /// <summary>
        /// Mean Mobility.
        /// </summary>
        private float? meanMobility;

        /// <summary>
        /// Mean Variance.
        /// </summary>
        private float? meanVariance;

        /// <summary>
        /// Unique identifier.
        /// </summary>
        private string uniqueIdentifier;

        /// <summary>
        /// Melodic Structures.
        /// </summary>
        [NonSerialized]
        private IEnumerable<RhythmicStructure> rhythmicStructures;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicMotive"/> class.
        /// </summary>
        public RhythmicMotive() {
            this.RhythmicStructures = new List<RhythmicStructure>();
        }

        /// <summary>
        /// Initializes a new instance of the RhythmicMotive class.
        /// </summary>
        /// <param name="name">Name of the motive.</param>
        /// <param name="rhythmicStructures">Rhythmical structures.</param>
        public RhythmicMotive(string name, IEnumerable<RhythmicStructure> rhythmicStructures) //// RhythmicCore core, 
            : this() {
            if (rhythmicStructures == null) {
                return;
            }

            this.Name = name;
            //// 2014/12 Time optimization ... int barNumber = 1;
            foreach (var rstruct in rhythmicStructures) {
                if (rstruct == null) {
                    continue;
                }

                //// 2014/12 Time optimization 
                //// rstruct.BarNumber = barNumber++;
                this.AddStructure(rstruct); //// 1, RhythmicStructures.Count()
            }
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xe = new XElement(
                                "Motive",
                                new XAttribute("Length", this.Length));

                var xstructs = new XElement("Structures");
                foreach (var rstruct in this.RhythmicStructures) {
                    var xstruct = rstruct.GetXElement;
                    xstructs.Add(xstruct);
                }

                xe.Add(xstructs);
                return xe;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        [UsedImplicitly]
        public string Shortcut { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        [UsedImplicitly]
        public string Name { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the first bar number.
        /// </summary>
        /// <value>
        /// The first bar number.
        /// </value>
        public int FirstBarNumber { get; set; }

        /// <summary>
        /// Gets or sets the occurrence.
        /// </summary>
        /// <value>
        /// The occurrence.
        /// </value>
        public int Occurrence { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic structures.
        /// </summary>
        /// <value>
        /// The rhythmic structures.
        /// </value>
        public IEnumerable<RhythmicStructure> RhythmicStructures {
            get {
                Contract.Ensures(Contract.Result<IEnumerable<RhythmicStructure>>() != null);
                if (this.rhythmicStructures == null) {
                    throw new InvalidOperationException("Rhythmic structures are null.");
                }

                return this.rhythmicStructures;
            }

            set => this.rhythmicStructures = value ?? throw new ArgumentException("Rhythmic structures cannot be empty.", nameof(value));
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value> Property description. </value>
        public int Length => ((List<RhythmicStructure>)this.RhythmicStructures).Count;

        /// <summary> Gets list of all already defined tones. </summary>
        /// <value> Property description. </value>
        public string UniqueIdentifier {
            get {
                if (this.uniqueIdentifier != null) {
                    return this.uniqueIdentifier;
                }

                var ident = new StringBuilder();
                var rhythmicMotiveStructureList = this.RhythmicStructures.ToList();
                ident.Append(string.Format(CultureInfo.CurrentCulture, "#{0}#", rhythmicMotiveStructureList.Count));
                //// ident.Append(rms.StructuralCode);   //// .ToString(CultureInfo.CurrentCulture)////  ElementSchema,  TRhythmicStructure.ElementSchema
                //// ElementSchema, DecimalNumber, ms.StructuralCode
                foreach (var b in from rms in rhythmicMotiveStructureList
                                  where rms?.GetStructuralCode != null
                                  from b in rms.GetStructuralCode
                                  select b) {
                    ident.Append(b);
                }

                this.uniqueIdentifier = ident.ToString();

                return this.uniqueIdentifier;
            }
        }

        /// <summary> Gets list of all already defined tones. </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string ElementSchema {
            get {
                if (this.elementSchema != null) {
                    return this.elementSchema;
                }

                var elemSchema = new StringBuilder();
                foreach (var rms in this.RhythmicStructures) {
                    elemSchema.Append(rms.ElementSchema.ToString(CultureInfo.CurrentCulture));   ////  ElementSchema,  TRhythmicStructure.ElementSchema
                    elemSchema.Append("/");
                }

                this.elementSchema = elemSchema.ToString();

                return this.elementSchema;
            }
        }
        #endregion

        #region Physical properties
        /// <summary> Gets list of all already defined tones. </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public float MeanMobility {
            get {
                var cnt = this.RhythmicStructures.Count();
                if (this.meanMobility != null || cnt <= 0) {
                    return this.meanMobility ?? 0;
                }

                var totalMobility = this.RhythmicStructures.Sum(rms => rms.RhythmicBehavior.Mobility);
                this.meanMobility = totalMobility / cnt;

                return (float)this.meanMobility;
            }
        }

        /// <summary> Gets list of all already defined tones. </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public float MeanVariance {
            get {
                var cnt = this.RhythmicStructures.Count();
                if (this.meanVariance != null || cnt <= 0) {
                    return this.meanVariance ?? 0;
                }

                var totalVariance = this.RhythmicStructures.Sum(rms => rms.FormalBehavior.Variance);
                this.meanVariance = totalVariance / cnt;

                return (float)this.meanVariance;
            }
        }
        #endregion

        #region Static Factory methods
        /// <summary>
        /// Simple RhythmicMotive.
        /// </summary>
        /// <param name="rhythmicStruct">Rhythmical structure.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static RhythmicMotive SimpleRhythmicMotive(RhythmicStructure rhythmicStruct) {
            if (rhythmicStruct == null) {
                return null;
            }

            var rm = new RhythmicMotive();
            //// 2014/12 Time optimization 
            //// rhythmicStruct.BarNumber = 1;
            rm.AddStructure(rhythmicStruct);
            return rm;
        }

        /// <summary>
        /// Simple Rhythmic Motive.
        /// </summary>
        /// <param name="rhythmicOrder">Rhythmical order.</param>
        /// <param name="structuralCode">The structural code.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static RhythmicMotive SimpleRhythmicMotive(byte rhythmicOrder, string structuralCode) { //// bool rhythmical, int barNumber, 
            var rm = new RhythmicMotive();
            var rs = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, rhythmicOrder);
            for (var ib = 0; ib < 4; ib++) {
                //// 2014/12 Time optimization 
                //// rstruct.BarNumber = barNumber + ib
                var rstruct = new RhythmicStructure(rs, structuralCode);
                rm.AddStructure(rstruct);
            }

            return rm;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Converts to order.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        public void ConvertToSystem(RhythmicSystem givenSystem) {
            var cnt = this.RhythmicStructures.Count();
            if (cnt == 0) {
                return;
            }

            var newStructures = this.RhythmicStructures.Select(s => s.ConvertToSystem(givenSystem)).ToList();
            this.RhythmicStructures = newStructures;
        }

        /// <summary> Returns value of characteristic planned for given musical bar.</summary>
        /// <param name="barNumber">Number of musical bar.</param>
        /// <returns> Returns value. </returns>
        public string RhythmicStructuralCodeForBar(int barNumber) {
            var structure = this.RhythmicStructureInBarNumber(barNumber);
            return structure?.GetStructuralCode;
        }

        /// <summary>
        /// Adds the structure.
        /// </summary>
        /// <param name="structure">The structure.</param>
        public void AddStructure(RhythmicStructure structure) {
            ((List<RhythmicStructure>)this.RhythmicStructures).Add(structure);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        [UsedImplicitly]
        public override string ToString() {
            return this.Name;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Rhythmic structure in bar.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        /// <returns> Returns value. </returns>
        private RhythmicStructure RhythmicStructureInBarNumber(int barNumber) {
            if (barNumber < 1) {
                throw new ArgumentOutOfRangeException(nameof(barNumber), "value must be positive");
            }

            var cnt = this.RhythmicStructures.Count();
            if (cnt == 0) {
                return null;
                //// throw new ArgumentException("No rhythmic motive structure!");
            }

            var idx = (barNumber - 1) % cnt;
            var structure = this.RhythmicStructures.ElementAt(idx);
            return structure;
        }
        #endregion
    }
}
