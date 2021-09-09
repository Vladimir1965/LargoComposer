// <copyright file="MusicalHeader.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using Abstract;
    using JetBrains.Annotations;
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Musical Header.
    /// </summary>
    public class MusicalHeader
    {
        #region Fields

        /// <summary>
        /// Musical metric.
        /// </summary>
        private MusicalMetric metric;

        /// <summary>
        /// Musical system.
        /// </summary>
        private MusicalSystem system;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalHeader"/> class.
        /// </summary>
        public MusicalHeader() {
            this.System = new MusicalSystem();  //// Needed !? (see MidiFileBridge!?)
            this.Metric = new MusicalMetric(1, 0);
            this.Tempo = DefaultValue.DefaultTempo;
            this.Number = 1;
            this.Origin = DateTime.Now;
            this.Changed = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalHeader" /> class.
        /// </summary>
        /// <param name="xheader">The xml header.</param>
        /// <param name="full">if set to <c>true</c> [full].</param>
        public MusicalHeader(XElement xheader, bool full)
            : this() {
            Contract.Requires(xheader != null);
            if (xheader == null) {
                return;
            }

            this.FileName = XmlSupport.ReadStringAttribute(xheader.Attribute("FileName"));
            this.Specification = XmlSupport.ReadStringAttribute(xheader.Attribute("Specification"));
            this.Number = XmlSupport.ReadIntegerAttribute(xheader.Attribute("Number"));
            this.Division = XmlSupport.ReadIntegerAttribute(xheader.Attribute("Division"));
            this.Tempo = XmlSupport.ReadByteAttribute(xheader.Attribute("Tempo"));
            this.NumberOfBars = XmlSupport.ReadIntegerAttribute(xheader.Attribute("NumberOfBars"));

            if (full) { //// For general use in ale music components
                var xorigin = xheader.Attribute("Origin");
                this.Origin = xorigin != null ? XmlSupport.ReadDateTimeAttribute(xorigin) : DateTime.Now;
                var xchanged = xheader.Attribute("Changed");
                this.Changed = xorigin != null ? XmlSupport.ReadDateTimeAttribute(xchanged) : DateTime.Now;

                this.System = new MusicalSystem(xheader.Element("System"));
                this.Metric = new MusicalMetric(xheader.Element("Metric"));
                this.NumberOfLines = XmlSupport.ReadIntegerAttribute(xheader.Attribute("NumberOfLines"));
                this.NumberOfMelodicLines = XmlSupport.ReadByteAttribute(xheader.Attribute("NumberOfMelodicLines"));
                this.NumberOfRhythmicLines = XmlSupport.ReadByteAttribute(xheader.Attribute("NumberOfRhythmicLines"));
            } else { //// The shortcut for import of harmonic streams
                this.System.HarmonicOrder = DefaultValue.HarmonicOrder;
                this.System.RhythmicOrder = XmlSupport.ReadByteAttribute(xheader.Attribute("RhythmicOrder"));
            }
        }

        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the default musical header.
        /// </summary>
        public static MusicalHeader GetDefaultMusicalHeader {
            get {
                var header = new MusicalHeader {
                    System = new MusicalSystem() {
                        RhythmicOrder = 12, //// 24
                        HarmonicOrder = DefaultValue.HarmonicOrder
                    },
                    Metric = new MusicalMetric(4, 2),
                    Division = 240
                };

                return header;
            }
        }
        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets the get x element.
        /// </summary>
        /// <value>
        /// The get x element.
        /// </value>
        public XElement GetXElement {
            get {
                XElement xheader = new XElement("Header");
                xheader.Add(new XAttribute("FileName", this.FileName ?? string.Empty));
                xheader.Add(new XAttribute("Specification", this.Specification ?? string.Empty));
                xheader.Add(new XAttribute("Number", this.Number));
                xheader.Add(new XAttribute("Origin", this.Origin ?? DateTime.Now));
                xheader.Add(new XAttribute("Changed", this.Changed ?? DateTime.Now));

                xheader.Add(this.System.GetXElement);
                xheader.Add(this.Metric.GetXElement);

                xheader.Add(new XAttribute("Division", this.Division));
                xheader.Add(new XAttribute("Tempo", this.Tempo));
                xheader.Add(new XAttribute("NumberOfBars", this.NumberOfBars));
                xheader.Add(new XAttribute("NumberOfLines", this.NumberOfLines));

                xheader.Add(new XAttribute("NumberOfMelodicLines", this.NumberOfMelodicLines));
                xheader.Add(new XAttribute("NumberOfRhythmicLines", this.NumberOfRhythmicLines));

                return xheader;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the file from where was the block imported.
        /// </summary>
        /// <value>
        /// The name of the original file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the number of the block in file.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the specification of the block (tempo, tonality).
        /// </summary>
        /// <value>
        /// The specification.
        /// </value>
        public string Specification { get; set; }

        /// <summary>
        /// Gets the full specification.
        /// </summary>
        /// <value>
        /// The full specification.
        /// </value>
        public string FullSpecification => string.Format(CultureInfo.InvariantCulture, "{0}/ {1}", this.Number, this.Specification.ClearSpecialChars().Trim());

        /// <summary>
        /// Gets the full name of the block.
        /// </summary>
        /// <value> Property description. </value>
        public string FullName => string.Format(CultureInfo.InvariantCulture, "{0}#{1}#{2}", this.FileName, this.Number, this.Specification.ClearSpecialChars().Trim());

        /// <summary>
        /// Gets the name of the two row.
        /// </summary>
        /// <value>
        /// The name of the two row.
        /// </value>
        public string TwoRowName => string.Format(CultureInfo.InvariantCulture, "{0}\n{1}", this.FileName, this.FullSpecification);

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>
        /// The origin.
        /// </value>
        public DateTime? Origin { get; set; }

        /// <summary>
        /// Gets or sets the changed.
        /// </summary>
        /// <value>
        /// The changed.
        /// </value>
        public DateTime? Changed { get; set; }

        /// <summary>
        /// Gets the rhythmic order.
        /// </summary>
        /// <value>
        /// The rhythmic order.
        /// </value>
        [UsedImplicitly]
        public int Timing => this.System.RhythmicOrder / 8;

        /// <summary>
        /// Gets or sets the metric.
        /// </summary>
        /// <value>
        /// The metric.
        /// </value>
        public MusicalMetric Metric {
            get {
                Contract.Ensures(Contract.Result<MusicalMetric>() != null);
                if (this.metric == null) {
                    throw new InvalidOperationException("Metric is null.");
                }

                return this.metric;
            }

            set => this.metric = value ?? throw new ArgumentException("Metric cannot be set null.", nameof(value));
        }

        /// <summary>
        /// Gets the metric string.
        /// </summary>
        /// <value>
        /// The metric string.
        /// </value>
        public string MetricString => this.Metric.ToString();

        /// <summary>
        /// Gets or sets the system.
        /// </summary>
        /// <value>
        /// The system.
        /// </value>
        public MusicalSystem System {
            get {
                Contract.Ensures(Contract.Result<MusicalSystem>() != null);
                if (this.system == null) {
                    throw new InvalidOperationException("System is null.");
                }

                return this.system;
            }

            set => this.system = value ?? throw new ArgumentException("System cannot be set null.", nameof(value));
        }

        /// <summary>
        /// Gets the system string.
        /// </summary>
        /// <value>
        /// The system string.
        /// </value>
        public string SystemString => this.System.ToString();

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        public int Division { get; set; }

        /// <summary>
        /// Gets or sets tempo.
        /// </summary>
        /// <value> Property description. </value>
        public int Tempo { get; set; }

        /// <summary>
        /// Gets or sets NumberOfBars.
        /// </summary>
        /// <value> General musical property.</value>
        public int NumberOfBars { get; set; }

        /// <summary>
        /// Gets or sets count of loaded parts.
        /// </summary>
        /// <value> General musical property.</value> 
        /// <returns> Returns value. </returns>
        public int NumberOfLines { get; set; }

        /// <summary>
        /// Gets or sets NumberOfBars.
        /// </summary>
        /// <value> General musical property.</value>
        public byte NumberOfMelodicLines { get; set; }

        /// <summary>
        /// Gets or sets NumberOfBars.
        /// </summary>
        /// <value> General musical property.</value>
        public byte NumberOfRhythmicLines { get; set; }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("System {0} Metric {1} Timing {2} Bars {3}", this.System, this.Metric, this.Timing, this.NumberOfBars);

            return s.ToString();
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the MusicalHeader object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var header = new MusicalHeader {
                Number = this.Number, //// 2019/02
                FileName = this.FileName,
                Specification = this.Specification,
                //// FilePath = this.FilePath,
                System = this.System,
                Metric = (MusicalMetric)this.Metric.Clone(),
                Division = this.Division,
                Tempo = this.Tempo,
                NumberOfBars = this.NumberOfBars,
                NumberOfLines = this.NumberOfLines,
                NumberOfMelodicLines = this.NumberOfMelodicLines,
                NumberOfRhythmicLines = this.NumberOfRhythmicLines 
            };

            return header;
        }
        #endregion
    }
}
