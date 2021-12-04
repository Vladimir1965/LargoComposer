using LargoSharedClasses.Abstract;
using LargoSharedClasses.Composer;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using LargoSharedClasses.Rhythm;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using LargoSharedWindows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

            if (!MusicalSettings.LoadSettingsStartup(SettingsApplication.ManufacturerName, SettingsApplication.ApplicationName)) {
                MessageBox.Show(LocalizedControls.String("Load of settings failed!"));
                return;
            }

            //// UserFileLoader.Singleton.LoadWindowManager("LargoModeler", "MainWindow", typeof(MainWindow));
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalData);
            PortCatalogs.Singleton.ReadXmlFiles(path);

            WindowManager.Singleton.LoadPosition(this);
            UserFileLoader.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();

            this.BodyComposer = new BodyComposer();
            this.MusicPort = PortAbstract.CreatePort(MusicalSourceType.MIFI);

            this.Header = MusicalHeader.GetDefaultMusicalHeader;
            this.Header.FileName = "Test-Modeler";
            this.Header.Metric.MetricBeat = 6;
            this.Header.Tempo = 200;

            var list1 = new List<KeyValuePair>();  //// DataEnums.GetHarmonicSystems;
            list1.Add(new KeyValuePair(12, "12-Tones"));
            this.controlHarmonicSystem.LoadData(list1);

            var list2 = new List<KeyValuePair>();  //// DataEnums.GetHarmonicSystems;
            list2.Add(new KeyValuePair(12, "12-Ticks"));
            this.controlRhythmicSystem.LoadData(list2);

            //// SharedWindows.Singleton.SideHarmonicModality(null, null);
            //// SharedWindows.Singleton.SideRhythmicModality(null, null);
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

        /// <summary>
        /// Gets or sets the staff zones.
        /// </summary>
        /// <value>
        /// The staff zones.
        /// </value>
        private List<StaffZone> StaffZones { get; set; }

        /// <summary>
        /// Gets or sets the staff bars.
        /// </summary>
        /// <value>
        /// The staff bars.
        /// </value>
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

        /// <summary>
        /// Generates the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Generate(object sender, RoutedEventArgs e)
        {
            var numberOfBars = 8;
            for (int barNumber = 1; barNumber <= numberOfBars; barNumber++) {
            }
        }

        /// <summary>
        /// Refreshes the board.
        /// </summary>
        private void RefreshBoard()
        {
            var harmonicSystem = this.Header.System.HarmonicSystem;
            var harmonicModality = this.PanelMusicalHarmony.HarmonicModality ?? new HarmonicModality(harmonicSystem, 1387);
            if (HarmonyBoard.Singleton.HarmonicModality?.Number != harmonicModality?.Number) {
                HarmonyBoard.Singleton.HarmonicModality = harmonicModality;
                var q = new GeneralQualifier();
                var hv = StructuralVarietyFactory.NewHarmonicStructModalVariety(StructuralVarietyType.BinarySubstructuresOfModality, harmonicModality, q, 100);
                if (hv.StructList.Count > 0) {
                    HarmonyBoard.Singleton.HarmonicStructures = hv.StructList.ToList();
                } else {
                    HarmonyBoard.Singleton.HarmonicStructures = PortCatalogs.DefaultHarmonicStructures(3, 3);
                }

                HarmonyBoard.Singleton.SelectedStructures = (from hs in HarmonyBoard.Singleton.HarmonicStructures where hs.Level == 3 select hs).ToList();
            }
        }

        private void RefreshBars()
        {
            this.StaffBars = new List<StaffBar>(); //// ObservableCollection
            for (int barNumber = 1; barNumber <= 8; barNumber++) {
                var bar = new StaffBar(barNumber);

                var harStructure = this.PanelMusicalHarmony.GenNextHarmonicStructure();
                if (harStructure == null) {
                    return;
                }

                harStructure.BitFrom = 0;
                harStructure.Length = this.Header.System.HarmonicOrder;
                
                var harBar = new HarmonicBar(0, 0);
                harBar.AddStructure(harStructure);
                var harmonicBar = new HarmonicBar(this.Header, harBar);
                bar.HarmonicBar = harmonicBar;

                this.StaffBars.Add(bar);
            }

            foreach (var bar in this.StaffBars) {
                foreach (var zone in this.StaffZones) {
                    var staffElement = new StaffElement(zone, bar, (zone.Name == "Bas") ? BeatValues.Beat : BeatValues.Light);
                    var rsystem = this.Header.System.RhythmicSystem;
                    staffElement.DetermineRhythm(rsystem);
                    zone.staffElements.Add(staffElement);
                }
            }

            this.Header.NumberOfBars = this.StaffBars.Count;
            this.dataGridBars.ItemsSource = this.StaffBars;
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.StaffZones = new List<StaffZone>(); //// ObservableCollection
            var zone1 = new StaffZone("Bas");
            this.StaffZones.Add(zone1);

            var zone2 = new StaffZone("Sopran");           
            this.StaffZones.Add(zone2);

            /// this.dataGrid1.DataContext = Records;
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
            this.ToMusic(null, null);
        }

        private void NumberOfBarsChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        /// <summary>
        /// Displays the selected elements.
        /// </summary>
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
            this.Context = new MusicalContext(MusicalSettings.Singleton, this.Header);
            var strip = new MusicalStrip(this.Context);
            var block = new MusicalBlock {
                Header = this.Header,
                Strip = strip
            };

            block.Header.Number = 1;

            foreach (var staffZone in this.StaffZones) {
                int lineNumInZone = 0;
                for (int li = 0; li < staffZone.Lines; li++) {
                    var line = new MusicalLine {
                        Purpose = LinePurpose.Composed
                    };

                    strip.AddLine(line, true);

                    staffZone.Status = new LineStatus();
                    staffZone.Orchestra = PortCatalogs.Singleton.OrchestraEssence[li];
                    if (staffZone.Orchestra != null && lineNumInZone < staffZone.Orchestra.ListVoices.Count) {
                        line.MainVoice = staffZone.Orchestra.ListVoices[lineNumInZone];
                        line.Voices = new List<IAbstractVoice> { line.MainVoice };
                        //// line.MainVoice.Channel = strip.FindFreeChannel(line.LineIndex);
                    }

                    var status = staffZone.Status;
                    status.Octave = line.MainVoice.Octave;
                    status.Loudness = staffZone.Loudness; //// line.MainVoice.Loudness;
                    //// status.Instrument = line.MainVoice.Instrument;
                    status.OrchestraUnit = staffZone.Orchestra;
                    status.LineType = MusicalLineType.Melodic;
                    status.LocalPurpose = LinePurpose.Composed;

                    foreach (var staffBar in this.StaffBars) {
                        var staffElement = (from elem in staffZone.staffElements where elem.StaffBar.Number == staffBar.Number select elem).FirstOrDefault();
                        var stn = (LineStatus)status.Clone();
                        stn.BarNumber = staffBar.Number;
                        line.StatusList.Add(stn);

                        /* if (stn.RhythmicStructure == null) {
                            stn.RhythmicStructure = zone.RhythmicStructure;
                        } */
                        stn.RhythmicStructure = staffElement.RhythmicStructure;

                        //// this.BuildMelodicPlan(this.Header, zone, stn);
                        stn.Voice = (byte)line.LineIndex;
                    }

                    //// line.FirstStatus = line.StatusList[0];
                    lineNumInZone++;
                }
            }

            block.ConvertStripToBody(true);

            foreach (var staffBar in this.StaffBars) {
                var mbar = block.Body.Bars[staffBar.Number-1];
                mbar.SetHarmonicBar(staffBar.HarmonicBar);
            }

                /*
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
                }*/

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
            bool playOnline = true;
            MusicalPlayer.Play(block, playOnline);
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            this.RefreshBoard();
            this.RefreshBars();
        }
    }
}
