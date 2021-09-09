// <copyright file="SettingsComposition.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// Settings of Composition Engine.
    /// </summary>
    public class SettingsComposition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsComposition"/> class.
        /// </summary>
        public SettingsComposition() {
            this.TypeOfRules = MusicalRulesType.StandardMusicalRules;
            this.CorrectResultPitch = true;
            this.CorrectOctaves = true;
            this.CorrectLoudness = false;
            this.HighlightMelodicVoices = false;
            this.NoteLowest = DefaultValue.LowestNote;
            this.NoteHighest = DefaultValue.HighestNote;
            this.HarmonicModalization = HarmonicModalizationType.Consecutive;
        }

        #region Properties - Xml

        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement markSettings = new XElement("Composition");
                markSettings.Add(new XAttribute("TypeOfRules", this.TypeOfRules));
                markSettings.Add(new XAttribute("IndividualizeMelodicVoices", this.IndividualizeMelodicVoices));
                markSettings.Add(new XAttribute("HighlightMelodicVoices", this.HighlightMelodicVoices));
                markSettings.Add(new XAttribute("CorrectResultPitch", this.CorrectResultPitch));
                markSettings.Add(new XAttribute("CorrectOctaves", this.CorrectOctaves));
                markSettings.Add(new XAttribute("CorrectLoudness", this.CorrectLoudness));
                markSettings.Add(new XAttribute("NoteLowest", this.NoteLowest));
                markSettings.Add(new XAttribute("NoteHighest", this.NoteHighest));

                return markSettings;
            }
        }
        #endregion

        #region Properties - Engine
        /// <summary>
        /// Gets or sets the type of rules.
        /// </summary>
        /// <value>
        /// The type of rules.
        /// </value>
        public MusicalRulesType TypeOfRules { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [individualize melodic voices].
        /// </summary>
        /// <value>
        /// <c>true</c> if [individualize melodic voices]; otherwise, <c>false</c>.
        /// </value>
        public bool IndividualizeMelodicVoices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [highlight melodic voices].
        /// </summary>
        /// <value>
        /// <c>true</c> if [highlight melodic voices]; otherwise, <c>false</c>.
        /// </value>
        public bool HighlightMelodicVoices { get; set; }

        #endregion

        #region Properties - Final corrections
        /// <summary>
        /// Gets or sets a value indicating whether [correct result pitch].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [correct result pitch]; otherwise, <c>false</c>.
        /// </value>
        public bool CorrectResultPitch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [correct octaves].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [correct octaves]; otherwise, <c>false</c>.
        /// </value>
        public bool CorrectOctaves { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [correct loudness].
        /// </summary>
        /// <value>
        ///   <c>True</c> if [correct loudness]; otherwise, <c>false</c>.
        /// </value>
        public bool CorrectLoudness { get; set; }

        /// <summary>
        /// Gets or sets the note lowest.
        /// </summary>
        /// <value>
        /// The note lowest.
        /// </value>
        public byte NoteLowest { get; set; }

        /// <summary>
        /// Gets or sets the note highest.
        /// </summary>
        /// <value>
        /// The note highest.
        /// </value>
        public byte NoteHighest { get; set; }

        #endregion

        #region Properties - Modality
        /// <summary>
        /// Gets or sets the forced modality.
        /// </summary>
        /// <value>
        /// The forced modality.
        /// </value>
        public HarmonicModality ForcedModality { get; set; }

        /// <summary>
        /// Gets or sets Harmonic Modalization - Used while generation of music.
        /// </summary>
        /// <value> Property description. </value>
        public HarmonicModalizationType HarmonicModalization { get; set; }

        #endregion

        #region Properties - Derived
        /// <summary>
        /// Gets of musical rule - Used while generation of music.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public MusicalRules Rules {
            get {
                var typeOfRules = this.TypeOfRules;

                var rt = typeOfRules;
                switch (rt) {
                    case MusicalRulesType.StandardMusicalRules: {
                            return MusicalRules.NewStandardMusicalRules();
                        }

                    case MusicalRulesType.SimpleHarmonicRules: {
                            return MusicalRules.NewSimpleHarmonicMusicalRules();
                        }

                    case MusicalRulesType.MusicalImpulseRules: {
                            return MusicalRules.NewMusicalImpulseRules();
                        }

                    case MusicalRulesType.None:
                        break;
                        //// resharper default: break;
                }

                return MusicalRules.NewStandardMusicalRules();
            }
        }
        #endregion

        /// <summary>
        /// Sets the x element.
        /// </summary>
        /// <param name="markSettings">The mark settings.</param>
        public void SetXElement(XElement markSettings) {
            Contract.Requires(markSettings != null);
            if (markSettings == null) {
                return;
            }

            this.TypeOfRules = DataEnums.ReadAttributeMusicalRulesType(markSettings.Attribute("TypeOfRules"));
            this.IndividualizeMelodicVoices = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("IndividualizeMelodicVoices"));
            this.HighlightMelodicVoices = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("HighlightMelodicVoices"));
            this.CorrectResultPitch = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("CorrectResultPitch"));
            this.CorrectOctaves = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("CorrectOctaves"));
            this.CorrectLoudness = XmlSupport.ReadBooleanAttribute(markSettings.Attribute("CorrectLoudness"));
            this.NoteLowest = XmlSupport.ReadByteAttribute(markSettings.Attribute("NoteLowest"));
            this.NoteHighest = XmlSupport.ReadByteAttribute(markSettings.Attribute("NoteHighest"));

            if (this.NoteLowest >= this.NoteHighest || this.NoteHighest < DefaultValue.LowestNote || this.NoteLowest > DefaultValue.HighestNote) {
                this.NoteLowest = DefaultValue.LowestNote; 
                this.NoteHighest = DefaultValue.HighestNote; 
            }
        }
    }
}