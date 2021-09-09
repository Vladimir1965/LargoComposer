// <copyright file="MusicalBody.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Harmony;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Midi;
using LargoSharedClasses.Models;
using LargoSharedClasses.Orchestra;
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical Body.
    /// </summary>
    public class MusicalBody
    {
        #region Fields

        /// <summary>
        /// Musical Bars.
        /// </summary>
        private List<MusicalBar> bars;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalBody"/> class.
        /// </summary>
        /// <param name="givenContext">The given context.</param>
        public MusicalBody(MusicalContext givenContext) {
            this.Context = givenContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalBody"/> class.
        /// </summary>
        /// <param name="givenStrip">The given strip.</param>
        public MusicalBody(MusicalStrip givenStrip) {
            this.Context = givenStrip.Context;
            this.Context.Header.NumberOfLines = givenStrip.Lines.Count;
            this.PrepareElements(givenStrip);
        }

        #endregion

        #region Properties - Xml

        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public virtual XElement GetXElement {
            get {
                XElement xbody = new XElement("Body");

                //// Bars
                XElement xbars = new XElement("Bars");
                ////  Musical Bars to XML
                foreach (MusicalBar mbar in this.Bars.Where(mbar => mbar != null)) {
                    var xbar = mbar.GetXElement;
                    xbars.Add(xbar);
                }

                //// var xtrack = MusicalTrackPort.WriteMusicalTrack(givenElement.Track); xelement.Add(xtrack); 
                xbody.Add(xbars);

                return xbody;
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

        /// <summary> Gets list of bars.</summary>
        /// <value> Property description. </value>
        public IList<MusicalBar> Bars {
            get {
                Contract.Ensures(Contract.Result<IList<MusicalBar>>() != null);
                return this.bars ?? (this.bars = new List<MusicalBar>());
            }

            //// private set { this.musicalBars = value; }
        }

        /// <summary>
        /// Gets or sets the tempo events.
        /// </summary>
        /// <value>
        /// The tempo events.
        /// </value>
        public IEnumerable<IMidiEvent> TempoEvents { get; set; } //// private

        /// <summary> Gets string with harmony. </summary>
        /// <value> General musical property.</value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public string HarmonyToString {
            get {
                var s = new StringBuilder();
                s.Append(" Bar  Modality            Harmony(Duration)    \n");
                s.Append("-------------------------------------------------------------------\n");
                var bs = from b in this.Bars orderby b.BarNumber select b;
                foreach (var bar in bs.Where(bar => bar != null && !bar.IsEmpty)) {
                    s.Append(bar.NumberToString());
                    s.Append(bar.HarmonicBar.ModalityToString);
                    s.Append(bar.PureChordsToString + "\n");
                }

                return s.ToString();
            }
        }

        /// <summary> Gets string of clusters. </summary>
        /// <value> General musical property.</value>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public string ClustersToString {
            get {
                var s = new StringBuilder();
                s.Append(" Bar    Cluster                         Harmony         (Modality) \n");
                s.Append("-------------------------------------------------------------------\n");
                var bs = from b in this.Bars orderby b.BarNumber select b;
                foreach (var bar in bs.Where(bar => bar != null && !bar.IsEmpty)) {
                    s.Append(bar.ClustersToString + "\n");
                }

                return s.ToString();
            }
        }

        /// <summary>
        /// Gets the editor elements.
        /// </summary>
        /// <value>
        /// The editor elements.
        /// </value>
        public IList<MusicalElement> AllElements {
            get {
                var list = new List<MusicalElement>();
                foreach (var bar in this.Bars) {
                    list.AddRange(bar.Elements);
                }

                //// var selectedList = (from element in list orderby element.Point.BarNumber, 
                //// element.Point.LineIndex select element).ToList();
                return list;
            }
        }

        #endregion

        #region Public static
        /// <summary>
        /// Sets the purpose to elements.
        /// </summary>
        /// <param name="givenPurpose">The given purpose.</param>
        /// <param name="givenElements">The given elements.</param>
        public static void SetPurposeToElements(LinePurpose givenPurpose, IEnumerable<MusicalElement> givenElements) {
            foreach (var element in givenElements) {
                if (element?.Status != null) {
                    if (givenPurpose == LinePurpose.Fixed && (!element.MusicalLine.HasContent || element.MusicalLine.IsEmpty)) {
                        continue;
                    }

                    element.Status.LocalPurpose = givenPurpose;
                }
            }
        }

        /// <summary>
        /// Sets the instrument to elements.
        /// </summary>
        /// <param name="givenInstrument">The given instrument.</param>
        /// <param name="givenElements">The given elements.</param>
        public static void SetInstrumentToElements(MusicalInstrument givenInstrument, IEnumerable<MusicalElement> givenElements) {
            foreach (var element in givenElements) {
                if (element?.Status != null) {
                    //// if (element.MusicalLine.IsEmpty)) { continue; }

                    element.Status.Instrument = givenInstrument;
                }
            }
        }

        /// <summary>
        /// Sets the octave to elements.
        /// </summary>
        /// <param name="givenOctave">The given octave.</param>
        /// <param name="givenElements">The given elements.</param>
        public static void SetOctaveToElements(MusicalOctave givenOctave, IEnumerable<MusicalElement> givenElements) {
            foreach (var element in givenElements) {
                if (element?.Status != null) {
                    //// if (element.MusicalLine.IsEmpty)) { continue; }

                    element.Status.Octave = givenOctave;
                }
            }
        }

        /// <summary>
        /// Sets the loudness to elements.
        /// </summary>
        /// <param name="givenLoudness">The given loudness.</param>
        /// <param name="givenElements">The given elements.</param>
        public static void SetLoudnessToElements(MusicalLoudness givenLoudness, IEnumerable<MusicalElement> givenElements) {
            foreach (var element in givenElements) {
                if (element?.Status != null) {
                    //// if (element.MusicalLine.IsEmpty)) { continue; }

                    element.Status.Loudness = givenLoudness;
                }
            }
        }

        /// <summary>
        /// Sets the orchestra unit to elements.
        /// </summary>
        /// <param name="givenOrchestraUnit">The given orchestra unit.</param>
        /// <param name="givenElements">The given elements.</param>
        public static void SetOrchestraUnitToElements(OrchestraUnit givenOrchestraUnit, IEnumerable<MusicalElement> givenElements) {
            foreach (var element in givenElements) {
                if (element?.Status != null) {
                    //// if (element.MusicalLine.IsEmpty)) { continue; }

                    element.Status.OrchestraUnit = givenOrchestraUnit;
                }
            }
        }
        #endregion

        #region Derived objects
        /// <summary>
        /// Gets the harmonic stream.
        /// </summary>
        /// <param name="givenMaxTonesInChord">The given maximum tones in chord.</param>
        /// <param name="givenFullHarmonization">if set to <c>true</c> [given full harmonization].</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public HarmonicStream GetHarmonicStream(byte givenMaxTonesInChord, bool givenFullHarmonization) {
            this.SetHarmonicStatusFromTones(givenMaxTonesInChord, givenFullHarmonization);
            var harmonicStream = new HarmonicStream(this.Context.Header);  ////  this.Name, "Derived from block" 
            foreach (MusicalBar bar in this.Bars) {
                var harBar = bar.HarmonicBar;
                harmonicStream.HarmonicBars.Add(harBar);
            }

            return harmonicStream;
        }

        /// <summary>
        /// Rhythmic of the harmony.
        /// </summary>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        public IList<RhythmicStructure> RhythmicOfHarmony() {
            var structs = new List<RhythmicStructure>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var bar1 in this.Bars.Where(bar1 => bar1 != null)) {
                if (bar1.HarmonicBar == null) {
                    continue;
                }

                var rstruct = bar1.HarmonicBar.RhythmicStructure; //// Clone?
                structs.Add(rstruct);
            }

            return structs;
        }
        #endregion        

        /// <summary>
        /// Reorders the line rhythmic.
        /// </summary>
        /// <param name="lineIndex">The line number.</param>
        /// <param name="model">The block model.</param>
        [UsedImplicitly]
        public void SetRandomRhythm(int lineIndex, RhythmicModel model) {
            Contract.Requires(model != null);

            var elements = this.ElementsOfLine(lineIndex);
            var master = new ElementMaster(elements);
            var structures = model.RhythmicStructuresOfMotives;
            master.SetRandomRhythm(structures);
        }

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("MusicalBody (Length {0})", this.Bars.Count);

            return s.ToString();
        }

        #endregion

        #region Bars - Harmony
        /// <summary>
        /// Gets the sorted bars.
        /// </summary>
        /// <param name="lineType">Type of the line.</param>
        /// <returns> Returns value. </returns>
        /// <value>
        /// The sorted bars.
        /// </value>
        public IList<MusicalBar> SortedBars(MusicalLineType lineType) {
            Contract.Ensures(Contract.Result<IList<MusicalBar>>() != null);
            var bs = this.Bars.ToList();
            foreach (var bar in bs) {
                bar.ResetValuesOfTicks();
            }

            bs.Sort(new BarComparer(lineType));
            return bs;
        }

        #endregion

        #region Public methods - Elements

        /// <summary>
        /// Marks the rhythmic motive.
        /// </summary>
        /// <param name="givenMotive">The given motive.</param>
        /// <param name="givenArea">The given area.</param>
        public void MarkRhythmicMotive(RhythmicMotive givenMotive, MusicalArea givenArea) {
            for (int barNumber = givenArea.StartPoint.BarNumber; barNumber <= givenArea.EndPoint.BarNumber; barNumber++) {
                var bar = this.GetBar(barNumber);
                if (givenArea.StartPoint.LineIndex >= bar?.Elements.Count) { ////2019/12 (?)
                    continue;
                }

                var element = bar?.Elements[givenArea.StartPoint.LineIndex];
                if (element?.Status == null) {
                    continue;
                }

                element.Status.RhythmicMotive = givenMotive;
            }
        }

        /// <summary>
        /// Marks the melodic motive.
        /// </summary>
        /// <param name="givenMotive">The given motive.</param>
        /// <param name="givenArea">The given area.</param>
        public void MarkMelodicMotive(MelodicMotive givenMotive, MusicalArea givenArea) {
            for (int barNumber = givenArea.StartPoint.BarNumber; barNumber <= givenArea.EndPoint.BarNumber; barNumber++) {
                var bar = this.GetBar(barNumber);

                if (givenArea.StartPoint.LineIndex >= bar?.Elements.Count) { ////2019/12 (?)
                    continue;
                }

                var element = bar?.Elements[givenArea.StartPoint.LineIndex];
                if (element?.Status == null) {
                    continue;
                }

                element.Status.MelodicMotive = givenMotive;
            }
        }

        /// <summary>
        /// Gets the bar.
        /// </summary>
        /// <param name="givenBarNumber">The given bar number.</param>
        /// <returns> Returns value. </returns>
        public MusicalBar GetBar(int givenBarNumber) {
            var barIdx = givenBarNumber - 1;
            if (barIdx < 0 || barIdx >= this.Bars.Count) {
                return null;
            }

            var bar = this.Bars[barIdx];
            return bar;
        }

        /// <summary>
        /// Prepares the rhythm in line.
        /// </summary>
        /// <param name="givenLineIdent">The given line identifier.</param>
        public void PrepareRhythmInLine(Guid givenLineIdent) {
            var lineElements = this.ElementsOfLine(givenLineIdent);
            foreach (var element in lineElements) {
                if (element == null) {
                    continue;
                }

                if (element.Status.RhythmicStructure != null) {
                    element.Status.RhythmicMotive = RhythmicMotive.SimpleRhythmicMotive(element.Status.RhythmicStructure);
                    //// element.Status.RhythmicMotive = element.Status.RhythmicMotive;
                }

                element.FillWithRequestedRhythm();
            }
        }

        /// <summary>
        /// Prepares the elements.
        /// </summary>
        /// <param name="givenStrip">The given strip.</param>
        public void PrepareElements(MusicalStrip givenStrip) {
            var h = this.Context.Header;
            var numberOfTracks = h.NumberOfLines; //// 2016 givenStrip. Lines .Count;
            var numberOfBars = h.NumberOfBars;
            var barMidiDuration = MusicalProperties.BarMidiDuration(
                                            h.System.RhythmicOrder,
                                            h.Metric.MetricBeat,
                                            h.Metric.MetricGround,
                                            h.Division);

            MusicalBar lastBar = null;
            for (int barNumber = 1; barNumber <= numberOfBars; barNumber++) {
                MusicalBar bar = new MusicalBar(barNumber, this) {
                    BarNumber = barNumber,
                    PreviousBar = lastBar,
                    TimePoint = (barNumber - 1) * barMidiDuration
                };

                bar.SetElements(new List<MusicalElement>());
                if (lastBar != null) {
                    lastBar.NextBar = bar;
                }

                for (byte lineIndex = 0; lineIndex < numberOfTracks; lineIndex++) {
                    var line = givenStrip.Lines.ElementAt(lineIndex);
                    var element = new MusicalElement(line.FirstStatus, null) {
                        Bar = bar,
                        Line = line
                    };

                    element.SetTones(line.MusicalTonesInBar(barNumber));
                    element.Status.BarNumber = barNumber; //// 2019/01
                    element.Status.MelodicVariety = line.FirstStatus.MelodicVariety; //// 2019/01
                    element.Status.MelodicPlan = new MelodicPlan(element.Tones); //// 2016/10 !?!?!?!? 
                    if (!element.Tones.HasAnySoundingTone) {
                        element.Status.LocalPurpose = LinePurpose.None;
                    }

                    bar.Elements.Add(element);
                }

                this.Bars.Add(bar);

                lastBar = bar;
            }
        }

        /// <summary>
        /// Sets the purpose to all elements.
        /// </summary>
        /// <param name="givenPurpose">The given purpose.</param>
        public void SetPurposeToAllElements(LinePurpose givenPurpose) {
            MusicalBody.SetPurposeToElements(givenPurpose, this.AllElements);
        }

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <param name="givenPoint">The given point.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public MusicalElement GetElement(MusicalPoint givenPoint) {
            MusicalElement element = null;
            var barIndex = givenPoint.BarNumber - 1;
            if (barIndex >= 0 && barIndex < this.Bars.Count) {
                var bar = this.Bars[barIndex];
                var lineIndex = givenPoint.LineIndex;
                if (lineIndex >= 0 && lineIndex < bar.Elements.Count) {
                    element = bar.Elements[lineIndex];
                }
            }

            return element;
        }

        /// <summary>
        /// Editors the elements of line.
        /// </summary>
        /// <param name="lineIndex">The line number.</param>
        /// <returns> Returns value. </returns>
        public IEnumerable<MusicalElement> ElementsOfLine(int lineIndex) {
            var list = (from v in this.AllElements
                        where v.Point.LineIndex == lineIndex
                        select v).ToList();

            //// var selectedList = (from element in list orderby element.Point.BarNumber
            //// select element).ToList();
            return list;
        }

        /// <summary>
        /// Elements the of line.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        /// <returns>Returns value.</returns>
        public IEnumerable<MusicalElement> ElementsOfLine(Guid lineIdent) {
            var list = (from v in this.AllElements
                        where v.Line.LineIdent == lineIdent
                        select v).ToList();

            //// var selectedList = (from element in list orderby element.Point.BarNumber
            //// select element).ToList();
            return list;
        }

        /// <summary> Enumerates rhythmic structures of line in this collection. </summary>
        /// <param name="lineIdent"> The line identifier. </param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process rhythmic structures of line in this
        /// collection.
        /// </returns>
        [UsedImplicitly]
        public IEnumerable<RhythmicStructure> RhythmicStructuresOfLine(Guid lineIdent) {
            var list = (from v in this.AllElements
                        where v.Line.LineIdent == lineIdent && v.Status?.RhythmicStructure != null
                        select v.Status.RhythmicStructure).ToList();

            //// var selectedList = (from element in list orderby element.Point.BarNumber
            //// select element).ToList();
            return list;
        }

        /// <summary>
        /// Clones the line elements.
        /// </summary>
        /// <param name="lineFrom">The line from.</param>
        /// <param name="lineTo">The line to.</param>
        [UsedImplicitly]
        public void CopyLineStatus(int lineFrom, int lineTo) {
            foreach (var bar in this.Bars) {
                var pointFrom = MusicalPoint.GetPoint(lineFrom, bar.BarNumber);
                var elementFrom = this.GetElement(pointFrom);
                var pointTo = MusicalPoint.GetPoint(lineTo, bar.BarNumber);
                var elementTo = this.GetElement(pointTo);
                elementTo.Status = (LineStatus)elementFrom.Status.Clone();
            }
        }

        /// <summary>
        /// Determines whether [is in editor] [the specified line].
        /// </summary>
        /// <param name="lineIndex">Index of the line.</param>
        /// <param name="bar">The bar.</param>
        /// <returns>
        ///   <c>true</c> if [is in editor] [the specified line]; otherwise, <c>false</c>.
        /// </returns>
        [UsedImplicitly]
        public bool IsInEditor(int lineIndex, int bar) {
            return lineIndex >= 0 && bar >= 1
                                  && lineIndex < this.Context.Header.NumberOfLines
                                  && bar <= this.Context.Header.NumberOfBars;
        }

        #endregion

        #region Public methods - Lines

        /// <summary>
        /// Deletes the line.
        /// </summary>
        /// <param name="lineIndex">The line number.</param>
        public void DeleteLine(int lineIndex) {
            foreach (var bar in this.Bars) {
                //// .Where(bar => bar != null)
                if (lineIndex >= bar.Elements.Count) {
                    continue;
                }

                var element = bar.Elements[lineIndex];
                bar.Elements.Remove(element);
            }
        }

        /// <summary>
        /// Deletes the line.
        /// </summary>
        /// <param name="lineIdent">The line identifier.</param>
        public void DeleteLine(Guid lineIdent) {
            foreach (var bar in this.Bars) {
                var element = bar.GetElement(lineIdent);
                if (element != null) {
                    bar.Elements.Remove(element);
                }
            }
        }

        #endregion

        #region Public methods - Bars

        /// <summary>
        /// Creates bars according to harmonic definition in the given segment.
        /// </summary>
        /// <param name="changes">The changes.</param>
        public void SetHarmonicBasis(MusicalChanges changes) {
            //// bool relative
            if (changes == null) {
                return;
            }

            var harmonicOrder = this.Context.Header.System.HarmonicOrder;
            //// var templateStatus = new BarStatus();
            int barNumberInMotive = 0;
            foreach (var bar in this.Bars.Where(bar => bar != null && harmonicOrder > 0)) {
                //// Musical block has harmonic background (so it is not rhythmic only)
                if (changes.CurrentChange(0, bar.BarNumber, MusicalChangeType.Harmonic) is HarmonicChange dc) {
                    bar.ApplyHarmonicChange(dc);
                }

                //// var ms = (BarStatus)templateStatus.Clone();
                //// ms.BarNumber = bar.BarNumber;
                //// ms.LineIndex = 0;

                if (bar.HasHarmonicMotive) {
                    barNumberInMotive = bar.BarNumber - bar.HarmonicStatus.BarNumber + 1;
                    var harmonicBar =
                        bar.HarmonicStatus.HarmonicMotive.HarmonicBarWithNumber(
                                    barNumberInMotive, this.Context.Header.System);
                    var newHarmonicBar = (HarmonicBar)harmonicBar.Clone();
                    newHarmonicBar.BarNumber = bar.BarNumber;
                    bar.SetHarmonicBar(newHarmonicBar);
                    bar.NumberInHarmonicMotive = barNumberInMotive;
                    if (barNumberInMotive == 1 && bar.PreviousBar != null) {
                        bar.PreviousBar.IsLastInHarmonicMotive = true;
                    }
                }
                else {
                    //// throw new ArgumentException("No har.motive bar!");
                    var hmb1 = new HarmonicBar(barNumberInMotive, bar.BarNumber);
                    bar.SetHarmonicBar(hmb1);
                }

                //// 2016 this.BarStatus = ms;
            }

            foreach (var bar in this.Bars.Where(bar => bar != null)) {
                bar.SetModalityToStructures(MusicalSettings.Singleton.SettingsAnalysis.MinimalModalityLevel);
            }
        }

        #endregion

        #region Public methods - Tempo

        /// <summary>
        /// Set Tempo Events From.
        /// </summary>
        /// <param name="timeFrom">Midi time from.</param>
        /// <param name="timeTo">Midi time To.</param>
        /// <param name="givenToneTracks">The given tone tracks.</param>
        public void SetTempoEventsFrom(long timeFrom, long timeTo, List<IMidiTrack> givenToneTracks) {
            if (givenToneTracks.Count == 0) {
                return;
            }

            var tempoEvents = new List<IMidiEvent>();
            var track0 = givenToneTracks[0];
            if (track0 == null) {
                return;
            }

            var originalEvents = track0.Sequence.GetTempoEvents(timeFrom, timeTo);
            if (originalEvents != null) {
                IEnumerable<IMidiEvent> midiEvents = originalEvents as IList<IMidiEvent> ?? originalEvents.ToList();
                if (midiEvents.Any()) {
                    tempoEvents.AddRange(midiEvents);
                }
            }

            //// Remove duplicity events
            var selectedTempoEvents = new List<IMidiEvent>();
            MetaTempo lastEvent = null;
            foreach (var ev in tempoEvents) {
                var tev = ev as MetaTempo;
                if (lastEvent != null && tev != null && tev.StartTime == lastEvent.StartTime &&
                    tev.Tempo == lastEvent.Tempo) {
                    continue;
                }

                selectedTempoEvents.Add(ev);
                lastEvent = tev;
            }

            this.TempoEvents = selectedTempoEvents;
            this.SetTempoStatusFromEvents(selectedTempoEvents);
        }

        #endregion

        #region Public methods - Rhythm

        /// <summary>
        /// Sets the line rhythm to bars.
        /// </summary>
        /// <param name="givenLineIdent">The given line identifier.</param>
        /// <param name="rhythmicStructure">The rhythmic structure.</param>
        public void SetRhythmToBars(Guid givenLineIdent, RhythmicStructure rhythmicStructure) {
            var lineElements = this.ElementsOfLine(givenLineIdent);
            var elements = lineElements as IList<MusicalElement> ?? lineElements.ToList();
            foreach (var element1 in elements.Where(element1 => element1 != null)) {
                element1.Status.RhythmicStructure = rhythmicStructure;
            }
        }

        /// <summary>
        /// Sets the simple rhythm to selected bars.
        /// </summary>
        /// <param name="givenLineIdent">The given line identifier.</param>
        /// <param name="lineRhythm">The line rhythm.</param>
        /// <param name="barScope">The bar scope.</param>
        /// <param name="targetRhythmicOrder">The target rhythmic order.</param>
        /// <param name="excludeSpaces">if set to <c>true</c> [exclude spaces].</param>
        public void SetRhythmToBars(Guid givenLineIdent, LineRhythm lineRhythm, BarScope barScope, byte targetRhythmicOrder, bool excludeSpaces) { 
            var lineElements = this.ElementsOfLine(givenLineIdent);
            var elements = lineElements as IList<MusicalElement> ?? lineElements.ToList();
            var rsystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, targetRhythmicOrder);
            var remptystruct = new RhythmicStructure(rsystem, 0);
            var rsimplestruct = new RhythmicStructure(rsystem, 1);
            RhythmicStructure tmpStructure = null;
            foreach (var element1 in elements.Where(element1 => element1 != null)) {
                var bar = element1.Bar as MusicalBar;
                RhythmicStructure rhyStructure = null;
                if (lineRhythm == LineRhythm.SimpleOneTone
                        || (barScope == BarScope.EvenBars && (bar.BarNumber % 2 == 1))
                        || (barScope == BarScope.OddBars && (bar.BarNumber % 2 == 0))) { 
                    rhyStructure = rsimplestruct;
                }
                else {
                    if (lineRhythm == LineRhythm.HarmonicStructure) {
                        tmpStructure = bar.GetHarmonicRhythm(true); 
                    }
                    else
                    if (lineRhythm == LineRhythm.HarmonicShape) {
                        tmpStructure = bar.GetHarmonicRhythm(false);
                    }

                    rhyStructure = tmpStructure.ConvertToSystem(rsystem);
                }

                if (excludeSpaces && !bar.HasFixedSoundingTones) {
                    rhyStructure = remptystruct;
                }

                element1.Status.RhythmicStructure = rhyStructure;
            }
        }

        #endregion

        #region Status

        /// <summary>
        /// Sets the melodic status from tones.
        /// </summary>
        [UsedImplicitly]
        public void SetStatusFromMusic() {
            var changes = this.ExtractTempoChanges();
            if (changes != null) {
                var tempoChanges = changes.ToList();

                TempoChange tempoStatus = null;
                foreach (var bar in this.Bars) {
                    var tempoChange = (from c in tempoChanges where c.BarNumber == bar.BarNumber select c)
                        .FirstOrDefault();
                    if (tempoChange != null) {
                        tempoStatus = tempoChange;
                    }

                    if (tempoStatus == null) {
                        continue;
                    }

                    bar.ApplyTempoChange(tempoStatus);
                }
            }

            for (int lineIndex = 0; lineIndex < this.Context.Header.NumberOfLines; lineIndex++) {
                foreach (var bar in this.Bars) {
                    var element = (from elem in bar.Elements
                                   where elem.Line != null && elem.Line.LineIndex == lineIndex
                                   select elem).FirstOrDefault();
                    //// var element = bar.Elements[lineIndex]; //// !!!!!!!!! attention!
                    //// Rhythmical and melodical structure in bar
                    if (element == null) {
                        continue;
                    }

                    element.Status.BarNumber = bar.BarNumber;
                    //// 2020/09  element.Status.BeatLevel = (byte)element.Tones.Count;  element.Status.ToneLevel = (byte)element.Tones.CountOfMelTones;

                    var musicalTonesInBar = element.Tones;
                    if (musicalTonesInBar == null || musicalTonesInBar.Count == 0) {
                        continue;
                    }

                    //// Rhythmical structure in bar - from tones
                    RhythmicStructure rstruct =
                        musicalTonesInBar.DetermineRhythmicStructure(this.Context.Header.System.RhythmicOrder);
                    element.Status.RhythmicStructure = rstruct;

                    //// Melodic structure in bar - from tones
                    var isMelodic = element.Status.IsMelodic;
                    if (isMelodic) {
                        var melodicTonesInBar = element.SingleMelodicTones();
                        if (melodicTonesInBar != null && melodicTonesInBar.Any()) {
                            //// lastMelodicTone
                            var mstruct = melodicTonesInBar.DetermineMelodicStructure(null, bar.HarmonicBar);
                            if (mstruct == null) {
                                var system = new MelodicSystem(1, 1);
                                mstruct = new MelodicStructure(system, 1);
                            }

                            element.Status.MelodicStructure = mstruct;
                        }
                    }

                    if (element.Status.LineType == MusicalLineType.Rhythmic) {
                        element.Status.Instrument = new MusicalInstrument(musicalTonesInBar.FirstRhythmicInstrument);
                    }

                    if (element.Status.LineType == MusicalLineType.Melodic) {
                        element.Status.Instrument = new MusicalInstrument(musicalTonesInBar.FirstMelodicInstrument);
                    }
                }
            }
        }

        /// <summary>
        /// Extracts the simple changes.
        /// </summary>
        public void SetBodyStatusFromTones() {
            var settings = this.Context.Settings;
            this.SetHarmonicStatusFromTones(3, settings.SettingsAnalysis.FullHarmonization);

            //// Status of tracks
            for (int lineIndex = 0; lineIndex < this.Context.Header.NumberOfLines; lineIndex++) {
                foreach (var bar in this.Bars) {
                    var element = bar.Elements[lineIndex];
                    //// 2018/10 Only fixed lines!?
                    //// 2018/10 element.Line.Status.LocalPurpose
                    if (element.Status.LocalPurpose == LinePurpose.Fixed) {
                        element.SetElementStatusFromTones();
                    }
                }
            }
        }

        /// <summary>
        /// Set Harmonic Status From Tones.
        /// </summary>
        /// <param name="givenMaxTonesInChord">The given maximum tones in chord.</param>
        /// <param name="givenFullHarmonization">if set to <c>true</c> [given full harmonization].</param>
        public void SetHarmonicStatusFromTones(byte givenMaxTonesInChord, bool givenFullHarmonization) {
            var numberOfBars = Math.Min(this.Context.Header.NumberOfBars, MusicalSettings.Singleton.SettingsProgram.MaxNumberOfBars);
            this.Context.Header.NumberOfBars = numberOfBars;

            var harmonicStreamAnalyzer = new HarmonicStreamAnalyzer(
                this.Context.Header,
                givenMaxTonesInChord,
                givenFullHarmonization) {
                SharpChordEdges = true,
                HarmonicSpace = new HarmonicSpace(this.Context.Header.System.HarmonicSystem) {
                    ConsiderEnergyDecreaseInTime = false,
                    ConsiderImpulseBindings = false,
                    StrongImpulseBindings = false,
                    ConsiderContinuityBindings = false
                }
            };

            foreach (MusicalBar bar in this.Bars) {
                var harmonicBar = harmonicStreamAnalyzer.DetermineHarmonyInBar(bar);
                //// var newHarmonicBar = (HarmonicBar)harmonicBar.Clone();
                //// bar.HarmonicBar = newHarmonicBar;
                bar.SetHarmonicBar(harmonicBar);
                bar.HarmonicBar.BarNumber = bar.BarNumber;
            }
        }

        #endregion

        #region Changes

        /// <summary>
        /// Extracts the simple changes.
        /// </summary>
        /// <param name="givenChangeType">Type of the given change.</param>
        /// <returns>
        /// Returns object.
        /// </returns>
        public IEnumerable<AbstractChange> ExtractSimpleChanges(MusicalChangeType givenChangeType) {
            var changes = new Collection<AbstractChange>();
            for (int lineIndex = 0; lineIndex < this.Context.Header.NumberOfLines; lineIndex++) {
                var lastOctave = -10;
                var lastLoudness = -10;
                var lastStaff = -10;
                var lastVoice = -10;
                var lastToneLevel = -10;
                var lastBeatLevel = -10;
                var lastRhythmicTension = -10;
                byte lastInstrument = (byte)MidiMelodicInstrument.None;

                foreach (var bar in this.Bars) {
                    if (bar.Elements.Count <= lineIndex) {
                        //// 2017/03 !?!?!
                        continue;
                    }

                    var element = bar.Elements[lineIndex];
                    if (element.Line == null) {
                        continue;
                    }

                    //// var isMelodic = element.Status.IsMelodic;
                    AbstractChange change;

                    //// *** Loudness ***
                    if (givenChangeType == MusicalChangeType.Loudness || givenChangeType == MusicalChangeType.All) {
                        var loudness = element.Status.Loudness;
                        if ((int)loudness != lastLoudness) {
                            change = new LoudnessChange(bar.BarNumber, element.Line.LineIndex) { LoudnessBase = loudness };
                            changes.Add(change);
                            lastLoudness = (int)loudness;
                        }
                    }

                    //// *** Energy *** 
                    if (givenChangeType == MusicalChangeType.Energy || givenChangeType == MusicalChangeType.All) {
                        var rstruct = element.Status.RhythmicStructure;
                        if (rstruct != null && (rstruct.ToneLevel != lastToneLevel || rstruct.Level != lastBeatLevel ||
                                                (byte)rstruct.FormalBehavior.Variance != lastRhythmicTension)) {
                            change = new EnergyChange(bar.BarNumber, element.Line.LineIndex) {
                                ToneLevel = rstruct.ToneLevel,
                                BeatLevel = rstruct.Level,
                                RhythmicTension = (byte)rstruct.FormalBehavior.Variance
                            };

                            changes.Add(change);
                            lastToneLevel = rstruct.ToneLevel;
                            lastBeatLevel = rstruct.Level;
                            lastRhythmicTension = (byte)rstruct.FormalBehavior.Variance;
                        }
                    }

                    //// *** StaffChange *** 
                    if (givenChangeType == MusicalChangeType.Staff || givenChangeType == MusicalChangeType.All) {
                        var staff = element.Status.Staff;
                        var voice = element.Status.Voice;
                        if (staff != lastStaff || voice != lastVoice) {
                            change = new StaffChange(bar.BarNumber, element.Line.LineIndex) { Staff = staff, Voice = voice };
                            changes.Add(change);
                        }

                        lastStaff = staff;
                        lastVoice = voice;
                    }

                    //// *** Octaves *** 
                    if (givenChangeType == MusicalChangeType.Octave || givenChangeType == MusicalChangeType.All) {
                        var octave = element.Status.Octave;
                        if ((int)octave != lastOctave) {
                            change = new OctaveChange(bar.BarNumber, element.Line.LineIndex) {
                                MusicalOctave = octave,
                                MusicalBand = MusicalProperties.BandTypeFromOctave(octave)
                            };
                            changes.Add(change);
                            lastOctave = (int)octave;
                        }
                    }

                    //// *** Instrument *** 
                    if (givenChangeType == MusicalChangeType.Instrument || givenChangeType == MusicalChangeType.All) {
                        if (element.Status.Instrument == null) {
                            element.Status.Instrument = new MusicalInstrument(MidiMelodicInstrument.None);
                        }

                        var instrument = element.Status.Instrument.Number;
                        var channel = element.Line.MainVoice.Channel;
                        if (instrument != lastInstrument) {
                            change = new InstrumentChange(bar.BarNumber, element.Line.LineIndex) {
                                Instrument = element.Status.Instrument,
                                Channel = channel
                            };

                            changes.Add(change);
                        }

                        lastInstrument = instrument;
                    }
                }
            }

            return changes;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Extracts the tempo changes.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        private IEnumerable<TempoChange> ExtractTempoChanges() {
            //// cyclomatic complexity 10:15
            //// var tracks = musicalBlock.MidiFile.AllTracks();
            //// IEnumerable<MidiEvent> tempoEvents = tracks.GetTempoEvents();
            var tempoEvents = this.TempoEvents;
            //// Music Xml
            if (tempoEvents == null) {
                return null;
            }

            var tempoChanges = new List<TempoChange>();
            int lastTempo = 0, lastBarNumber = 0;
            var header = this.Context.Header;
            foreach (var ev in tempoEvents) {
                if (!(ev is MetaTempo tempoEvent)) {
                    continue;
                }

                var currentTempo = tempoEvent.Tempo;
                if (currentTempo > 0 && currentTempo != lastTempo) {
                    var metricGround = MusicalProperties.GetMetricGround(header.Metric.MetricBase);
                    var barDivision =
                        MusicalProperties.BarDivision(header.Division, header.Metric.MetricBeat, metricGround);
                    var barNumber = (int)Math.Floor((double)tempoEvent.StartTime / barDivision) + 1;
                    if (barNumber > lastBarNumber) {
                        var change = new TempoChange(barNumber) { TempoNumber = currentTempo };
                        tempoChanges.Add(change);
                        lastBarNumber = barNumber;
                    }
                }

                lastTempo = currentTempo;
            }

            return tempoChanges;
        }

        /// <summary>
        /// Fists playing element in line.
        /// </summary>
        /// <param name="lineIdx">Index of the line.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private MusicalElement FistPlayingElementInTrack(int lineIdx) {
            MusicalElement element = null;
            foreach (var bar in this.Bars) {
                var elem = bar.Elements[lineIdx];
                if (elem?.Status == null || !elem.Status.HasInstrument) {
                    continue;
                }

                element = elem;
                break;
            }

            return element;
        }

        #endregion

        /// <summary>
        /// Sets the tempo status from events.
        /// </summary>
        /// <param name="tempoEvents">The tempo events.</param>
        private void SetTempoStatusFromEvents(List<IMidiEvent> tempoEvents) {
            //// 2019/02 taken from ConvertStripToBody
            var h = this.Context.Header;
            var barMidiDuration = MusicalProperties.BarMidiDuration(h.System.RhythmicOrder, h.Metric.MetricBeat, h.Metric.MetricGround, h.Division);
            if (tempoEvents != null) {
                var midiEvents = tempoEvents as IList<IMidiEvent>;
                if (midiEvents.Any()) {
                    int currentTempoNumber = 0;
                    foreach (var bar in this.Bars) {
                        //// tev.BarNumber == bar.BarNumber ?
                        if ((from tev in midiEvents
                             where tev.StartTime <= bar.TimePoint && bar.TimePoint <= tev.StartTime + barMidiDuration
                             select tev).FirstOrDefault() is MetaTempo metaTempo) {
                            currentTempoNumber = metaTempo.Tempo;
                        }

                        bar.TempoNumber = currentTempoNumber;
                    }
                }
            }
        }

        #region Sorting (internal only)

        /// <summary>
        /// Enables comparison of two events based on delta times.
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IComparer{MusicalBar}" />
        private sealed class BarComparer : IComparer<MusicalBar>
        {
            /// <summary>
            /// The line type
            /// </summary>
            private readonly MusicalLineType lineType;

            /// <summary>
            /// Initializes a new instance of the <see cref="BarComparer"/> class.
            /// </summary>
            /// <param name="givenLineType">Type of the given line.</param>
            public BarComparer(MusicalLineType givenLineType) {
                this.lineType = givenLineType;
            }

            #region Implementation of IComparer

            /// <summary>
            /// Compares two MidiEvents based on delta times.
            /// </summary>
            /// <param name="x">The first MidiEvent to compare.</param>
            /// <param name="y">The second MidiEvent to compare.</param>
            /// <returns>
            /// Returns -1 if x.StartTime is larger, 0 if they're the same, 1 otherwise.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// x
            /// or
            /// y
            /// </exception>
            public int Compare(MusicalBar x, MusicalBar y) {
                var barX = x;
                var barY = y;

                // Make sure they're valid
                if (barX == null) {
                    throw new ArgumentNullException(nameof(x));
                }

                if (barY == null) {
                    throw new ArgumentNullException(nameof(y));
                }

                // Compare the delta times
                return barX.ValueOfTicks(this.lineType).CompareTo(barY.ValueOfTicks(this.lineType)); //// CountOfTones
            }

            #endregion
        }

        #endregion
    }
}
