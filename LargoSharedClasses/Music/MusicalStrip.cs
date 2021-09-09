// <copyright file="MusicalStrip.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Orchestra;
using LargoSharedClasses.Settings;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Strip.
    /// </summary>
    public class MusicalStrip
    {
        #region Fields

        /// <summary>
        /// Musical Lines.
        /// </summary>
        private IList<MusicalLine> lines;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalStrip"/> class.
        /// </summary>
        /// <param name="givenContext">The given context.</param>
        public MusicalStrip(MusicalContext givenContext)
            : this() {
            this.Context = givenContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalStrip"/> class.
        /// </summary>
        public MusicalStrip() {
            this.lines = new List<MusicalLine>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalStrip" /> class.
        /// </summary>
        /// <param name="markStrip">The mark strip.</param>
        /// <param name="givenContext">The given context.</param>
        public MusicalStrip(XElement markStrip, MusicalContext givenContext)
            : this() {
            Contract.Requires(markStrip != null);
            if (markStrip == null) {
                return;
            }

            this.Context = givenContext;

            XElement xlines = markStrip.Element("Lines");
            if (xlines == null) {
                return;
            }

            var header = givenContext.Header;
            var xelements = xlines.Elements();
            foreach (XElement xline in xelements) {
                MusicalLine line = new MusicalLine(xline, header) { Strip = this };
                this.AddLine(line, false);
            }
        }

        #endregion

        #region Properties - Xml

        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                var xstrip = new XElement(
                    "Strip",
                    new XAttribute("HasLines", this.HasAnyMusicalLine));

                //// Lines
                var xlines = new XElement("Lines");
                ////  Musical Lines to XML
                foreach (MusicalLine line in this.Lines.Where(line => line != null)) {
                    //// line.MusicalBlock = givenMusicalBlock;
                    var xline = line.GetXElement;
                    xlines.Add(xline);
                }

                xstrip.Add(xlines);
                return xstrip;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public MusicalContext Context { get; set; }

        /// <summary>
        /// Gets Musical Lines.
        /// </summary>
        /// <value> Property description. </value>
        public IList<MusicalLine> Lines {
            get {
                Contract.Ensures(Contract.Result<IEnumerable<MusicalLine>>() != null);
                if (this.lines == null) {
                    throw new InvalidOperationException("Musical lines are null.");
                }

                return this.lines;
            }
            //// Remove private set - DevExpress
            private set => this.lines = value;
        }

        /// <summary>
        /// Gets a value indicating whether HasMusicalLine.
        /// </summary>
        /// <value> General musical property.</value>
        public bool HasAnyMusicalLine {
            get {
                var exists = (from mt in this.Lines where !mt.IsEmpty select 1).Any(); //// mt.IsSelected &&
                return exists;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the lines.
        /// </summary>
        /// <param name="givenLines">The given lines.</param>
        public void SetLines(IList<MusicalLine> givenLines) {
            this.lines = givenLines;
        }

        /// <summary>
        /// Writes the body.
        /// </summary>
        /// <param name="givenBody">The given body.</param>
        public void WriteBody(MusicalBody givenBody) {
            this.Context = givenBody.Context;
            foreach (var line in this.lines) {
                line.Strip = this; //// 2019/11 !?!
                line.Tones.Clear();
            }

            foreach (var bar in givenBody.Bars) {
                foreach (var element in bar.Elements) {
                    //// if (bar.BarNumber == 1 && element.Status != null) {
                    ////  element.Status.Channel = element.Line.MainVoice.Channel; //// ?!? }
                    foreach (var tone in element.Tones) {
                        element.MusicalLine.Tones.Add(tone);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the line.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        public void DeleteLine(Guid lineIdent) {
            var line = this.GetLine(lineIdent);
            if (line != null) {
                this.Lines.Remove(line);
                this.RenumberLines();
            }
        }

        /// <summary> String with line details. </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public string LinesToString() {
            var s = new StringBuilder();
            s.Append(" Line   Tones    \n");
            s.Append("-------------------------------------------------------------------\n");
            var musicLines = from p in this.Lines orderby p.LineIndex descending select p;
            foreach (var line in
                musicLines.Where(line => line?.Tones != null).Where(line => !line.IsEmpty)) {
                s.Append(line + "\r\n\n");
            }

            return s.ToString();
        }

        /// <summary>
        /// Finds the free channel.
        /// </summary>
        /// <param name="lineIndex">The line number.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MidiChannel FindFreeChannel(int lineIndex) {
            //// int barNumber            
            //// Exclude not used lines like LinePurpose.Mute (?!)
            var channels = (from line in this.Lines
                            where line.LineIndex != lineIndex
                            select line.MainVoice.Channel).Distinct().ToList();

            for (byte channel = 0; channel <= MusicalProperties.MidiCountChannels; channel++) {
                var midiChannel = (MidiChannel)channel;
                if (midiChannel != MidiChannel.DrumChannel && !channels.Contains(midiChannel)) {
                    return midiChannel;
                }
            }

            var c = (MidiChannel)(lineIndex % 16);
            if (c != MidiChannel.DrumChannel) {
                return c;
            }

            return MidiChannel.C00;
        }

        /// <summary>
        /// Clones the specified include tones.
        /// </summary>
        /// <param name="includeTones">if set to <c>true</c> [include tones].</param>
        /// <returns> Returns value. </returns>
        public MusicalStrip Clone(bool includeTones) {
            var strip = (MusicalStrip)this.Clone();
            strip.Lines = new List<MusicalLine>();

            foreach (var newLine in this.Lines.Select(line => line.Clone(includeTones))) {
                //// newLine.IsComposed = false;
                var ts = (List<MusicalLine>)strip.Lines;
                ts.Add(newLine);
            }

            return strip;
        }

        /// <summary>
        /// Remove the lines.
        /// </summary>
        public void ResetLines() {
            this.lines = new List<MusicalLine>();
        }

        /// <summary>
        /// Resets the tones.
        /// </summary>
        public void ResetTones() {
            foreach (var line in this.lines) {
                line.SetTones(new ToneCollection());
            }
        }

        /// <summary>
        /// Gets the musical line.
        /// </summary>
        /// <param name="lineIndex">The line number.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public MusicalLine GetLine(int lineIndex) {
            //// bool exists = (from mt in this.MusicalLines where mt.IsSelected && !mt.IsEmpty select 1).Any();
            if (lineIndex < 0 || lineIndex >= this.Lines.Count) {
                return null;
            }

            var ts = (List<MusicalLine>)this.Lines;
            return ts[lineIndex];
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        /// <returns>Returns value.</returns>
        [UsedImplicitly]
        public MusicalLine GetLine(Guid lineIdent) {
            var ts = (List<MusicalLine>)this.Lines;
            var line = (from t in ts where t.LineIdent == lineIdent select t).FirstOrDefault();
            return line;
        }

        /// <summary>
        /// Add Musical Line.
        /// </summary>
        /// <param name="line">Musical Line.</param>
        /// <param name="renumber">if set to <c>true</c> [renumber].</param>
        public void AddLine(MusicalLine line, bool renumber) {
            if (line == null) {
                return;
            }

            if (renumber) {
                //// if (line.LineIndex == 0) { 2016/07
                var num = this.Lines.Count;
                line.LineIndex = (byte)num;
                //// }
                //// 2016/07   //// 2015/01
                //// if (line.LineIndex == 0) { line.LineIndex = line.LineIndex;  } 
            }

            var ts = (List<MusicalLine>)this.Lines;
            ts.Add(line);
            this.Context.Header.NumberOfLines = (byte)ts.Count;
        }

        /// <summary>
        /// Removes the line.
        /// </summary>
        /// <param name="line">The line.</param>
        [UsedImplicitly]
        public void RemoveLine(MusicalLine line) {
            if (line == null) {
                return;
            }

            var ts = (List<MusicalLine>)this.Lines;
            ts.Remove(line);
        }

        /// <summary>
        /// Moves the tones from edges.
        /// </summary>
        /// <param name="minNote">The min note.</param>
        /// <param name="maxNote">The max note.</param>
        public void MoveTonesFromEdges(byte minNote, byte maxNote) {
            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var mt in this.Lines.SelectMany(line => line.Tones.OfType<MusicalTone>().Where(mt => !mt.IsPause))) {
                if (mt.IsTrueTone) {
                    mt.Pitch.MoveFromEdges(minNote, maxNote);
                }
            }
        }

        /// <summary>
        /// Corrects the octaves.
        /// </summary>
        public void CorrectOctaves() {
            var settings = MusicalSettings.Singleton;
            if (settings.SettingsComposition.CorrectOctaves) {
                foreach (var line in this.Lines) {
                    OrchestraChecker.Singleton.CorrectOctavesOfInstrumentedTones(line.Tones);
                }
            }

            if (settings.SettingsComposition.CorrectResultPitch) {
                this.MoveTonesFromEdges(settings.SettingsComposition.NoteLowest, settings.SettingsComposition.NoteHighest);
            }
        }

        /// <summary>
        /// Rebuild Channels.
        /// </summary>
        public void RebuildChannels() {
            if (this.Lines.Count <= 15) {
                foreach (var line in this.Lines) {
                    line.MainVoice.Channel = line.FirstStatus.IsMelodic ? MusicalProperties.ChannelForPartNumber(line.LineIndex) : MidiChannel.DrumChannel;
                }

                return;
            }

            if (this.Lines.Count <= 30) {
                foreach (var line in this.Lines) {
                    line.MainVoice.Channel = line.FirstStatus.IsMelodic ? MusicalProperties.ChannelForPartNumber(line.LineIndex / 2) : MidiChannel.DrumChannel;
                }
            }
        }

        /// <summary>
        /// Splits the same tones.
        /// </summary>
        [UsedImplicitly]
        public void SplitTheFollowUpTones() {
            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            foreach (var line in this.Lines) {
                SplitTheFollowUpTonesInLine(line);
            }
        }

        #endregion

        #region Public methods - Transformation

        /// <summary>
        /// Total Horizontal Inversion.
        /// </summary>
        [UsedImplicitly]
        public void HorizontalInversion() {
            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            this.Lines.ForAll(musicalLine => musicalLine.TotalHorizontalInversion());
        }

        /// <summary>
        /// Total Vertical Inversion.
        /// </summary>
        [UsedImplicitly]
        public void VerticalInversion() {
            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var mt in this.Lines.SelectMany(line => line.Tones.OfType<MusicalTone>().Where(mt => !mt.IsPause))) {
                mt.Pitch?.SetAltitude(128 - mt.Pitch.SystemAltitude);
            }
        }

        /// <summary>
        /// Bar Horizontal Inversion.
        /// </summary>
        [UsedImplicitly]
        public void BarInversion() {
            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            this.Lines.ForAll(musicalLine => musicalLine.BarHorizontalInversion());
        }

        /// <summary>
        /// Modular Deformation.
        /// </summary>
        [UsedImplicitly]
        public void ModularDeformation() {
            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            const int deformation = 5;
            var absDeformation = Math.Abs(deformation);
            if (absDeformation <= 0) {
                return;
            }

            foreach (var line in this.Lines) {
                foreach (var tone in line.Tones) {
                    MusicalTone mt = tone as MusicalTone;
                    if (mt == null || mt.IsPause) {
                        continue;
                    }

                    var altitude = mt.Pitch.SystemAltitude;
                    var steps = mt.Pitch.SystemAltitude % absDeformation; //// MusMath.RandomNatural(3);  
                    altitude = altitude + (Math.Sign(deformation) * steps);
                    mt.Pitch.SetAltitude(altitude);
                }
            }
        }

        /// <summary>
        /// Vertical Extension.
        /// </summary>
        [UsedImplicitly]
        public void VerticalExtension() {
            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            foreach (var line in this.Lines) {
                foreach (var tone in line.Tones) {
                    //// PlannedTones
                    MusicalTone mt = tone as MusicalTone;
                    if (mt == null || mt.IsPause || mt.IsEmpty) {
                        continue;
                    }

                    var altitude = mt.Pitch.SystemAltitude;
                    altitude = Math.Max((2 * altitude) - DefaultValue.MeanNoteAltitude, 0);
                    altitude = Math.Min(altitude, 127);
                    mt.Pitch.SetAltitude(altitude);
                }
            }
        }

        /// <summary>
        /// Vertical Narrowing.
        /// </summary>
        [UsedImplicitly]
        public void VerticalNarrowing() {
            const byte nivelizationFactor = 2;
            const byte altitudeShift = 48;

            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            foreach (var line in this.Lines) {
                foreach (var tone in line.Tones) {
                    //// PlannedTones
                    MusicalTone mt = tone as MusicalTone;
                    if (mt == null || mt.IsPause || mt.IsEmpty) {
                        continue;
                    }

                    var altitude = mt.Pitch.SystemAltitude;
                    altitude = Math.Max((altitude / nivelizationFactor) + altitudeShift, 0);
                    altitude = Math.Min(altitude, 127);
                    mt.Pitch.SetAltitude(altitude);
                }
            }
        }

        /// <summary>
        /// Shifts Octave Up.
        /// </summary>
        [UsedImplicitly]
        public void OctaveUp() {
            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            foreach (var line in this.Lines) {
                foreach (var mt in
                    line.Tones.OfType<MusicalTone>().Where(mt => !mt.IsPause && !mt.IsEmpty)) {
                    mt.Pitch.ShiftOctave(+1);
                }
            }
        }

        /// <summary>
        /// Shifts Octave Down.
        /// </summary>
        [UsedImplicitly]
        public void OctaveDown() {
            if (this.Lines == null || !this.Lines.Any()) {
                return;
            }

            foreach (var line in this.Lines) {
                foreach (var mt in
                    line.Tones.OfType<MusicalTone>().Where(mt => !mt.IsPause && !mt.IsEmpty)) {
                    mt.Pitch.ShiftOctave(-1);
                }
            }
        }

        #endregion

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.Append(string.Format(CultureInfo.CurrentCulture, "Lines {0}", this.Lines.Count));
            return s.ToString();
        }

        #endregion

        #region Tones

        /// <summary> List of tones in one bar of musical part. </summary>
        /// <param name="barNumber">Number of musical bar.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public MusicalToneCollection MelodicTonesInBar(int barNumber) {
            var tonesInBar = new List<MusicalTone>();
            foreach (var mtones in from line in this.Lines
                                   where line?.Tones != null ////  && mline.IsSelected 
                                   where line.FirstStatus.LineType == MusicalLineType.Melodic
                                   select line.MelodicTonesInBar(barNumber)
                into mtones
                                   where mtones != null && mtones.Any()
                                   select mtones) {
                tonesInBar.AddRange(mtones);
            }

            return new MusicalToneCollection(tonesInBar);
        }

        #endregion

        #region Private static methods

        /// <summary>
        /// Splits the follow up tones in line.
        /// </summary>
        /// <param name="line">The line.</param>
        private static void SplitTheFollowUpTonesInLine(MusicalLine line) {
            Contract.Requires(line != null);

            MusicalTone lastLine = null;
            foreach (var mt in
                line.Tones.OfType<MusicalTone>().Where(mt => mt.IsTrueTone && !mt.IsPause)) {
                if (lastLine != null && lastLine.IsGoingToNextBar
                                      && lastLine.BarNumber ==
                                      mt.BarNumber + 1 //// && lastLine.BarNumberTo == mt.BarNumber
                                      && mt.IsFromPreviousBar
                                      && lastLine.Pitch.SystemAltitude == mt.Pitch.SystemAltitude
                                      && lastLine.BitTo == mt.BitFrom - 1) {
                    lastLine.Duration += mt.Duration;
                    //// lastLine.BarNumberTo = mt.BarNumberTo;
                    mt.Loudness = 0;
                }

                lastLine = mt;
            }
        }

        #endregion

        #region Private methods

        /// <summary> Makes a deep copy of the MusicalStrip object. </summary>
        /// <returns> Returns object. </returns>
        private object Clone() {
            var strip = new MusicalStrip(this.Context);

            return strip;
        }

        /// <summary>
        /// Renumbers the lines.
        /// </summary>
        private void RenumberLines() {
            int lineIndex = 0;
            foreach (var line in this.Lines) {
                line.LineIndex = lineIndex++;
            }
        }

        #endregion
    }
}
