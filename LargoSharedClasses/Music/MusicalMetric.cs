// <copyright file="MusicalMetric.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using System.Text;
using System.Xml.Linq;
using LargoSharedClasses.Abstract;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Metric.
    /// </summary>
    public sealed class MusicalMetric {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalMetric"/> class.
        /// </summary>
        /// <param name="givenBeat">The given beat.</param>
        /// <param name="givenBase">The given base.</param>
        public MusicalMetric(byte givenBeat, byte givenBase) {
            this.MetricBeat = givenBeat;
            this.MetricBase = givenBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalMetric"/> class.
        /// </summary>
        /// <param name="xmetric">The element of metric.</param>
        public MusicalMetric(XElement xmetric)
        {
            Contract.Requires(xmetric != null);
            if (xmetric == null)
            {
                return;
            }

            this.MetricBeat = XmlSupport.ReadByteAttribute(xmetric.Attribute("Beat"));
            this.MetricBase = XmlSupport.ReadByteAttribute(xmetric.Attribute("Base"));
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
                XElement xsystem = new XElement("Metric", null);
                xsystem.Add(new XAttribute("Beat", this.MetricBeat));
                xsystem.Add(new XAttribute("Base", this.MetricBase));

                return xsystem;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets metric beat.
        /// </summary>
        /// <value> Property description. </value>
        public byte MetricBeat { get; set; }

        /// <summary>
        /// Gets or sets metric base.
        /// </summary>
        /// <value> Property description. </value>
        public byte MetricBase { get; set; }

        /// <summary>  Gets musical tempo. </summary>
        /// <value> Property description. </value>
        public byte MetricGround => MusicalProperties.GetMetricGround(this.MetricBase);

        /// <summary>
        /// Gets the metric value.
        /// </summary>
        /// <value> Property description. </value>
        public string MetricValue => MusicalProperties.GetMetricValue(this.MetricBeat, this.MetricGround);

        #endregion

        #region Public methods
        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() {
            this.MetricBeat = 0;
            this.MetricBase = 0;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns value. </returns>
        public object Clone() {
            var m = new MusicalMetric(this.MetricBeat, this.MetricBase);
            return m;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendFormat("{0}/{1}", this.MetricBeat, this.MetricGround); //// Metric 

            return s.ToString();
        }
        #endregion
    }
}
