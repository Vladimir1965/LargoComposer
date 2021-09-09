// <copyright file="MelodicElement.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using System.Diagnostics.Contracts;
using System.Text;
using System.Xml.Linq;

namespace ConductorPanels
{
    /// <summary>
    /// Melodic Element.
    /// </summary>
    public class MelodicElement
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicElement"/> class.
        /// </summary>
        public MelodicElement() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicElement"/> class.
        /// </summary>
        /// <param name="enterStep">The enter step.</param>
        /// <param name="innerStep">The inner step.</param>
        /// <param name="name">The name.</param>
        public MelodicElement(short enterStep, short innerStep, string name) {
            this.Name = name;
            this.EnterStep = enterStep;
            this.InnerStep = innerStep;
            this.IsEmpty = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicElement"/> class.
        /// </summary>
        /// <param name="isEmpty">if set to <c>true</c> [is empty].</param>
        /// <param name="name">The name.</param>
        public MelodicElement(bool isEmpty, string name) {
            this.Name = name;
            this.EnterStep = 0;
            this.InnerStep = 0;
            this.IsEmpty = isEmpty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MelodicElement"/> class.
        /// </summary>
        /// <param name="markCell">The mark cell.</param>
        public MelodicElement(XElement markCell) {
            Contract.Requires(markCell != null);
            if (markCell == null) {
                return;
            }

            this.Name = XmlSupport.ReadStringAttribute(markCell.Attribute("Name"));
            this.IsEmpty = XmlSupport.ReadBooleanAttribute(markCell.Attribute("IsEmpty"));
            this.EnterStep = XmlSupport.ReadShortIntegerAttribute(markCell.Attribute("EnterStep"));
            this.InnerStep = XmlSupport.ReadShortIntegerAttribute(markCell.Attribute("InnerStep"));
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xcell = new XElement("Element", null);
                xcell.Add(new XAttribute("Name", this.Name));
                xcell.Add(new XAttribute("IsEmpty", this.IsEmpty));
                if (!this.IsEmpty) {
                    xcell.Add(new XAttribute("EnterStep", this.EnterStep));
                    xcell.Add(new XAttribute("InnerStep", this.InnerStep));
                }

                return xcell;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>        
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Gets or sets the enter step.
        /// </summary>
        /// <value>
        /// The enter step.
        /// </value>
        public short EnterStep { get; set; }

        /// <summary>
        /// Gets or sets the inner step.
        /// </summary>
        /// <value>
        /// The inner step.
        /// </value>
        public short InnerStep { get; set; }
        #endregion

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(this.Name);
            //// s.AppendFormat("{0,4} - {1,4}", this.EnterStep, this.InnerStep);

            return s.ToString();
        }
        #endregion
    }
}
