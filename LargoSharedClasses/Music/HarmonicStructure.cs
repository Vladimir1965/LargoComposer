// <copyright file="HarmonicStructure.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Abstract;
    using LargoSharedClasses.Harmony;
    using LargoSharedClasses.Interfaces;

    //// Do not remove! (see FOR ALL call)

    /// <summary> Harmonic structure. </summary>
    /// <remarks> Properties: continuity, impulse, measure of dissonance, genus
    /// in modality: continuity, impulse, potential, tonicity
    /// to tonic: continuity, impulse ...  </remarks>
    [Serializable]
    [XmlRoot]
    public sealed class HarmonicStructure : BinarySchema, IHarmonic, IModalStruct
    {
        #region Fields
        /// <summary> Root and principal element. </summary>
        private short rootElement, principalElement;

        /// <summary> String of musical symbols. </summary>
        private string toneSchema;

        /// <summary>
        /// The shortcut
        /// </summary>
        private string shortcut;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the HarmonicStructure class.  Serializable. </summary>
        public HarmonicStructure() {
            this.rootElement = -1;
            this.principalElement = -1;
            this.toneSchema = null;
            this.HarmonicBehavior = new HarmonicBehavior();
            this.BindingToTonic = new BindingBehavior();
            this.BindingToModality = new BindingBehavior();
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicStructure class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public HarmonicStructure(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
            Contract.Requires(givenSystem != null);
            this.rootElement = -1;
            this.principalElement = -1;
            this.toneSchema = null;
            this.HarmonicBehavior = new HarmonicBehavior();
            this.BindingToTonic = new BindingBehavior();
            this.BindingToModality = new BindingBehavior();
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicStructure class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of structure.</param>
        public HarmonicStructure(GeneralSystem givenSystem, long number)
            : base(givenSystem, number) {
            Contract.Requires(givenSystem != null);
            this.rootElement = -1;
            this.principalElement = -1;
            this.toneSchema = null;
            this.HarmonicBehavior = new HarmonicBehavior();
            this.BindingToTonic = new BindingBehavior();
            this.BindingToModality = new BindingBehavior();
            this.DetermineLevel();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicStructure"/> class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">The number.</param>
        public HarmonicStructure(GeneralSystem givenSystem, decimal number)
            : base(givenSystem, (long)number) {
            Contract.Requires(givenSystem != null);
            this.rootElement = -1;
            this.principalElement = -1;
            this.toneSchema = null;
            this.HarmonicBehavior = new HarmonicBehavior();
            this.BindingToTonic = new BindingBehavior();
            this.BindingToModality = new BindingBehavior();
            this.DetermineLevel();
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicStructure class.  Serializable.
        /// </summary>
        /// <param name="harmonicOrder">Harmonic order.</param>
        /// <param name="melodicTones">Melodic tones.</param>
        public HarmonicStructure(byte harmonicOrder, Collection<MusicalTone> melodicTones)
            : base(HarmonicSystem.GetHarmonicSystem(harmonicOrder), (string)null) {
            Contract.Requires(melodicTones != null);
            this.HarmonicBehavior = new HarmonicBehavior();
            this.BindingToTonic = new BindingBehavior();
            this.BindingToModality = new BindingBehavior();

            //// if (melodicTones == null) { return; }
            melodicTones.ForAll(mt => this.On(mt.Pitch.Element));

            this.DetermineLevel();
            this.ComputeVariance();
            this.ComputeBalance();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonicStructure"/> class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="textItem">The text item.</param>
        /// <param name="flag">if set to <c>true</c> [flag].</param>
        public HarmonicStructure(GeneralSystem givenSystem, string textItem, bool flag)
            : this(givenSystem, string.Empty) {
            var s = textItem.ToLower();
            var n = string.Empty;
            string[] tones1 = new string[] { n, "c#", n, "d#", n, n, "f#", n, "g#", n, "a#", n };
            string[] tones2 = new string[] { n, "db", n, "eb", n, n, "gb", n, "ab", n, "b", n };
            string[] tones3 = new string[] { "c", n, "d", n, "e", "f", n, "g", n, "a", n, "h" };

            for (byte i = 0; i < tones1.Count(); i++) {
                var nt = tones1[i];
                if (!string.IsNullOrEmpty(nt) && s.Contains(nt)) {
                    s = s.Replace(nt, string.Empty);
                    this.On(i);
                }
            }

            for (byte i = 0; i < tones1.Count(); i++) {
                var nt = tones2[i];
                if (!string.IsNullOrEmpty(nt) && s.Contains(nt)) {
                    s = s.Replace(nt, string.Empty);
                    this.On(i);
                }
            }

            for (byte i = 0; i < tones1.Count(); i++) {
                var nt = tones3[i];
                if (!string.IsNullOrEmpty(nt) && s.Contains(nt)) {
                    s = s.Replace(nt, string.Empty);
                    this.On(i);
                }
            }

            this.DetermineLevel();
            this.ComputeVariance();
            this.ComputeBalance();
        }

        /// <summary>
        /// Initializes a new instance of the HarmonicStructure class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="givenBitArray">Bit array.</param>
        public HarmonicStructure(GeneralSystem givenSystem, BitArray givenBitArray)
            : base(givenSystem, givenBitArray) {
            Contract.Requires(givenSystem != null);
            this.HarmonicBehavior = new HarmonicBehavior();
            this.BindingToTonic = new BindingBehavior();
            this.BindingToModality = new BindingBehavior();
        }

        /// <summary> Initializes a new instance of the HarmonicStructure class. </summary>
        /// <exception cref="ContractException"> Thrown when a method Contract has been broken. </exception>
        /// <param name="structure"> Binary structure. </param>
        public HarmonicStructure(BinaryStructure structure)
            : base(structure) {
            Contract.Requires(structure != null);
            this.HarmonicBehavior = new HarmonicBehavior();
            this.BindingToTonic = new BindingBehavior();
            this.BindingToModality = new BindingBehavior();
        }

        /// <summary> Initializes a new instance of the HarmonicStructure class. </summary>
        /// <param name="givenSystem"> The given system. </param>
        /// <param name="markHarmony"> The mark harmony. </param>
        public HarmonicStructure(GeneralSystem givenSystem, XElement markHarmony)
                            : base(givenSystem, (string)null) {
            string code = XmlSupport.ReadStringAttribute(markHarmony.Attribute("Code"));
            this.SetStructuralCode(code);
            this.DetermineLevel();
        }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xe = new XElement(
                                "Structure",
                                new XAttribute("Shortcut", this.Shortcut ?? string.Empty),
                                new XAttribute("Tones", this.ToneSchema ?? string.Empty),
                                new XAttribute("Code", this.GetStructuralCode),
                                new XAttribute("Start", this.BitFrom),
                                new XAttribute("Length", this.Length));
                return xe;
            }
        }

        /// <summary>
        /// Gets the get x element2.
        /// </summary>
        /// <value>
        /// The get x element2.
        /// </value>
        public XElement GetXElement2 {
            get {
                long classNumber = BinaryNumber.DetermineClassNumber(this.GSystem.Order, this.Number);
                var xe = new XElement(
                                "Structure",
                                new XAttribute("Level", this.Level),
                                new XAttribute("Number", this.Number),
                                new XAttribute("ClassNumber", classNumber),
                                new XAttribute("Tones", this.ToneSchema ?? string.Empty),
                                new XAttribute("Code", this.GetStructuralCode),
                                new XElement(
                                        "Chord",
                                        new XAttribute("Root", this.Root),
                                        new XAttribute("Shortcut", this.Shortcut ?? string.Empty),
                                        new XAttribute("Consonance", Math.Round(this.HarmonicBehavior.Consonance, 0)),
                                        new XAttribute("Names", MusicalProperties.ChordNames(classNumber))),
                                new XElement(
                                        "Modality",
                                        new XAttribute("Base", this.Root),
                                        new XAttribute("Names", MusicalProperties.ModalityNames(classNumber))));
                return xe;
            }
        }
        #endregion

        #region Properties - Behavior
        /// <summary>
        /// Gets or sets the harmonic behavior.
        /// </summary>
        /// <value>
        /// The harmonic behavior.
        /// </value>
        public HarmonicBehavior HarmonicBehavior { get; set; }

        /// <summary>
        /// Gets or sets the binding from previous.
        /// </summary>
        /// <value>
        /// The binding from previous.
        /// </value>
        public BindingBehavior BindingFromPrevious { get; set; }

        /// <summary>
        /// Gets or sets the binding to tonic.
        /// </summary>
        /// <value>
        /// The binding to tonic.
        /// </value>
        public BindingBehavior BindingToTonic { get; set; }

        /// <summary>
        /// Gets or sets the binding to modality.
        /// </summary>
        /// <value>
        /// The binding to modality.
        /// </value>
        public BindingBehavior BindingToModality { get; set; }
        #endregion

        #region Interface - simple properties
        /// <summary> Gets or sets the string of musical symbols. </summary>
        /// <value> Property description. </value>
        [XmlAttribute]
        public string ToneSchema {
            get => this.toneSchema ?? (this.toneSchema = this.SchemaOfTones());

            set => this.toneSchema = value;
        }

        /// <summary>
        /// Gets or sets the formal energy.
        /// </summary>
        /// <value>
        /// The formal energy.
        /// </value>
        public HarmonicBehavior FormalEnergy { get; set; }

        /// <summary> Gets the root tone. </summary>
        /// <value> Property description. </value>
        public string Root => ((HarmonicSystem)this.GSystem).Symbol(this.RootElement, true);

        /// <summary> Gets the principal tone. </summary>
        /// <value> Property description. </value>
        public string Principal => ((HarmonicSystem)this.GSystem).Symbol(this.PrincipalElement, true);

        #endregion

        #region Interface - object properties
        /// <summary> Gets harmonic system. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public HarmonicSystem HarmonicSystem => (HarmonicSystem)this.GSystem;

        /// <summary> Gets or sets harmonic modality. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public HarmonicModality HarmonicModality {
            get => (HarmonicModality)this.Modality;

            set {
                this.Modality = value;
                if (this.Modality != null) {
                    this.DetermineBehaviorInModality();
                }
            }
        }

        /// <summary> Gets or sets the previous structure. </summary>
        /// <value> Property description. </value>
        public HarmonicStructure PreviousStruct { get; set; }

        /// <summary>
        /// Gets or sets shortcut of harmonic structure.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        [XmlIgnore]
        public string Shortcut {
            get {
                if (this.shortcut != null) {
                    return this.shortcut;
                }
                else {
                    return this.ToneSchema;
                }
            }

            set => this.shortcut = value;
        }

        //// string ts = this.ToneSchema;

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public byte BitFrom { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public byte Length { get; set; }

        /// <summary>
        /// Gets BitRange.
        /// </summary>
        /// <value> General musical property.</value>
        public BitRange BitRange {
            get {
                var range = new BitRange(this.Order, this.BitFrom, this.Length);
                return range;
            }
        }
        #endregion

        #region Interface - private properties
        /// <summary> Gets or sets the root element. </summary>
        /// <value> Property description. </value>
        private short RootElement {
            get {
                if (this.rootElement == -1) {
                    this.DetermineRootElement();
                }

                return this.rootElement;
            }

            set => this.rootElement = value;
        }

        /// <summary> Gets or sets the principal element. </summary>
        /// <value> Property description. </value>
        private short PrincipalElement {
            get {
                if (this.principalElement == -1) {
                    this.DeterminePrincipalElement();
                }

                return this.principalElement;
            }

            set => this.principalElement = value;
        }
        #endregion

        #region Static factory methods
        /// <summary>
        /// Get new harmonic structure.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Structural number.</param>
        /// <returns> Returns value. </returns>
        public static HarmonicStructure GetNewHarmonicStructure(GeneralSystem givenSystem, long number) {
            Contract.Requires(givenSystem != null);
            var hs = new HarmonicStructure(givenSystem, number);
            hs.DetermineBehavior();
            hs.Shortcut = hs.ToneSchema;
            return hs;
        }

        /// <summary>
        /// Get new harmonic structure.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static HarmonicStructure GetNewHarmonicStructure(GeneralSystem givenSystem, string structuralCode) {
            Contract.Requires(givenSystem != null);
            var hs = new HarmonicStructure(givenSystem, structuralCode);
            hs.DetermineBehavior();
            hs.DetermineShortcut();
            return hs;
        }

        /// <summary>
        /// Get NewHarmonicStruct.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Structural Number.</param>
        /// <param name="transposition">Transposition number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static HarmonicStructure GetNewHarmonicStruct(GeneralSystem givenSystem, long number, byte transposition) {
            Contract.Requires(givenSystem != null);
            var hs = new HarmonicStructure(givenSystem, BinaryNumber.Transposition(givenSystem, number, transposition));
            hs.DetermineBehavior();
            return hs;
        }

        /// <summary>
        /// To the binary.
        /// </summary>
        /// <param name="numeral">The numeral.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static BitArray ToBinary(int numeral) {
            return new BitArray(new[] { numeral });
        }

        /// <summary>
        /// To the numeral.
        /// </summary>
        /// <param name="binary">The binary.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">bit array</exception>
        /// <exception cref="System.ArgumentException">Must be at most 32 bits long</exception>
        public static int ToNumeral(BitArray binary) {
            if (binary == null) {
                throw new ArgumentNullException(nameof(binary));
            }

            if (binary.Length > 32) {
                throw new ArgumentException("must be at most 32 bits long");
            }

            var result = new int[1];
            binary.CopyTo(result, 0);
            return result[0];
        }

        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the HarmonicStructure object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var newStruct = GetNewHarmonicStructure(this.GSystem, this.GetStructuralCode);
            newStruct.Shortcut = this.Shortcut;
            newStruct.RootElement = this.RootElement;
            newStruct.PrincipalElement = this.PrincipalElement;
            return newStruct;
        }

        /// <summary> Evaluate properties of the structure. </summary>
        public override void DetermineBehavior() {
            this.ComputeVariance();
            this.ComputeBalance();
            var fs = new HarmonicStateFormal((HarmonicSystem)this.GSystem, this);
            this.HarmonicBehavior.Continuity = fs.FormalContinuity;
            this.HarmonicBehavior.Impulse = fs.FormalImpulse;
            this.HarmonicBehavior.Consonance = fs.FormalConsonance;
            const float formalGenus = 0;
            this.HarmonicBehavior.Genus = formalGenus;
        }

        /// <summary>
        /// Writes the behavior to properties.
        /// </summary>
        public override void WriteBehaviorToProperties() {
            if (this.HarmonicBehavior != null) {
                this.Properties[GenProperty.InnerContinuity] = this.HarmonicBehavior.Continuity ?? 0;
                this.Properties[GenProperty.InnerImpulse] = this.HarmonicBehavior.Impulse ?? 0;
                this.Properties[GenProperty.Consonance] = this.HarmonicBehavior.Consonance;
                this.Properties[GenProperty.Genus] = this.HarmonicBehavior.Genus;
            }

            if (this.BindingToTonic != null) {
                this.Properties[GenProperty.TonicContinuity] = this.BindingToTonic.Continuity ?? 0;
                this.Properties[GenProperty.TonicImpulse] = this.BindingToTonic.Impulse ?? 0;
            }
        }

        /// <summary>
        /// Determines the shortcut.
        /// </summary>
        public void DetermineShortcut() {
            string sc = string.Empty; //// this.ToneSchema;
            long number = ToNumeral(this.BitArray);
            long classNumber = BinaryNumber.DetermineClassNumber(this.GSystem.Order, number);
            string rootSymbol = this.Root.ToUpper(CultureInfo.InvariantCulture);

            if (this.Level == 3) {
                switch (classNumber) {
                    /* Determine Shortcut unused variants
                    case 37: { //// Sixth
                            sc = rootSymbol + "6";
                            break;
                        }
                    case 41: { //// Septime
                            sc = rootSymbol + "7";
                            break;
                        }

                    case 69: { //// Septime
                            sc = rootSymbol + "7";
                            break;
                        }
                   */

                    case 133: { //// Sus 2
                            sc = rootSymbol + "sus2";
                            break;
                        }

                    case 137: { //// Minor
                            sc = rootSymbol + "mi";
                            break;
                        }

                    case 145: { //// Major
                            sc = rootSymbol;
                            break;
                        }

                    case 273: { //// Extended
                            sc = rootSymbol + "5+";
                            break;
                        }
                }
            }

            if (this.Level == 4) {
                switch (classNumber) {
                    case 329: { //// Seventh
                            sc = rootSymbol + "7";
                            break;
                        }

                    case 291: { //// Major Seventh
                            sc = rootSymbol + "maj7";
                            break;
                        }

                    case 275: { //// Major Seventh
                            sc = rootSymbol + "mi-maj7";
                            break;
                        }

                    case 585: { //// Diminished
                            sc = rootSymbol + "dim";
                            break;
                        }
                }
            }

            this.Shortcut = !string.IsNullOrEmpty(sc) ? sc : this.ToneSchema;
        }

        /// <summary>
        /// Modulates the specified given modality.
        /// </summary>
        /// <param name="givenModality">The given modality.</param>
        public void Modulate(HarmonicModality givenModality) {
            this.Modality = givenModality;
            this.toneSchema = string.Empty;

            var limit = (2 * Math.Min(this.HarmonicSystem.Order, this.BitArray.Count)) - 1;
            for (byte i = 0; i < limit; i++) {
                var e = (byte)(i % this.HarmonicSystem.Order);
                if (this.IsOn(e)) {
                    if (givenModality.IsOn(e)) {
                        continue;
                    }
                    else {
                        this.Off(e);
                        var f = (byte)((i + 1) % this.HarmonicSystem.Order);
                        if (givenModality.IsOn(f) && f < this.BitArray.Count) {
                            this.On(f);
                        }
                    }
                }
            }

            this.DetermineLevel();
            this.ResetStructure();
            this.ResetSchema();
            this.DetermineBehavior();
            this.DetermineShortcut();
            this.shortcut = null;
            this.toneSchema = null;
            //// this.toneSchema = this.SchemaOfTones();
        }
        #endregion

        #region Comparison
        /// <summary> Support sorting according to level and ElementSchema. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override int CompareTo(object obj) {
            if (!(obj is HarmonicStructure hstruct)) {
                return 0;
            }

            if (this.Level < hstruct.Level) {
                return -1;
            }

            return this.Level > hstruct.Level ? 1 : string.Compare(hstruct.ElementSchema, this.ElementSchema, StringComparison.Ordinal);
            //// This kills the DataGrid 
            //// throw new ArgumentException("Object is not a HarmonicStructure");
        }

        /// <summary> Test of equality. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public override bool Equals(object obj) {
            //// check null (this pointer is never null in C# methods)
            if (object.ReferenceEquals(obj, null)) {
                return false;
            }

            if (object.ReferenceEquals(this, obj)) {
                return true;
            }

            if (this.GetType() != obj.GetType()) {
                return false;
            }

            return this.CompareTo(obj) == 0;
        }

        /// <summary> Support of comparison. </summary>
        /// <returns> Returns value. </returns>
        public override int GetHashCode() {
            return this.Number.GetHashCode();
        }
        #endregion

        #region Determination of properties

        /// <summary>
        /// Sets properties of the structure with regard to tonic. 
        /// </summary>
        /// <param name="tonic">Harmonical tonic.</param>
        public void DetermineBehaviorToTonic(BinarySchema tonic) {
            if (tonic == null) {
                return;
            }

            var harmonicSystem = (HarmonicSystem)this.GSystem;
            var harRelation = new HarmonicRelation(harmonicSystem, this, tonic);
            var tcontinuity = harRelation.MeanValueOfProperty(GenProperty.InnerContinuity, false, true);
            var timpulse = harRelation.MeanValueOfProperty(GenProperty.InnerImpulse, false, true);
            this.BindingToTonic.Continuity = tcontinuity;
            this.BindingToTonic.Impulse = timpulse;
        }

        // <summary> Sets property of variability. </summary>
        // public void DetermineVariabilityInStream(HarFlow hStream) {
        // float variability = hStream.VariabilityForHarmonicStructure(this);
        // Properties[GenProperty.HarmonicVariability] = variability; // add with repetition
        // }
        #endregion

        #region Static characteristics
        /// <summary> Returns root values of the given element in the structure. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value. </returns>
        public float RootValueOfElement(byte element) {
            var values = HarmonicStateFormal.RootValues(this);
            if (values != null && element < values.Count) {
                return values[element];
            }

            return 0;
        }

        /// <summary> Returns principal value of the given element in the structure. </summary>
        /// <param name="element">Requested element.</param>
        /// <returns> Returns value. </returns>
        public float PrincipalValueOfElement(byte element) {
            var values = HarmonicStateFormal.PrincipalValues(this);
            if (values != null && element < values.Count) {
                return values[element];
            }

            return 0;
        }

        #endregion

        #region String representation

        /// <summary>
        /// Symbols at place.
        /// </summary>
        /// <param name="givenPlace">The given place.</param>
        /// <returns> Returns value. </returns>
        public string SymbolAtPlace(short givenPlace) {
            var c = this.HarmonicSystem.Symbol(givenPlace, true);
            return c;
        }

        /// <summary>
        /// Returns symbols for given level.
        /// </summary>
        /// <param name="givenLevel">Given level.</param>
        /// <returns> Returns value. </returns>
        public string SymbolAtLevel(short givenLevel) {
            var p = this.RealPlaceAtLevel(givenLevel);
            var c = this.SymbolAtPlace(p);
            return c;
        }

        /// <summary>
        /// Makes tone representation of the structure.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        public string SchemaOfTones() {
            if (this.Level <= 0) {
                return string.Empty;
            }

            var str = new StringBuilder();
            if (this.Modality is HarmonicModality modality) {
                var lastL = (byte)(this.Level - 1);
                string s;
                for (byte level = 0; level < lastL; level++) {
                    s = modality.SymbolAtPlace(this.PlaceAtLevel(level));
                    str.Append(s);
                    str.Append("-");
                }

                s = modality.SymbolAtPlace(this.PlaceAtLevel(lastL));
                str.Append(s);
            }
            else {
                var lastL = (byte)(this.Level - 1);
                string s;
                for (byte level = 0; level < lastL; level++) {
                    s = this.SymbolAtLevel(level); //// this.PlaceAtLevel
                    str.Append(s);
                    //// str.Append(" ");
                }

                s = this.SymbolAtLevel(lastL); //// this.PlaceAtLevel
                str.Append(s);
            }

            return str.ToString().Trim();
        }

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(base.ToString());
            s.Append(this.ToneSchema);
            if (this.HarmonicModality != null) {
                s.Append(string.Format(CultureInfo.InvariantCulture, "\t (M:{0})\n", this.HarmonicModality.ToneSchema));
            }
            //// s.Append(this.StringOfProperties());
            //// [s appendFormat:@"C%7.2f I%7.2f s%7.2f G%1d", [this.continuity] floatValue], [this.impulse] floatValue], [this.sonance] floatValue], [this.genus] intValue]];
            //// [s appendFormat:@"(%2s,%2s)", [[mxHSystem symbolAtIndex: this.rootElement]] cString], [[mxHSystem symbolAtIndex: this.principalElement]] cString]];
            return s.ToString();
        }
        #endregion

        #region Public behavior
        /// <summary> Sets previous harmonic structure and Compute corresponding properties. </summary>
        /// <param name="structure">Harmonic structure.</param>
        public void SetPreviousStruct(HarmonicStructure structure) {
            Contract.Requires(structure != null);
            this.PreviousStruct = structure;
            this.DetermineBehaviorFromPreviousStruct();
        }

        /// <summary> Sets properties of the structure with regard to modality. </summary>
        public void DetermineBehaviorInModality() {
            Contract.Requires(this.HarmonicModality != null);
            const byte sonanceFactorToTonicity = 5;
            var harmonicSystem = (HarmonicSystem)this.GSystem;
            var harRelation = new HarmonicRelation(harmonicSystem, this.HarmonicModality, this);
            var continuity = harRelation.MeanValueOfProperty(GenProperty.InnerContinuity, true, true);
            var impulse = harRelation.MeanValueOfProperty(GenProperty.InnerImpulse, false, true);
            var potential = harRelation.MeanValueOfProperty(GenProperty.FormalPotentialInfluence, true, true);
            //// var mpotential2 = this.PotentialInModality();
            //// var mpotential3 = this.Potential;
            var formalConsonance = this.Properties.ContainsKey(GenProperty.Consonance) ? this.Properties[GenProperty.Consonance] : 0;
            var mtonicity = potential + (formalConsonance / sonanceFactorToTonicity);
            this.Properties[GenProperty.ModalContinuity] = continuity;
            this.Properties[GenProperty.ModalImpulse] = impulse;
            this.Properties[GenProperty.Potential] = potential;
            this.Properties[GenProperty.Tonicity] = mtonicity;
            this.HarmonicBehavior.Potential = potential;
        }
        #endregion

        #region Private methods
        /// <summary> Returns potential values of elements in the structure. </summary>
        /// <returns> Returns value. </returns>
        private Collection<float> PotentialValues() {
            Contract.Requires(this.HarmonicModality != null);
            //// HarmonicSystem hS = (HarmonicSystem)this.GSystem;
            var potentialValues = new Collection<float>();
            foreach (var p in this.Places.Select(e => this.HarmonicModality.PotentialOfElement(e))) {
                potentialValues.Add(p);
            }

            return potentialValues;
        }

        /// <summary> Sets properties of the structure with regard to previous structure. </summary>
        private void DetermineBehaviorFromPreviousStruct() {
            Contract.Requires(this.PreviousStruct != null);
            var harmonicSystem = (HarmonicSystem)this.GSystem;
            var harRelation = new HarmonicRelation(harmonicSystem, this.PreviousStruct, this);
            var continuity = harRelation.MeanValueOfProperty(GenProperty.InnerContinuity, false, true);
            var impulse = harRelation.MeanValueOfProperty(GenProperty.InnerImpulse, false, true);
            this.Properties[GenProperty.RelatedContinuity] = continuity; //// add with repetition
            this.Properties[GenProperty.RelatedImpulse] = impulse; //// add with repetition
        }

        /// <summary> Sets principal element. </summary>
        private void DeterminePrincipalElement() {
            var va = HarmonicStateFormal.PrincipalValues(this);
            float maxValue = -1000;
            short maxElement = -1;
            byte e = 0;
            if (va != null) {
                foreach (var v in va) {
                    if (this.IsOn(e) && (v > maxValue)) {
                        maxValue = v;
                        maxElement = e;
                    }

                    e++;
                }
            }

            this.principalElement = maxElement;
            //// indexOfObject MAX([self PrincipalValues]);
        }

        /// <summary> Sets root element. </summary>
        private void DetermineRootElement() {
            var va = HarmonicStateFormal.RootValues(this);
            float maxValue = -1000;
            short maxElement = -1;
            byte e = 0;
            if (va != null) {
                foreach (var v in va) {
                    if (this.IsOn(e) && (v > maxValue)) {
                        maxValue = v;
                        maxElement = e;
                    }

                    e++;
                }
            }

            this.rootElement = maxElement;
        }
        #endregion

        #region Relation to modality
        /// <summary> Returns total potential of the structure in modality. </summary>
        /// <returns> Returns value. </returns>
        private float PotentialInModality() {
            Contract.Requires(this.HarmonicModality != null);
            var values = this.PotentialValues();
            var sum = values?.Sum() ?? 0;

            return sum;
        }
        #endregion
    }
}
