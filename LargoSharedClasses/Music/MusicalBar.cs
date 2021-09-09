// <copyright file="MusicalBar.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Models;
using LargoSharedClasses.Orchestra;
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Bar.
    /// </summary>
    public class MusicalBar : IAbstractBar
    {
        #region Fields
        /// <summary> Cluster level for tick. </summary>
        private readonly Dictionary<byte, byte> clusterLevelForTick;

        /// <summary>
        /// List of harmonic clusters.
        /// </summary>
        private List<HarmonicCluster> harmonicClusters;

        /// <summary>
        /// The elements.
        /// </summary>
        private List<MusicalElement> elements;

        /// <summary>
        /// The count of tones
        /// </summary>
        private int countOfTones;

        /// <summary>
        /// The value of melodic ticks
        /// </summary>
        private int valueOfMelodicTicks;

        /// <summary>
        /// The value of rhythmic ticks
        /// </summary>
        private int valueOfRhythmicTicks;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalBar"/> class.
        /// </summary>
        public MusicalBar() {
            this.clusterLevelForTick = new Dictionary<byte, byte>();
            this.elements = new List<MusicalElement>();
            this.TempoNumber = 120;
            this.HarmonicBar = new HarmonicBar(0, 0);
        }

        /// <summary>
        /// Initializes a new instance of the MusicalBar class.  Serializable.
        /// </summary>
        /// <param name="barNumber">The bar number.</param>
        /// <param name="body">The body.</param>
        public MusicalBar(int barNumber, MusicalBody body)
            : this() {
            this.Body = body;
            this.BarNumber = barNumber;
        }

        /// <summary> Initializes a new instance of the <see cref="MusicalBar" /> class. </summary>
        /// <param name="givenBlock"> The given musical block. </param>
        /// <param name="givenCode">  The given code. </param>
        /// <param name="givenLength"> Length of the given. </param>
        public MusicalBar(MusicalBlock givenBlock, string givenCode, int givenLength) : this() {
            this.Body = givenBlock.Body;
            this.elements = new List<MusicalElement>();
            this.IsEmpty = string.IsNullOrEmpty(givenCode);
            this.SystemLength = givenLength;
            this.StructuralCode = givenCode;
            var chord = new HarmonicStructure(this.Body.Context.Header.System.HarmonicSystem, givenCode);  //// HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder)
            this.HarmonicBar.SetStructure(chord);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalBar" /> class.
        /// </summary>
        /// <param name="givenBlock">The given musical block.</param>
        /// <param name="markBar">The mark bar.</param>
        /// <exception cref="ContractException">Thrown when a method Contract has been broken.</exception>
        public MusicalBar(MusicalBlock givenBlock, XElement markBar) {
            Contract.Requires(markBar != null);
            if (markBar == null) {
                return;
            }

            this.Body = givenBlock.Body;
            this.elements = new List<MusicalElement>();

            var xcell = markBar.Element("Chord");
            this.SystemLength = XmlSupport.ReadIntegerAttribute(markBar.Attribute("Length"));
            this.IsEmpty = XmlSupport.ReadBooleanAttribute(markBar.Attribute("IsEmpty"));

            if (xcell != null) {
                this.StructuralCode = XmlSupport.ReadStringAttribute(xcell.Attribute("Code"));
            }

            this.HarmonicStructure = new HarmonicStructure(givenBlock.Header.System.HarmonicSystem, this.StructuralCode);  //// HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder)
                                                                                                                           //// this.HarmonicPotential = XmlSupport.ReadByteAttribute(markCell.Attribute("HarmonicPotential"));
            var xelements = markBar.Elements("Element");
            foreach (var xe in xelements) {
                MusicalElement element = new MusicalElement(xe, this.Header);
                //// int barNumber = XmlSupport.ReadIntegerAttribute(xe.Attribute("Bar"));
                int lineIndex = XmlSupport.ReadIntegerAttribute(xe.Attribute("LineIndex"));
                var line = givenBlock.ContentLines[lineIndex];
                element.Bar = this;
                element.Line = line;
                this.Elements.Add(element);
            }
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement xbar = new XElement("Bar");
                xbar.Add(new XAttribute("Number", this.BarNumber));

                //// Lines
                XElement xelements = new XElement("Elements");
                ////  Musical Element to XML
                foreach (MusicalElement element in this.Elements.Where(element => element != null)) {
                    var xelement = element.GetXElement;
                    xelements.Add(xelement);
                }

                xbar.Add(xelements);
                return xbar;
            }
        }
        #endregion

        #region Properties - Harmonic Status
        /// <summary>
        /// Gets Harmonic Status.
        /// </summary>
        /// <value> Property description. </value>
        public HarmonicChange HarmonicStatus { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has harmonic motive.
        /// </summary>
        /// <value>
        /// Is <c>true</c> if this instance has harmonic motive; otherwise, <c>false</c>.
        /// </value>
        public bool HasHarmonicMotive {
            get {
                if (this.HarmonicStatus == null) {
                    return false;
                }

                var hm = this.HarmonicStatus.HarmonicMotive;
                return hm?.HarmonicStream.HarmonicBars != null && hm.HarmonicStream.HarmonicBars.Any();
            }
        }

        /// <summary>
        /// Gets or sets current harmonical bar.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        [XmlIgnore]
        public HarmonicBar HarmonicBar { get; set; }

        /// <summary> Gets or sets the harmonic structure. </summary>
        /// <value> The harmonic structure. </value>
        public HarmonicStructure HarmonicStructure {
            get => this.HarmonicBar?.HarmonicStructures?.FirstOrDefault();

            set => this.HarmonicBar.SetStructure(value);
        }

        #endregion

        #region Properties - Tempo Status
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public int TempoNumber { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public MusicalBody Body { get; set; }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public MusicalHeader Header => this.Body.Context.Header;

        /// <summary> Gets or sets the zero-based index of the bar. </summary>
        /// <value> The bar index. </value>
        public int BarIndex { get; set; }

        /// <summary> Gets or sets number o bar. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public int BarNumber {
            get => this.BarIndex + 1;

            set => this.BarIndex = value - 1;
        }

        /// <summary> Gets or sets the system length. </summary>
        /// <value> The length of the system. </value>
        public int SystemLength { get; set; }

        /// <summary> Gets or sets the structural code. </summary>
        /// <value> The structural code. </value>
        public string StructuralCode { get; set; }
        //// public byte HarmonicPotential { get; set; }

        /// <summary> Gets or sets a value indicating whether this object is empty. </summary>
        /// <value> True if this object is empty, false if not. </value>
        public bool IsEmpty {
            get => (this.HarmonicBar == null) || this.HarmonicBar.IsEmpty;

            set {
                //// this.Status.HarmonicBar.IsEmpty = value;
            }
        }

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>
        /// The elements.
        /// </value>
        public IList<MusicalElement> Elements => this.elements;

        /// <summary>
        /// Gets the rhythmic order divisor.
        /// </summary>
        /// <value>
        /// The rhythmic order divisor.
        /// </value>
        public byte RhythmicOrderDivisor {
            get {
                var factor = 0; //// rstruct.GSystem.Order;
                foreach (var elem in this.elements) {
                    if (elem.Status.RhythmicStructure == null || elem.Status.RhythmicStructure.Level == 0) {
                        continue;
                    }

                    var header = this.Body.Context.Header;
                    var rhythmicShape = new RhythmicShape(header.System.RhythmicOrder, elem.Status.RhythmicStructure);
                    var listDistances = rhythmicShape.BitDistances;
                    foreach (byte d in listDistances) {
                        if (factor == 0) {
                            factor = d;
                        }
                        else {
                            factor = (int)MathSupport.GreatestCommonDivisor(factor, d);
                        }
                    }
                }

                return (byte)(factor == 0 ? 1 : factor);
            }
        }

        /// <summary> Gets or sets time tag. </summary>
        /// <value> In ticks according to order of rhythmical system.. </value>
        [XmlIgnore]
        public int TimePoint { get; set; }

        /// <summary> Gets or sets common rhythmical shape. </summary>
        /// <value> Property description. </value>
        public RhythmicShape CommonRhythmicShape { get; set; }

        #endregion

        #region Internal Properties
        /// <summary> Gets list of harmonic clusters. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public Collection<HarmonicCluster> HarmonicClusters {
            get {
                Contract.Ensures(Contract.Result<Collection<HarmonicCluster>>() != null);
                if (this.harmonicClusters == null) {
                    this.harmonicClusters = new List<HarmonicCluster>();
                }

                //// 2019/09 if (this.harmonicClusters == null) {  throw new InvalidOperationException("List of clusters is null.");  } 
                return new Collection<HarmonicCluster>(this.harmonicClusters);
            }
        }

        /// <summary>
        /// Gets or sets the previous bar.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        [XmlIgnore]
        public MusicalBar PreviousBar { get; set; }

        /// <summary> Gets or sets the previous bar. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        [XmlIgnore]
        public MusicalBar NextBar { get; set; }

        #endregion

        #region Tone Properties
        /// <summary>
        /// Gets the tones.
        /// </summary>
        /// <value>
        /// The tones.
        /// </value>
        public IList<IMusicalTone> Tones {
            get {
                var ts = new List<IMusicalTone>();
                foreach (var elem in this.elements) {
                    ts.AddRange(elem.Tones);
                }

                return ts;
            }
        }

        /// <summary>
        /// Gets the count of tones.
        /// </summary>
        /// <value>
        /// The count of tones.
        /// </value>
        [UsedImplicitly]
        public int CountOfTones {
            get {
                if (this.countOfTones > 0) {
                    return this.countOfTones;
                }

                this.countOfTones = this.Tones.Count;
                return this.countOfTones;
            }
        }

        /// <summary>
        /// Gets the melodic tones.
        /// </summary>
        /// <value>
        /// The melodic tones.
        /// </value>
        public IList<MusicalStrike> MelodicTones {
            get {
                var ts = new List<MusicalStrike>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var elem in this.elements) {
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var t in elem.Tones) {
                        if (t is MusicalTone mt && mt.IsTrueTone) {
                            ts.Add(mt);
                        }
                    }
                }

                return ts;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has fixed sounding tones.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has fixed sounding tones; otherwise, <c>false</c>.
        /// </value>
        public bool HasFixedSoundingTones {
            get {
                bool hasTones = false;
                foreach (var barElement in this.Elements) {
                    if (barElement.Status.LocalPurpose == LinePurpose.Fixed && barElement.Tones.CountOfSoundingTones > 0) {
                        hasTones = true;
                        break;
                    }
                }

                return hasTones;
            }
        }

        #endregion

        #region Relation to harmonic motive
        /// <summary> Gets or sets a value indicating whether is block nested. </summary>
        /// <value> Is bar nested from other block. </value>
        public int NumberInHarmonicMotive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [last in harmonic motive].
        /// </summary>
        /// <value>
        /// <c>True</c> if [is last in harmonic motive]; otherwise, <c>false</c>.
        /// </value>
        public bool IsLastInHarmonicMotive { get; set; }
        #endregion

        #region ToString properties
        /// <summary> Gets Write particular structures to string. </summary>
        /// <returns> Returns value.</returns>
        /// <value> General musical property.</value>
        public string ChordsToString => this.HarmonicBar.ChordsToString;

        /// <summary> Gets Write particular structures to string. </summary>
        /// <returns> Returns value.</returns>
        /// <value> General musical property.</value>
        public string PureChordsToString => this.HarmonicBar.PureChordsToString;

        /// <summary> Gets String representation of clusters. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public string ClustersToString {
            get {
                if (this.CommonRhythmicShape == null) {
                    return string.Empty;
                }

                var rhyShape = this.CommonRhythmicShape;
                var s = new StringBuilder();
                //// s.Append("Clusters:"); //// +"\n"
                var shapeLevel = this.CommonRhythmicShape.Level;
                for (byte level = 0; level < shapeLevel; level++) {
                    var hc = this.harmonicClusters != null && level < this.HarmonicClusters.Count ? this.HarmonicClusterAtLevel(level) : null;
                    if (hc == null) {
                        continue;
                    }

                    if (level == 0) {
                        s.AppendFormat(CultureInfo.CurrentCulture, "{0,4}:\n", this.BarNumber.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
                    }
                    else {
                        s.Append(" ");
                    }

                    //// Cluster 
                    s.AppendFormat(CultureInfo.CurrentCulture, "{0,16}", hc); //// {0,12}
                    s.Append(string.Format(CultureInfo.CurrentCulture, "({0} ticks) ", rhyShape.DistanceAtLevel(level).ToString("D", CultureInfo.CurrentCulture.NumberFormat)));
                    s.Append("\t\t      ");
                    if (hc.HarmonicStructure != null) {
                        s.AppendFormat(CultureInfo.CurrentCulture, hc.HarmonicStructure.ToneSchema.PadRight(15), CultureInfo.CurrentCulture);

                        if (hc.HarmonicStructure.HarmonicModality != null) {
                            s.Append(string.Format(CultureInfo.CurrentCulture, " ({0})", hc.HarmonicStructure.HarmonicModality.ToneSchema)); //// ????
                        }
                    }

                    s.Append("\n");
                }

                return s.ToString().Trim();
            }
        }
        #endregion

        /// <summary> Gets a list of identifiers. </summary>
        /// <value> A list of identifiers. </value>
        public IList<KeyValuePair> Identifiers {
            get {
                var items = new List<KeyValuePair>();
                string s;
                var item = new KeyValuePair("Bar number", this.BarNumber.ToString());
                items.Add(item);

                item = new KeyValuePair("Time point", this.TimePoint.ToString());
                items.Add(item);

                item = new KeyValuePair("Harmonic modality", this.HarmonicBar?.HarmonicModality?.ToString());
                items.Add(item);

                item = new KeyValuePair("Structures", this.HarmonicBar?.SimpleStructuralOutline);
                items.Add(item);

                item = new KeyValuePair("Structures in detail", this.HarmonicBar?.StructuralOutline);
                items.Add(item);

                var rhythmicOrder = this.Header?.System?.RhythmicOrder; //// Body.Context.Header..., HarmonicBar?.Header?
                if (rhythmicOrder != null) {
                    var barBitRange = new BitRange((byte)rhythmicOrder, 0, (byte)rhythmicOrder);
                    var harmonicStructure = this.HarmonicBar.PrevailingHarmonicStructure(barBitRange, out var simpleHarmony);
                    item = new KeyValuePair("Prevailing structure", harmonicStructure?.ToneSchema);
                    items.Add(item);

                    item = new KeyValuePair("Simple harmony", simpleHarmony.ToString());
                    items.Add(item);

                    s = string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,6:F2} ", this.HarmonicBar?.MeanRhythmicMobility);
                    item = new KeyValuePair("Mean rhythmic mobility", s);
                    items.Add(item);

                    s = string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,6:F2} ", this.HarmonicBar?.MeanRhythmicTension);
                    item = new KeyValuePair("Mean rhythmic tension", s);
                    items.Add(item);
                }

                s = string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,6:F2} ", this.HarmonicBar?.MeanConsonance);
                item = new KeyValuePair("Mean consonance", s);
                items.Add(item);

                s = string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,6:F2} ", this.HarmonicBar?.MeanContinuity);
                item = new KeyValuePair("Mean continuity", s);
                items.Add(item);

                s = string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,6:F2} ", this.HarmonicBar?.MeanImpulse);
                item = new KeyValuePair("Mean impulse", s);
                items.Add(item);

                s = string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,6:F2} ", this.HarmonicBar?.MeanPotential);
                item = new KeyValuePair("Mean potential", s);
                items.Add(item);

                item = new KeyValuePair("Tempo number", this.TempoNumber.ToString());
                items.Add(item);

                return items;
            }
        }

        #region Public methods  

        /// <summary>
        /// Gets the harmonic rhythm.
        /// </summary>
        /// <param name="hasToBeStructure">if set to <c>true</c> [has to be structure].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public RhythmicStructure GetHarmonicRhythm(bool hasToBeStructure) {
            var rstruct = this.HarmonicBar?.RhythmicStructure;
            RhythmicStructure rhyStructure; //// = null;
            if (hasToBeStructure && rstruct != null) {
                rhyStructure = rstruct;
            }
            else {
                var rshape = this.DetermineCommonRhythmicShape(true);
                rhyStructure = new RhythmicStructure(rshape.Order, rshape);
            }

            return rhyStructure;
        }

        /// <summary>
        /// Elements the of line.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        /// <returns>Returns value.</returns>
        public MusicalElement ElementOfLine(Guid lineIdent) {
            var element = (from v in this.Elements
                           where v.Line.LineIdent == lineIdent
                           select v).FirstOrDefault();

            //// var selectedList = (from element in list orderby element.Point.BarNumber
            //// select element).ToList();
            return element;
        }

        /// <summary>
        /// Counts the of ticks.
        /// </summary>
        /// <param name="breakForRests">if set to <c>true</c> [break for rests].</param>
        /// <param name="lineType">Type of the line.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public int CountOfSelectedTicks(bool breakForRests, MusicalLineType lineType) {
            var rhythmicOrder = this.Body.Context.Header.System.RhythmicOrder;
            var bitArray = new BitArray(rhythmicOrder);
            //// for (byte tick = 0; tick < rhythmicOrder; tick++) {
            foreach (var elem in this.Elements) {
                if (!elem.MusicalLine.IsSelected || elem.MusicalLine.FirstStatus.LineType != lineType) {
                    continue;
                }

                if (elem.Status.RhythmicStructure == null) {
                    continue;
                }

                //// var binStruct = elem.Status.RhythmicStructure.BinaryStructure(breakForRests);
                //// bitArray = bitArray.Or(binStruct.BitArray);
                var binStructArray = elem.Status.RhythmicStructure.BitArray(breakForRests);
                bitArray = bitArray.Or(binStructArray);
            }

            byte sum = 0;
            foreach (var bit in bitArray) {
                sum += (byte)(((bool)bit) ? 1 : 0);
            }

            return sum;
        }

        /// <summary>
        /// Resets the values of ticks.
        /// </summary>
        public void ResetValuesOfTicks() {
            this.valueOfMelodicTicks = 0;
            this.valueOfRhythmicTicks = 0;
        }

#endregion

#region Apply Changes
/// <summary>
/// Applies the harmonic change.
/// </summary>
/// <param name="harmonicChange">The harmonic change.</param>
public void ApplyHarmonicChange(HarmonicChange harmonicChange) {
            //// Contract.Requires(harmonicChange != null);
            if (harmonicChange == null) {
                return;
            }

            this.HarmonicStatus = harmonicChange;
        }

        /// <summary>
        /// Applies the tempo change.
        /// </summary>
        /// <param name="tempoChange">The tempo change.</param>
        public void ApplyTempoChange(TempoChange tempoChange) {
            //// Contract.Requires(harmonicChange != null);
            if (tempoChange == null) {
                return;
            }

            this.TempoNumber = tempoChange.TempoNumber;
        }
        #endregion

        /* Clone bar - Not needed, not used.
/// <summary> Makes a deep copy of the MusicalBarStatus object. </summary>
/// <returns> Returns object. </returns>
public object Clone() {
    var tmc = new BarStatus(this.BarNumber) {
        HarmonicStatus = this.HarmonicStatus,
        TempoNumber = this.TempoNumber
    };

    if (this.HarmonicBar != null) {
        tmc.HarmonicBar = (HarmonicBar)this.HarmonicBar.Clone();
    }

    return tmc;
} */

        #region Public methods
        /// <summary>
        /// Sets the T harmonic motive bar.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetHarmonicBar(HarmonicBar value) {
            if (value == null) {
                throw new ArgumentException("Empty harmonic bar!");
            }

            if (value.Header == null) {
                value.Header = this.Body.Context.Header;
            }

            this.HarmonicBar = (HarmonicBar)value.Clone(); //// value
            if (this.HarmonicBar.IsEmpty) {
                this.HarmonicBar.CheckStructures();
            }

            var rhythmicOrder = this.HarmonicBar.Header.System.RhythmicOrder;

            var metre = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, rhythmicOrder);
            var metricStructuralCode = this.HarmonicBar.GetBarMetricCode;
            this.HarmonicBar.RhythmicShape = RhythmicShape.GetNewRhythmicShape(metre, metricStructuralCode);

            //// Have to be clone of bar above ?!?!?!?
            //// else { this.HarmonicBar.RegenerateStructures();  } 
        }

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        /// <returns> Returns value. </returns>
        public MusicalElement GetElement(Guid lineIdent) {
            var element = (from elem in this.elements where elem.Line.LineIdent == lineIdent select elem).FirstOrDefault();
            return element;
        }

        /// <summary> Makes a deep copy of the MusicalPitch object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            var bar = new MusicalBar(this.BarNumber, this.Body) {
                TimePoint = this.TimePoint,
                CommonRhythmicShape = this.CommonRhythmicShape
            };

            foreach (var element in this.Elements) {
                var newElement = (MusicalElement)element.Clone();
                newElement.Bar = bar;
                bar.Elements.Add(newElement);
            }

            bar.MakeHarmonicClusters();

            return bar;
        }

        /// <summary>
        /// Sets the modality to structures.
        /// </summary>
        /// <param name="minModalityLevel">The minimum modality level.</param>
        public void SetModalityToStructures(byte minModalityLevel) {
            foreach (var harStr in this.HarmonicBar.HarmonicStructures.Where(harStr => this.Body.Bars != null && this.Body != null)) {
                harStr.HarmonicModality = this.HarmonicModalityFromStructures(minModalityLevel);
            }
        }

        /// <summary>
        /// Returns harmonic modality from all harmonic structures.
        /// </summary>
        /// <param name="minModalityLevel">The minimum modality level.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public HarmonicModality HarmonicModalityFromStructures(byte minModalityLevel) {
            if (this.HarmonicBar?.HarmonicStructures == null || this.HarmonicBar?.HarmonicStructures?.Count <= 0) {
                return null;
            }

            var bits = this.BitsOccupiedByHarmony(minModalityLevel);

            var firstStructure = this.HarmonicBar.HarmonicStructures[0];
            var modality = firstStructure != null ? HarmonicModality.GetNewHarmonicModality(firstStructure.HarmonicSystem, bits) : null;

            return modality;
        }

        /// <summary>
        /// Sets the elements.
        /// </summary>
        /// <param name="givenElements">The given elements.</param>
        public void SetElements(IList<MusicalElement> givenElements) {
            this.elements = (List<MusicalElement>)givenElements;
        }

        /// <summary>
        /// Writes musical block to Scores - Orchestration.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public OrchestraStrip OrchestraStrip() {
            var strip = new OrchestraStrip();

            //// Cycle through all the sequence 
            //// byte number = 0;
            this.elements.ForEach(mt => {
                if (mt != null) { //// && !mt.IsEmpty && mt.IsSelected
                    var barTones = mt.SingleMelodicTones();
                    if (barTones.Count > 0) {
                        //// MelodicFunction melodicType = barTones.DetermineMelodicType(mbar.HarmonicBar);
                        //// MusicalLoudness loudness = barTones.MeanLoudness;
                        var mstatus = mt.Status;
                        if (mstatus == null) {
                            return;
                        }

                        //// var instrument = new GeneralChannel(InstrumentGenus.Melodical, mstatus.Instrument, mstatus.Channel);
                        if (!mstatus.Instrument.IsEmpty) { //// 2019/10
                            var line = new OrchestraVoice(barTones.MeanOctave, mstatus.Instrument);
                            strip.OrchestraVoices.Add(line);
                        }
                        //// number++;
                    }
                    else {
                        var barStrikes = mt.Tones;
                        if (barStrikes.Count > 0) {
                            //// MelodicFunction melodicType = barTones.DetermineMelodicType(mbar.HarmonicBar);
                            //// MusicalLoudness loudness = barTones.MeanLoudness;
                            var mstatus = mt.Status;
                            if (mstatus == null) {
                                return;
                            }

                            //// var instrument = new GeneralChannel(InstrumentGenus.Melodical, mstatus.Instrument, mstatus.Channel);
                            if (!mstatus.Instrument.IsEmpty) { //// 2019/10
                                var line = new OrchestraVoice(mstatus.Instrument);
                                strip.OrchestraVoices.Add(line);
                            }
                            //// number++;
                        }
                    }
                }
            });

            strip.RecomputeProperties();
            return strip;
        }

        /// <summary>
        /// Swaps the harmonic with.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        public void SwapHarmonyWith(MusicalBar givenBar) {
            Contract.Requires(givenBar != null);

            var harmonicBar = givenBar.HarmonicBar;
            var newHarmonicBar = (HarmonicBar)harmonicBar.Clone();
            newHarmonicBar.BarNumber = givenBar.BarNumber;
            givenBar.SetHarmonicBar(newHarmonicBar);
            this.SetHarmonicBar(harmonicBar);
        }

        /// <summary>
        /// Melodic tones around.
        /// </summary>
        /// <param name="barNumbersBack">The bar numbers back.</param>
        /// <param name="barNumbersForward">The bar numbers forward.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public IEnumerable<MusicalStrike> MelodicTonesAround(int barNumbersBack, int barNumbersForward) {
            var ts = new List<MusicalStrike>();
            foreach (var elem in this.Elements) {
                ts.AddRange(elem.MelodicTonesAround(barNumbersBack, barNumbersForward));
            }

            return ts;
        }

        /// <summary>
        /// Surroundings the bars.
        /// </summary>
        /// <param name="barsBefore">The bars before.</param>
        /// <param name="barsAfter">The bars after.</param>
        /// <returns> Returns value. </returns>
        public IList<MusicalBar> SurroundingBars(byte barsBefore, byte barsAfter) {
            var musicalBars = new List<MusicalBar>();
            var currentBar = this;
            musicalBars.Add(currentBar);

            for (byte i = 0; i <= barsBefore; i++) {
                if (currentBar == null) {
                    break;
                }

                currentBar = currentBar.PreviousBar;
                if (currentBar != null) {
                    musicalBars.Add(currentBar);
                }
            }

            currentBar = this;
            for (byte i = 0; i <= barsAfter; i++) {
                if (currentBar == null) {
                    break;
                }

                currentBar = currentBar.NextBar;
                if (currentBar != null) {
                    musicalBars.Add(currentBar);
                }
            }

            return musicalBars;
        }

        /// <summary>
        /// Compose rhythm of lines in given bar.
        /// </summary>
        public void FillLinesWithRhythm() {
            //// Ordering is not needed here, motives are loaded from status //// All - not only (&& mt.Status != null)
            if (this.Body.Context.Settings.SettingsProgram.ParallelMode) {
                Parallel.ForEach(
                    this.Elements,
                    mt => {
                        if (mt == null) {
                            return;
                        }

                        mt.FillWithRhythm();
                    });
            }
            else {
                this.elements.ForEach(mt => {
                    if (mt == null) {
                        return;
                    }

                    mt.FillWithRhythm();
                });
            }
        }

        /// <summary>
        /// Transfers the status to tones.
        /// </summary>
        public void SendStatusToTones() {
            foreach (var element in this.Elements) {
                if (element?.Status == null) {
                    continue;
                }

                var status = element.Status;
                foreach (var mt in element.Tones) {
                    if (!(mt is MusicalStrike mtone)) {
                        continue;
                    }

                    mtone.InstrumentNumber = status.Instrument.Number;
                    mtone.Staff = status.Staff;
                    mtone.Loudness = status.Loudness;
                }
            }
        }

        /// <summary>
        /// Eliminates the parallel melodies.
        /// Correct melodic type - eliminate parallel individualized melodic voices.
        /// </summary>
        public void EliminateParallelMelodies() {
            var ems = this.Elements;
            var hasMelody = false;
            foreach (var me in ems) {
                if (me == null || me.Line.Purpose != LinePurpose.Composed || me.Status == null) {
                    continue;
                }

                var mstatus = me.Status;
                if (mstatus == null) {
                    continue;
                }

                var melFilling = mstatus.MelodicFunction == MelodicFunction.MelodicFilling;
                var melMotion = mstatus.MelodicFunction == MelodicFunction.MelodicMotion;

                if (hasMelody) {
                    if (melFilling) {
                        mstatus.MelodicFunction = MelodicFunction.HarmonicFilling;
                    }
                    else {
                        if (melMotion) {
                            mstatus.MelodicFunction = MelodicFunction.HarmonicMotion;
                        }
                    }
                }
                else {
                    hasMelody = melFilling || melMotion;
                }
            }
        }
        #endregion

        #region Planned Tones
        /// <summary>
        /// Prepare Planned Tones.
        /// </summary>
        public void PreparePlannedTones() {
            //// var hs = this.Body.Context.Header.System.HarmonicSystem;
            var hm = this.HarmonicModalityFromStructures(MusicalSettings.Singleton.SettingsAnalysis.MinimalModalityLevel);
            ////HarmonicModality hm = MusicalTrackEngine.DetermineHarmonicModality(musicalBar, null, HarmonicModalizationType.Consecutive); 
            if (hm == null || hm.Level <= 1) { //// 2016/07
                return;
            }

            foreach (var elem in this.Elements) {
                if (elem == null || elem.Status?.LocalPurpose != LinePurpose.Composed) {
                    continue;
                }

                //// bool figural = mline.CurrentMelodicStructure != null; //// mp.Status.HasMelodicMotive
                //// if ((this.Status == null) || !figural) {  continue; } 

                var tones = elem.Tones;
                if (tones != null) {
                    //// bool fromMelodicStructure = false;
                    //// if (fromMelodicStructure) { elem.ProjectIntoPlannedTones(hs, hm, tones); }
                    //// else { elem.Status.PlannedTones = tones; }
                }
            }
        }
        #endregion

        #region Public Clusters
        /// <summary>
        /// Makes common rhythmical shape, i.e. elementary rhythm from all parts.
        /// </summary>
        /// <returns> Returns value. </returns>
        public bool MakeCommonRhythmicShape() {
            this.CommonRhythmicShape = this.DetermineCommonRhythmicShape(true); //// , this.BarNumber
            if (this.CommonRhythmicShape == null) {
                return false;
            }

            this.AssignClusterLevelsForTicks();
            return true;
        }

        /// <summary>
        ///  Add harmonic cluster to the list of clusters. 
        /// </summary>
        /// <param name="givenCluster">Given Cluster.</param>
        public void AddHarmonicCluster(HarmonicCluster givenCluster) {
            Contract.Requires(givenCluster != null);

            this.HarmonicClusters.Add(givenCluster);
            givenCluster.BarNumber = this.BarNumber;
        }

        /// <summary>
        /// Makes harmonic clusters for this bar.
        /// </summary>
        public void MakeHarmonicClusters() { //// byte rhythmicOrder, IEnumerable<MusicalLine> givenMusicalTracks
            //// Contract.Requires(givenMusicalTracks != null);
            //// var musicalLines = givenMusicalLines as IList<MusicalLine> ?? givenMusicalTracks.ToList();

            if (!this.MakeCommonRhythmicShape()) { //// this.Number, true //// rhythmicOrder, musicalLines
                return;
            }

            this.harmonicClusters = new List<HarmonicCluster>();
            if (this.CommonRhythmicShape == null) {
                return;
            }

            var level = this.CommonRhythmicShape.Level;
            for (byte lev = 0; lev < level; lev++) {
                if (this.CommonRhythmicShape == null) {
                    continue;
                }

                var hc = this.MakeHarmonicClusterAtLevel(lev);
                if (hc == null) {
                    continue;
                }

                this.AddHarmonicCluster(hc);
            }
        }

        /// <summary> Re-compute properties of harmonic clusters in this bar. </summary>
        /// <param name="range">Given range.</param>
        public void RecomputeHarmonicClusters(BitRange range) {
            Contract.Requires(this.CommonRhythmicShape != null);
            if (range == null) {
                return;
            }

            //// byte lastLevel = 127;
            var rhyShape = this.CommonRhythmicShape;

            var levelFrom = rhyShape.LevelOfBit(range.BitFrom);
            var levelTo = rhyShape.LevelOfBit(range.BitTo);
            for (var level = levelFrom; level <= levelTo; level++) {
                if (level >= this.HarmonicClusters.Count) {
                    break;
                }

                var harCluster = this.HarmonicClusters[level];
                harCluster?.Recompute();
            }
        }

        /// <summary> Returns harmonic clusters at given place. </summary>
        /// <param name="givenLevel">Requested level.</param>
        /// <returns> Returns value. </returns>
        public HarmonicCluster HarmonicClusterAtLevel(byte givenLevel) {
            Contract.Requires(givenLevel < this.HarmonicClusters.Count);
            if (this.HarmonicClusters == null || this.HarmonicClusters.Count == 0 || givenLevel >= this.HarmonicClusters.Count) {
                return null;
            }

            var harCluster = this.HarmonicClusters[givenLevel];
            return harCluster;
        }

        /// <summary> Returns harmonic clusters at given tick. </summary>
        /// <param name="givenTick">Requested tick.</param>
        /// <returns> Returns value. </returns>
        public HarmonicCluster HarmonicClusterAtTick(byte givenTick) {
            var rhyShape = this.CommonRhythmicShape;
            if (rhyShape == null) {
                return null;
            }

            var level = this.clusterLevelForTick[givenTick];
            //// this.CommonRhythmicShape.LevelOfBit(givenTick);
            if (level >= this.HarmonicClusters.Count) {
                return null;
            }

            var harCluster = this.HarmonicClusters[level];
            return harCluster;
        }
        #endregion

        #region Common Rhythmic Shape
        /// <summary>
        /// Common Rhythmic Shape.
        /// </summary>
        /// <param name="considerLineStatus">if set to <c>true</c> [consider line status].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        /// <exception cref="InvalidOperationException">Incompatible rhythmical orders.</exception>
        /// <exception cref="System.InvalidOperationException">Incompatible rhythmical orders.</exception>
        public RhythmicShape DetermineCommonRhythmicShape(bool considerLineStatus) {
            var rhythmicOrder = this.Body.Context.Header.System.RhythmicOrder;
            //// in variable "bits" is assembled the common rhythmic shape
            var bits = new BitArray(rhythmicOrder);

            bits.SetAll(false);
            //// .Where(mt => mt != null && mt.IsSelected)
            foreach (var mt in this.Elements.Where(mt => mt.Status?.LineType == MusicalLineType.Melodic)) {
                if (considerLineStatus) {
                    if (mt.Status.RhythmicStructure == null) {
                        var tones = mt.Tones; ////Track.MusicalTonesInBar(givenBarNumber);
                        if (tones != null) {
                            var rstruct = new RhythmicStructure(rhythmicOrder, tones);
                            mt.Status.RhythmicStructure = rstruct;
                        }
                    }

                    if (mt.Status.RhythmicStructure == null) {
                        continue;
                    }
                }

                var binStr = mt.Status.RhythmicStructure.BinaryStructure(false); //// 2015/01
                if (binStr == null) {
                    continue;
                }

                if (binStr.BitArray.Length == bits.Length) { //// 2012/06 //// null test not needed (resharper)
                    bits = bits.Or(binStr.BitArray);
                }
                else {
                    throw new InvalidOperationException("Incompatible rhythmical orders.");
                }
            }

            var commonRhythmicShape = RhythmicShape.GetNewRhythmicShape(this.Body.Context.Header.System.RhythmicSystem, bits);
            return commonRhythmicShape;
        }

        /// <summary>
        /// Common Rhythmic Structure.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public RhythmicStructure CommonRhythmicStructure() {
            Contract.Requires(this.Elements != null);

            var rhythmicOrder = this.Body.Context.Header.System.RhythmicOrder;
            //// in variable "elements" is assembled the common rhythmic shape
            var list = new short[rhythmicOrder];

            //// each of elements = 0
            foreach (var mt in this.Elements.Where(mt => mt.Status.LineType == MusicalLineType.Melodic)) {
                var tones = mt.Tones; //// Track.MusicalTonesInBar(givenBarNumber);
                if (tones != null) {
                    var rhyStr = new RhythmicStructure(rhythmicOrder, tones);

                    for (byte tick = 0; tick < rhythmicOrder; tick++) {
                        if (rhyStr.ElementList == null || tick > rhyStr.ElementList.Count - 1) {  //// 2014/01
                            break;
                        }

                        var elem = rhyStr.ElementList[tick];
                        if (list[tick] == 0 && elem > 0) {
                            list[tick] = elem;
                        }
                    }
                }
            }

            var rs = new RhythmicStructure {
                GSystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, rhythmicOrder)
            };
            rs.SetElementList(new Collection<short>(list));

            return rs;
        }

        /// <summary>
        /// Gets the value of ticks.
        /// </summary>
        /// <param name="lineType">Type of the line.</param>
        /// <returns> Returns value. </returns>
        /// <value>
        /// The value of ticks.
        /// </value>
        public int ValueOfTicks(MusicalLineType lineType) {
            if (lineType == MusicalLineType.Melodic) {
                if (this.valueOfMelodicTicks > 0) {
                    return this.valueOfMelodicTicks;
                }
            }

            if (lineType == MusicalLineType.Rhythmic) {
                if (this.valueOfRhythmicTicks > 0) {
                    return this.valueOfRhythmicTicks;
                }
            }

            var value1 = this.CountOfSelectedTicks(false, lineType);
            //// var value2 = this.CountOfSelectedTicks(true, lineType);
            //// int value = (2 * value1) + value2 + this.CountOfTones;
            return value1;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(CultureInfo.CurrentCulture, "Bar n.{0,3}: ", this.BarNumber); //\r\n
            s.Append(this.HarmonicBar);
            s.Append(this.ChordsToString);
            s.Append(this.ClustersToString);
            return s.ToString();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public string NumberToString() {
            var s = new StringBuilder();
            s.AppendFormat(CultureInfo.CurrentCulture, "{0,3}: ", this.BarNumber);
            return s.ToString();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Makes harmonic cluster for a given rhythmical place.
        /// </summary>
        /// <param name="givenLevel">Requested level.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private HarmonicCluster MakeHarmonicClusterAtLevel(byte givenLevel) {
            //// Contract.Requires(musicalLines != null);
            Contract.Requires(this.CommonRhythmicShape != null);
            if (this.HarmonicBar == null) {
                return null;
            }

            var tick = this.CommonRhythmicShape.PlaceAtLevel(givenLevel);
            var duration = this.CommonRhythmicShape.DistanceAtLevel(givenLevel);
            var harmonicStructure = this.HarmonicBar.HarmonicStructureAtTick(tick);
            var harCluster = new HarmonicCluster(harmonicStructure, tick, duration);

            foreach (var mt in from elem in this.Elements
                               where elem?.Tones != null //// && line.IsSelected
                               where elem.Status?.LineType == MusicalLineType.Melodic
                               select elem.MusicalToneAt(tick, elem.Status.RhythmicStructure) as MusicalTone
                                   into mt
                               where mt != null && mt.ToneType == MusicalToneType.Melodic
                               select mt) {
                harCluster.AddMelodicTone(mt);
            }

            harCluster.Recompute();
            return harCluster;
        }

        /// <summary>
        /// Assign Cluster Levels For Ticks.
        /// </summary>
        private void AssignClusterLevelsForTicks() {
            Contract.Requires(this.CommonRhythmicShape != null);

            var rhyShape = this.CommonRhythmicShape;
            for (byte tick = 0; tick < rhyShape.Order; tick++) {
                var level = rhyShape.LevelOfBit(tick);
                this.clusterLevelForTick[tick] = level;
            }
        }
        #endregion

        /// <summary>
        /// Bit sets the occupied by harmony.
        /// </summary>
        /// <param name="minModalityLevel">The min modality level.</param>
        /// <returns> Returns value. </returns>
        private BitArray BitsOccupiedByHarmony(byte minModalityLevel) {
            var harmonicOrder = this.Body.Context.Header.System.HarmonicOrder;
            var bits = new BitArray(harmonicOrder); //// long number = 0L;
            var bars = this.SurroundingBars(3, 3);
            foreach (var currentBar in bars.Where(currentBar => currentBar?.HarmonicBar.HarmonicStructures != null)) {
                bits = currentBar.HarmonicBar.HarmonicStructures.Where(hs => hs != null).Aggregate(bits, (current, hs) => current.Or(hs.BitArray));
                var bitLevel = (from bool m in bits where m select m).Count();
                if (bitLevel >= minModalityLevel) {
                    break;
                }
            }

            return bits;
        }
    }
}
