// <copyright file="DataEnums.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>DataEnums</summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Port;
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Data enumerations.
    /// </summary>
    public static class DataEnums {
        #region Private static 
        /// <summary>
        /// The list musical loudness
        /// </summary>
        private static IEnumerable<KeyValuePair> listMusicalLoudness;

        /// <summary>
        /// The list musical tempo
        /// </summary>
        private static IEnumerable<KeyValuePair> listMusicalTempo;

        /// <summary>
        /// The list musical octave
        /// </summary>
        private static IEnumerable<KeyValuePair> listMusicalOctave;

        /// <summary>
        /// The list melodic shape
        /// </summary>
        private static IEnumerable<KeyValuePair> listMelodicShape;

        /// <summary>
        /// The list melodic function
        /// </summary>
        private static IEnumerable<KeyValuePair> listMelodicFunction;
        #endregion

        #region General Musical Enums

        /// <summary> Gets List of musical octaves. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public static IEnumerable<KeyValuePair> GetListMusicalOctave => DataEnumsLocalization.ReverseListEnum(typeof(MusicalOctave), "Octave", false);

        /// <summary> Gets List of file split. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public static IEnumerable<KeyValuePair> GetListFileSplit => DataEnumsLocalization.ListEnum(typeof(FileSplit), "FileSplit", true);

        /// <summary> Gets List of orders of harmonic systems. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value.</returns>
        public static IEnumerable<KeyValuePair> GetHarmonicSystems => DataEnumsLocalization.ListEnum(typeof(HarmonicOrder), null, false);

        /// <summary> Gets List of raw tempo. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public static Collection<KeyValuePair> GetListRawTempo => DataEnumsLocalization.ListEnum(typeof(RawTempo), "RawTempo", false);

        /// <summary> Gets the list musical tempo. </summary>
        /// <value> The list musical tempo. </value>
        public static Collection<KeyValuePair> GetListMusicalTempo => DataEnumsLocalization.ListEnum(typeof(MusicalTempo), "Tempo", false);

        /// <summary> Gets List of types of melodic parts. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public static IEnumerable<KeyValuePair> GetListMelodicFunction => DataEnumsLocalization.ListEnum(typeof(MelodicFunction), "MelodicFunction", false);

        /// <summary> Gets List of types of melodic parts. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public static IEnumerable<KeyValuePair> GetListMelodicShape => DataEnumsLocalization.ListEnum(typeof(MelodicShape), "MelodicShape", false);

        /// <summary> Gets List of musical loudness. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public static IEnumerable<KeyValuePair> GetListMusicalLoudness => DataEnumsLocalization.ListEnum(typeof(MusicalLoudness), "Loud", false);
        #endregion

        #region Instruments
        /// <summary> Gets List of melodical instruments. </summary>
        /// <value> General musical property.</value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static IEnumerable<KeyValuePair> MelodicInstruments => DataEnumsLocalization.ListEnumSortedByText(typeof(MidiMelodicInstrument), "MelInstr", true);

        /// <summary> Gets List of rhythmical instruments. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public static IEnumerable<KeyValuePair> RhythmicInstruments => DataEnumsLocalization.ListEnumSortedByText(typeof(MidiRhythmicInstrument), "RhyInstr", true);

        /// <summary> Gets List of melodical instruments. </summary>
        /// <value> General musical property.</value>
        /// <returns> Returns value. </returns>
        public static IEnumerable<KeyValuePair> MelodicInstrumentGroups => DataEnumsLocalization.ListEnumSortedByText(typeof(InstrumentGroupMelodic), "InstrClass", false);

        /// <summary> Gets List of melodical instruments. </summary>
        /// <value> General musical property.</value>
        /// <returns> Returns value. </returns>
        public static IEnumerable<KeyValuePair> RhythmicInstrumentGroups => DataEnumsLocalization.ListEnumSortedByText(typeof(InstrumentGroupRhythmic), "InstrClass", false);

        /// <summary> Gets List of types of harmonic line.  </summary>
        /// <returns> Returns value. </returns>
        /// <value> General musical property.</value>
        public static IEnumerable<KeyValuePair> ListMidChannel => DataEnumsLocalization.ListEnum(typeof(MidiChannel), string.Empty, true);

        #endregion

        #region Culture
        /// <summary> Gets List of supported UICultures. </summary>
        /// <value> Property description. </value>
        /// <returns> Returns value. </returns>
        public static Collection<KeyValuePair> ListUiCulture {
            get {
                //// Object page = null;
                var list = new Collection<KeyValuePair>
                {
                    new KeyValuePair("en", LocalizedMusic.String("UIEnglish")),
                    new KeyValuePair("it", LocalizedMusic.String("UIItalian")),
                    new KeyValuePair("cs", LocalizedMusic.String("UICzech"))
                };
                /* Unused localization languages
                    new KeyValuePair("de", LocalizedMusic.String("UIGerman")),
                    new KeyValuePair("fr", LocalizedMusic.String("UIFrench")),
                    new KeyValuePair("pl", LocalizedMusic.String("UIPolish")),
                    new KeyValuePair("es", LocalizedMusic.String("UISpain"))
                */
                return list;
            }
        }
        #endregion

        #region Public static
        /// <summary>
        /// Gets the list musical loudness.
        /// </summary>
        /// <value>
        /// The list musical loudness.
        /// </value>
        public static IEnumerable<KeyValuePair> ListMusicalLoudness => listMusicalLoudness ?? (listMusicalLoudness = DataEnums.GetListMusicalLoudness);

        /// <summary>
        /// Gets the list musical tempo.
        /// </summary>
        /// <value>
        /// The list musical tempo.
        /// </value>
        public static IEnumerable<KeyValuePair> ListMusicalTempo => listMusicalTempo ?? (listMusicalTempo = DataEnums.GetListMusicalTempo);

        /// <summary>
        /// Gets the list musical octave.
        /// </summary>
        /// <value>
        /// The list musical octave.
        /// </value>
        public static IEnumerable<KeyValuePair> ListMusicalOctave => listMusicalOctave ?? (listMusicalOctave = DataEnums.GetListMusicalOctave);

        /// <summary>
        /// Gets the list melodic shape.
        /// </summary>
        /// <value>
        /// The list melodic shape.
        /// </value>
        public static IEnumerable<KeyValuePair> ListMelodicShape => listMelodicShape ?? (listMelodicShape = DataEnums.GetListMelodicShape);

        /// <summary>
        /// Gets the list melodic function.
        /// </summary>
        /// <value>
        /// The list melodic function.
        /// </value>
        public static IEnumerable<KeyValuePair> ListMelodicFunction => listMelodicFunction ?? (listMelodicFunction = DataEnums.GetListMelodicFunction);

        /// <summary>
        /// Lists the limited tempo.
        /// </summary>
        /// <param name="lowestValue">The lowest value.</param>
        /// <param name="highestValue">The highest value.</param>
        /// <returns> Returns value.</returns>
        public static Collection<KeyValuePair> ListLimitedTempo(int lowestValue, int highestValue) {
            return DataEnumsLocalization.ListLimitedEnum(typeof(MusicalTempo), "Tempo", lowestValue, highestValue);
        }
        #endregion

        #region Public Enum Attributes methods
        /// <summary>
        /// Reads the attribute file split.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static FileSplit ReadAttributeFileSplit(XAttribute attribute) {
            if (attribute == null) {
                return FileSplit.None;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? FileSplit.None : (FileSplit)Enum.Parse(typeof(FileSplit), svalue);

            return value;
        }

        /// <summary>
        /// Reads the attribute purpose of line.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static LinePurpose ReadAttributeLinePurpose(XAttribute attribute) {
            if (attribute == null) {
                return LinePurpose.None;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? LinePurpose.None : (LinePurpose)Enum.Parse(typeof(LinePurpose), svalue);

            return value;
        }

        /// <summary>
        /// Reads the attribute melodic function.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static MelodicFunction ReadAttributeMelodicFunction(XAttribute attribute) {
            if (attribute == null) {
                return MelodicFunction.None;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? MelodicFunction.None : (MelodicFunction)Enum.Parse(typeof(MelodicFunction), svalue);

            return value;
        }

        /// <summary>
        /// Reads the type of the attribute musical band.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static MusicalBand ReadAttributeMusicalBandType(XAttribute attribute) {
            if (attribute == null) {
                return MusicalBand.Any;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? MusicalBand.Any : (MusicalBand)Enum.Parse(typeof(MusicalBand), svalue);

            return value;
        }

        /// <summary>
        /// Reads the attribute musical octave.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static MusicalOctave ReadAttributeMusicalOctave(XAttribute attribute) {
            if (attribute == null) {
                return MusicalOctave.None;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? MusicalOctave.None : (MusicalOctave)Enum.Parse(typeof(MusicalOctave), svalue);

            return value;
        }

        /// <summary>
        /// Reads the attribute melodic shape.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static MelodicShape ReadAttributeMelodicShape(XAttribute attribute) {
            if (attribute == null) {
                return MelodicShape.None;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? MelodicShape.None : (MelodicShape)Enum.Parse(typeof(MelodicShape), svalue);

            return value;
        }

        /// <summary>
        /// Reads the attribute midi channel.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static MidiChannel ReadAttributeMidiChannel(XAttribute attribute) {
            if (attribute == null) {
                return MidiChannel.Unknown;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? MidiChannel.Unknown : (MidiChannel)Enum.Parse(typeof(MidiChannel), svalue);

            return value;
        }
        
        /// <summary>
        /// Reads the attribute instrument genus.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static InstrumentGenus ReadAttributeInstrumentGenus(XAttribute attribute) {
            if (attribute == null) {
                return InstrumentGenus.None;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? InstrumentGenus.None : (InstrumentGenus)Enum.Parse(typeof(InstrumentGenus), svalue);

            return value;
        }

        /// <summary>
        /// Reads the attribute musical loudness.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static MusicalLoudness ReadAttributeMusicalLoudness(XAttribute attribute) {
            if (attribute == null) {
                return MusicalLoudness.MeanLoudness;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? MusicalLoudness.MeanLoudness : (MusicalLoudness)Enum.Parse(typeof(MusicalLoudness), svalue);

            return value;
        }

        /// <summary>
        /// Reads the type of the attribute musical line.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static MusicalLineType ReadAttributeMusicalLineType(XAttribute attribute) {
            if (attribute == null) {
                return MusicalLineType.None;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? MusicalLineType.None : (MusicalLineType)Enum.Parse(typeof(MusicalLineType), svalue);

            return value;
        }

        /// <summary>
        /// Reads the type of the attribute musical rules.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        public static MusicalRulesType ReadAttributeMusicalRulesType(XAttribute attribute) {
            if (attribute == null) {
                return MusicalRulesType.None;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? MusicalRulesType.None : (MusicalRulesType)Enum.Parse(typeof(MusicalRulesType), svalue);

            return value;
        }

        /// <summary>
        /// Reads the type of the attribute source.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns> Returns value. </returns>
        [System.Diagnostics.Contracts.Pure]
        public static MusicalSourceType ReadAttributeSourceType(XAttribute attribute) {
            if (attribute == null) {
                return MusicalSourceType.MIDI;
            }

            string svalue = XmlSupport.ReadStringAttribute(attribute);
            var value = string.IsNullOrEmpty(svalue) ? MusicalSourceType.MIDI : (MusicalSourceType)Enum.Parse(typeof(MusicalSourceType), svalue);

            return value;
        }
        #endregion

        /// <summary>
        /// Gets the list low notes.
        /// </summary>
        /// <param name="minNote">The min note.</param>
        /// <param name="maxNote">The max note.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        /// <value>
        /// Property description.
        /// </value>
        [System.Diagnostics.Contracts.Pure]
        public static IEnumerable<KeyValuePair> ListNotes(byte minNote, byte maxNote) {
            var list = new Collection<KeyValuePair>();
            for (int note = maxNote; note >= minNote; note--) {
                var s = MusicalProperties.GetNoteNameAndOctave((byte)note, DefaultValue.HarmonicOrder);
                var vt = new KeyValuePair(note, s);
                list.Add(vt);
            }

            return list;
        }
    }
}
