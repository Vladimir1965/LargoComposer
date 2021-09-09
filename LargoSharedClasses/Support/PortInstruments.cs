// <copyright file="PortInstruments.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// MusicalInstruments Port.
    /// </summary>
    public static class PortInstruments {
        #region Public properties

        /// <summary>
        /// Gets or sets the melodic instruments.
        /// </summary>
        /// <value>
        /// The melodic instruments.
        /// </value>
        public static IList<MelodicInstrument> MelodicInstruments { get; set; }

        /// <summary>
        /// Gets or sets the rhythmic instruments.
        /// </summary>
        /// <value>
        /// The rhythmic instruments.
        /// </value>
        public static IList<RhythmicInstrument> RhythmicInstruments { get; set; }

        #endregion

        /// <summary>
        /// Loads from Xml file.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public static void LoadInstruments(string givenPath) {
            string path = Path.Combine(givenPath, @"MelodicInstruments.xml");
            var xinstruments = XmlSupport.GetXDocRoot(path);
            if (xinstruments != null && xinstruments.Name == "Instruments") {
                MelodicInstruments = ReadMelodicInstruments(xinstruments);
            }

            path = Path.Combine(givenPath, @"RhythmicInstruments.xml");
            xinstruments = XmlSupport.GetXDocRoot(path);
            if (xinstruments != null && xinstruments.Name == "Instruments") {
                RhythmicInstruments = ReadRhythmicInstruments(xinstruments);
            }

            OrchestraChecker.Singleton.SetMelodicInstruments(MelodicInstruments);
        }

        /// <summary>
        /// Gets the melodic instrument.
        /// </summary>
        /// <param name="givenNumber">The given number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static MelodicInstrument GetMelodicInstrument(byte givenNumber) {
            var instr = (from obj in MelodicInstruments where obj.Id == givenNumber select obj).FirstOrDefault();
            return instr;
        }

        /// <summary>
        /// Gets the rhythmic instrument.
        /// </summary>
        /// <param name="givenNumber">The given number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static RhythmicInstrument GetRhythmicInstrument(byte givenNumber) {
            var instr = (from obj in RhythmicInstruments where obj.Id == givenNumber select obj).FirstOrDefault();
            return instr;
        }

        /// <summary>
        /// Gets the group of melodic instrument.
        /// </summary>
        /// <param name="instrumentNumber">The instrument number.</param>
        /// <returns> Returns value. </returns>
        public static InstrumentGroupMelodic GetGroupOfMelodicInstrument(byte instrumentNumber) {
            var instruments = MelodicInstruments;
            if (instruments == null) {
                return InstrumentGroupMelodic.None;
            }

            var arr = from c in MelodicInstruments
                      where c.Id == instrumentNumber
                      select c.InstrumentGroup;
            var instrumentGroup = arr.FirstOrDefault();

            return (InstrumentGroupMelodic)instrumentGroup;
        }

        /// <summary>
        /// Gets the group of rhythmic instrument.
        /// </summary>
        /// <param name="instrumentNumber">The instrument number.</param>
        /// <returns> Returns value. </returns>
        public static InstrumentGroupRhythmic GetGroupOfRhythmicInstrument(byte instrumentNumber) {
            var instruments = RhythmicInstruments;
            if (instruments == null) {
                return InstrumentGroupRhythmic.None;
            }

            var arr = from c in RhythmicInstruments
                      where c.Id == instrumentNumber
                      select c.InstrumentGroup; //// MidiSection
            var instrumentGroup = arr.FirstOrDefault();
            return (InstrumentGroupRhythmic)instrumentGroup;
        }

        /// <summary>
        /// Sections the instruments.
        /// </summary>
        /// <param name="instrumentGroup">The instrument group.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static IList<int> MelodicInstrumentsOfGroup(InstrumentGroupMelodic instrumentGroup) {
            var instruments = MelodicInstruments;
            if (instruments == null) {
                return null;
            }

            var list = new List<int>();
            foreach (var instr in instruments) {
                if (instr.InstrumentGroup == (int)instrumentGroup) {
                    list.Add(instr.Id);
                }
            }

            return list;
        }

        /// <summary>
        /// Prepares the rhythmic instruments.
        /// </summary>
        /// <param name="instrumentGroup">The instrument group.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static IList<KeyValuePair> PrepareRhythmicInstruments(InstrumentGroup instrumentGroup) {
            if ((int)instrumentGroup <= (int)InstrumentGroup.MelodicDrums) {
                return null;
            }

            IList<KeyValuePair> pairs = new List<KeyValuePair>();
            var instrNumbers = from instr in RhythmicInstruments
                            where instr.InstrumentGroup == (int)instrumentGroup
                               select instr.Id;
            //// DataLink.BridgeInstrumentation.GetRhythmicInstrumentsOfGroup(instrumentGroup);

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var instrNum in instrNumbers) {
                var instrName = LocalizedMusic.String("RhyInstr" + instrNum.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
                var pair = new KeyValuePair(instrNum, instrName);
                pairs.Add(pair);
            }

            return pairs;
        }

        /// <summary>
        /// Prepares the melodic instruments.
        /// </summary>
        /// <param name="instrumentGroup">The instrument group.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static IList<KeyValuePair> PrepareMelodicInstruments(InstrumentGroup instrumentGroup) {
            IList<KeyValuePair> pairs = new List<KeyValuePair>();

            var instrNumbers = from instr in MelodicInstruments
                            where instr.InstrumentGroup == (byte)instrumentGroup
                            select instr.Id;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var instrNum in instrNumbers) {
                var instrName = LocalizedMusic.String("MelInstr" + instrNum.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
                var pair = new KeyValuePair(instrNum, instrName);
                pairs.Add(pair);
            }

            return pairs;
        }

        /// <summary>
        /// Reads the melodic instruments.
        /// </summary>
        /// <param name="xinstruments">The instruments Xml.</param>
        /// <returns> Returns value. </returns>
        private static IList<MelodicInstrument> ReadMelodicInstruments(XElement xinstruments) {
            Contract.Requires(xinstruments != null);

            var list = new List<MelodicInstrument>();
            var xelements = xinstruments.Elements("Instrument").ToList();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var xelement in xelements) {
                var id = (int)xelement.Element("Id");
                var name = LocalizedMusic.String("MelInstr" + id.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
                //// var name = (string)xelement.Element("Name");
                var midiSection = (int)xelement.Element("MidiSection");
                var instrumentGroup = (int)xelement.Element("InstrumentGroup");
                var bass = (bool)xelement.Element("Bass");
                var middle = (bool)xelement.Element("Middle");
                var high = (bool)xelement.Element("High");
                var minTone = (int)xelement.Element("MinTone");
                var minToneSymbol = (string)xelement.Element("MinToneSymbol");
                var maxTone = (int)xelement.Element("MaxTone");
                var maxToneSymbol = (string)xelement.Element("MaxToneSymbol");

                var instrument = new MelodicInstrument {
                    Id = id,
                    Name = name,
                    MidiSection = midiSection,
                    InstrumentGroup = instrumentGroup,
                    Bass = bass,
                    Middle = middle,
                    High = high,
                    MinTone = minTone,
                    MinToneSymbol = minToneSymbol,
                    MaxTone = maxTone,
                    MaxToneSymbol = maxToneSymbol
                };

                list.Add(instrument);
            }

            return list;
        }

        /// <summary>
        /// Reads the rhythmic instruments.
        /// </summary>
        /// <param name="xinstruments">The instruments Xml.</param>
        /// <returns> Returns value. </returns>
        private static IList<RhythmicInstrument> ReadRhythmicInstruments(XElement xinstruments) {
            Contract.Requires(xinstruments != null);

            var list = new List<RhythmicInstrument>();
            var xelements = xinstruments.Elements("Instrument").ToList();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var xelement in xelements) {
                var id = (int)xelement.Element("Id");
                var name = LocalizedMusic.String("RhyInstr" + id.ToString("D", CultureInfo.CurrentCulture.NumberFormat));
                //// var name = (string)xelement.Element("Name");
                var midiSection = (int)xelement.Element("MidiSection");
                var instrumentGroup = (int)xelement.Element("InstrumentGroup");
                var bass = (bool)xelement.Element("Bass");
                var middle = (bool)xelement.Element("Middle");
                var high = (bool)xelement.Element("High");

                var instrument = new RhythmicInstrument {
                    Id = id,
                    Name = name,
                    InstrumentGroup = instrumentGroup,
                    MidiSection = midiSection,
                    Bass = bass,
                    Middle = middle,
                    High = high
                };

                list.Add(instrument);
            }

            return list;
        }
    }
}
