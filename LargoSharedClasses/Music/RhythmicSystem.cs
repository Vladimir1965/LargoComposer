// <copyright file="RhythmicSystem.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Music {
    /// <summary>  Rhythmical system. </summary>
    /// <remarks> Rhythmical system is subclass of binary GSystem. It is defined by 
    /// its order, that represents number of "ticks" in one bar. </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class RhythmicSystem : GeneralSystem {
        #region Fields

        /// <summary>
        /// Used systems.
        /// </summary>
        private static readonly Dictionary<string, RhythmicSystem> UsedSystems = new Dictionary<string, RhythmicSystem>();

        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the RhythmicSystem class.  Serializable. </summary>
        public RhythmicSystem() {
        }

        /// <summary> Initializes a new instance of the RhythmicSystem class. </summary>
        /// <param name="givenDegree">Degree of system.</param>       
        /// <param name="order">Order of system.</param>
        public RhythmicSystem(RhythmicDegree givenDegree, byte order)
            : base((byte)givenDegree, order) {
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Approximate Musical Tempo.
        /// </summary>
        /// <param name="tempoValue">Musical tempo.</param>
        /// <returns> Returns value. </returns>
        public static MusicalTempo ApproximateMusicalTempo(int tempoValue) {
            var items = SupportCommon.GetEnumValues(typeof(MusicalTempo));
            var tempo = items != null ? (from val in items
                                                  where tempoValue <= val
                                                  select (MusicalTempo)val).FirstOrDefault() : MusicalTempo.Tempo120;

            return tempo;
        }

        /// <summary>
        /// Get RhythmicSystem.
        /// </summary>
        /// <param name="degree">Degree of the system.</param>
        /// <param name="order">Order of the system.</param>
        /// <returns> Returns value. </returns>
        public static RhythmicSystem GetRhythmicSystem(RhythmicDegree degree, byte order) {
            Contract.Ensures(Contract.Result<RhythmicSystem>() != null); 
            var key = string.Format(CultureInfo.InvariantCulture, "{0}#{1}", degree, order.ToString(CultureInfo.CurrentCulture));
            var rs = UsedSystems.ContainsKey(key) ? UsedSystems[key] : null;

            if (rs != null) {
                return rs;
            }

            rs = new RhythmicSystem(degree, order);
            UsedSystems[key] = rs;

            return rs;
        }
        #endregion

        #region Substructures
        /// <summary>
        /// Modality Classes.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicModality> ModalityClasses(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewRhythmicModalityVariety(
                                        StructuralVarietyType.RhythmicModalityClasses,
                                        this,
                                        genQualifier,
                                        limit);
            return hv.StructList;
        }

        /// <summary>
        /// Modality Classes.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicModality> ModalityClasses() {
            var hv = StructuralVarietyFactory.NewRhythmicModalityVariety(
                                        StructuralVarietyType.RhythmicModalityClasses,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }

        /// <summary>
        /// Modality Instances.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicModality> ModalityInstances(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewRhythmicModalityVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        genQualifier,
                                        limit);
            return hv.StructList;
        }

        /// <summary>
        /// Modality Instances.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicModality> ModalityInstances() {
            var hv = StructuralVarietyFactory.NewRhythmicModalityVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }

        /// <summary>
        /// Shape Classes.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicShape> ShapeClasses(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewRhythmicShapeVariety(
                                        StructuralVarietyType.BinaryClasses,
                                        this,
                                        genQualifier,
                                        limit);
            return new Collection<RhythmicShape>(hv.StructList);
        }

        /// <summary>
        /// Shape Classes.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicShape> ShapeClasses() {
            var hv = StructuralVarietyFactory.NewRhythmicShapeVariety(
                                        StructuralVarietyType.BinaryClasses,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }

        /// <summary>
        /// Shape Instances.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicShape> ShapeInstances(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewRhythmicShapeVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        genQualifier,
                                        limit);
            return hv.StructList;
        }

        /// <summary>
        /// Shape Instances.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicShape> ShapeInstances() {
            var hv = StructuralVarietyFactory.NewRhythmicShapeVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }

        /// <summary>
        /// Struct Classes.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicStructure> StructClasses(GeneralQualifier genQualifier, int limit) {
            var rv = StructuralVarietyFactory.NewRhythmicStructuralVariety(
                                        StructuralVarietyType.Classes,
                                        this,
                                        genQualifier,
                                        limit);
            return rv.StructList;
        }

        /// <summary>
        /// Struct Classes.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicStructure> StructClasses() {
            var hv = StructuralVarietyFactory.NewRhythmicStructuralVariety(
                                        StructuralVarietyType.Classes,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }

        /// <summary>
        /// Struct Instances.
        /// </summary>
        /// <param name="genQualifier">General Qualifier.</param>
        /// <param name="limit">Upper limit.</param>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicStructure> StructInstances(GeneralQualifier genQualifier, int limit) {
            var hv = StructuralVarietyFactory.NewRhythmicStructuralVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        genQualifier,
                                        limit);
            return hv.StructList;
        }

        /// <summary>
        /// Struct Instances.
        /// </summary>
        /// <returns> Returns value. </returns>
        public Collection<RhythmicStructure> StructInstances() {
            var hv = StructuralVarietyFactory.NewRhythmicStructuralVariety(
                                        StructuralVarietyType.Instances,
                                        this,
                                        null,
                                        10000);
            return hv.StructList;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendLine("Rhythmical system");
            s.AppendLine(base.ToString());
            return s.ToString();
        }
        #endregion
    }
}
