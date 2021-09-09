// <copyright file="UserWindows.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Support;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows;
using EditorPanels;

namespace EditorWindows
{
    /// <summary>User File Loader</summary>
    public class UserWindows
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static UserWindows singleton = new UserWindows();

        /// <summary> The inspector window. </summary>
        private WinAbstract inspectorWindow;

        /// <summary> The tools (means/instrument) window. </summary>
        private WinAbstract sideInstrumentWindow;

        /// <summary> The harmony window. </summary>
        private WinAbstract sideHarmonicStructuresWindow;

        /// <summary>
        /// The side rhythmic structures window
        /// </summary>
        private WinAbstract sideRhythmicStructuresWindow;

        /// <summary>
        /// The saved harmony window.
        /// </summary>
        private WinAbstract sideHarmonicModalityWindow;

        /// <summary>
        /// The side rhythmic modality window
        /// </summary>
        private WinAbstract sideRhythmicModalityWindow;

        /// <summary> The melody window. </summary>
        private WinAbstract sideMelodyWindow;

        /* <summary> The rhythm window. </summary>
        private WinAbstract sideRhythmWindow; */

        /// <summary> The tempo window. </summary>
        private WinAbstract sideTempoWindow;

        /// <summary> The voices window. </summary>
        private WinAbstract sideVoicesWindow;

        /// <summary> The orchestra window. </summary>
        private WinAbstract sideOrchestraWindow;
        
        /// <summary> The saved harmony window. </summary>
        private WinAbstract savedHarmonicTemplates;

        /// <summary> The saved rhythmic material. </summary>
        private WinAbstract savedRhythmicTemplates;

        /// <summary> The saved orchestra window. </summary>
        private WinAbstract savedOrchestraTemplates;
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static UserWindows Singleton {
            get {
                Contract.Ensures(Contract.Result<UserWindows>() != null);
                return singleton;
            }

            private set => singleton = value;
        }
        #endregion

        #region Public methods - Tools
        /// <summary> Adds resource dictionary. </summary>
        /// <param name="directories">  The directories. </param>
        /// <param name="resourcePath"> Full pathname of the resource file. </param>
        public void AddResourceDictionary(Collection<ResourceDictionary> directories, string resourcePath) {
            directories.Add(new ResourceDictionary() { Source = new Uri(resourcePath, UriKind.Absolute) });
            //// Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(src, UriKind.Relative) });
        }

        /// <summary> Loads a theme. </summary>
        /// <param name="directories">  The directories. </param>
        public void LoadTheme(Collection<ResourceDictionary> directories) {
            //// string src = "pack://application:,,,/LargoSharedWindows;component/BlueTheme/Office2010Blue.MSControls.Core.Implicit.xaml";
            //// this.AddResourceDictionary(dirs, src);
            string resource = "pack://application:,,,/LargoSharedControls;component/SharedTheme.xaml";
            this.AddResourceDictionary(directories, resource);
        }

        #region Site Windows - Inspector
        /// <summary> Open Window - Inspector. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Routed event information. </param>
        public void Inspector(object sender, RoutedEventArgs e) {
            if (this.inspectorWindow != null && this.inspectorWindow.IsVisible) {
                this.inspectorWindow.Close();
                return;
            }

            this.inspectorWindow = WindowManager.OpenWindow("EditorWindows", "InspectorWindow", null);
        }
        #endregion

        #region Site Windows - Elementary
        /// <summary> Open Window - Tools. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Routed event information. </param>
        public void SideInstrument(object sender, RoutedEventArgs e) {
            if (this.sideInstrumentWindow != null && this.sideInstrumentWindow.IsVisible) {
                this.sideInstrumentWindow.Close();
                return;
            }

            this.sideInstrumentWindow = WindowManager.OpenWindow("EditorWindows", "SideInstrumentWindow", null);
            ///// WindowManager.OpenWindow("XLargo", "DetailInstrumentWindow", null);
        }

        /// <summary> Open Window - Harmonies. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Routed event information. </param>
        public void SideHarmonicStructures(object sender, RoutedEventArgs e) {
            if (this.sideHarmonicStructuresWindow != null && this.sideHarmonicStructuresWindow.IsVisible) {
                this.sideHarmonicStructuresWindow.Close();
                return;
            }

            this.sideHarmonicStructuresWindow = WindowManager.OpenWindow("EditorWindows", "SideHarmonicStructuresWindow", null);
            if (SideHarmonicStructuresWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        /// <summary>
        /// Sides the rhythmic structures.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SideRhythmicStructures(object sender, RoutedEventArgs e) {
            if (this.sideRhythmicStructuresWindow != null && this.sideRhythmicStructuresWindow.IsVisible) {
                this.sideRhythmicStructuresWindow.Close();
                return;
            }

            this.sideRhythmicStructuresWindow = WindowManager.OpenWindow("EditorWindows", "SideRhythmicStructuresWindow", null);
            if (SideRhythmicStructuresWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        /// <summary> Open Window - Harmonies. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> Routed event information. </param>
        public void SideHarmonicModality(object sender, RoutedEventArgs e) {
            if (this.sideHarmonicModalityWindow != null && this.sideHarmonicModalityWindow.IsVisible) {
                this.sideHarmonicModalityWindow.Close();
                return;
            }

            this.sideHarmonicModalityWindow = WindowManager.OpenWindow("EditorWindows", "SideHarmonicModalityWindow", null);
            if (SideHarmonicModalityWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        /// <summary>
        /// Sides the rhythmic modality.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SideRhythmicModality(object sender, RoutedEventArgs e) {
            if (this.sideRhythmicModalityWindow != null && this.sideRhythmicModalityWindow.IsVisible) {
                this.sideRhythmicModalityWindow.Close();
                return;
            }

            this.sideRhythmicModalityWindow = WindowManager.OpenWindow("EditorWindows", "SideRhythmicModalityWindow", null);
            if (SideRhythmicModalityWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        /// <summary>
        /// Sides the voices.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SideOrchestra(object sender, RoutedEventArgs e) {
            if (this.sideOrchestraWindow != null && this.sideOrchestraWindow.IsVisible) {
                this.sideOrchestraWindow.Close();
                return;
            }

            this.sideOrchestraWindow = WindowManager.OpenWindow("EditorWindows", "SideOrchestraWindow", null);
            //// if (SideOrchestraWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            //// }
        }

        /// <summary>
        /// Sides the voices.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SideVoices(object sender, RoutedEventArgs e) {
            if (this.sideVoicesWindow != null && this.sideVoicesWindow.IsVisible) {
                this.sideVoicesWindow.Close();
                return;
            }

            this.sideVoicesWindow = WindowManager.OpenWindow("EditorWindows", "SideVoicesWindow", null);
            if (SideVoicesWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        /// <summary> Open Window - Melody. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Routed event information. </param>
        public void SideMelody(object sender, RoutedEventArgs e)
        {
            if (this.sideMelodyWindow != null && this.sideMelodyWindow.IsVisible) {
                this.sideMelodyWindow.Close();
                return;
            }

            this.sideMelodyWindow = WindowManager.OpenWindow("EditorWindows", "SideMelodyWindow", null);
            if (SideMelodyWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        /// <summary> Open Window - Tempo. </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> Routed event information. </param>
        public void SideTempo(object sender, RoutedEventArgs e)
        {
            if (this.sideTempoWindow != null && this.sideTempoWindow.IsVisible) {
                this.sideTempoWindow.Close();
                return;
            }

            this.sideTempoWindow = WindowManager.OpenWindow("EditorWindows", "SideTempoWindow", null);
            ///// WindowManager.OpenWindow("XLargo", "DetailInstrumentWindow", null);
        }

        #endregion

        #region Site Windows - Saved
        /// <summary>
        /// Saved orchestra.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SavedOrchestraTemplates(object sender, RoutedEventArgs e)
        {
            if (this.savedOrchestraTemplates != null && this.savedOrchestraTemplates.IsVisible) {
                this.savedOrchestraTemplates.Close();
                return;
            }

            this.savedOrchestraTemplates = WindowManager.OpenWindow("EditorWindows", "TemplatesSavedOrchestra", null);
            ///// WindowManager.OpenWindow("XLargo", "DetailInstrumentWindow", null);
        }

        /// <summary>
        /// Saved harmony.
        /// </summary>
        public void SavedHarmonicTemplates()
        {
            if (this.savedHarmonicTemplates != null && this.savedHarmonicTemplates.IsVisible) {
                this.savedHarmonicTemplates.Close();
                return;
            }

            this.savedHarmonicTemplates = WindowManager.OpenWindow("EditorWindows", "TemplatesSavedHarmonic", null);
            ///// WindowManager.OpenWindow("XLargo", "DetailInstrumentWindow", null);
        }

        /// <summary>
        /// Saved rhythmic material.
        /// </summary>
        public void SavedRhythmicTemplates()
        {
            if (this.savedRhythmicTemplates != null && this.savedRhythmicTemplates.IsVisible) {
                this.savedRhythmicTemplates.Close();
                return;
            }

            this.savedRhythmicTemplates = WindowManager.OpenWindow("EditorWindows", "TemplatesSavedRhythmic", null);
            ///// WindowManager.OpenWindow("XLargo", "DetailInstrumentWindow", null);
        }
        #endregion

        /// <summary>
        /// Basic windows.
        /// </summary>
        public void UserSidePanels() {
            var setting = EditorSettings.Singleton;
            if (setting.SidePanels.IsOpen("SideInstrument")) {
                UserWindows.Singleton.SideInstrument(null, null);
            }

            if (setting.SidePanels.IsOpen("SideHarmonicStructures")) {
                UserWindows.Singleton.SideHarmonicStructures(null, null);
            }

            if (setting.SidePanels.IsOpen("SideRhythmicStructures")) {
                UserWindows.Singleton.SideRhythmicStructures(null, null);
            }

            if (setting.SidePanels.IsOpen("SideHarmonicModality")) {
                UserWindows.Singleton.SideHarmonicModality(null, null);
            }

            if (setting.SidePanels.IsOpen("SideRhythmicModality")) {
                UserWindows.Singleton.SideHarmonicModality(null, null);
            }

            /*
            if (setting.SidePanels.IsOpen("SideRhythm")) {
                UserFileLoader.Singleton.SideRhythm(null, null);
            }*/

            if (setting.SidePanels.IsOpen("SideMelody")) {
                UserWindows.Singleton.SideMelody(null, null);
            }

            if (setting.SidePanels.IsOpen("SideTempo")) {
                UserWindows.Singleton.SideTempo(null, null);
            }

            if (setting.SidePanels.IsOpen("SideVoices")) {
                UserWindows.Singleton.SideVoices(null, null);
            }
        }

        /*
        /// <summary>
        /// Component windows.
        /// </summary>
        public void AllSidePanels() {
            this.SideInstrument(null, null);
            this.SideHarmonicStructures(null, null);
            this.SideHarmonicModality(null, null);
            this.SideRhythmicStructures(null, null);
            this.SideRhythmicModality(null, null);
            //// this.SideRhythm(null, null);
            this.SideMelody(null, null);
            this.SideTempo(null, null);
            this.SideVoices(null, null);
        } */

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <param name="intTag">The integer tag.</param>
        /// <returns> Returns value. </returns>
        public WinAbstract GetPanel(int intTag)
        {
            WinAbstract w = null;
            switch (intTag) {
                //// Harmony
                case 1: {
                        w = this.sideHarmonicStructuresWindow;
                        break;
                    }
                //// Rhythm
                case 2: {
                        w = this.sideRhythmicStructuresWindow;
                        break;
                    }
                //// Melody
                case 3: {
                        w = this.sideMelodyWindow;
                        break;
                    }
                //// Instruments
                case 4: {
                        w = this.sideInstrumentWindow;
                        break;
                    }
                //// Tempo
                case 5: {
                        w = this.sideTempoWindow;
                        break;
                    }
                //// Voices
                case 6: {
                        w = this.sideVoicesWindow;
                        break;
                    }

                //// Harmonic Modality
                case 7: {
                        w = this.sideHarmonicModalityWindow;
                        break;
                    }

                //// Rhythmic Modality
                case 8: {
                        w = this.sideRhythmicModalityWindow;
                        break;
                    }

                case 9: {
                        w = this.sideOrchestraWindow;
                        break;
                    }
            }

            return w;
        }

        /// <summary>
        /// Views the panel.
        /// </summary>
        /// <param name="intTag">The integer tag.</param>
        public void ViewPanel(int intTag) {
            switch (intTag) {
                //// Harmony
                case 1: {
                        this.SideHarmonicStructures(null, null);
                        break;
                    }
                //// Rhythm
                case 2: {
                        this.SideRhythmicStructures(null, null);
                        break;
                    }
                //// Melody
                case 3: {
                        this.SideMelody(null, null);
                        break;
                    }
                //// Instruments
                case 4: {
                        this.SideInstrument(null, null);
                        break;
                    }
                //// Tempo
                case 5: {
                        this.SideTempo(null, null);
                        break;
                    }
                //// Voices
                case 6: {
                        this.SideVoices(null, null);
                        break;
                    }

                //// Harmonic Modality
                case 7: {
                        this.SideHarmonicModality(null, null);
                        break;
                    }

                //// Rhythmic Modality
                case 8: {
                        this.SideRhythmicModality(null, null);
                        break;
                    }

                case 9: {
                        this.SideOrchestra(null, null);
                        break;
                    }
            }
        }

        #endregion
    }
}