// <copyright file="ConductorWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using ConductorPanels;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Composer;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;
using LargoSharedClasses.Port;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using LargoSharedWindows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LargoConductor
{
    /// <summary>
    /// Interaction logic for ConductorWindow.
    /// </summary>
    public partial class ConductorWindow {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConductorWindow"/> class.
        /// </summary>
        public ConductorWindow() {
            InitializeComponent();

            if (!MusicalSettings.LoadSettingsStartup(SettingsApplication.ManufacturerName, SettingsApplication.ApplicationName)) {
                MessageBox.Show(LocalizedControls.String("Load of settings failed!"));
                return;
            }

            UserFileLoader.Singleton.LoadWindowManager("LargoConductor", "ConductorWindow", typeof(ConductorWindow));
            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalData);
            PortCatalogs.Singleton.ReadXmlFiles(path);

            WindowManager.Singleton.LoadPosition(this);
            UserFileLoader.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();

            this.InternalNumber = 0;
            this.NumberOfBars = 1;
            this.BodyComposer = new BodyComposer();
            this.MusicPort = PortAbstract.CreatePort(MusicalSourceType.MIFI);

            this.Header = MusicalHeader.GetDefaultMusicalHeader;
            this.Header.NumberOfBars = this.NumberOfBars;
            this.Header.FileName = "Test-Kit";
            this.Header.Metric.MetricBeat = 6;
            this.Header.Tempo = 200;
            this.Context = new MusicalContext(MusicalSettings.Singleton, this.Header);

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            var list1 = new List<KeyValuePair>();  //// DataEnums.GetHarmonicSystems;
            list1.Add(new KeyValuePair(12, "12-Tones"));
            this.controlHarmonicSystem.LoadData(list1);

            var list2 = new List<KeyValuePair>();  //// DataEnums.GetHarmonicSystems;
            list2.Add(new KeyValuePair(12, "12-Ticks"));
            this.controlRhythmicSystem.LoadData(list2);

            SharedWindows.Singleton.SideHarmonicModality(null, null);
            SharedWindows.Singleton.SideRhythmicModality(null, null);

            this.AllowDrop = true;
            //// this.Drop += this.DropImage;

            this.Section = new KitSection(1, 1, "Test");
            this.Section.ListZones = new List<MusicalZone>();
            //// this.ListPanels = new Hashtable();
            //// this.ListPanels = new List<PanelMusicalZone>();

            /*
            this.ListZoneNumbers = new List<int>();
            this.ListZoneNumbers.Add(1);
            */
            this.ZoneGrid.ItemsSource = this.Section.ListZones;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        private MusicalHeader Header { get; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        private MusicalContext Context { get; }

        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <value>
        /// The section.
        /// </value>
        private KitSection Section { get; }

        //// private List<int> ListZoneNumbers { get; set; }
        //// private Hashtable ListPanels { get; set; }
        //// <PanelMusicalZone, PanelMusicalZone>
        //// private List<PanelMusicalZone> ListPanels { get; set; }

        /// <summary>
        /// Gets or sets the internal number.
        /// </summary>
        /// <value>
        /// The internal number.
        /// </value>
        private int InternalNumber { get; set; }

        /// <summary>
        /// Gets or sets the bar number.
        /// </summary>
        /// <value>
        /// The bar number.
        /// </value>
        private int BarNumber { get; set; }

        /// <summary>
        /// Gets the number of bars.
        /// </summary>
        /// <value>
        /// The number of bars.
        /// </value>
        private int NumberOfBars { get; }

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
        #endregion

        /// <summary>
        /// Gets the data grid rows.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns> Returns value. </returns>
        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid) {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (itemsSource == null) {
                yield return null;
            }

            foreach (var item in itemsSource) {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (row != null) {
                    yield return row;
                }
            }
        }

        /// <summary>
        /// Prepares the panels.
        /// </summary>
        private void PreparePanels() {
            if (this.Section == null) {
                return;
            }

            var rows = this.GetDataGridRows(this.ZoneGrid);
            var list = this.Section.ListZones;
            foreach (DataGridRow r in rows) {
                var idx = r.GetIndex();
                //// e.Row.Header = (idx).ToString();
                var zone = list[idx];
                if (zone == null) {
                    continue;
                }

                //// DataRowView rv = (DataRowView)r.Item;
                foreach (DataGridColumn column in this.ZoneGrid.Columns) {
                    if (column is DataGridTemplateColumn) {
                        var cp = column.GetCellContent(r) as ContentPresenter;
                        DataTemplate dt = cp.ContentTemplate; //// dt.Template
                        if (dt != null) {
                            var panel = dt.FindName("PanelZone", cp) as PanelMusicalZone;
                            if (panel != null) {
                                panel.LoadPanel(zone);
                                //// this.ListPanels[zone] = panel;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Goes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Go(object sender, RoutedEventArgs e) {
            var s = SupportFiles.FileToString(ConductorSettings.Singleton.PathToStatusFile);
            if (!string.IsNullOrEmpty(s)) {
                this.BarNumber = int.Parse(s);
                this.StatusText.Text = s;
            }

            var rows = this.GetDataGridRows(this.ZoneGrid);
            var list = this.Section.ListZones;
            foreach (DataGridRow r in rows) {
                var idx = r.GetIndex();
                //// e.Row.Header = (idx).ToString();
                var zone = list[idx];

                //// DataRowView rv = (DataRowView)r.Item;
                foreach (DataGridColumn column in this.ZoneGrid.Columns) {
                    if (column is DataGridTemplateColumn) {
                        var cp = column.GetCellContent(r) as ContentPresenter;
                        DataTemplate dt = cp.ContentTemplate; //// dt.Template
                        if (dt != null) {
                            var panel = dt.FindName("PanelZone", cp) as PanelMusicalZone;
                            if (panel != null) {
                                panel.Zone = zone;
                                panel.UpdateZoneFromPanel(this.BarNumber);
                                if (panel.Zone.RhythmicModality != null) {
                                    panel.Zone.RhythmicStructure = this.DetermineRhythmicStructure(panel.Zone.RhythmicModality, panel.Zone.Mobility);
                                }

                                panel.Refresh();
                            }
                        }
                    }
                }
            }

            SupportStructures.BuildElements(this.Section);
            this.ToMusic(null, null);
        }

        /// <summary>
        /// Determines the rhythmic structure.
        /// </summary>
        /// <param name="modality">The modality.</param>
        /// <param name="mobility">The mobility.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private RhythmicStructure DetermineRhythmicStructure(RhythmicModality modality, int mobility) {
            var order = this.Header.System.RhythmicSystem.Order;
            var bitArray = new BitArray(order);
            var modArray = modality?.BitArray;
            for (int i = 0; i < mobility; i++) {
                var idx = (int)Math.Round(1.00 * order / mobility * i, 0) % 12;
                if (modArray != null && idx < modArray.Count && !modArray[idx]) {
                    bitArray[idx] = false;
                }
                else {
                    bitArray[idx] = true;
                }
            }

            //// for (int i = 0; i < order; i++) {
            //// bitArray[i] = MathSupport.RandomNatural(this.Length) < this.Mobility;  } 

            //// var tickSystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Shape, order);
            var shape = new RhythmicShape(this.Header.System.RhythmicSystem, bitArray);
            var structure = new RhythmicStructure(order, shape);

            return structure;
        }

        /// <summary>
        /// Converts to music.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ToMusic(object sender, RoutedEventArgs e) {
            var strip = new MusicalStrip(this.Context);
            var block = new MusicalBlock {
                Header = this.Header,
                Strip = strip
            };

            block.Header.Number = 1;

            var listZones = this.Section.ListZones;
            foreach (var z in listZones) {
                var zone = z as MusicalZone;
                ////var zone = this.GridZone.SelectedItem as MusicalZone;

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

                    for (int bar = 1; bar <= zone.Bars; bar++) {
                        var stn = (LineStatus)status.Clone();
                        stn.BarNumber = bar;
                        line.StatusList.Add(stn);

                        if (stn.RhythmicStructure == null) {
                            stn.RhythmicStructure = zone.RhythmicStructure;
                        }

                        this.BuildMelodicPlan(this.Header, zone, stn);
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
        /// Builds the melodic plan.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="zone">The zone.</param>
        /// <param name="stn">The STN.</param>
        private void BuildMelodicPlan(MusicalHeader header, MusicalZone zone, LineStatus stn) {
            if (stn.RhythmicStructure == null) {
                return;        
            }

            foreach (var element in zone.Elements) {
                var altitude = 80;
                var p1 = new MusicalPitch(header.System.HarmonicSystem, altitude);
                var t1 = new MusicalTone(p1, new BitRange(), MusicalLoudness.MeanLoudness, stn.BarNumber);
                stn.MelodicPlan.PlannedTones.Add(t1);

                altitude += 2 * element.EnterStep;
                for (int i = 0; i <= stn.RhythmicStructure.ToneLevel; i++) {
                    var p2 = new MusicalPitch(header.System.HarmonicSystem, altitude);
                    var t2 = new MusicalTone(p2, new BitRange(), MusicalLoudness.MeanLoudness, stn.BarNumber);
                    stn.MelodicPlan.PlannedTones.Add(t2);
                    altitude += 2 * element.InnerStep;
                }
            }
        }

        /// <summary>
        /// Exports the and play.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="block">The block.</param>
        private void ExportAndPlay(MusicalHeader header, MusicalBlock block) {
            BlockComposer blockComposer = new BlockComposer {
                ComposedBlock = block
            };
            blockComposer.PrepareLinesForComposition();
            this.BodyComposer.ComposeMusic(block.Body);
            blockComposer.ComposedBlock.ConvertBodyToStrip(true, true);

            var bundle = MusicalBundle.GetEnvelopeOfBlock(block, "Kit");
            var name = header.FileName + (this.InternalNumber++).ToString();
            var path = ConductorSettings.Singleton.PathToInternalStream;
            this.MusicPort.WriteMusicFile(bundle, Path.Combine(path, name + ".mif"));
        }

        /// <summary>
        /// Handles the Loaded event of the PanelZone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void PanelZone_Loaded(object sender, RoutedEventArgs e) {
            /*
            var list = this.Section.ListZones;
            var panel = sender as PanelMusicalZone;
            if (panel.Zone == null) {
                var zone = list.LastOrDefault();
                panel.LoadPanel(zone);
                //// list.Add(panel.Zone);
                //// if (!this.ListPanels.Contains(panel)) {
                this.ListPanels.Add(panel);
                //// }
            } */
        }

        /// <summary>
        /// Handles the LoadingRow event of the ZoneGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridRowEventArgs"/> instance containing the event data.</param>
        private void ZoneGrid_LoadingRow(object sender, DataGridRowEventArgs e) {
            /*
            var list = this.Section.ListZones;
            var idx = e.Row.GetIndex();
            //// e.Row.Header = (idx).ToString();
            var zone = list[idx];
            foreach (DataGridColumn t in this.ZoneGrid.Columns) {
                if (t is DataGridTemplateColumn) {
                   var obj = t.GetCellContent(e.Row);
                   var panel = t.GetCellContent(e.Row) as PanelMusicalZone;
                    if (panel != null) {
                        panel.LoadPanel(zone);
                        this.ListPanels[zone] = panel;
                    }
                    //// label.SetBinding(DitatDateTimeLabel.DisplayUtcDateProperty, new Binding(t.SortMemberPath));
                }
            } */

            //// this.PreparePanels();
        }

        /// <summary>
        /// Adds the zone.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AddZone(object sender, RoutedEventArgs e) {
            //// var listZones = this.Section.ListZones;
            var zone = new MusicalZone(24, 1, 1, 1, 1, string.Empty, null);
            this.Section.ListZones.Add(zone);
            this.ZoneGrid.ItemsSource = null;
            this.ZoneGrid.ItemsSource = this.Section.ListZones;
            this.UpdateLayout();
            this.PreparePanels();
            /*
            this.ListZoneNumbers.Add(1);
            this.ZoneGrid.ItemsSource = null;
            this.ZoneGrid.ItemsSource = this.ListZoneNumbers;
            */
            //// this.Go(null, null);
        }

        /// <summary>
        /// Saves the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Save(object sender, RoutedEventArgs e) {
            var listZones = this.Section.ListZones;
            foreach (var z in listZones) {
                var zone = z as MusicalZone;
            }
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the ZoneGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ZoneGrid_LayoutUpdated(object sender, EventArgs e) {
           //// this.PreparePanels();
        }

        /// <summary>
        /// Handles the Tick event of the dispatcherTimer control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DispatcherTimer_Tick(object source, EventArgs e)
        {
            this.RefreshBoard();
            this.Go(null, null);
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
    }
}

/*
/// <summary>
/// Builds the harmony.
/// </summary>
/// <param name="header">The header.</param>
/// <param name="block">The block.</param>
/// <param name="listHarmony">The list harmony.</param>
private void BuildHarmony(MusicalHeader header, MusicalBlock block, List<KitHarmony> listHarmony) {
    foreach (var bar in block.Body.Bars) {
        var kitHarmony = listHarmony[bar.BarNumber - 1];
        var harBar = new HarmonicBar(0, 0);

        var system = header.System.HarmonicSystem;
        var hs = new HarmonicStructure(system, kitHarmony.StructuralCode) {
            BitFrom = 0,
            Length = 12
        };

        harBar.AddStructure(hs);
        var harmonicBar = new HarmonicBar(header, harBar);
        bar.SetHarmonicBar(harmonicBar);
    }
}*/