// <copyright file="MelodicMaterial.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Models
{
    /// <summary>  Musical material. </summary>
    /// <remarks> Musical class. </remarks>
    ////    [Serializable]
    [ContractVerification(false)]
    public sealed class MelodicMaterial {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MelodicMaterial class.
        /// </summary>
        public MelodicMaterial() {
            this.Structures = new List<MelodicStructure>();
        }

        /// <summary> Initializes a new instance of the MelodicMaterial class. </summary>
        /// <param name="name">Name of the content.</param>
        [UsedImplicitly]
        public MelodicMaterial(string name)
            : this() {
            this.Name = name;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The melodic order.
        /// </value>
        public string Name { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets the structures.
        /// </summary>
        /// <value>
        /// The structures.
        /// </value>
        public IList<MelodicStructure> Structures { get; }

        #endregion

        #region MelodicProducer
        /// <summary>
        /// Random Melodic Materials.
        /// </summary>
        /// <param name="number">Number of material sets.</param>
        /// <param name="melStructs">Melodic structs.</param>
        /// <param name="numberOfStructs">Number Of Structs.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static Collection<MelodicMaterial> RandomMelodicMaterials(int number, Collection<MelodicStructure> melStructs, int numberOfStructs) {
            var coll = new Collection<MelodicMaterial>();
            for (var i = 0; i < number; i++) {
                var name = string.Format(CultureInfo.CurrentCulture, "Automatic ({0}) {1}", i.ToString(CultureInfo.CurrentCulture.NumberFormat).PadLeft(3), SupportCommon.DateTimeIdentifier);
                var melMaterial = RandomMelodicMaterial(name, melStructs, numberOfStructs);
                coll.Add(melMaterial);
            }

            return coll;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            //// s.Append("\t" + this.RhythmicOrder.ToString(CultureInfo.CurrentCulture));
            //// s.Append("\t" + this.HarmonicOrder.ToString(CultureInfo.CurrentCulture));
            return s.ToString();
        }
        #endregion

        #region MelodicProducer - private
        /// <summary>
        /// Random Melodic Material.
        /// </summary>
        /// <param name="name">Name of material.</param>
        /// <param name="melStructs">Melodic Structs.</param>
        /// <param name="numberOfStructs">Number Of Structs.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static MelodicMaterial RandomMelodicMaterial(string name, Collection<MelodicStructure> melStructs, int numberOfStructs) {
            Contract.Requires(name != null);
            //// var dc = this.GetDataContext;
            var melMaterial = new MelodicMaterial { Name = name };
            if (melStructs == null) {
                return melMaterial;
            }

            for (var im = 0; im < numberOfStructs; im++) {
                var ms = ExtendCollection<MelodicStructure>.GetRandomObject(melStructs);
                if (ms == null) {
                    continue;
                }

                var mms = ms;  //// Clone?
                melMaterial.Structures?.Add(mms);
            }

            return melMaterial;
        }
        #endregion
    }
}
