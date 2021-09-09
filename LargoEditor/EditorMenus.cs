// <copyright file="EditorMenus.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Localization;
using LargoSharedControls.Abstract;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LargoEditor
{
    /// <summary>
    /// Editor Menus.
    /// </summary>
    public class EditorMenus {
        #region Context Menus
        /// <summary>
        /// The context menu of save
        /// </summary>
        private ContextMenu contextMenuOfSave;

        /// <summary>
        /// The context menu of corner cell
        /// </summary>
        private ContextMenu contextMenuOfCorner;

        /// <summary>
        /// The context menu content type
        /// </summary>
        private ContextMenu contextMenuContentType;

        /// <summary>
        /// The context menu of line
        /// </summary>
        private ContextMenu contextMenuOfLine;

        /// <summary>
        /// The context menu of bar
        /// </summary>
        private ContextMenu contextMenuOfBar;

        /// <summary>
        /// The context menu of main grid
        /// </summary>
        private ContextMenu contextMenuOfContent;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorMenus"/> class.
        /// </summary>
        /// <param name="givenEditorWindow">The given editor window.</param>
        /// <param name="givenActions">The given actions.</param>
        public EditorMenus(EditorWindow givenEditorWindow, EditorActions givenActions) {
            this.Editor = givenEditorWindow;
            this.Actions = givenActions;
        }

        /// <summary>
        /// Gets or sets the master.
        /// </summary>
        /// <value>
        /// The master.
        /// </value>
        public EditorWindow Editor { get; set; }

        /// <summary>
        /// Gets or sets the master.
        /// </summary>
        /// <value>
        /// The master.
        /// </value>
        public EditorActions Actions { get; set; }

        #region Private context menus
        /// <summary>
        /// Gets the context menu of single line.
        /// </summary>
        /// <value>
        /// The context menu of single line.
        /// </value>
        public ContextMenu ContextMenuOfSave {
            get {
                if (this.contextMenuOfSave != null) {
                    return this.contextMenuOfSave;
                }

                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 240
                };

                //// Export
                var item = new MenuItem {
                    Header = "Save as MIFI",
                    Tag = "2",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.DefaultIcon
                };
                item.Click += this.Actions.ExportFile;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Save as MIDI",
                    Tag = "1",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.DefaultIcon
                };
                item.Click += this.Actions.ExportFile;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Save as Music Xml",
                    Tag = "3",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon
                };
                item.Click += this.Actions.ExportFile;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Save as Music Mxl",
                    Tag = "4",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon
                };

                item.Click += this.Actions.ExportFile;
                contextMenu.Items.Add(item);

                this.contextMenuOfSave = contextMenu;
                return contextMenu;
            }
        }
        #endregion

        #region Context Menus Properties
        /// <summary>
        /// Gets the context menu of corner cell.
        /// </summary>
        /// <value>
        /// The context menu of corner cell.
        /// </value>
        public ContextMenu ContextMenuOfCorner {
            get {
                if (this.contextMenuOfCorner != null) {
                    return this.contextMenuOfCorner;
                }

                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 200
                };

                //// Lines
                var item = new MenuItem {
                    Header = LocalizedControls.String("Mark all lines as composed"),
                    Tag = 2,
                    Icon = UserInterfaceHelper.Icon("icon_composed"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.MarkEditorPurpose;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = LocalizedControls.String("Mark all lines as fixed"),
                    Tag = 1,
                    Icon = UserInterfaceHelper.Icon("icon_fixed"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.MarkEditorPurpose;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = LocalizedControls.String("Mark all lines as muted"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_muted"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.MarkEditorPurpose;
                contextMenu.Items.Add(item);

                contextMenu.Items.Add(new Separator()); //// -----------

                item = new MenuItem {
                    Header = LocalizedControls.String("Select all cells"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_select"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.SelectAllCells;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = LocalizedControls.String("Unelect all cells"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_unselect"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.UnselectAllCells;
                contextMenu.Items.Add(item);

                contextMenu.Items.Add(new Separator()); //// -----------

                item = new MenuItem {
                    Header = LocalizedControls.String("Show muted lines"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_show_muted"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.ShowMutedLines;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = LocalizedControls.String("Hide muted lines"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_hide_muted"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.HideMutedLines;
                contextMenu.Items.Add(item);

                this.contextMenuOfCorner = contextMenu;
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets the context menu of block.
        /// </summary>
        /// <value>
        /// The context menu of block.
        /// </value>
        public ContextMenu ContextMenuOfBlock {
            get {
                //// if (this.contextMenuOfBlock != null) {  return this.contextMenuOfBlock; }
                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 200
                };

                var item = new MenuItem {
                    Header = "Inner harmonic patterns",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_harmony"),
                    Tag = "2"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.BlockHarmonic;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Inner rhythmic patterns",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_rhythm"),
                    Tag = "1"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.BlockRhythmic;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Inner melodic patterns",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_melody"),
                    Tag = "2"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.BlockMelodic;
                contextMenu.Items.Add(item);

                //// this.contextMenuOfBlock = contextMenu;
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets the context menu of add line.
        /// </summary>
        /// <value>
        /// The context menu of add line.
        /// </value>
        public ContextMenu ContextMenuOfAddLine {
            get {
                //// if (this.contextMenuOfAddLine != null) { return this.contextMenuOfAddLine; }
                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 300
                };

                var item = new MenuItem {
                    Header = "Add line - Simple rhythm",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_simple"),
                    Tag = "1"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.EditorAddLine;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Add line - Harmonic rhythm",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_r_structure"),
                    Tag = "2"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.EditorAddLine;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Add line - Harmonic shape",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_r_shape"),
                    Tag = "3"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.EditorAddLine;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Add line - Harmonic rhythm in odd bars",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_r_structure_odd"),
                    Tag = "4"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.EditorAddLine;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Add line - Harmonic rhythm in even bars",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_r_structure_even"),
                    Tag = "5"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.EditorAddLine;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Add line - Harmonic shape in odd bars",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_r_shape_odd"),
                    Tag = "6"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.EditorAddLine;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Add line - Harmonic shape in even bars",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_r_shape_even"),
                    Tag = "7"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.EditorAddLine;
                contextMenu.Items.Add(item);

                //// this.contextMenuOfAddLine = contextMenu;
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets the context menu of add line.
        /// </summary>
        /// <value>
        /// The context menu of add line.
        /// </value>
        public ContextMenu ContextMenuOfHarmony {
            get {
                //// if (this.contextMenuOfHarmony != null) { return this.contextMenuOfHarmony; }

                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 200
                };

                var item = new MenuItem {
                    Header = "Saved harmonic templates",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "1"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.SavedHarmonicTemplates;
                contextMenu.Items.Add(item);

                //// this.contextMenuOfAddLine = contextMenu;
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets the context menu of add line.
        /// </summary>
        /// <value>
        /// The context menu of add line.
        /// </value>
        public ContextMenu ContextMenuOfRhythm {
            get {
                //// if (this.contextMenuOfRhythm != null) { return this.contextMenuOfRhythm; }

                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 200
                };

                var item = new MenuItem {
                    Header = "Saved rhythmic templates",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.Icon("icon_rhythm"),
                    Tag = "1"
                }; //// LocalizedControls.String(
                item.Click += this.Editor.SavedRhythmicTemplates;
                contextMenu.Items.Add(item);

                //// this.contextMenuOfAddLine = contextMenu;
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets the context menu of single line.
        /// </summary>
        /// <value>
        /// The context menu of single line.
        /// </value>
        public ContextMenu ContextMenuOfLine {
            get {
                if (this.contextMenuOfLine != null) {
                    return this.contextMenuOfLine;
                }

                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 200
                };

                //// Lines
                var item = new MenuItem {
                    Header = LocalizedControls.String("Mark line as composed"),
                    Tag = 2,
                    Icon = UserInterfaceHelper.Icon("icon_composed"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.MarkEditorLinePurpose;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = LocalizedControls.String("Mark line as fixed"),
                    Tag = 1,
                    Icon = UserInterfaceHelper.Icon("icon_fixed"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    IsCheckable = false
                };

                item.Click += this.Actions.MarkEditorLinePurpose;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = LocalizedControls.String("Mark line as muted"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_muted"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.MarkEditorLinePurpose;
                contextMenu.Items.Add(item);

                contextMenu.Items.Add(new Separator()); //// -----------

                item = new MenuItem {
                    Header = LocalizedControls.String("Clone line"),
                    Icon = UserInterfaceHelper.Icon("icon_clone"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                item.Click += this.Editor.CloneLineAsIs;
                contextMenu.Items.Add(item);
                               
                contextMenu.Items.Add(new Separator()); //// -----------

                item = new MenuItem {
                    Header = LocalizedControls.String("Convert line to melodic"),
                    Icon = UserInterfaceHelper.Icon("icon_convert_m"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                item.Click += this.Editor.ConvertEditorLineToMelodic;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = LocalizedControls.String("Convert line to rhythmic"),
                    Icon = UserInterfaceHelper.Icon("icon_convert_r"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                item.Click += this.Editor.ConvertEditorLineToRhythmic;
                contextMenu.Items.Add(item);

                contextMenu.Items.Add(new Separator()); //// -----------

                //// Item Delete
                item = new MenuItem {
                    Header = LocalizedControls.String("Delete line"),
                    Icon = UserInterfaceHelper.Icon("icon_delete"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                item.Click += this.Actions.DeleteEditorLine;
                contextMenu.Items.Add(item);
                
                contextMenu.Items.Add(new Separator()); //// -----------

                item = new MenuItem {
                    Header = LocalizedControls.String("Select line cells"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_select"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.SelectLineCells;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = LocalizedControls.String("Select odd line cells"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_select_odd"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.SelectOddLineCells;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = LocalizedControls.String("Select even line cells"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_select_even"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.SelectEvenLineCells;
                contextMenu.Items.Add(item);

                contextMenu.Items.Add(new Separator()); //// -----------

                item = new MenuItem {
                    Header = LocalizedControls.String("Unselect line cells"),
                    Tag = 0,
                    Icon = UserInterfaceHelper.Icon("icon_unselect"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += this.Actions.UnselectLineCells;
                contextMenu.Items.Add(item);

                this.contextMenuOfLine = contextMenu;
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets the context menu of single line.
        /// </summary>
        /// <value>
        /// The context menu of single line.
        /// </value>
        public ContextMenu ContextMenuOfBar {
            get {
                if (this.contextMenuOfBar != null) {
                    return this.contextMenuOfBar;
                }

                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 200
                };

                //// Lines
                var item = new MenuItem {
                    Header = "Add bar",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.DefaultIcon,
                    IsCheckable = false,
                    Width = 150
                };
                item.Click += this.Editor.EditorAddBar;
                contextMenu.Items.Add(item);

                this.contextMenuOfBar = contextMenu;
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets the context menu without selection.
        /// </summary>
        /// <value>
        /// The context menu without selection.
        /// </value>
        public ContextMenu ContextMenuOfContent {
            get {
                if (this.contextMenuOfContent != null) {
                    return this.contextMenuOfContent;
                }

                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray,
                    Width = 200
                };

                //// Rhythm
                var item = contextMenu;

                var subItem = new MenuItem {
                    Header = LocalizedControls.String("Set simple rhythm"),
                    Icon = UserInterfaceHelper.Icon("icon_simple"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                subItem.Click += this.Actions.SetSimpleRhythm;
                item.Items.Add(subItem);

                subItem = new MenuItem {
                    Header = LocalizedControls.String("Enrich rhythm"),
                    Icon = UserInterfaceHelper.Icon("icon_enrich"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                subItem.Click += this.Actions.EnrichRhythm;
                item.Items.Add(subItem);

                subItem = new MenuItem {
                    Header = LocalizedControls.String("Reduce rhythm"),
                    Icon = UserInterfaceHelper.Icon("icon_reduce"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                subItem.Click += this.Actions.ReduceRhythm;
                item.Items.Add(subItem);

                item.Items.Add(new Separator()); //// -----------

                subItem = new MenuItem {
                    Header = LocalizedControls.String("Set harmonic rhythm (structure)"),
                    Icon = UserInterfaceHelper.Icon("icon_r_structure"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Tag = "1"
                };
                subItem.Click += this.Actions.SetHarmonicRhythm;
                item.Items.Add(subItem);

                subItem = new MenuItem {
                    Header = LocalizedControls.String("Set harmonic rhythm (shape)"),
                    Icon = UserInterfaceHelper.Icon("icon_r_shape"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Tag = "0"
                };
                subItem.Click += this.Actions.SetHarmonicRhythm;
                item.Items.Add(subItem);

                item.Items.Add(new Separator()); //// -----------

                subItem = new MenuItem {
                    Header = LocalizedControls.String("Shift content right"),
                    Icon = UserInterfaceHelper.Icon("icon_shift_right"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                subItem.Click += this.Actions.ShiftContentRight;
                item.Items.Add(subItem);

                subItem = new MenuItem {
                    Header = LocalizedControls.String("Shift content left"),
                    Icon = UserInterfaceHelper.Icon("icon_shift_left"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                subItem.Click += this.Actions.ShiftContentLeft;
                item.Items.Add(subItem);

                item.Items.Add(new Separator()); //// -----------

                subItem = new MenuItem {
                    Header = LocalizedControls.String("Shift octave above"),
                    Icon = UserInterfaceHelper.Icon("icon_shift_above"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                subItem.Click += this.Actions.ShiftOctaveAbove;
                item.Items.Add(subItem);

                subItem = new MenuItem {
                    Header = LocalizedControls.String("Shift octave below"),
                    Icon = UserInterfaceHelper.Icon("icon_shift_below"),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                subItem.Click += this.Actions.ShiftOctaveBelow;
                item.Items.Add(subItem);

                this.contextMenuOfContent = contextMenu;
                return contextMenu;
            }
        }

        #endregion

        #region Context Menus Properties
        /// <summary>
        /// Gets the type of the context menu content.
        /// </summary>
        /// <value>
        /// The type of the context menu content.
        /// </value>
        public ContextMenu ContextMenuContentType {
            get {
                if (this.contextMenuContentType != null) {
                    return this.contextMenuContentType;
                }

                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.DimGray
                };

                var item = contextMenu;

                var subitem = new MenuItem {
                    Header = LocalizedMusic.String("Raster"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "1"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                subitem = new MenuItem {
                    Header = LocalizedMusic.String("Cell"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "2"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                subitem = new MenuItem {
                    Header = LocalizedMusic.String("Instrument / Loudness"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "3"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                subitem = new MenuItem {
                    Header = LocalizedMusic.String("Octave / Band"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "4"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                subitem = new MenuItem {
                    Header = LocalizedMusic.String("Tone / Beat level"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "5"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                subitem = new MenuItem {
                    Header = LocalizedMusic.String("Melodic function / shape"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "6"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                subitem = new MenuItem {
                    Header = LocalizedMusic.String("Rhythmic structure"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "7"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                subitem = new MenuItem {
                    Header = LocalizedMusic.String("Melodic structure"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "8"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                subitem = new MenuItem {
                    Header = LocalizedMusic.String("Rhythmic motive"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "9"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                subitem = new MenuItem {
                    Header = LocalizedMusic.String("Melodic motive"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    Tag = "10"
                };
                subitem.Click += this.Editor.ContentTypeChanged;
                item.Items.Add(subitem);

                this.contextMenuContentType = item;
                return item;
            }
        }

        #endregion
    }
}
