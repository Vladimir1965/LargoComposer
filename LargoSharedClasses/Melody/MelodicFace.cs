// <copyright file="MelodicFace.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Melody
{
    /// <summary> A melodic face. </summary>
    public class MelodicFace
    {
        /// <summary> Initializes a new instance of the <see cref="MelodicFace" /> class. </summary>
        public MelodicFace() {
        }

        /// <summary> Initializes a new instance of the <see cref="MelodicFace" /> class. </summary>
        /// <param name="xface"> The element face. </param>
        public MelodicFace(XElement xface) {
            this.Name = (string)xface.Attribute("Name");
            this.Length = (int?)xface.Attribute("Length") ?? 0;
            this.StructuralCode = (string)xface.Attribute("Code");
            this.MelodicDirection = XmlSupport.ReadShortIntegerAttribute(xface.Attribute("Direction"));
            if (string.IsNullOrEmpty(this.Name)) {
                this.DetermineName();
            }
            
            //// this.Name = (string)xface.Attribute("Name");
            //// this.Length = (int?)xface.Attribute("Length") ?? 0;
            //// this.StructuralCode = (string)xface.Attribute("Code");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicFace" /> class.
        /// </summary>
        /// <param name="givenLength">Length of the given.</param>
        /// <param name="givenDirection">The given direction.</param>
        public MelodicFace(int givenLength, short givenDirection) {
            this.Length = givenLength;
            this.MelodicDirection = givenDirection;
            this.DetermineName();
        }

        /// <summary> Gets or sets the name. </summary>
        /// <value> The name. </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length { get; set; }

        /// <summary> Gets or sets the structural code. </summary>
        /// <value> The structural code. </value>
        public string StructuralCode { get; set; }

        /// <summary>
        /// Gets or sets the melodic direction.
        /// </summary>
        /// <value>
        /// The melodic direction.
        /// </value>
        public short MelodicDirection { get; set; }

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xe = new XElement(
                                "MelodicFace",
                                new XAttribute("Name", this.Name ?? string.Empty),
                                new XAttribute("Length", this.Length),
                                new XAttribute("Code", this.StructuralCode ?? string.Empty),
                                new XAttribute("Direction", this.MelodicDirection));
                return xe;
            }
        }
        #endregion

        /// <summary>
        /// Determines the name.
        /// </summary>
        public void DetermineName() {
            if (this.Length == 0) {
                this.Name = string.Empty;
                return;
            }

            StringBuilder sb = new StringBuilder();
            if (this.MelodicDirection == 0) {
                sb.Append("Equal");
            }
            else {
                sb.Append(this.MelodicDirection < 0 ? "Decreasing" : "Increasing");
            }

            this.Name = sb.ToString();
        }

        /// <summary> Value for place. </summary>
        /// <param name="percentPlace"> The percent place. </param>
        /// <returns> A number. </returns>
        [UsedImplicitly]
        public int ValueForPlace(byte percentPlace) {
            int value = 0;
            return value;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            return this.Name;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns value. </returns>
        public object Clone() {
            var tmc = new MelodicFace(this.Length, this.MelodicDirection) {
                Name = this.Name
            };

            return tmc;
        }
    }
}