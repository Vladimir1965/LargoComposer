// <copyright file="ScorePartObject.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.MusicXml
{
    /// <summary>
    /// Score Part object.
    /// </summary>
    public sealed class ScorePartObject
    {
        #region Properties
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value> Property description. </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets PartName.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string PartName { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets ScoreInstrumentId.
        /// </summary>
        /// <value> Property description. </value>
        public string ScoreInstrumentId { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets InstrumentName.
        /// </summary>
        /// <value> Property description. </value>
        public string InstrumentName { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets MidiInstrumentId.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string MidiInstrumentId { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets MidiChannel.
        /// </summary>
        /// <value> Property description. </value>
        public MidiChannel MidiChannel { get; set; }

        /// <summary>
        /// Gets or sets MidiProgram.
        /// </summary>
        /// <value> Property description. </value>
        public byte MidiProgram { get; set; }

        /// <summary>
        /// Gets or sets Volume.
        /// </summary>
        /// <value> Property description. </value>
        private string Volume { get; set; }
        //// public byte? Volume { get; set; }

        /// <summary>
        /// Gets or sets Pan.
        /// </summary>
        /// <value> Property description. </value>
        private byte? Pan { get; set; }
        #endregion

        #region Public static methods
        /// <summary>
        /// Read Musical Part.
        /// </summary>
        /// <param name="scorePart">Score part.</param>
        /// <returns> Returns value. </returns> 
        public static ScorePartObject ReadScorePart(XElement scorePart) {
            if (scorePart == null) {
                return null;
            }

            var part = new ScorePartObject {
                Id = (string)scorePart.Attribute("id"),
                PartName = (string)scorePart.Element("part-name")
            };

            var si = scorePart.Element("score-instrument");
            if (si != null) {
                part.ScoreInstrumentId = (string)si.Attribute("id");
                part.InstrumentName = (string)si.Element("instrument-name");
            }

            var mi = scorePart.Element("midi-instrument");
            if (mi == null) {
                return part;
            }

            part.MidiInstrumentId = (string)mi.Attribute("id");
            part.MidiChannel = (MidiChannel)XmlSupport.ReadByteAttribute(mi.Attribute("midi-channel"));
            part.MidiProgram = XmlSupport.ReadByteAttribute(mi.Attribute("midi-program"));
            part.Volume = (string)mi.Element("volume"); //// (byte?)(int?)
            part.Pan = (byte?)(int?)mi.Element("pan");

            return part;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Score Part.
        /// </summary>
        /// <returns> Returns value. </returns>
        public XElement ScorePart() {
            Contract.Requires(this.Id != null);
            var scorePart = new XElement("score-part");
            scorePart.Add(new XAttribute("id", this.Id));
            scorePart.Add(new XElement("part-name", this.PartName));
            /* Music Xml -  Not used now.
            XElement si = new XElement("score-instrument");
            if (!string.IsNullOrEmpty(this.ScoreInstrumentId)) {
                si.Add(new XAttribute("id", this.ScoreInstrumentId));
            }

            if (!string.IsNullOrEmpty(this.InstrumentName)) {
                si.Add(new XAttribute("instrument-name", this.InstrumentName));
            }
            scorePart.Add(si);
            */

            var mi = new XElement("midi-instrument");
            if (!string.IsNullOrEmpty(this.MidiInstrumentId)) {
                mi.Add(new XAttribute("id", this.MidiInstrumentId));
            }

            mi.Add(new XAttribute("midi-channel", (byte)this.MidiChannel));
            mi.Add(new XAttribute("midi-program", this.MidiProgram));
            mi.Add(new XElement("volume", this.Volume ?? string.Empty)); //// 0
            mi.Add(new XElement("pan", this.Pan ?? 0));
            scorePart.Add(mi);

            return scorePart;
        }
        #endregion
    }
}
