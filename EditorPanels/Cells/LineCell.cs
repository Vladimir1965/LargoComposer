// <copyright file="LineCell.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace EditorPanels.Cells
{
    using EditorPanels;
    using EditorPanels.Abstract;
    using LargoSharedClasses.Interfaces;
    using LargoSharedClasses.Melody;
    using LargoSharedClasses.Music;
    using LargoSharedClasses.Rhythm;
    using LargoSharedControls.Abstract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Xml.Linq;

    /// <summary>
    /// Interact logic for LineCell.
    /// </summary>
    public class LineCell : BaseCell
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineCell" /> class.
        /// </summary>
        /// <param name="givenMaster">The given master.</param>
        /// <param name="givenLine">The given line.</param>
        public LineCell(EditorSpace givenMaster, IAbstractLine givenLine) : base(givenMaster) {
            this.VoiceCells = new List<VoiceCell>();
            this.LineIndex = givenLine.LineIndex;
            this.Line = givenLine;
            this.Point = new MusicalPoint(this.Line.LineIndex, -1);
        }

        #endregion

        #region Main Properties

        /// <summary> Gets or sets the voice cells. </summary>
        /// <value> The voice cells. </value>
        public List<VoiceCell> VoiceCells { get; set; }

        /// <summary> Gets the main voice cell. </summary>
        /// <value> The main voice cell. </value>
        public VoiceCell MainVoiceCell {
            get {
                var vc = this.VoiceCells.FirstOrDefault();
                return vc;
            }
        }

        #endregion

        #region Main Properties

        /// <summary> Gets or sets the instrument. </summary>
        /// <value> The instrument. </value>
        public MusicalInstrument Instrument {
            get => this.Line.FirstStatus.Instrument;

            set {
                this.Line.FirstStatus.Instrument = value;
                if (this.MainVoiceCell != null) {
                    this.MainVoiceCell.Voice.Instrument = value;
                }
            }
        }

        /// <summary> Gets or sets the octave. </summary>
        /// <value> The octave. </value>
        public MusicalOctave Octave {
            get => this.Line.FirstStatus.Octave;

            set {
                this.Line.FirstStatus.Octave = value;
                if (this.MainVoiceCell != null) {
                    this.MainVoiceCell.Voice.Octave = value;
                }
            }
        }

        /// <summary> Gets or sets the loudness. </summary>
        /// <value> The loudness. </value>
        public MusicalLoudness Loudness {
            get => this.Line.FirstStatus.Loudness;

            set {
                this.Line.FirstStatus.Loudness = value;
                if (this.MainVoiceCell != null) {
                    this.MainVoiceCell.Voice.Loudness = value;
                }
            }
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public IAbstractLine Line { get; set; }

        /// <summary>
        /// Gets the context menu of line purpose.
        /// </summary>
        /// <value>
        /// The context menu of line purpose.
        /// </value>
        public ContextMenu ContextMenuOfLinePurpose {
            get {
                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                //// Lines
                var item = new MenuItem { Header = "Mark as composed", 
                    Tag = 2,
                    Icon = UserInterfaceHelper.DefaultIcon,
                    HorizontalAlignment = HorizontalAlignment.Center }; //// LocalizedControls.String

                item.Click += this.MarkLinePurpose;
                contextMenu.Items.Add(item);

                item = new MenuItem { Header = "Mark as fixed", 
                    Tag = 1,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    HorizontalAlignment = HorizontalAlignment.Center }; //// LocalizedControls.String

                item.Click += this.MarkLinePurpose;
                contextMenu.Items.Add(item);

                item = new MenuItem { Header = "Mark as muted", 
                    Tag = 0,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    HorizontalAlignment = HorizontalAlignment.Center }; //// LocalizedControls.String

                item.Click += this.MarkLinePurpose;
                contextMenu.Items.Add(item);

                return contextMenu;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="givenData">The given data.</param>
        /// <returns> Returns value. </returns>
        public bool SetData(IDataObject givenData) {
            if (this.Master is EditorSpace space) {
                var block = space.MusicalContent as MusicalBlock;

                if (givenData.GetData("MelodicInstrument") is MelodicInstrument melodicInstrument) {
                    var instrument = new MusicalInstrument((MidiMelodicInstrument)melodicInstrument.Id);
                    this.Instrument = instrument;

                    if (block != null) {
                        var elements = block.Body.ElementsOfLine(this.Line.LineIdent).ToList();
                        MusicalBody.SetInstrumentToElements(instrument, elements);
                        this.Line.MainVoice.Instrument = instrument;
                    }

                    return true;
                }

                if (givenData.GetData("RhythmicInstrument") is RhythmicInstrument rhythmicInstrument) {
                    var instrument = new MusicalInstrument((MidiRhythmicInstrument)rhythmicInstrument.Id);
                    this.Instrument = instrument;

                    if (block != null) {
                        var elements = block.Body.ElementsOfLine(this.Line.LineIdent).ToList();
                        MusicalBody.SetInstrumentToElements(instrument, elements);
                        this.Line.MainVoice.Instrument = instrument;
                    }

                    return true;
                }

                if (givenData.GetData("MusicalOctave") is MusicalOctave musicalOctave) {
                    MusicalOctave musicalOctave1 = musicalOctave;
                    this.Octave = musicalOctave1;

                    if (block != null) {
                        var elements = block.Body.ElementsOfLine(this.Line.LineIdent).ToList();
                        MusicalBody.SetOctaveToElements(musicalOctave1, elements);
                    }

                    return true;
                }

                if (givenData.GetData("MusicalLoudness") is MusicalLoudness musicalLoudness) {
                    MusicalLoudness musicalLoudness1 = musicalLoudness;
                    this.Loudness = musicalLoudness1;

                    if (block != null) {
                        var elements = block.Body.ElementsOfLine(this.Line.LineIdent).ToList();
                        MusicalBody.SetLoudnessToElements(musicalLoudness1, elements);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary> Gets or sets the formatted text. </summary>
        /// <returns> The formatted text. </returns>
        public override FormattedText FormattedText() {
            var sb = new StringBuilder();
            if (this.Line?.FirstStatus != null) { //// this.VoiceCells != null && this.MainVoiceCell != null) 
                sb.AppendFormat("{0}/ {1} {2}\n", this.Line.LineNumber, this.Octave, this.Loudness);
                sb.Append(this.Instrument);
            }

            var ft = AbstractText.Singleton.FormatText(sb.ToString(), (int)this.Width - SeedSize.BasicMargin);
            //// var s = string.Format("{0,2} {1}", this.Line?.LineNumber, CultureMaster.Localize(this.Line?.Purpose.ToString()));
            return ft;
        }

        #endregion

        #region Copy-Paste
        /// <summary>
        /// Copies this instance.
        /// </summary>
        public override void Copy() {
            if (this.MainVoiceCell == null) {
                return;
            }

            StringBuilder sb = new StringBuilder();
            var xstatus = this.MainVoiceCell.GetXElement;

            var item = xstatus.ToString();
            sb.AppendFormat("{0};", item);
            Clipboard.SetText(sb.ToString());
            //// Clipboard.SetDataObject(xstatus); 

            Console.Beep(880, 180);
        }

        /// <summary>
        /// Pastes this instance.
        /// </summary>
        public override void Paste() {
            if (this.MainVoiceCell == null) {
                return;
            }

            var s = Clipboard.GetText();
            if (string.IsNullOrEmpty(s)) {
                return;
            }

            var splitArray = s.Split(';');
            if (!splitArray.Any()) {
                return;
            }

            var item = splitArray.First();
            var xstatus = XElement.Parse(item);

            //// var header = this.Master.GetMusicalHeader;
            //// var status = new LineStatus(xstatus, header);
            this.MainVoiceCell.SetXElement(xstatus);

            //// var xstatus = XElement.Parse(item);
            //// this.Refresh();
            Console.Beep(990, 180);
            //// this.RedrawCell(false);
            var space = this.Master as EditorSpace;
            space?.InvalidateVisual();
        }

        #endregion

        #region Mark lines
        /// <summary>
        /// Marks the tracks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void MarkLinePurpose(object sender, RoutedEventArgs e) {
            var menuItem = (MenuItem)sender;
            switch ((int)menuItem.Tag) {
                case 0:
                    this.Line.Purpose = LinePurpose.Mute;
                    break;
                case 1:
                    this.Line.Purpose = LinePurpose.Fixed;
                    break;
                case 2:
                    this.Line.Purpose = LinePurpose.Composed;
                    break;
            }

            if (this.Master is EditorSpace space && space.MusicalContent is MusicalBlock block) {
                var elements = block.Body.ElementsOfLine(this.Line.LineIdent).ToList();
                MusicalBody.SetPurposeToElements(this.Line.Purpose, elements);
            }

            //// this.Refresh();
        }

        #endregion
    }
}