// <copyright file="HarmonicMaterial.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Models
{
    using Abstract;
    using JetBrains.Annotations;
    using LargoSharedClasses.Music;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;

    /// <summary>  Musical material. </summary>
    /// <remarks> Musical class. </remarks>
    ////    [Serializable]
    [ContractVerification(false)]
    public sealed class HarmonicMaterial {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the HarmonicMaterial class.
        /// </summary>
        public HarmonicMaterial() {
            this.Structures = new List<HarmonicStructure>();
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
        /// Gets or sets the harmonic order.
        /// </summary>
        /// <value>The harmonic order.</value>
        public byte HarmonicOrder { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets the structures.
        /// </summary>
        /// <value>
        /// The structures.
        /// </value>
        public IList<HarmonicStructure> Structures { get; }
        #endregion

        #region HarmonicProducer
        /// <summary>
        /// Random Harmonic Material.
        /// </summary>
        /// <param name="number">Number of material sets.</param>
        /// <param name="harmonicStructs">Harmonic structures.</param>
        /// <param name="numberOfStructs">Number Of Structs.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static Collection<HarmonicMaterial> RandomHarmonicMaterials(int number, Collection<HarmonicStructure> harmonicStructs, int numberOfStructs) { //// byte harmonicOrder,
            Contract.Requires(harmonicStructs != null);
            var coll = new Collection<HarmonicMaterial>();
            for (var i = 0; i < number; i++) {
                var name = string.Format(CultureInfo.CurrentCulture, "Automatic ({0}) {1}", i.ToString(CultureInfo.CurrentCulture.NumberFormat).PadLeft(3), SupportCommon.DateTimeIdentifier);
                var harMaterial = RandomHarmonicMaterial(name, harmonicStructs, numberOfStructs);
                coll.Add(harMaterial);
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
            return s.ToString();
        }
        #endregion

        #region HarmonicProducer - private

        /// <summary>
        /// Random Harmonic Material.
        /// </summary>
        /// <param name="name">Name of material.</param>
        /// <param name="harStructs">Harmonic structures.</param>
        /// <param name="numberStructs">Number Structs.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static HarmonicMaterial RandomHarmonicMaterial(string name, Collection<HarmonicStructure> harStructs, int numberStructs) {
            Contract.Requires(name != null);
            Contract.Requires(harStructs != null);
            //// var dc = this.GetDataContext;
            var harMaterial = new HarmonicMaterial { Name = name };

            //// HarmonicSystem harSystem = HarmonicSystem.GetHarmonicSystem(harmonicOrder);
            for (var im = 0; im < numberStructs; im++) {
                var hs = ExtendCollection<HarmonicStructure>.GetRandomObject(harStructs);
                if (hs == null || harMaterial.Structures == null) {
                    continue;
                }

                var mhs = hs; //// Clone()
                harMaterial.Structures.Add(mhs);
            }

            return harMaterial;
        }
        #endregion
    }
}
