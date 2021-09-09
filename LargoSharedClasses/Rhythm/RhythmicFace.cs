// <copyright file="RhythmicFace.cs" company="Traced-Ideas, Czech republic">
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

namespace LargoSharedClasses.Rhythm
{
    /// <summary> A rhythmic face. </summary>
    public class RhythmicFace
    {
        /// <summary> Initializes a new instance of the <see cref="RhythmicFace" /> class. </summary>
        public RhythmicFace() {
        }

        /// <summary> Initializes a new instance of the <see cref="RhythmicFace" /> class. </summary>
        /// <param name="xface"> The element face. </param>
        public RhythmicFace(XElement xface) {
            this.Name = (string)xface.Attribute("Name");
            this.StructuralCode = (string)xface.Attribute("Code");
            if (this.StructuralCode?.Length <= 3) { //// !??
                this.DetermineStructuralCode();
            }

            this.BeatLevel = XmlSupport.ReadByteAttribute(xface.Attribute("BeatLevel"));
            this.ToneLevel = XmlSupport.ReadByteAttribute(xface.Attribute("ToneLevel"));
            this.RhythmicTension = XmlSupport.ReadByteAttribute(xface.Attribute("RhythmicTension"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhythmicFace"/> class.
        /// </summary>
        /// <param name="givenCode">The given code.</param>
        /// <param name="givenBeatLevel">The given beat level.</param>
        /// <param name="givenToneLevel">The given tone level.</param>
        /// <param name="givenRhythmicTension">The given rhythmic tension.</param>
        public RhythmicFace(string givenCode, byte givenBeatLevel, byte givenToneLevel, byte givenRhythmicTension) {
            this.StructuralCode = givenCode;
            this.BeatLevel = givenBeatLevel;
            this.ToneLevel = givenToneLevel;
            this.RhythmicTension = givenRhythmicTension;
            this.DetermineName();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the beat level.
        /// </summary>
        /// <value>
        /// The beat level.
        /// </value>
        public byte BeatLevel { get; set; }

        /// <summary>
        /// Gets or sets the tone level.
        /// </summary>
        /// <value>
        /// The tone level.
        /// </value>
        public byte ToneLevel { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic tension.
        /// </summary>
        /// <value>
        /// The rhythmic tension.
        /// </value>
        public byte RhythmicTension { get; set; }
        #endregion 

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                var xe = new XElement(
                                "RhythmicFace",
                                new XAttribute("Name", this.Name ?? string.Empty),
                                new XAttribute("Code", this.StructuralCode ?? string.Empty),
                                new XAttribute("BeatLevel", this.BeatLevel),
                                new XAttribute("ToneLevel", this.ToneLevel),
                                new XAttribute("RhythmicTension", this.RhythmicTension));
                return xe;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary> Gets or sets the structural code. </summary>
        /// <value> The structural code. </value>
        public string StructuralCode { get; set; }

        /// <summary>
        /// Determines the name.
        /// </summary>
        public void DetermineName() {
            if (this.ToneLevel == 0) {
                this.Name = string.Empty;
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(this.RhythmicTension == 0 ? "Regular" : "Irregular");

            switch (this.ToneLevel) {
                case 1: {
                        sb.Append(" whole-note");
                        break;
                    }

                case 2: {
                        sb.Append(" half-notes");
                        break;
                    }

                case 3: {
                        sb.Append(" triads");
                        break;
                    }

                case 4: {
                        sb.Append(" quarters");
                        break;
                    }

                case 6: {
                        sb.Append(" sixths");
                        break;
                    }

                case 8: {
                        sb.Append(" eighths");
                        break;
                    }

                case 12: {
                        sb.Append(" twelfths");
                        break;
                    }

                default: {
                        sb.AppendFormat("{0}-tones", this.ToneLevel);
                        break;
                    }
            }

            this.Name = sb.ToString();
        }

        /// <summary>
        /// Determines the structural code.
        /// </summary>
        public void DetermineStructuralCode() {
            switch (this.Name) {
                case "Regular whole-note": {
                        this.StructuralCode = "1,23*0";
                        break;
                    }

                case "Regular half-notes": {
                        this.StructuralCode = "1,11*0,1,11*0";
                        break;
                    }

                case "Regular triads": {
                        this.StructuralCode = "1,7*0,1,7*0,1,7*0";
                        break;
                    }

                case "Regular quarters": {
                        this.StructuralCode = "1,5*0,1,5*0,1,5*0,1,5*0";
                        break;
                    }

                case "Regular sixths": {
                        this.StructuralCode = "1,3*0,1,3*0,1,3*0,1,3*0,1,3*0,1,3*0";
                        break;
                    }

                case "Regular eighths": {
                        this.StructuralCode = "1,2*0,1,2*0,1,2*0,1,2*0,1,2*0,1,2*0,1,2*0,1,2*0";
                        break;
                    }

                case "Regular twelfths": {
                        this.StructuralCode = "1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0";
                        break;
                    }

                case "Empty": {
                        this.StructuralCode = string.Empty;
                        break;
                    }
            }
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
            var tmc = new RhythmicFace(this.StructuralCode, this.BeatLevel, this.ToneLevel, this.RhythmicTension) {
                Name = this.Name
            };

            return tmc;
        }
    }
}