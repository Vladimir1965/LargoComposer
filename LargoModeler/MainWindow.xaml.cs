using LargoSharedClasses.Composer;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using LargoSharedClasses.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace LargoModeler
{

    /// <summary>
    /// Main Window.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Properties
        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        private MusicalHeader Header { get; set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        private MusicalContext Context { get; set; }

        private List<StaffZone> StaffZones { get; set; }
        private List<StaffBar> StaffBars { get; set; }

        /// <summary>
        /// Gets the music port.
        /// </summary>
        /// <value>
        /// The music port.
        /// </value>
        private PortAbstract MusicPort { get; }

        /// <summary>
        /// Gets the body composer.
        /// </summary>
        /// <value>
        /// The body composer.
        /// </value>
        private BodyComposer BodyComposer { get; }

        /// <summary>
        /// Gets or sets the internal number.
        /// </summary>
        /// <value>
        /// The internal number.
        /// </value>
        private int InternalNumber { get; set; }
        #endregion 

        private void Generate(object sender, RoutedEventArgs e)
        {
            var numberOfBars = 8;
            for (int barNumber = 1; barNumber <= numberOfBars; barNumber++) {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.StaffBars = new List<StaffBar>(); //// ObservableCollection
            for (int barNumber = 1; barNumber <= 8; barNumber++) {
                var bar = new StaffBar(barNumber);
                this.StaffBars.Add(bar);
            }

            this.StaffZones = new List<StaffZone>(); //// ObservableCollection
            var zone1 = new StaffZone("Bas");
            foreach (var bar in this.StaffBars) {
                zone1.staffElements.Add(new StaffElement(zone1, bar));
            }

            this.StaffZones.Add(zone1);

            var zone2 = new StaffZone("Sopran");
            foreach (var bar in this.StaffBars) {
                zone2.staffElements.Add(new StaffElement(zone2, bar));
            }

            this.StaffZones.Add(zone2);

            /// this.dataGrid1.DataContext = Records;
            this.dataGridBars.ItemsSource = this.StaffBars;
            this.dataGridZones.ItemsSource = this.StaffZones;
            //// var staffZones = new List<StaffZone>(); //// ObservableCollection
            //// this.dataGrid1.DataContext = Records;
            //// this.dataGridZones.ItemsSource = staffZones;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
        }

        private void Play(object sender, RoutedEventArgs e)
        {
        }

        private void NumberOfBarsChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void DisplaySelectedElements()
        {
            var staffZone = this.dataGridZones.SelectedItem as StaffZone;
            if (staffZone == null) {
                return;
            }

            var staffBar = this.dataGridBars.SelectedItem as StaffBar;
            if (staffBar == null) {
                return;
            }

            var elements = from elem in staffZone.staffElements where elem.StaffBar.Number == staffBar.Number select elem;
            this.dataGridElements.ItemsSource = elements.ToList();
        }



        private void dataGridZones_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.DisplaySelectedElements();
        }

        private void dataGridBars_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.DisplaySelectedElements();
        }

        /// <summary>
        /// Converts to music.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ToMusic(object sender, RoutedEventArgs e)
        {
            this.Header = MusicalHeader.GetDefaultMusicalHeader;
            this.Header.NumberOfBars = this.StaffBars.Count;
            this.Header.FileName = "Test-Modeler";
            this.Header.Metric.MetricBeat = 6;
            this.Header.Tempo = 200;

            this.Context = new MusicalContext(MusicalSettings.Singleton, this.Header);

            var strip = new MusicalStrip(this.Context);
            var block = new MusicalBlock {
                Header = this.Header,
                Strip = strip
            };

            block.Header.Number = 1;

            foreach (var zone in this.StaffZones) {
                int lineNumInZone = 0;
                for (int li = 0; li < zone.Lines; li++) {
                    var line = new MusicalLine {
                        Purpose = LinePurpose.Composed
                    };

                    strip.AddLine(line, true);

                    if (zone.Orchestra != null && lineNumInZone < zone.Orchestra.ListVoices.Count) {
                        line.MainVoice = zone.Orchestra.ListVoices[lineNumInZone];
                        line.Voices = new List<IAbstractVoice> { line.MainVoice };
                        //// line.MainVoice.Channel = strip.FindFreeChannel(line.LineIndex);
                    }

                    var status = zone.Status;
                    status.Octave = line.MainVoice.Octave;
                    status.Loudness = zone.Loudness; //// line.MainVoice.Loudness;
                    //// status.Instrument = line.MainVoice.Instrument;
                    status.OrchestraUnit = zone.Orchestra;

                    foreach (var bar in this.StaffBars) {
                        var stn = (LineStatus)status.Clone();
                        stn.BarNumber = bar.Number;
                        line.StatusList.Add(stn);

                        /* if (stn.RhythmicStructure == null) {
                            stn.RhythmicStructure = zone.RhythmicStructure;
                        } */

                        //// this.BuildMelodicPlan(this.Header, zone, stn);
                        stn.Voice = (byte)line.LineIndex;
                    }

                    //// line.FirstStatus = line.StatusList[0];
                    lineNumInZone++;
                }
            }

            block.ConvertStripToBody(true);

            foreach (var mbar in block.Body.Bars) {
                var harStructure = this.PanelMusicalHarmony.GenNextHarmonicStructure();
                if (harStructure == null) {
                    return;
                }

                harStructure.BitFrom = 0;
                harStructure.Length = this.Header.System.HarmonicOrder;
                var harBar = new HarmonicBar(0, 0);
                harBar.AddStructure(harStructure);
                var harmonicBar = new HarmonicBar(this.Header, harBar);
                mbar.SetHarmonicBar(harmonicBar);
            }

            //// var listHarmony = KitFactory.Singleton.GetHarmony();
            //// this.BuildHarmony(this.Header, block, listHarmony);
            this.ExportAndPlay(this.Header, block);
        }


        /// <summary>
        /// Exports the and play.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="block">The block.</param>
        private void ExportAndPlay(MusicalHeader header, MusicalBlock block)
        {
            BlockComposer blockComposer = new BlockComposer {
                ComposedBlock = block
            };
            blockComposer.PrepareLinesForComposition();
            this.BodyComposer.ComposeMusic(block.Body);
            blockComposer.ComposedBlock.ConvertBodyToStrip(true, true);

            var bundle = MusicalBundle.GetEnvelopeOfBlock(block, "Kit");
            var name = header.FileName + (this.InternalNumber++).ToString();
            var path = @"C:\temp"; //// ConductorSettings.Singleton.PathToInternalStream;
            this.MusicPort.WriteMusicFile(bundle, Path.Combine(path, name + ".mif"));
        }


    }
}
