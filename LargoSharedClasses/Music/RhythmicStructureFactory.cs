// <copyright file="RhythmicStructureFactory.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Rhythm;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Rhythmic Structure Factory.
    /// </summary>
    public static class RhythmicStructureFactory
    {
        #region Public static methods
        /// <summary>
        /// Gets the regular structures.
        /// </summary>
        /// <param name="givenRhythmicOrder">The given rhythmic order.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        /// <value> Property description. </value>
        public static IList<RhythmicStructure> RegularStructures(byte givenRhythmicOrder) {
            var structs = new List<RhythmicStructure>();
            for (var parts = 1; parts < givenRhythmicOrder - 1; parts++) {
                if (parts > 16 || givenRhythmicOrder % parts != 0) {
                    continue;
                }

                var rms = RegularRhythmicStructure(givenRhythmicOrder, parts);
                structs.Add(rms);
            }

            return structs;
        }

        /// <summary>
        /// Inverted structures.
        /// </summary>
        /// <param name="givenStructures">The given structures.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static IEnumerable<RhythmicStructure> InvertedStructures(IList<RhythmicStructure> givenStructures) {
            var structures = new List<RhythmicStructure>();

            foreach (var rstruct in givenStructures) {
                var newStructure = rstruct.InvertedStructure();
                structures.Add(newStructure);
            }

            return structures;
        }

        /// <summary>
        /// Enriched structures.
        /// </summary>
        /// <param name="givenStructures">The given structures.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static IEnumerable<RhythmicStructure> EnrichedStructures(IList<RhythmicStructure> givenStructures) {
            var structures = new List<RhythmicStructure>();

            foreach (var rstruct in givenStructures) {
                var newStructure = rstruct.HalfEnrichedStructure();
                structures.Add(newStructure);
            }

            return structures;
        } 
        #endregion

        #region Private static methods - Regular structures      

        /// <summary>
        /// Regulars the rhythmic structure.
        /// </summary>
        /// <param name="givenRhythmicOrder">The given rhythmic order.</param>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static RhythmicStructure RegularRhythmicStructure(byte givenRhythmicOrder, int parts) {
            var rs = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, givenRhythmicOrder);

            var sb = new StringBuilder();
            var length = givenRhythmicOrder / parts;
            var s = string.Format(CultureInfo.CurrentCulture, "1,{0}*0,", length - 1);
            for (var i = 0; i < parts; i++) {
                sb.Append(s);
            }

            sb.Remove(sb.Length - 1, 1);
            var rms = new RhythmicStructure(rs, sb.ToString());

            return rms;
        }
        #endregion
    }
}
