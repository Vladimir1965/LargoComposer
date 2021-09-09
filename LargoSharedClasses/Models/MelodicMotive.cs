// <copyright file="MelodicMotive.cs" company="Traced-Ideas, Czech republic">
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
using JetBrains.Annotations;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Harmonic Motive.
    /// </summary>
    [Serializable]
    public sealed class MelodicMotive
    {
        #region Fields
        /// <summary>
        /// Unique identifier.
        /// </summary>
        private string uniqueIdentifier;

        /// <summary>
        /// Melodic Structures.
        /// </summary>
        [NonSerialized]
        private IEnumerable<MelodicStructure> melodicStructures;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MelodicMotive class.
        /// </summary>
        /// <param name="name">Name of the motive.</param>
        /// <param name="melodicStructures">Melodic Structures.</param>
        public MelodicMotive(string name, IEnumerable<MelodicStructure> melodicStructures) //// MelodicCore core, 
            : this() {
            if (melodicStructures == null) {
                return;
            }

            this.Name = name;
            //// BandType = (byte)mp.BandType;
            //// LineIndex = mp.Number;
            //// existsMelLine = (PartType == (byte)MelodicFunction.MelodicMotion);   
            var barNumber = 1;
            foreach (var mstruct in melodicStructures) {
                if (mstruct == null) {
                    continue;
                }

                mstruct.BarNumber = barNumber++;
                this.AddStructure(mstruct);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicMotive"/> class.
        /// </summary>
        public MelodicMotive() {
            this.melodicStructures = new List<MelodicStructure>();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name.
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
        /// Gets or sets the octave.
        /// </summary>
        /// <value>
        /// The octave.
        /// </value>
        [UsedImplicitly]
        public MusicalOctave Octave { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets the type of the part.
        /// </summary>
        /// <value>
        /// The type of the part.
        /// </value>
        [UsedImplicitly]
        public MelodicFunction MelodicFunction { get; set; }   //// CA1044 (FxCop)

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
        /// Gets or sets the melodic structures.
        /// </summary>
        /// <value>
        /// The melodic structures.
        /// </value>
        public IEnumerable<MelodicStructure> MelodicStructures {
            get {
                Contract.Ensures(Contract.Result<IEnumerable<MelodicStructure>>() != null);
                if (this.melodicStructures == null) {
                    throw new InvalidOperationException("Melodic structures are null.");
                }

                return this.melodicStructures;
            }

            set => this.melodicStructures = value ?? throw new ArgumentException(Localization.LocalizedMusic.String("Argument cannot be empty."), nameof(value));
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value> Property description. </value>
        public int Length => ((List<MelodicStructure>)this.MelodicStructures).Count;

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        /// The is empty.
        /// </value>
        [UsedImplicitly]
        public bool IsEmpty => this.MelodicStructures.FirstOrDefault() == null;

        /// <summary> Gets list of all already defined tones. </summary>
        /// <value> Property description. </value>
        public string UniqueIdentifier {
            get {
                if (this.uniqueIdentifier != null) {
                    return this.uniqueIdentifier;
                }

                var ident = new StringBuilder();
                var melodicMotiveStructureList = this.MelodicStructures.ToList();
                ident.Append(string.Format(CultureInfo.CurrentCulture, "#{0}#", melodicMotiveStructureList.Count));
                //// ElementSchema, DecimalNumber, ms.StructuralCode
                foreach (var b in from mms in melodicMotiveStructureList
                                  where mms?.GetStructuralCode != null
                                  from b in mms.GetStructuralCode
                                  select b) {
                    ident.Append(b);
                }

                this.uniqueIdentifier = ident.ToString();

                return this.uniqueIdentifier;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        [UsedImplicitly]
        public string Shortcut { get; set; }   //// CA1044 (FxCop)
        #endregion

        #region Static Factory methods
        /// <summary>
        /// Simple melodic motive.
        /// </summary>
        /// <param name="melodicStruct">The melodic struct.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static MelodicMotive SimpleMelodicMotive(MelodicStructure melodicStruct) {
            if (melodicStruct == null) {
                return null;
            }

            var rm = new MelodicMotive();
            melodicStruct.BarNumber = 1;
            rm.AddStructure(melodicStruct);
            return rm;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Melodic structure in bar.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        /// <returns> Returns value. </returns>
        public MelodicStructure MelodicStructureInBar(int barNumber) {
            if (barNumber < 1) {
                throw new ArgumentOutOfRangeException(nameof(barNumber), "value must be positive");
            }

            var cnt = this.MelodicStructures.Count();
            if (cnt == 0) {
                return null;
                //// throw new ArgumentException("No melodic motive structure!");
            }

            var idx = (barNumber - 1) % cnt;
            var structure = this.MelodicStructures.ElementAt(idx);
            return structure;
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

        /// <summary>
        /// Adds the structure.
        /// </summary>
        /// <param name="structure">The structure.</param>
        public void AddStructure(MelodicStructure structure) {
            ((List<MelodicStructure>)this.MelodicStructures).Add(structure);
        }
        #endregion
    }
}
