// <copyright file="HarmonicBar.cs" company="Traced-Ideas, Czech republic">
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
    using LargoSharedClasses.Harmony;
    using LargoSharedClasses.Models;
    using LargoSharedClasses.Rhythm;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Harmonic Motive.
    /// </summary>
    [Serializable]
    public sealed class HarmonicBar
    {
        #region Fields
        /// <summary>
        /// Harmonic Structures.
        /// </summary>
        [NonSerialized]
        private IList<HarmonicStructure> harmonicStructures;

        /// <summary>
        /// Rhythmic Structure.
        /// </summary>
        private RhythmicStructure rhythmicStructure;

        /// <summary>
        /// Harmonic Modality.
        /// </summary>
        private HarmonicModality harmonicModality;

        /// <summary>
        /// Simple structural Outline.
        /// </summary>
        private string simpleStructuralOutline;

        /// <summary>
        /// Structural Outline.
        /// </summary>
        private string structuralOutline;

        /// <summary>
        /// The bar metric code.
        /// </summary>
        private string barMetricCode;

        /// <summary>
        /// The harmonic modality code.
        /// </summary>
        private string harmonicModalityCode;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicBar" /> class.
        /// </summary>
        /// <param name="givenBarNumber">The given bar number.</param>
        /// <param name="originalBarNumber">The original bar number.</param>
        public HarmonicBar(int givenBarNumber, int originalBarNumber) {
            this.RhythmicBehavior = new RhythmicBehavior();
            this.HarmonicBehavior = new HarmonicBehavior();
            this.harmonicStructures = new List<HarmonicStructure>();
            this.BarNumber = givenBarNumber;
            this.OriginalBarNumber = originalBarNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicBar"/> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="givenBar">The given bar.</param>
        public HarmonicBar(MusicalHeader givenHeader, HarmonicBar givenBar) {
            this.Header = givenHeader; //// Convert given bar under given (style) header...
            this.RhythmicBehavior = givenBar.RhythmicBehavior;
            this.HarmonicBehavior = givenBar.HarmonicBehavior;
            this.harmonicStructures = new List<HarmonicStructure>();
            this.BarNumber = givenBar.BarNumber;
            this.OriginalBarNumber = givenBar.OriginalBarNumber;
            this.HarmonicModality = givenBar.HarmonicModality;
            this.RhythmicStructure = givenBar.RhythmicStructure.ConvertToSystem(givenHeader.System.RhythmicSystem);
            this.RhythmicShape = this.RhythmicStructure.GetRhythmicShape;
            int idx = 0;
            var distances = this.RhythmicShape.BitDistances;
            foreach (var givenHarStruct in givenBar.harmonicStructures) {
                var harStruct = (HarmonicStructure)givenHarStruct.Clone();
                if (idx < distances.Count) {
                    harStruct.Length = distances[idx++];
                }

                this.harmonicStructures.Add(harStruct);
            }

            //// var rsystem = this.RhythmicSystemFromStructures;
            this.RhythmicShape = this.RhythmicShapeFromStructures(givenHeader.System.RhythmicSystem);
            this.barMetricCode = this.BarMetricFromStructures(givenHeader.System.RhythmicSystem);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicBar"/> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="markHarmony">The mark bar.</param>
        public HarmonicBar(MusicalHeader givenHeader, XElement markHarmony) {
            int barNumber = XmlSupport.ReadIntegerAttribute(markHarmony.Attribute("Number"));
            int originalBarNumber = XmlSupport.ReadIntegerAttribute(markHarmony.Attribute("OriginalNumber"));
            this.RhythmicBehavior = new RhythmicBehavior();
            this.HarmonicBehavior = new HarmonicBehavior();
            this.harmonicStructures = new List<HarmonicStructure>();
            this.BarNumber = barNumber;
            this.OriginalBarNumber = originalBarNumber;
            this.Header = givenHeader;
            var xstructure = markHarmony.Elements("Structure");
            if (xstructure != null) {
                //// var xstructure = xElement.Elements("Structure");
                StringBuilder sb = new StringBuilder();
                foreach (var xst in xstructure) {
                    var code = (string)xst.Attribute("Code");
                    var start = (int)xst.Attribute("Start");
                    var length = (int)xst.Attribute("Length");
                    var st = new HarmonicStructure(this.Header.System.HarmonicSystem, code) {
                        BitFrom = (byte)start,
                        Length = (byte)length
                    };
                    var shortcut = XmlSupport.ReadStringAttribute(xst.Attribute("Shortcut"));
                    st.DetermineBehavior();
                    st.Shortcut = shortcut;
                    //// st.Base = chordBase;
                    this.AddStructure(st);
                    sb.Append(shortcut);
                    sb.Append(",");
                }

                //// bar.RhythmicStructure;
                this.StructuralOutline = sb.ToString();

                //// 2020/10 test
                //// this.RhythmicShape = this.RhythmicShapeFromStructures(givenHeader.System.RhythmicSystem);
                //// this.barMetricCode = this.BarMetricFromStructures(givenHeader.System.RhythmicSystem);

                var rhyStruct = this.RhythmicStructure;
                this.RhythmicShape = new RhythmicShape(rhyStruct.RhythmicSystem.Order, rhyStruct);
                this.SetBarMetricCode(this.RhythmicShape.GetStructuralCode); 
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicBar"/> class.
        /// </summary>
        /// <param name="givenHeader">The given header.</param>
        /// <param name="harmonicStructure">The harmonic structure.</param>
        public HarmonicBar(MusicalHeader givenHeader, HarmonicStructure harmonicStructure) {
            this.Header = givenHeader;
            this.RhythmicBehavior = new RhythmicBehavior();
            this.HarmonicBehavior = new HarmonicBehavior();
            this.harmonicStructures = new List<HarmonicStructure>();
            if (harmonicStructure != null) {
                this.SetStructure(harmonicStructure);
            }
        }

        #endregion

        #region Properties - Xml

        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xelement = new XElement("Harmony");
                xelement.Add(new XAttribute("Length", this.Length));
                foreach (var harStruct in this.HarmonicStructures) {
                    var xstruct = harStruct.GetXElement;
                    xelement.Add(xstruct);
                }

                return xelement;
            }
        }

        #endregion

        #region Main Properties
        /// <summary>
        /// Gets or sets the musical header.
        /// </summary>
        /// <value>
        /// The musical header.
        /// </value>
        public MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the Bar Number In Motive.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public int BarNumber { get; set; }

        /// <summary>
        /// Gets or sets the bar number.
        /// </summary>
        /// <value>
        /// The bar number.
        /// </value>
        public int OriginalBarNumber { get; set; }

        /// <summary>
        /// Gets or sets the occurrence.
        /// </summary>
        /// <value>
        /// The occurrence.
        /// </value>
        public int Occurrence { get; set; }

        /// <summary>
        /// Gets or sets the harmonic structures.
        /// </summary>
        /// <value>
        /// The harmonic structures.
        /// </value>
        public IList<HarmonicStructure> HarmonicStructures {
            get {
                Contract.Ensures(Contract.Result<IList<HarmonicStructure>>() != null);
                if (this.harmonicStructures == null) {
                    throw new InvalidOperationException("Harmonic structures are null.");
                }

                return this.harmonicStructures;
            }

            set => this.harmonicStructures = value ?? throw new ArgumentException("Argument cannot be empty.", nameof(value));
        }

        /// <summary> Gets a value indicating whether if bar is empty. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public bool IsEmpty => this.HarmonicStructures.Count == 0;

        /// <summary> Gets unique identifier. </summary>
        /// <value> Property description. </value>
        public string UniqueIdentifier {
            get {
                var ident = new StringBuilder();
                foreach (var b in
                    from ms in this.HarmonicStructures where ms != null from b in ms.GetStructuralCode select b) {
                    ident.Append(b); //// ElementSchema, DecimalNumber, ms.StructuralCode
                }

                return ident.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the Harmonic Modality.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public HarmonicModality HarmonicModality {
            get {
                if (this.harmonicModality != null) {
                    return this.harmonicModality;
                }

                var modalityCode = this.GetHarmonicModalityCode;
                if (string.IsNullOrEmpty(modalityCode)) {
                    return this.harmonicModality;
                }

                const byte order = DefaultValue.HarmonicOrder; //// this.harMotiveBar.HarmonicMotive.THarmonicMotive.THarmonicCore.HarmonicOrder;
                var hs = HarmonicSystem.GetHarmonicSystem(order);
                this.harmonicModality = HarmonicModality.GetNewHarmonicModality(hs, modalityCode);

                return this.harmonicModality;
            }

            set => this.harmonicModality = value;
        }

        /// <summary>
        /// Gets or sets the harmonic motive.
        /// </summary>
        /// <value>
        /// The harmonic motive.
        /// </value>
        public HarmonicMotive HarmonicMotive { get; set; }

        #endregion

        #region Derived Properties
        /// <summary>
        /// Gets or sets the simple structural outline.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public string SimpleStructuralOutline {
            get {
                if (!string.IsNullOrEmpty(this.simpleStructuralOutline)) {
                    return this.simpleStructuralOutline;
                }

                var sb = new StringBuilder();
                //// HarmonicModality modality = this.HarmonicModality;
                foreach (var harStruct in this.HarmonicStructures) {
                    if (sb.Length > 0) {
                        sb.Append(" ");
                    }

                    sb.Append(harStruct.Shortcut);
                }

                this.simpleStructuralOutline = sb.ToString();

                return this.simpleStructuralOutline;
            }

            set => this.simpleStructuralOutline = value;
        }

        /// <summary>
        /// Gets the outline.
        /// </summary>
        /// <value>
        /// The outline.
        /// </value>
        [UsedImplicitly]
        public string Outline => $"{this.BarNumber}/{this.StructuralOutline}";

        /// <summary>
        /// Gets or sets HarmonicStructureOutline.
        /// </summary>
        /// <value> General musical property.</value>
        public string StructuralOutline {
            get {
                if (this.structuralOutline != null) {
                    return this.structuralOutline;
                }

                var sb = new StringBuilder();
                //// HarmonicModality modality = this.HarmonicModality;
                foreach (var harStruct in this.HarmonicStructures) {
                    if (sb.Length > 0) {
                        sb.Append(" ");
                    }

                    sb.Append(harStruct.Shortcut);
                    sb.Append(string.Format(CultureInfo.InvariantCulture, "({0})", harStruct.Length.ToString(CultureInfo.InvariantCulture)));
                }

                this.structuralOutline = sb.ToString();
                return this.structuralOutline;
            }

            set => this.structuralOutline = value;
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value> Property description. </value>
        public int Length => this.HarmonicStructures.Count;

        /// <summary>
        /// Gets the bar metric code.
        /// </summary>
        /// <returns> Returns value. </returns>
        public string GetBarMetricCode => this.barMetricCode?.Trim();

        /// <summary>
        /// Gets the harmonic modality code.
        /// </summary>
        /// <returns> Returns value. </returns>
        public string GetHarmonicModalityCode => this.harmonicModalityCode?.Trim();

        #endregion

        #region Physical properties - Public
        /// <summary>
        /// Gets or sets the harmonic behavior.
        /// </summary>
        /// <value>
        /// The harmonic behavior.
        /// </value>
        public HarmonicBehavior HarmonicBehavior { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic behavior.
        /// </summary>
        /// <value>
        /// The rhythmic behavior.
        /// </value>
        public RhythmicBehavior RhythmicBehavior { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic structure.
        /// </summary>
        /// <value>
        /// General musical property.
        /// </value>
        public RhythmicStructure RhythmicStructure {
            get {
                //// Contract.Requires(this.RhythmicSystemFromStructures != null);
                if (this.rhythmicStructure != null) {
                    return this.rhythmicStructure;
                }

                byte rorder = 0;  //// ?!?!?
                if (this.Header != null) {
                    rorder = this.Header.System.RhythmicOrder;
                }

                var musicalHeader = this.Header;
                if (musicalHeader != null) {
                    var system = musicalHeader.System;
                    //// if (HarmonicMotive?.Core != null) {
                    rorder = system.RhythmicOrder;
                    //// }
                }

                var rsystem = rorder > 0 ? RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, rorder) : this.RhythmicSystemFromStructures;

                if (rsystem == null) {
                    return null;
                    //// throw new InvalidOperationException("Unknown Rhythmic System.");
                }

                this.RhythmicShape = this.RhythmicShapeFromStructures(rsystem);
                var rs = new RhythmicStructure(rsystem.Order, this.RhythmicShape);
                return rs;
            }

            set => this.rhythmicStructure = value;
        }
        #endregion

        #region Properties - Rhythmic Status

        /// <summary> Gets or sets rhythmical shape. </summary>
        /// <value> Property description. </value>
        public RhythmicShape RhythmicShape { 
            get; 
            set; 
        }

        #endregion

        #region Public properties - String representation
        /// <summary> Gets Write particular structures to string. </summary>
        /// <returns> Returns value.</returns>
        /// <value> General musical property.</value>
        public string PureChordsToString {
            get {
                var s = new StringBuilder();
                //// s.Append("Chords:"); //// +"\n"
                byte lev = 0;
                foreach (var hs in this.HarmonicStructures.Where(hs => (hs != null && this.RhythmicShape != null) && this.RhythmicShape.Number > 0)) {
                    var duration = this.RhythmicShape.DistanceAtLevel(lev++);
                    s.AppendFormat(CultureInfo.CurrentCulture, "{0,15} ", hs.ToneSchema);
                    s.AppendFormat(CultureInfo.CurrentCulture, "({0,2}), ", duration.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
                }

                return s.ToString().Trim();
            }
        }

        /// <summary>
        /// Gets modality as string.
        /// </summary>
        /// <value> General musical property.</value>
        /// <returns> Returns value. </returns>
        public string ModalityToString {
            get {
                var s = new StringBuilder();
                if (this.HarmonicStructures.Count <= 0) {
                    return s.ToString();
                }

                var harStruct = this.HarmonicStructures[0];
                if (harStruct?.HarmonicModality != null) {
                    s.AppendFormat(CultureInfo.CurrentCulture, "{0,20}   ", harStruct.HarmonicModality.ToneSchema);
                }

                return s.ToString();
            }
        }

        /// <summary> Gets particular structures to string. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value.</returns>
        [UsedImplicitly]
        public string ModalitiesToString {
            get {
                var s = new StringBuilder();
                //// byte lev = 0; //// byte duration;
                long lastNumber = 0;
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var hs in this.HarmonicStructures
                    .Where(hs => hs?.HarmonicModality != null)) {
                    if (hs.HarmonicModality.Number == lastNumber) {
                        continue;
                    }

                    //// duration = RhythmicShape.DistanceAtLevel(lev++);
                    s.Append(hs.HarmonicModality.ToneSchema + "\r");
                    lastNumber = hs.HarmonicModality.Number;
                }

                return s.ToString();
            }
        }

        /// <summary>
        /// Gets to string.
        /// </summary>
        /// <value> General musical property.</value>
        public string ChordsToString {
            get {
                var s = new StringBuilder();
                //// s.Append("Chords:"); //// +"\n"
                byte level = 0;
                foreach (var hs in this.HarmonicStructures.Where(hs => hs != null && this.RhythmicShape != null)) {
                    var duration = this.RhythmicShape.DistanceAtLevel(level++);
                    //// s.AppendFormat(CultureInfo.CurrentCulture, "{0,15}", hs.HarmonicModality != null ? hs.HarmonicModality.ToneSchema : string.Empty);

                    s.AppendFormat(CultureInfo.CurrentCulture, "{0}", hs.ToneSchema);
                    s.AppendFormat(CultureInfo.CurrentCulture, "({0,2})\n", duration.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
                }

                return s.ToString().Trim();
            }
        }
        #endregion

        #region Public properties
        /// <summary> Gets inner measure of dissonance. </summary>
        /// <value> Property description. </value>
        public float MeanConsonance {
            get {
                if (this.Length == 0) {
                    return 0;
                }

                var total = (from HarmonicStructure s in this.HarmonicStructures select s.HarmonicBehavior.Consonance).Sum();
                var meanConsonance = total / this.Length;
                return meanConsonance;
            }
        }

        /// <summary> Gets inner continuity. </summary>
        /// <value> Property description. </value>
        public float MeanPotential {
            get {
                if (this.Length == 0) {
                    return 0;
                }

                float total = 0;
                var cnt = 0;
                foreach (var s in this.HarmonicStructures) {
                    if (s != null) {
                        total += s.HarmonicBehavior.Potential;
                        cnt++;
                    }
                }

                var result = cnt != 0 ? total / cnt : 0;
                return result;
            }
        }

        /// <summary> Gets inner continuity. </summary>
        /// <value> Property description. </value>
        public float MeanContinuity {
            get {
                if (this.Length == 0) {
                    return 0;
                }

                HarmonicStructure p = null;
                float total = 0;
                var cnt = 0;
                foreach (var s in this.HarmonicStructures) {
                    if (p != null) {
                        var r = new HarmonicRelation(s.HarmonicSystem, p, s);
                        total += r.FormalContinuity;
                        cnt++;
                    }

                    p = s;
                }

                var result = cnt != 0 ? total / cnt : 100;

                return result;
            }
        }

        /// <summary> Gets inner impulse. </summary>
        /// <value> Property description. </value>
        public float MeanImpulse {
            get {
                if (this.Length == 0) {
                    return 0;
                }

                HarmonicStructure p = null;
                float total = 0;
                var cnt = 0;
                foreach (var s in this.HarmonicStructures) {
                    if (p != null) {
                        var r = new HarmonicRelation(s.HarmonicSystem, p, s);

                        total += r.FormalImpulse;
                        cnt++;
                    }

                    p = s;
                }

                var result = cnt != 0 ? total / cnt : 0;

                return result;
            }
        }

        /// <summary>
        /// Gets the mean rhythmic mobility.
        /// </summary>
        public float MeanRhythmicMobility {
            get {
                //// Contract.Requires(this.RhythmicSystemFromStructures != null);
                var rs = this.RhythmicStructure;
                if (rs == null) {
                    return 0;
                }

                if (!rs.HasProperties) {
                    rs.DetermineBehavior();
                }

                return rs.RhythmicBehavior.Mobility;
            }
        }

        /// <summary>
        /// Gets the mean rhythmic tension.
        /// </summary>
        public float MeanRhythmicTension {
            get {
                Contract.Requires(this.RhythmicSystemFromStructures != null);
                var rs = this.RhythmicStructure;
                if (rs == null) {
                    return 0;
                }

                if (!rs.HasProperties) {
                    rs.DetermineBehavior();
                }

                return rs.FormalBehavior.Variance;
            }
        }

        /// <summary>
        /// Gets the rhythmic system from structures.
        /// </summary>
        private RhythmicSystem RhythmicSystemFromStructures {
            get {
                var structures = this.HarmonicStructures;
                var rhythmicOrder = structures.Aggregate<HarmonicStructure, byte>(0, (current, structure) => (byte)(current + structure.Length));
                if (rhythmicOrder == 0) {
                    return null;
                }

                var rsystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, rhythmicOrder);
                return rsystem;
            }
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Empties the bar.
        /// </summary>
        /// <param name="harmonicOrder">The harmonic order.</param>
        /// <param name="rhythmicOrder">The rhythmic order.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static HarmonicBar EmptyBar(byte harmonicOrder, byte rhythmicOrder) {
            var harmonicBar = new HarmonicBar(0, 0);
            var structuralCode = string.Empty; //// new byte[0];
            var system = HarmonicSystem.GetHarmonicSystem(harmonicOrder);
            var hs = new HarmonicStructure(system, structuralCode) {
                BitFrom = 0,
                Length = rhythmicOrder
            };

            harmonicBar.AddStructure(hs);
            return harmonicBar;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the structure.
        /// </summary>
        /// <param name="harmonicStructure">The harmonic structure.</param>
        public void SetStructure(HarmonicStructure harmonicStructure) {
            this.simpleStructuralOutline = null;
            this.structuralOutline = null;
            this.RhythmicBehavior = new RhythmicBehavior();
            this.HarmonicBehavior = new HarmonicBehavior();
            this.harmonicStructures = new List<HarmonicStructure>();
            this.AddStructure(harmonicStructure);
            
            if (this.RhythmicStructure == null && this.Header != null) { //// 2020/11
                this.RhythmicStructure = RhythmicStructure.GetNewRhythmicStructure(this.Header.System.RhythmicSystem, 1);
            }

            if (this.RhythmicStructure != null) {
                var rhyStruct = this.RhythmicStructure;
                var shape = new RhythmicShape(rhyStruct.RhythmicSystem.Order, rhyStruct);
                this.SetBarMetricCode(shape.GetStructuralCode);
                this.RhythmicShape = shape; //// 2023/01
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(CultureInfo.CurrentCulture, "Bar n.{0,3}: ", this.BarNumber); //\r\n
            s.Append(this.SimpleStructuralOutline);
            return s.ToString();
        }

        /// <summary> Makes a deep copy of the HarmonicBar object. </summary>
        /// <returns>
        /// Returns object.
        /// </returns>
        public object Clone() {
            var bar = new HarmonicBar(0, this.OriginalBarNumber) {
                Header = this.Header,
                HarmonicStructures = this.HarmonicStructures
            };

            bar.SetBarMetricCode(this.GetBarMetricCode);
            bar.SetHarmonicModalityCode(this.GetHarmonicModalityCode);
            this.RegenerateStructures();
            return bar;
        }

        /// <summary>
        /// Adds the motive.
        /// </summary>
        /// <param name="structure">The structure.</param>
        public void AddStructure(HarmonicStructure structure) {
            ((List<HarmonicStructure>)this.HarmonicStructures).Add(structure);
        }

        /// <summary>
        /// Determines the bar metric.
        /// </summary>
        /// <param name="rhythmicSystem">The rhythmic system.</param>
        /// <returns> Returns value.</returns>
        public string BarMetricFromStructures(RhythmicSystem rhythmicSystem) {
            //// rewrite it with help of constructor using BitArray 
            if (rhythmicSystem == null) {
                return string.Empty;
            }

            var barMetric = new BinaryStructure(rhythmicSystem, 0);
            byte start = 0;
            foreach (var hs in this.HarmonicStructures) {
                barMetric.On(start);
                start += hs.Length;
            }

            barMetric.DetermineLevel();
            return barMetric.GetStructuralCode;
        }

        /// <summary>
        /// Sets the bar metric code.
        /// </summary>
        /// <param name="givenCode">The given code.</param>
        public void SetBarMetricCode(string givenCode) {
            this.barMetricCode = givenCode;
        }

        /// <summary>
        /// Sets the harmonic modality code.
        /// </summary>
        /// <param name="givenCode">The given code.</param>
        public void SetHarmonicModalityCode(string givenCode) {
            this.harmonicModalityCode = givenCode;
        }

        /// <summary>
        /// Re-computes this instance.
        /// </summary>
        public void Recompute() {
            if (this.Length == 0 || this.RhythmicStructure == null) {
                return;
            }

            this.HarmonicBehavior.Consonance = this.MeanConsonance;
            if (this.HarmonicModality != null) {
                this.HarmonicBehavior.Potential = this.MeanPotential;
            }

            this.HarmonicBehavior.Continuity = this.MeanContinuity;
            this.HarmonicBehavior.Impulse = this.MeanImpulse;
            this.RhythmicBehavior.Mobility = this.MeanRhythmicMobility;
            this.RhythmicBehavior.Tension = this.MeanRhythmicTension;
        }

        /// <summary>
        /// Checks the structures.
        /// </summary>
        public void CheckStructures() {
            if (!this.HarmonicStructures.Any()) {
                var harmonicOrder = this.Header.System.HarmonicOrder;
                //// throw new ArgumentException("Empty har.motive bar structs!");
                var harSys = HarmonicSystem.GetHarmonicSystem(harmonicOrder);
                var harStr = new HarmonicStructure(harSys, 0L);
                //// harStr.DetermineBehavior();
                this.AddHarmonicStructure(harStr);
            }
        }

        /// <summary>
        /// Regenerates the structures.
        /// </summary>
        public void RegenerateStructures() {
            if (this.Header == null) {
                return;
            }

            var harmonicOrder = this.Header.System.HarmonicOrder;
            //// var rhythmicOrder = this.Header.System.RhythmicOrder;

            var harStructures = new List<HarmonicStructure>();
            //// Microtonal support
            //// harStructNumber = GeneralSystem.ConvertStruct(harStructNumber, hms.HarmonicOrder, harSys.Order); //// harSys.Degree

            //// Do not convert to Linq!
            foreach (var hms in this.HarmonicStructures) {
                if (hms == null) {
                    continue;
                }

                var harSys = HarmonicSystem.GetHarmonicSystem(harmonicOrder);
                var structuralCode = hms.GetStructuralCode;
                var harStruct = HarmonicStructure.GetNewHarmonicStructure(harSys, structuralCode);
                //// harStr.DetermineBehavior(); 
                harStruct.BitFrom = hms.BitFrom;
                harStruct.Length = hms.Length;
                harStructures.Add(harStruct);
            }

            this.HarmonicStructures = harStructures;
        }

        /// <summary> Returns harmonic structure at given position. </summary>
        /// <value> Property description. </value>
        /// <param name="givenLevel">Requested level.</param>
        /// <returns> Returns value. </returns>
        public HarmonicStructure HarmonicStructureAtRhythmicLevel(byte givenLevel) {
            if (this.HarmonicStructures.Count == 0 || givenLevel >= this.HarmonicStructures.Count) {
                return null;
            }

            return this.HarmonicStructures[givenLevel];
        }

        /// <summary> Returns harmonic structure at given bit. </summary>
        /// <param name="givenTick">Requested tick.</param>
        /// <returns> Returns value. </returns>
        public HarmonicStructure HarmonicStructureAtTick(byte givenTick) {
            if (this.RhythmicShape == null) {
                return null;
            }

            var level = this.RhythmicShape.LevelOfBit(givenTick);
            return this.HarmonicStructureAtRhythmicLevel(level);
        }

        /// <summary>
        /// Returns harmonic structure that prevails in given time Range. 
        /// </summary>
        /// <param name="toneRange">Tone Range.</param>
        /// <param name="simpleHarmony">Single Harmony.</param>
        /// <returns> Returns value. </returns>
        public HarmonicStructure PrevailingHarmonicStructure(BitRange toneRange, out bool simpleHarmony) {
            simpleHarmony = false;
            if (this.RhythmicShape == null) {
                return null;
            }

            byte extremeLength = 0, rhythmicLevelAtExtreme = 0;
            var level = this.RhythmicShape.Level;
            for (byte i = 0; i < level; i++) {
                var rhythmicRange = this.RhythmicShape.RangeForLevel(i);
                var single = rhythmicRange.CoverRange(toneRange); // 2008/12
                if (single) {
                    simpleHarmony = true;
                    return this.HarmonicStructureAtRhythmicLevel(i);
                }

                var interRange = rhythmicRange.IntersectionWith(toneRange);
                if (interRange == null || interRange.Length <= extremeLength) {
                    continue;
                }

                extremeLength = interRange.Length;
                rhythmicLevelAtExtreme = i;
            }

            //// simpleHarmony = false;
            return this.HarmonicStructureAtRhythmicLevel(rhythmicLevelAtExtreme);
        }

        /// <summary> Returns number of bits containing given pitch element. </summary>
        /// <param name="givenPitchElement">Element of pitch.</param>
        /// <param name="range">Given range.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public int TotalHarmonicBits(byte givenPitchElement, BitRange range) {
            if (range == null) {
                return 0;
            }

            byte i, total = 0;
            byte bitFrom = range.BitFrom, bitTo = range.BitTo;
            for (i = bitFrom; i <= bitTo; i++) {
                var harmonicStructure = this.HarmonicStructureAtTick(i);
                if (harmonicStructure != null) {
                    total += (byte)(harmonicStructure.IsOn(givenPitchElement) ? 1 : 0);
                }
            }

            return total;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Rhythmic shape from structures.
        /// </summary>
        /// <param name="rhythmicSystem">The rhythmic system.</param>
        /// <returns> Returns value.</returns>
        private RhythmicShape RhythmicShapeFromStructures(RhythmicSystem rhythmicSystem) {
            var bitArray = new BitArray(rhythmicSystem.Order);
            var shape = new RhythmicShape(rhythmicSystem, bitArray);
            byte start = 0;
            foreach (var hs in this.HarmonicStructures) {
                if (start < shape.Order) { //// 2015/01
                    shape.On(start);
                }

                start += hs.Length;
            }

            //// 2020/10
            shape.DetermineLevel();
            shape.DetermineBehavior();
            //// shape.DetermineStructuralCode();
            return shape;
        }

        /// <summary>
        /// Add Harmonic Struct.
        /// </summary>
        /// <param name="harmonicStructure">Harmonic structure.</param>
        private void AddHarmonicStructure(HarmonicStructure harmonicStructure) {
            if (harmonicStructure != null) { //// && !harmonicStructure.IsEmptyStruct() !?
                //// // harStr.HarmonicModality = aHarmonicVariety.HarmonicModality;
                this.HarmonicStructures.Add(harmonicStructure);
            }
        }

        #endregion
    }
}
