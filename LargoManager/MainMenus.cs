// <copyright file="MainMenus.cs" company="Traced-Ideas, Czech republic">
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

namespace LargoManager
{
    /// <summary>
    /// Main Menus.
    /// </summary>
    public class MainMenus
    {
        #region Context Menus
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenus"/> class.
        /// </summary>
        /// <param name="givenWindow">The given window.</param>
        /// <param name="givenActions">The given actions.</param>
        public MainMenus(MainWindow givenWindow, MainActions givenActions) {
            this.Main = givenWindow;
            this.Actions = givenActions;
        }

        #region Public Properties
        /// <summary>
        /// Gets or sets the master.
        /// </summary>
        /// <value>
        /// The master.
        /// </value>
        public MainWindow Main { get; set; }

        /// <summary>
        /// Gets or sets the master.
        /// </summary>
        /// <value>
        /// The master.
        /// </value>
        public MainActions Actions { get; set; }
        #endregion

        #region Context Menus Properties
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

                //// AbstractText.LoadImage("dragdot.png");
                //// new Icon(@"C:\Private\SOLUTIONS-2020\PrivateWPF\Largo2020\LargoSharedControls\Images\button_play.png");
                item.Click += CommonActions.Singleton.Play;
                //// item.Icon = UserInterfaceHelper.DefaultIcon;
                contextMenu.Items.Add(item);

                item = new MenuItem { Header = LocalizedMusic.String("Edit"), HorizontalAlignment = HorizontalAlignment.Left };
                item.Click += CommonActions.Singleton.Edit;
                //// item.Icon = UserInterfaceHelper.DefaultIcon;
                contextMenu.Items.Add(item);

                item = new MenuItem { Header = LocalizedMusic.String("Rename"), HorizontalAlignment = HorizontalAlignment.Left };
                item.Click += this.Actions.Rename;
                //// item.Icon = UserInterfaceHelper.DefaultIcon;
                contextMenu.Items.Add(item);

                //// ----------------------------
                item = new MenuItem {
                    Header = "Save as MIFI",
                    Tag = "1",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Icon = UserInterfaceHelper.DefaultIcon
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

        #endregion
    }
}
