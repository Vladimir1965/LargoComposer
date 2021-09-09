// <copyright file="MusicalPitch.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Music
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Abstract;

    /// <summary> Tone pitch. </summary>
    /// <remarks> Pitch is an object for keeping given octave and element (formal pitch).
    /// It allows to determine (real or formal) distances (intervals) to other pitches. </remarks>
    /// MidiPitch 0..127, MidiPower 0..127
    [Serializable]
    [XmlRoot]
    public sealed class MusicalPitch : ICloneable, IComparable
    {
        #region Fields
        /// <summary> Inner number of the lowest musical octave. </summary>
        [XmlIgnore]
        public const int MinOctave = 1; //// -3;

        /// <summary> Inner number of the highest musical octave. </summary>
        [XmlIgnore]
        public const int MaxOctave = 9; // +5;

        /// <summary> Referential MIDI key number. </summary>
        [XmlIgnore]
        public const byte BaseKeyNumber = 0; // 48; MIDI

        /// <summary>
        /// Harmonic system.
        /// </summary>
        private readonly HarmonicSystem harSystem;
        #endregion

        #region Constructors
        /// <summary> Initializes a new instance of the MusicalPitch class.  Serializable. </summary>
        public MusicalPitch() {
        }

        /// <summary>
        /// Initializes a new instance of the MusicalPitch class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        public MusicalPitch(HarmonicSystem givenSystem) {
            Contract.Requires(givenSystem != null);
            this.harSystem = givenSystem;
        }

        /// <summary>
        /// Initializes a new instance of the MusicalPitch class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="givenOctave">Musical octave.</param>
        /// <param name="element">Element of system.</param>
        public MusicalPitch(HarmonicSystem givenSystem, short givenOctave, byte element) {
            Contract.Requires(givenSystem != null);
            this.harSystem = givenSystem;
            this.Octave = givenOctave;
            this.Element = element;
            this.RecomputeAltitude();
        }

        /// <summary>
        /// Initializes a new instance of the MusicalPitch class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="givenAltitude">Pitch altitude.</param>
        public MusicalPitch(HarmonicSystem givenSystem, int givenAltitude) {
            Contract.Requires(givenSystem != null);
            this.harSystem = givenSystem;
            this.SetAltitude(givenAltitude);
        }

        /// <summary> Initializes a new instance of the MusicalPitch class. </summary>
        /// <param name="midiKeyNumber">Midi Key Number.</param>
        public MusicalPitch(byte midiKeyNumber) {
            this.harSystem = HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder);
            this.SetAltitude(midiKeyNumber - BaseKeyNumber);
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets musical octave of the pitch. </summary>
        /// <value> Property description. </value>
        public short Octave { get; set; }  // MinOctave..MaxOctave

        /// <summary> Gets musical octave of the pitch. </summary>
        /// Middle C has StandardOctave = 4
        /// <value> Property description. </value>
        public short StandardOctave => (byte)(this.Octave - 1);

        /// <summary> Gets or sets musical element of the pitch. </summary>
        /// <value> Property description. </value>
        public byte Element { get; set; } // 0..Order-1

        /// <summary> Gets or sets musical element of the pitch. </summary>
        /// <value> Property description. </value>
        public int SystemAltitude { get; set; } // 0..127

        /// <summary> Gets harmonic system. </summary>
        /// <value> Property description. </value>
        [XmlIgnore]
        public HarmonicSystem HarmonicSystem {
            get {
                Contract.Ensures(Contract.Result<GeneralSystem>() != null);
                Contract.Ensures(Contract.Result<GeneralSystem>().Order > 0);
                if (this.harSystem == null) {
                    throw new InvalidOperationException("Harmonic system is null.");
                }

                return this.harSystem;
            }
        }

        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XAttribute GetXAttribute => new XAttribute("Pitch", this.SystemAltitude);

        /// <summary>
        /// Gets MIDI key number.
        /// </summary>
        /// <value> Property description. </value>        
        /// <returns> Returns value. </returns>
        public byte MidiKeyNumber => (byte)(BaseKeyNumber + this.Halftones);

        /// <summary>
        /// Gets Halftones - MIDI support.
        /// </summary>
        private int Halftones {
            get {
                const float tolerance = 0.001f;
                var totalHalftones = this.HarmonicSystem.HalftonesForInterval(this.SystemAltitude);
                var roundHalfTones = (int)Math.Round(totalHalftones);
                var halftones = Math.Abs(totalHalftones - roundHalfTones) < tolerance ? roundHalfTones : (int)totalHalftones;

                return halftones;
            }
        }
        #endregion

        #region Static operators
        //// TICS rule 7@526: Reference types should not override the equality operator (==)
        //// public static bool operator ==(MusicalPitch pitch1, MusicalPitch pitch2) { return object.Equals(pitch1, pitch2);  }
        //// public static bool operator !=(MusicalPitch pitch1, MusicalPitch pitch2) { return !object.Equals(pitch1, pitch2);  }
        //// but TICS rule 7@530: Class implements interface 'IComparable' but does not implement '==' and '!='.

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="pitch1">The pitch1.</param>
        /// <param name="pitch2">The pitch2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <(MusicalPitch pitch1, MusicalPitch pitch2) {
            if (pitch1 != null && pitch2 != null) {
                return pitch1.SystemAltitude < pitch2.SystemAltitude;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="pitch1">The pitch1.</param>
        /// <param name="pitch2">The pitch2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >(MusicalPitch pitch1, MusicalPitch pitch2) {
            if (pitch1 != null && pitch2 != null) {
                return pitch1.SystemAltitude > pitch2.SystemAltitude;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="pitch1">The pitch1.</param>
        /// <param name="pitch2">The pitch2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator <=(MusicalPitch pitch1, MusicalPitch pitch2) {
            if (pitch1 != null && pitch2 != null) {
                return pitch1.SystemAltitude <= pitch2.SystemAltitude;
            }

            return false;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="pitch1">The pitch1.</param>
        /// <param name="pitch2">The pitch2.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static bool operator >=(MusicalPitch pitch1, MusicalPitch pitch2) {
            if (pitch1 != null && pitch2 != null) {
                return pitch1.SystemAltitude >= pitch2.SystemAltitude;
            }

            return false;
        }

        #endregion

        #region Public static methods
        /// <summary>
        /// Real Octave Number.
        /// </summary>
        /// <param name="octave">Musical octave.</param>
        /// <returns> Returns value. </returns>
        public static int RealOctaveNumber(int octave) {
            checked {
                return octave - 4;
            }
        }
        #endregion

        #region Public methods
        /// <summary> Makes a deep copy of the MusicalPitch object. </summary>
        /// <returns> Returns object. </returns>
        public object Clone() {
            return new MusicalPitch(this.HarmonicSystem, this.Octave, this.Element);
        }

        /// <summary> Tests equality with the given pitch. </summary>
        /// <param name="pitch">Musical pitch.</param>
        /// <returns> Returns value. </returns>
        public bool IsEqualTo(MusicalPitch pitch) {
            Contract.Requires(pitch != null);
            if (pitch == null) {
                return false;
            }

            return (this.Octave == pitch.Octave) && (this.Element == pitch.Element);
        }
        #endregion

        #region Comparison
        /// <summary> Comparer of pitches. </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns> Returns value. </returns>
        public int CompareTo(object obj) {
            //// This kills the DataGrid                 
            //// throw new ArgumentException("Object is not a MusicalPitch");
            return obj is MusicalPitch mi ? this.SystemAltitude.CompareTo(mi.SystemAltitude) : 0;
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
            return this.SystemAltitude.GetHashCode();
        }
        #endregion

        #region Altitude
        /// <summary> Sets the pitch location. </summary>
        /// <param name="octave">Musical octave.</param>        
        /// <param name="element">Element of system.</param>        
        public void SetValues(short octave, byte element) {
            this.Octave = octave;
            this.Element = element;
            this.RecomputeAltitude();
        }

        /// <summary>
        /// Inverts the altitude.
        /// </summary>
        /// <param name="harmonicOrder">The harmonic order.</param>
        public void InvertAltitude(byte harmonicOrder) {
            this.SetAltitude((MaxOctave * harmonicOrder) - this.SystemAltitude);
        }

        /// <summary> Sets the pitch location number. </summary>
        /// <param name="givenAltitude">A real system altitude from zero.</param>
        public void SetAltitude(int givenAltitude) {
            this.SystemAltitude = givenAltitude;
            this.Octave = (short)(givenAltitude / this.HarmonicSystem.Order);
            this.Element = (byte)(givenAltitude % this.HarmonicSystem.Order);
        }

        /// <summary> Returns the pitch octave location. </summary>
        /// <param name="baseAltitude">Base of musical pitch measurement.</param>
        /// <returns> Returns value. </returns>
        public float OctaveAltitude(float baseAltitude) {
            //// return (this.Octave + ((float)this.Element / this.HarmonicSystem.Order)) - baseAltitude;
            //// Not to be exact, but quick
            return this.Octave - baseAltitude;
        }

        /// <summary>
        /// Inverts the octave.
        /// </summary>
        public void InvertOctave() {
            this.Octave = (byte)(MaxOctave - this.Octave);
            this.RecomputeAltitude();
        }

        /// <summary>
        /// Sets the octave.
        /// </summary>
        /// <param name="givenOctave">The given octave.</param>
        public void SetOctave(int givenOctave) {
            this.Octave = (byte)givenOctave;
            this.RecomputeAltitude();
        }

        /// <summary>
        /// Sets the element.
        /// </summary>
        /// <param name="givenElement">The given element.</param>
        public void SetElement(int givenElement) {
            this.Element = (byte)givenElement;
            this.RecomputeAltitude();
        }

        /// <summary> Shifts octave by given number. </summary>
        /// <param name="givenShift">An octave shift.</param>
        public void ShiftOctave(int givenShift) {
            this.Octave = (short)(this.Octave + givenShift);
            this.RecomputeAltitude();
        }

        /// <summary> Shifts element by given number. </summary>
        /// <param name="givenShift">A shift of pitch element.</param>
        public void ShiftElement(int givenShift) {
            this.SetAltitude(this.SystemAltitude + givenShift);
        }

        /// <summary>
        /// Moves from edges.
        /// </summary>
        /// <param name="minNote">The min note.</param>
        /// <param name="maxNote">The max note.</param>
        public void MoveFromEdges(byte minNote, byte maxNote) {
            if (this.SystemAltitude < minNote) {
                this.ShiftElement(minNote - this.SystemAltitude);
            }

            if (this.SystemAltitude > maxNote) {
                this.ShiftElement(maxNote - this.SystemAltitude);
            }
        }
        #endregion

        #region Relations
        /// <summary> Compute interval from the given pitch. </summary>
        /// <param name="givenPitch">Musical pitch.</param>
        /// <returns> Returns value. </returns>
        public int IntervalFrom(MusicalPitch givenPitch) {
            if (givenPitch == null) {
                return 0;
            }

            return this.SystemAltitude - givenPitch.SystemAltitude;
        }

        /// <summary> Compute distance from the given pitch. </summary>
        /// <param name="givenPitch">Musical pitch.</param>
        /// <returns> Returns value. </returns>
        public int DistanceFrom(MusicalPitch givenPitch) {
            var interval = this.IntervalFrom(givenPitch);
            Contract.Assume(interval > short.MinValue);
            return Math.Abs(interval);
        }

        /// <summary> Compute formal distance from the given pitch. </summary>
        /// <param name="givenPitch">Musical pitch.</param>
        /// <returns> Returns value. </returns>
        public int FormalDistanceFrom(MusicalPitch givenPitch) {
            var sysLength = givenPitch?.Element - this.Element ?? 0;

            var interval = this.HarmonicSystem.FormalMedianLength(sysLength);
            Contract.Assume(interval > short.MinValue);
            var formalLength = (byte)Math.Abs(interval);
            return formalLength;
        }
        #endregion

        #region MIDI support
        /// <summary> Compute MIDI pitch bend for micro intervals. </summary>
        /// <returns> Returns value. </returns>
        public byte MidiPitchBend() {
            byte bend = 64;
            if (this.HarmonicSystem.Order == DefaultValue.HarmonicOrder) {
                return bend;
            }

            var diff = (float)(this.Halftones - Math.Floor((double)this.Halftones));
            var bendDiff = (byte)Math.Floor(64 * diff);
            bend += bendDiff;
            return bend;
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Sets Xml representation.
        /// </summary>
        /// <param name="element">Xml Element.</param>
        public void SetXAttribute(XAttribute element) {  //// XElement
            Contract.Requires(element != null);
            //// if (element == null) { return; }

            var altitude = (int?)element;   ////element.Element("Pitch");
            if (altitude != null) {
                this.SetAltitude((int)altitude);
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            //// s.Append(this.Element.ToString("D",System.Globalization.CultureInfo.CurrentCulture.NumberFormat));
            var symbol = this.HarmonicSystem.Symbol(this.Element, true);
            var normalOctave = RealOctaveNumber(this.Octave);
            if (normalOctave < 0 && normalOctave > short.MinValue && symbol != null) {
                symbol = symbol.ToUpper(CultureInfo.CurrentCulture);
                normalOctave = -normalOctave;
            }

            s.Append(symbol);
            s.Append(normalOctave.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
            return s.ToString();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Re-computes the altitude.
        /// </summary>
        private void RecomputeAltitude() {
            this.SystemAltitude = (this.Octave * this.HarmonicSystem.Order) + this.Element;
        }
        #endregion
    }
}
