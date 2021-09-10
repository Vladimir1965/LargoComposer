// <copyright file="CommonMenus.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Localization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LargoSharedControls.Abstract
{
    /// <summary>
    /// Common Menus.
    /// </summary>
    public class CommonMenus
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        public static readonly CommonMenus Singleton = new CommonMenus();

        /// <summary>
        /// The context menu of save
        /// </summary>
        private ContextMenu contextMenuOfSave;

        #endregion

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
                item.Click += CommonActions.Singleton.ExportFile;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Save as MIDI",
                    Tag = "1",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.DefaultIcon
                };
                item.Click += CommonActions.Singleton.ExportFile;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Save as Music Xml",
                    Tag = "3",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon
                };
                item.Click += CommonActions.Singleton.ExportFile;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Save as Music Mxl",
                    Tag = "4",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    //// Icon = UserInterfaceHelper.DefaultIcon
                };

                item.Click += CommonActions.Singleton.ExportFile;
                contextMenu.Items.Add(item);

                this.contextMenuOfSave = contextMenu;
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets the context menu in grid.
        /// </summary>
        /// <value>
        /// The context menu in grid.
        /// </value>
        public ContextMenu ContextMenuInGrid {
            get {
                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = Brushes.Black,
                    Foreground = Brushes.White,
                    Width = 240
                };

                //// Lines
                var item = new MenuItem {
                    Header = LocalizedMusic.String("Play"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.DefaultIcon
                };

                //// AbstractText.LoadImage("icon_dragdot.png ");
                //// new Icon(@"C:\Private\SOLUTIONS-2020\PrivateWPF\Largo2020\LargoSharedControls\Images\button_play.png");
                item.Click += CommonActions.Singleton.Play;
                //// item.Icon = UserInterfaceHelper.DefaultIcon;
                contextMenu.Items.Add(item);

                item = new MenuItem { Header = LocalizedMusic.String("Edit") };
                item.Click += CommonActions.Singleton.Edit;
                //// item.Icon = UserInterfaceHelper.Icon_E;
                item.HorizontalAlignment = HorizontalAlignment.Left;
                contextMenu.Items.Add(item);

                //// ----------------------------
                item = new MenuItem {
                    Header = "Save as MIFI",
                    Tag = "1",
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                item.Click += CommonActions.Singleton.ExportFile;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Save as MIDI",
                    Tag = "2",
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                item.Click += CommonActions.Singleton.ExportFile;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Save as Music Xml",
                    Tag = "3",
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                item.Click += CommonActions.Singleton.ExportFile;
                contextMenu.Items.Add(item);

                item = new MenuItem {
                    Header = "Save as Music Mxl",
                    Tag = "4",
                    //// Icon = UserInterfaceHelper.DefaultIcon,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                item.Click += CommonActions.Singleton.ExportFile;
                contextMenu.Items.Add(item);
                //// ----------------------------
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets the context menu in image.
        /// </summary>
        /// <value>
        /// The context menu in image.
        /// </value>
        public ContextMenu ContextMenuInImage {
            get {
                var contextMenu = new ContextMenu {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                var item = new MenuItem { Header = LocalizedMusic.String("About"), HorizontalAlignment = HorizontalAlignment.Center };
                item.Click += CommonActions.Singleton.OpenAbout;
                contextMenu.Items.Add(item);

                return contextMenu;
            }
        }
        #endregion
    }
}
