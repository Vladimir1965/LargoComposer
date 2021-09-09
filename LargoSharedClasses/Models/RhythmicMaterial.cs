// <copyright file="RhythmicMaterial.cs" company="Traced-Ideas, Czech republic">
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
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Models
{
    /// <summary>  Musical material. </summary>
    /// <remarks> Musical class. </remarks>
    ////    [Serializable]
    [ContractVerification(false)]
    public sealed class RhythmicMaterial {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RhythmicMaterial class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        public RhythmicMaterial(MusicalHeader givenHeader) {
            this.Header = givenHeader;
            this.Structures = new List<RhythmicStructure>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicMaterial" /> class.
        /// </summary>
        /// <param name="xmaterial">The mark material.</param>
        public RhythmicMaterial(XElement xmaterial) {
            XElement xheader = xmaterial.Element("Header");
            this.Header = new MusicalHeader(xheader, true);
            
            this.RhythmicOrder = XmlSupport.ReadByteAttribute(xmaterial.Attribute("RhythmicOrder"));
            this.Structures = new List<RhythmicStructure>();

            var xstructs = xmaterial.Element("Structures");
            if (xstructs == null) {
                return;
            }

            foreach (var xstruct in xstructs.Elements()) {
                var rs = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, this.RhythmicOrder);
                var code = XmlSupport.ReadStringAttribute(xstruct.Attribute("Code"));
                var structure = new RhythmicStructure(rs, code);
                structure.DetermineBehavior();
                this.Structures.Add(structure);
            }
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                XElement xmaterial = new XElement("RhythmicMaterial");
                xmaterial.Add(this.Header.GetXElement);

                xmaterial.Add(new XAttribute("RhythmicOrder", this.RhythmicOrder)); 

                //// Structures
                XElement xstructs = new XElement("Structures");
                foreach (RhythmicStructure structure in this.Structures) {
                    var xstruct = structure.GetXElement;
                    xstructs.Add(xstruct);
                }

                xmaterial.Add(xstructs);
                return xmaterial;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName => this.Header.FileName;

        /// <summary>
        /// Gets or sets the rhythmic order.
        /// </summary>
        /// <value>
        /// The rhythmic order.
        /// </value>
        public byte RhythmicOrder { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets the structures.
        /// </summary>
        /// <value>
        /// The structures.
        /// </value>
        public IList<RhythmicStructure> Structures { get; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => this.Structures.Count;

        #endregion

        #region Rhythmic producer
        /// <summary>
        /// Random Rhythmic Materials.
        /// </summary>
        /// <param name="number">Number of material sets.</param>
        /// <param name="rhyStructs">Rhythmic Structs.</param>
        /// <param name="numberOfStructs">Number Of Structs.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static Collection<RhythmicMaterial> RandomRhythmicMaterials(int number, Collection<RhythmicStructure> rhyStructs, int numberOfStructs) {  //// byte rhythmicOrder,
            var coll = new Collection<RhythmicMaterial>();
            for (var i = 0; i < number; i++) {
                var name = string.Format(CultureInfo.CurrentCulture, "Automat ({0}) {1}", i.ToString(CultureInfo.CurrentCulture.NumberFormat).PadLeft(3), SupportCommon.DateTimeIdentifier);
                var rhyMaterial = RandomRhythmicMaterial(name, rhyStructs, numberOfStructs); //// rhythmicOrder,
                coll.Add(rhyMaterial);
            }

            return coll;
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the object. </summary>
        /// <returns> Returns object. </returns>
        [UsedImplicitly]
        public object Clone() {
            var def = new RhythmicMaterial(this.Header) {               
            }; ////  Name = this.Name

            return def;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            ////s.Append("\t" + this.HarmonicOrder.ToString(CultureInfo.CurrentCulture));
            return s.ToString();
        }
        #endregion

        #region Rhythmic producer - private
        /// <summary>
        /// Random Rhythmic Material.
        /// </summary>
        /// <param name="name">Name of material.</param>
        /// <param name="rhythmicStructs">Rhythmic Structs.</param>
        /// <param name="numberOfStructs">Number Of Structs.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static RhythmicMaterial RandomRhythmicMaterial(string name, Collection<RhythmicStructure> rhythmicStructs, int numberOfStructs) { //// byte rhythmicOrder,
            Contract.Requires(name != null);
            if (rhythmicStructs == null) {
                return null;
            }

            var rhythmicMaterial = new RhythmicMaterial(MusicalHeader.GetDefaultMusicalHeader); //// Name = name
            for (var im = 0; im < numberOfStructs; im++) {
                var rs = ExtendCollection<RhythmicStructure>.GetRandomObject(rhythmicStructs);
                if (rs == null) {
                    continue;
                }

                var mrs = rs; //// Clone?
                rhythmicMaterial.Structures?.Add(mrs);
            }

            return rhythmicMaterial;
        }
        #endregion
    }
}
