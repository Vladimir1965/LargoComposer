// <copyright file="PortCatalogs.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Xml Data Reader.
    /// </summary>
    public class PortCatalogs {
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly PortCatalogs InternalSingleton = new PortCatalogs();

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PortCatalogs"/> class from being created.
        /// </summary>
        private PortCatalogs() {
        }
        #endregion

        #region Public static properties
        /// <summary>
        /// Gets the CatalogsPorter Singleton.
        /// </summary>
        /// <value> Property description. </value>/// 
        public static PortCatalogs Singleton {
            get {
                Contract.Ensures(Contract.Result<PortCatalogs>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton PortCatalogs is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the musical notators.
        /// </summary>
        /// <value>
        /// The musical notators.
        /// </value>
        public IList<MusicalNotator> MusicalNotators { get; set; }

        /// <summary>
        /// Gets or sets the tone structures.
        /// </summary>
        /// <value>
        /// The tone structures.
        /// </value>
        public IList<ToneStructure> HarmonicEssence { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic essence.
        /// </summary>
        /// <value>
        /// The rhythmic essence.
        /// </value>
        public IList<RhythmicModality> RhythmicEssence { get; set; }

        /// <summary>
        /// Gets or sets the orchestra essence.
        /// </summary>
        /// <value>
        /// The orchestra essence.
        /// </value>
        public IList<OrchestraUnit> OrchestraEssence { get; set; }
        #endregion

        #region Static public methods

        /// <summary>
        /// Rhythmical faces.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static List<RhythmicFace> RhythmicFaces(string givenPath) {
            string filePath = Path.Combine(givenPath, "RhythmicFaces.xml"); 
            var list = new List<RhythmicFace>();
            var root = XmlSupport.GetXDocRoot(filePath);
            if (root != null && root.Name == "RhythmicFaces") {
                var xfaces = root.Elements("Face");
                foreach (var xface in xfaces) {
                    var face = new RhythmicFace(xface);
                    list.Add(face);
                }
            }

            return list;
        }

        /// <summary>
        /// Melodical faces.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static List<MelodicFace> MelodicFaces(string givenPath) {
            string filePath = Path.Combine(givenPath, "MelodicFaces.xml");
            var list = new List<MelodicFace>();
            var root = XmlSupport.GetXDocRoot(filePath);
            if (root != null && root.Name == "MelodicFaces") {
                var xfaces = root.Elements("MelodicFace");
                foreach (var xface in xfaces) {
                    var face = new MelodicFace(xface);
                    list.Add(face);
                }
            }

            return list;
        }

        /// <summary>
        /// Defaults the harmonic structures.
        /// </summary>
        /// <param name="levelFrom">The level from.</param>
        /// <param name="levelTo">The level to.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static List<HarmonicStructure> DefaultHarmonicStructures(int levelFrom, int levelTo) {
            var system = HarmonicSystem.GetHarmonicSystem(DefaultValue.HarmonicOrder);
            var structures = PortCatalogs.Singleton.HarmonicEssence;
            if (structures == null) {
                return null;
            }

            var chords = (from ms in structures 
                          where ms.Level >= levelFrom && ms.Level <= levelTo 
                          select ms).ToList(); //// ms.Shortcut.Length > 0 && 
            var list = new List<HarmonicStructure>();
            foreach (var chord in chords) {
                var hs = new HarmonicStructure(system, chord.StructuralCode) {
                    ClassCode = chord.ClassNumber.ToString()
                };

                hs.Shortcut = !string.IsNullOrWhiteSpace(chord.Shortcut) ? chord.Shortcut : hs.ToneSchema;
                hs.DetermineBehavior();
                list.Add(hs);
            }

            return list;
        }

        /// <summary>
        /// Saves the blocks.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        /// <param name="orchestraUnits">The orchestra units.</param>
        public static void SaveOrchestraEssence(string givenPath, IList<OrchestraUnit> orchestraUnits) {
            XElement xlist = new XElement("OrchestraEssence");
            foreach (var orchestraUnit in orchestraUnits) {
                if (orchestraUnit.ListVoices.Count == 0) {
                    continue;
                }

                var xunit = orchestraUnit.GetXElement;
                xlist.Add(xunit);
            }

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xlist);
            var fileName = "OrchestraEssence.xml";
            var filepath = Path.Combine(givenPath, fileName); 
            xdoc.Save(filepath);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the orchestra unit.
        /// </summary>
        /// <param name="givenName">Name of the given.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public OrchestraUnit GetOrchestraUnit(string givenName) {
            if (this.OrchestraEssence == null) {
                return null;
            }

            var unit = (from data in this.OrchestraEssence
                        where data.Name == givenName
                        select data).FirstOrDefault();
            return unit;
        }

        /// <summary>
        /// Loads from Xml file.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public void ReadXmlFiles(string givenPath) {
            PortInstruments.LoadInstruments(givenPath);

            string path = Path.Combine(givenPath, @"MusicalNotators.xml");
            if (File.Exists(path)) {
                var xnotators = XmlSupport.GetXDocRoot(path);
                if (xnotators != null && xnotators.Name == "Notators") {
                    this.MusicalNotators = ReadMusicalNotators(xnotators);
                }
            }

            path = Path.Combine(givenPath, @"HarmonicEssence.xml");
            var xtoneStructs = XmlSupport.GetXDocRoot(path);
            if (xtoneStructs != null && xtoneStructs.Name == "HarmonicEssence") {
                this.HarmonicEssence = ReadHarmonicEssence(xtoneStructs);
            }

            path = Path.Combine(givenPath, @"RhythmicEssence.xml");
            var xrhyShapes = XmlSupport.GetXDocRoot(path);
            if (xrhyShapes != null && xrhyShapes.Name == "RhythmicEssence") {
                this.RhythmicEssence = ReadRhythmicEssence(xrhyShapes);
            }

            path = Path.Combine(givenPath, @"OrchestraEssence.xml");
            var xorchestra = XmlSupport.GetXDocRoot(path);
            if (xorchestra != null && xorchestra.Name == "OrchestraEssence") {
                this.OrchestraEssence = ReadOrchestraEssence(xorchestra);
            }
        }

        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "Catalogs Porter";
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Reads the musical notators.
        /// </summary>
        /// <param name="xnotators">The notators Xml.</param>
        /// <returns> Returns value. </returns>
        private static IList<MusicalNotator> ReadMusicalNotators(XElement xnotators) {
            Contract.Requires(xnotators != null);

            var list = new List<MusicalNotator>();
            var xelements = xnotators.Elements("Notator").ToList();

            foreach (var xelement in xelements) {
                var notator = new MusicalNotator();
                var id = (int)xelement.Element("Id");
                var name = (string)xelement.Element("Name");
                var path = (string)xelement.Element("Path");
                var midiFiles = (bool)xelement.Element("MidiFiles");
                var mxlFiles = (bool)xelement.Element("MxlFiles");

                notator.Id = id;
                notator.Name = name;
                notator.Path = path;
                notator.MidiFiles = midiFiles;
                notator.MxlFiles = mxlFiles;

                list.Add(notator);
            }

            return list;
        }

        /// <summary>
        /// Reads the tone structures.
        /// </summary>
        /// <param name="markEssence">The xml with tone structs.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static IList<ToneStructure> ReadHarmonicEssence(XElement markEssence) {
            Contract.Requires(markEssence != null);

            var list = new List<ToneStructure>();
            var xelements = markEssence.Elements("Structure").ToList();

            foreach (var xelement in xelements) {
                var xchord = xelement.Element("Chord");
                var xmodality = xelement.Element("Modality");

                var number = XmlSupport.ReadIntegerAttribute(xelement.Attribute("Number"));
                var classNumber = XmlSupport.ReadIntegerAttribute(xelement.Attribute("ClassNumber"));
                var level = XmlSupport.ReadByteAttribute(xelement.Attribute("Level"));
                var tones = XmlSupport.ReadStringAttribute(xelement.Attribute("Tones"));
                var code = XmlSupport.ReadStringAttribute(xelement.Attribute("Code"));

                var toneStructure = new ToneStructure() {
                    Number = number,
                    ClassNumber = classNumber,
                    Level = level,
                    Tones = tones,
                    StructuralCode = code
                };

                if (xchord != null) {
                    var chordStep = XmlSupport.ReadByteAttribute(xchord.Attribute("Step"));
                    var chordBase = XmlSupport.ReadStringAttribute(xchord.Attribute("Base"));
                    var chordName = XmlSupport.ReadStringAttribute(xchord.Attribute("Names"));
                    var shortcut = XmlSupport.ReadStringAttribute(xchord.Attribute("Shortcut"));

                    toneStructure.ChordBase = chordBase;
                    toneStructure.ChordStep = chordStep;
                    toneStructure.ChordName = chordName;
                    toneStructure.Shortcut = shortcut;
                }
               
                if (xmodality != null) {
                        var modalityName = XmlSupport.ReadStringAttribute(xmodality.Attribute("Names"));
                    if (xchord != null) {
                        //// var modalityBase = XmlSupport.ReadStringAttribute(xchord.Attribute("Base"));
                    }

                    toneStructure.ModalityName = modalityName;
                    //// toneStructure.ModalityBase = modalityBase;
                }

                list.Add(toneStructure);
            }

            return list;
        }

        /// <summary>
        /// Reads the rhythmic essence.
        /// </summary>
        /// <param name="markEssence">The xml with rhythmic.</param>
        /// <returns> Returns value. </returns>
        private static IList<RhythmicModality> ReadRhythmicEssence(XElement markEssence) {
            Contract.Requires(markEssence != null);

            var list = new List<RhythmicModality>();
            var xelements = markEssence.Elements("Shape").ToList();

            foreach (var xelement in xelements) {
                var code = (string)xelement.Attribute("Code");
                var schema = (string)xelement.Attribute("Schema");
                var system = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, (byte)schema.Length);
                var modality = new RhythmicModality(system, code);
                list.Add(modality);
            }

            return list;
        }

        /// <summary>
        /// Reads the orchestra essence.
        /// </summary>
        /// <param name="markEssence">The xml with orchestra.</param>
        /// <returns> Returns value. </returns>
        private static IList<OrchestraUnit> ReadOrchestraEssence(XElement markEssence) {
            Contract.Requires(markEssence != null);

            var list = new List<OrchestraUnit>();
            var xelements = markEssence.Elements("Orchestra").ToList();

            foreach (var xelement in xelements) {
                OrchestraUnit unit = new OrchestraUnit(xelement);
                if (unit.ListVoices.Count > 0) {
                    list.Add(unit);
                }
            }

            return list;
        }
        #endregion
    }
}
