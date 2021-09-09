// <copyright file="HarmonicStructuralVariety.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Xml.Serialization;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Harmony;

namespace LargoSharedClasses.Music
{
    /// <summary> Harmonic variety. </summary>
    /// <remarks> Harmonic variety is subclass of variety. It it designed to keep 
    /// selected harmonic structures of given Modality.  </remarks>
    [Serializable] 
    [XmlInclude(typeof(HarmonicSystem))]
    public class HarmonicStructuralVariety : StructuralVariety<HarmonicStructure> {
        #region Fields
        /// <summary> Harmonic functions. </summary>
        private Dictionary<HarmonicFunctionType, HarmonicStructure> functions;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the HarmonicStructuralVariety class.  Serializable.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        public HarmonicStructuralVariety(GeneralSystem givenSystem)
            : base(givenSystem) {
            this.functions = new Dictionary<HarmonicFunctionType, HarmonicStructure>();
        }

        #endregion

        #region Properties

        /// <summary> Gets table of harmonic functions. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        private Dictionary<HarmonicFunctionType, HarmonicStructure> Functions {
            get {
                Contract.Ensures(Contract.Result<Dictionary<HarmonicFunctionType, HarmonicStructure>>() != null);
                if (this.functions == null) {
                    throw new InvalidOperationException("Functions is null.");
                }

                return this.functions; 
            }
        }
        #endregion

        #region Public static methods
        /// <summary> Initializes a new instance of the HarmonicStructuralVariety class. </summary>
        /// <param name="modality">Abstract modality.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static HarmonicStructuralVariety NewHarmonicStructuralVariety(BinaryStructure modality, GeneralQualifier qualifier, int limitCount) {
            Contract.Requires(modality != null);
            if (modality == null) {
                return null;
            }

            GeneralSystem givenSystem = modality.GSystem;
            HarmonicStructuralVariety gsv = new HarmonicStructuralVariety(givenSystem)
                                                {
                                                    VarType = StructuralVarietyType.BinarySubstructuresOfModality,
                                                    Modality = modality,
                                                    Qualifier = qualifier,
                                                    LimitCount = limitCount
                                                };
            gsv.Generate();
            gsv.DetermineFunctions();
            gsv.SortStructList(GenProperty.Consonance, GenSortDirection.Descending);
            return gsv;
        }
        #endregion

        #region Public methods
        /// <summary> Returns requested harmonic Function. </summary>
        /// <param name="functionItem">Harmonic function.</param>
        /// <returns> Returns value. </returns>
        public HarmonicStructure Function(HarmonicFunctionType functionItem) {
            return this.Functions[functionItem];
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append("* H-struct variety *\r\n");
            s.Append(base.ToString());
            if (this.Modality != null) {
                s.Append("Modality:" + this.Modality);
            }

            s.Append("Functions:" + this.Functions);
            return s.ToString();
        }
        #endregion

        #region Harmonic functions
        /// <summary> Determine harmonic functions. </summary>
        private void DetermineFunctions() {
            if (this.StructList.Count == 0) {
                return;
            }

            this.functions = new Dictionary<HarmonicFunctionType, HarmonicStructure>();
            //// this.StructList.ForAll((harmonicStructure) => harmonicStructure.HarmonicModality = (HarmonicModality)Modality);

            this.SortStructList(GenProperty.Tonicity, GenSortDirection.Descending); // HarmonicTonicity,FormalPotential
            HarmonicStructure tonic = null;
            if (this.StructList.Count > 0) {
                tonic = this.StructList[0];
                this.Functions.Add(HarmonicFunctionType.Tonic, tonic);
            }

            if (this.StructList.Count > 0) {
                HarmonicStructure antitonic = this.StructList[this.StructList.Count - 1];
                this.Functions.Add(HarmonicFunctionType.AntiTonic, antitonic);
                if (tonic != null) {
                    this.StructList.ForAll(harmonicStructure => harmonicStructure.DetermineBehaviorToTonic(tonic));
                }
            }

            this.SortStructList(GenProperty.TonicContinuity, GenSortDirection.Descending);
            if (this.StructList.Count > 0)
            {
                HarmonicStructure dominant = this.StructList[0];
                this.Functions.Add(HarmonicFunctionType.Dominant, dominant);
            }

            if (this.StructList.Count > 0)
            {
                HarmonicStructure subdominant = this.StructList[this.StructList.Count - 1];
                this.Functions.Add(HarmonicFunctionType.Subdominant, subdominant);
            }

            this.SortStructList(GenProperty.TonicImpulse, GenSortDirection.Descending);
            if (this.StructList.Count <= 0)
            {
                return;
            }

            HarmonicStructure sensitive = this.StructList[0];
            this.Functions.Add(HarmonicFunctionType.Sensitive, sensitive);
        }
        #endregion
    }
}
