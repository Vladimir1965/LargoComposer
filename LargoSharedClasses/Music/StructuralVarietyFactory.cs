// <copyright file="StructuralVarietyFactory.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// StructuralVariety Factory.
    /// </summary>
    public static class StructuralVarietyFactory {
        #region Factory of structures
        /// <summary>
        /// Initializes a new instance of the StructuralVariety class.
        /// </summary>
        /// <param name="varietyType">Type of variety.</param>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.PureAttribute]
        public static StructuralVariety<HarmonicStructure> NewHarmonicStructuralVariety(
                          StructuralVarietyType varietyType,
                          GeneralSystem givenSystem,
                          GeneralQualifier qualifier,
                          int limitCount) {
            //// Contract.Requires(givenSystem != null);
            var gsv =
                new StructuralVariety<HarmonicStructure>(givenSystem) { VarType = varietyType, Qualifier = qualifier, LimitCount = limitCount };
            gsv.Generate();
            return gsv;
        }

        /// <summary>
        /// Initializes a new instance of the StructuralVariety class.
        /// </summary>
        /// <param name="varietyType">Type of variety.</param>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.PureAttribute]
        public static StructuralVariety<RhythmicStructure> NewRhythmicStructuralVariety(
                          StructuralVarietyType varietyType,
                          GeneralSystem givenSystem,
                          GeneralQualifier qualifier,
                          int limitCount) {
            //// Contract.Requires(givenSystem != null);
            var gsv =
                new StructuralVariety<RhythmicStructure>(givenSystem) { VarType = varietyType, Qualifier = qualifier, LimitCount = limitCount };
            gsv.Generate();
            return gsv;
        }

        /// <summary>
        /// Initializes a new instance of the StructuralVariety class.
        /// </summary>
        /// <param name="varietyType">Type of variety.</param>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.PureAttribute]
        public static StructuralVariety<RhythmicShape> NewRhythmicShapeVariety(
                          StructuralVarietyType varietyType,
                          GeneralSystem givenSystem,
                          GeneralQualifier qualifier,
                          int limitCount) {
            //// Contract.Requires(givenSystem != null);
            var gsv = new StructuralVariety<RhythmicShape>(givenSystem) {
                VarType = varietyType,
                Qualifier = qualifier,
                LimitCount = limitCount
            };
            gsv.Generate();
            return gsv;
        }

        /// <summary>
        /// Initializes a new instance of the StructuralVariety class.
        /// </summary>
        /// <param name="varietyType">Type of variety.</param>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.PureAttribute]
        public static StructuralVariety<MelodicStructure> NewMelStructuralVariety(
                          StructuralVarietyType varietyType,
                          GeneralSystem givenSystem,
                          GeneralQualifier qualifier,
                          int limitCount) {
            //// Contract.Requires(givenSystem != null);
            var gsv =
                new StructuralVariety<MelodicStructure>(givenSystem) { VarType = varietyType, Qualifier = qualifier, LimitCount = limitCount };
            gsv.Generate();
            return gsv;
        }

        /// <summary>
        /// Initializes a new instance of the StructuralVariety class.
        /// </summary>
        /// <param name="varietyType">Type of variety.</param>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.PureAttribute]
        public static StructuralVariety<HarmonicModality> NewHarmonicModalityVariety(
                          StructuralVarietyType varietyType,
                          GeneralSystem givenSystem,
                          GeneralQualifier qualifier,
                          int limitCount) {
            //// Contract.Requires(givenSystem != null);
            var gsv =
                new StructuralVariety<HarmonicModality>(givenSystem) { 
                    VarType = varietyType, Qualifier = qualifier, LimitCount = limitCount 
                };
            gsv.Generate();
            return gsv;
        }

        /// <summary>
        /// Initializes a new instance of the StructuralVariety class.
        /// </summary>
        /// <param name="varietyType">Type of variety.</param>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [JetBrains.Annotations.PureAttribute]
        public static StructuralVariety<RhythmicModality> NewRhythmicModalityVariety(
                          StructuralVarietyType varietyType,
                          GeneralSystem givenSystem,
                          GeneralQualifier qualifier,
                          int limitCount) {
            //// Contract.Requires(givenSystem != null);
            var gsv =
                new StructuralVariety<RhythmicModality>(givenSystem) { VarType = varietyType, Qualifier = qualifier, LimitCount = limitCount };
            gsv.Generate();
            return gsv;
        }

        #endregion

        #region Factory of substructures
        /// <summary> Initializes a new instance of the StructuralVariety class. </summary>
        /// <param name="varietyType">Type of variety.</param>
        /// <param name="modality">Abstract modality.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static StructuralVariety<HarmonicStructure> NewHarmonicStructModalVariety( //// Type T
                StructuralVarietyType varietyType,
                BinaryStructure modality,
                GeneralQualifier qualifier,
                int limitCount) {
            Contract.Requires(modality != null);
            
            //// if (modality == null) { return null; }
            var givenSystem = modality.GSystem;
            var gsv = new StructuralVariety<HarmonicStructure>(givenSystem) {
                VarType = varietyType,
                Modality = modality,
                Qualifier = qualifier,
                LimitCount = limitCount
            };
            gsv.Generate();
            return gsv;
        }

        /// <summary> Initializes a new instance of the StructuralVariety class. </summary>
        /// <param name="varietyType">Type of variety.</param>
        /// <param name="modality">Abstract modality.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static StructuralVariety<RhythmicStructure> NewRhythmicStructModalVariety( //// Type T
                StructuralVarietyType varietyType,
                BinaryStructure modality,
                GeneralQualifier qualifier,
                int limitCount) {
            Contract.Requires(modality != null);
            if (modality == null) {
                return null;
            }

            var givenSystem = new GeneralSystem(3, modality.GSystem.Order);
            var gsv = new StructuralVariety<RhythmicStructure>(givenSystem) {
                VarType = varietyType,
                Modality = modality,
                Qualifier = qualifier,
                LimitCount = limitCount
            };
            gsv.Generate();
            return gsv;
        }

        /// <summary> Initializes a new instance of the StructuralVariety class. </summary>
        /// <param name="varietyType">Type of variety.</param>
        /// <param name="modality">Abstract modality.</param>
        /// <param name="qualifier">Abstract qualifier.</param>
        /// <param name="limitCount">Limit for number od structures.</param>
        /// <returns> Returns value. </returns>
        [JetBrains.Annotations.PureAttribute]
        public static StructuralVariety<RhythmicShape> NewRhythmicShapeModalVariety( //// Type T
                StructuralVarietyType varietyType,
                BinaryStructure modality,
                GeneralQualifier qualifier,
                int limitCount) {
            Contract.Requires(modality != null);
            
            //// if (modality == null) { return null; }
            var givenSystem = new GeneralSystem(2, modality.GSystem.Order);
            var gsv = new StructuralVariety<RhythmicShape>(givenSystem) {
                VarType = varietyType,
                Modality = modality,
                Qualifier = qualifier,
                LimitCount = limitCount
            };
            gsv.Generate();
            return gsv;
        }
        #endregion
    }
}
