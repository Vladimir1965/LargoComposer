// <copyright file="TemplateLine.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Templates
{
    /// <summary>
    /// Line Chunk.
    /// </summary>
    public class TemplateLine
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateLine" /> class.
        /// </summary>
        /// <param name="givenLineIndex">Index of the given line.</param>
        /// <param name="givenLineType">Type of the given line.</param>
        /// <param name="givenLineRhythm">The given line rhythm.</param>
        /// <param name="givenInstrument">The given instrument.</param>
        /// <param name="givenOctave">The given octave.</param>
        /// <param name="givenLoudness">The given loudness.</param>
        /// <param name="givenFunction">The given function.</param>
        /// <param name="givenShape">The given shape.</param>
        public TemplateLine(
                        int givenLineIndex,
                        MusicalLineType givenLineType,
                        LineRhythm givenLineRhythm,
                        MusicalInstrument givenInstrument,
                        MusicalOctave givenOctave,
                        MusicalLoudness givenLoudness,
                        MelodicFunction givenFunction,
                        MelodicShape givenShape)
        {
            this.Status = new LineStatus(givenLineIndex, givenLineType, givenInstrument, LinePurpose.Composed, MidiChannel.C00)
            {
                Octave = givenOctave,
                Loudness = givenLoudness,
                MelodicFunction = givenFunction,
                MelodicShape = givenShape
            };
            this.LineRhythm = givenLineRhythm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateLine" /> class.
        /// </summary>
        /// <param name="givenLineIndex">Index of the given line.</param>
        /// <param name="givenLineType">Type of the given line.</param>
        public TemplateLine(int givenLineIndex, MusicalLineType givenLineType)
        {
            this.Status = new LineStatus();
            this.LineIndex = givenLineIndex;
            this.Status.LineType = givenLineType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateLine"/> class.
        /// </summary>
        [UsedImplicitly]
        public TemplateLine()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateLine" /> class.
        /// </summary>
        /// <param name="xline">The mark line.</param>
        /// <param name="header">The header.</param>
        public TemplateLine(XElement xline, MusicalHeader header) 
            : this()
        {
            if (xline == null) {
                return;
            }

            var xstatus = xline.Element("Status");
            if (xstatus != null) {
                this.Status = new LineStatus(xstatus, header); 
            }

            this.LineRhythm = LineRhythm.SimpleOneTone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the index of the line.
        /// </summary>
        /// <value>
        /// The index of the line.
        /// </value>
        public int LineIndex { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public LineStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the line rhythm.
        /// </summary>
        /// <value>
        /// The line rhythm.
        /// </value>
        public LineRhythm LineRhythm { get; set; }
        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement
        {
            get
            {
                XElement xline = new XElement("Line");

                var xstatus = this.Status.GetXElement;
                xline.Add(xstatus);

                return xline;
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("PrototypeTrack {0}", this.Status);

            return s.ToString();
        }
        #endregion
    }
}