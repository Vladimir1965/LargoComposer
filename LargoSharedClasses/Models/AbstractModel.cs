// <copyright file="AbstractModel.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Musical Block Model.
    /// </summary>
    [Serializable]
    public class AbstractModel
    {
        #region Fields

        /// <summary>
        /// Source Musical Block.
        /// </summary>
        private MusicalBlock sourceMusicalBlock;

        /// <summary>
        /// Block Changes.
        /// </summary>
        [NonSerialized]
        private MusicalChanges blockChanges;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractModel"/> class.
        /// </summary>
        public AbstractModel()
        {
            this.BlockChanges = new MusicalChanges();
            this.Header = new MusicalHeader();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractModel" /> class.
        /// </summary>
        /// <param name="markBlockModel">The mark block model.</param>
        public AbstractModel(XElement markBlockModel)
        {
            Contract.Requires(markBlockModel != null);
            if (markBlockModel == null) {
                return;
            }

            this.Name = (string)markBlockModel.Attribute("Name");

            var xheader = markBlockModel.Element("Header");
            this.Header = new MusicalHeader(xheader, true);

            this.BlockChanges = new MusicalChanges(markBlockModel);
        }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement
        {
            get
            {
                XElement xmblockModel = new XElement(
                           "Model",
                           new XAttribute("Name", this.Name ?? "New model"),
                           new XAttribute("NumberOfBars", this.Header.NumberOfBars),
                           new XAttribute("Tempo", this.Header.Tempo));

                xmblockModel.Add(this.Header.System.GetXElement);

                XElement xchanges = this.BlockChanges.GetXElement;
                xmblockModel.Add(xchanges);

                return xmblockModel;
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets Source Musical Block.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalBlock SourceMusicalBlock
        {
            get
            {
                Contract.Ensures(Contract.Result<MusicalBlock>() != null);
                if (this.sourceMusicalBlock == null) {
                    throw new InvalidOperationException("Source musical block is null.");
                }

                return this.sourceMusicalBlock;
            }

            set => this.sourceMusicalBlock = value ?? throw new ArgumentException("Source musical block cannot be set null.", nameof(value));
        }

        /// <summary>
        /// Gets or sets the musical block changes.
        /// </summary>
        /// <value> Property description. </value>
        public MusicalChanges BlockChanges
        {
            get
            {
                Contract.Ensures(Contract.Result<MusicalChanges>() != null);
                if (this.blockChanges == null) {
                    throw new InvalidOperationException("Block changes are null.");
                }

                return this.blockChanges;
            }

            set => this.blockChanges = value ?? throw new ArgumentException("Block changes cannot be set null.", nameof(value));
        }

        #endregion

        #region Naming public properties
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        /// <value> Property description. </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Is Selected.
        /// </summary>
        /// <value> Property description. </value>
        public bool IsSelected { get; set; }

        #endregion

        #region Other public properties
        /// <summary>
        /// Gets the order value.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string OrderValue => MusicalProperties.GetOrderValue(this.Header.System.HarmonicOrder, this.Header.System.RhythmicOrder);

        /// <summary> Gets total duration of the block. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public long TotalDuration
        {
            get
            {
                long num = this.Header.System.RhythmicOrder * this.Header.NumberOfBars;
                return num;
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}
