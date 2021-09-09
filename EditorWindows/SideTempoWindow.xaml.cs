// <copyright file="SideTempoWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Abstract;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EditorPanels;

namespace EditorWindows
{
    /// <summary>
    /// Interaction logic for Tempo Window.
    /// </summary>
    public partial class SideTempoWindow
    {
        #region Fields
        /// <summary>
        /// The start point
        /// </summary>
        private Point startPoint;

        #endregion

        /// <summary> Initializes a new instance of the <see cref="SideTempoWindow" /> class. </summary>
        public SideTempoWindow() {
            this.InitializeComponent();

            WindowManager.Singleton.LoadPosition(this);
            UserWindows.Singleton.LoadTheme(this.Resources.MergedDictionaries);
            this.Show();
            EditorSettings.Singleton.SidePanels.PanelOpen("SideTempo");
            //// this.Localize();

            this.ComboRawTempo.ItemsSource = DataEnums.GetListRawTempo;
            this.RefreshGridTempo();
            this.ComboRawTempo.SelectedIndex = 0;

            //// this.uCMusTempo1.SetTempoValue(120); //// this.bar.Status.TempoNumber
        }

        /// <summary> Refresh grid octave. </summary>
        private void RefreshGridTempo() {
            if (this.DataGridTempo == null) {
                return;
            }

            this.DataGridTempo.ItemsSource = DataEnums.ListMusicalTempo;

            if (this.DataGridTempo.Items.Count > 0) {
                this.DataGridTempo.SelectedIndex = 0;
            }
        }

        #region Properties
        /* Tempo
        /// <summary>
        /// Gets the musical tempo.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public int Tempo {
            get {
                var tempo = this.uCMusTempo1.Combo.SelectedItem is KeyValuePair gvt ? (int)gvt.NumericKey : (int)MusicalTempo.Tempo120;

                return tempo;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the current tempo.
        /// </summary>
        /// <param name="givenTempo">The given tempo.</param>
        [UsedImplicitly]
        public void SetCurrentTempo(int givenTempo) {
            this.uCMusTempo1.SetTempoValue(givenTempo);
        }*/
        #endregion

        #region Drag-drop
        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (!(e.Source is Image image)) {
                return;
            }

            // Store the mouse position
            // Initialize the drag & drop operation
            this.startPoint = e.GetPosition(null);
            var contextType = image.DataContext.GetType(); //// .ToString() LargoSharedClasses.Music.RhythmicFace
            if (contextType == typeof(KeyValuePair) && image.Tag.ToString() == "Tempo" && this.DataGridTempo.SelectedItem is KeyValuePair tempo) {
                DataObject data = new DataObject("MusicalTempo", (MusicalTempo)tempo.NumericKey);
                DragDrop.DoDragDrop(image, data, DragDropEffects.All);
                return;
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void List_MouseMove(object sender, MouseEventArgs e) {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = this.startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) {
            }
        }
        #endregion

        /// <summary>
        /// Raw tempo selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void RawTempoSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.ComboRawTempo.SelectedItem is KeyValuePair m) {
                var rawTempoIndex = (int)m.NumericKey;
                switch ((RawTempo)rawTempoIndex) {
                    case RawTempo.VerySlow: {
                        this.DataGridTempo.ItemsSource = DataEnums.ListLimitedTempo(1, 63);
                            break;
                        }

                    case RawTempo.Slow: {
                        this.DataGridTempo.ItemsSource = DataEnums.ListLimitedTempo(64, 87);
                            break;
                        }

                    case RawTempo.Middle: {
                        this.DataGridTempo.ItemsSource = DataEnums.ListLimitedTempo(88, 127);
                            break;
                        }

                    case RawTempo.Fast: {
                        this.DataGridTempo.ItemsSource = DataEnums.ListLimitedTempo(128, 177);
                            break;
                        }

                    case RawTempo.VeryFast: {
                        this.DataGridTempo.ItemsSource = DataEnums.ListLimitedTempo(178, 300);
                            break;
                        }
                }
            }

            this.DataGridTempo.SelectedIndex = 0;
            e.Handled = true;
        }

        #region Closing
        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            WindowManager.Singleton.SavePosition(this);
            EditorSettings.Singleton.SidePanels.PanelClose("SideTempo");
        }

        #endregion
    }
}
