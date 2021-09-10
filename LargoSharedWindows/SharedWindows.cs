// <copyright file="SharedWindows.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedWindows
{
    using LargoSharedClasses.Support;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Windows;

    public class SharedWindows
    {

        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static SharedWindows singleton = new SharedWindows();

        /// <summary>
        /// The saved harmony window.
        /// </summary>
        private WinAbstract sideHarmonicModalityWindow;

        /// <summary>
        /// The side rhythmic modality window
        /// </summary>
        private WinAbstract sideRhythmicModalityWindow;


        /// <summary> The harmony window. </summary>
        private WinAbstract sideHarmonicStructuresWindow;

        /// <summary>
        /// The side rhythmic structures window
        /// </summary>
        private WinAbstract sideRhythmicStructuresWindow;

        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static SharedWindows Singleton {
            get {
                Contract.Ensures(Contract.Result<SharedWindows>() != null);
                return singleton;
            }

            private set => singleton = value;
        }
        #endregion

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
            }

            return w;
        }


        #region Public methods - Tools

        /// <summary> Open Window - Harmonies. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> Routed event information. </param>
        public void SideHarmonicModality(object sender, RoutedEventArgs e)
        {
            if (this.sideHarmonicModalityWindow != null && this.sideHarmonicModalityWindow.IsVisible) {
                this.sideHarmonicModalityWindow.Close();
                return;
            }

            this.sideHarmonicModalityWindow = WindowManager.OpenWindow("LargoSharedWindows", "SideHarmonicModalityWindow", null);
            if (SideHarmonicModalityWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        /// <summary>
        /// Sides the rhythmic modality.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SideRhythmicModality(object sender, RoutedEventArgs e)
        {
            if (this.sideRhythmicModalityWindow != null && this.sideRhythmicModalityWindow.IsVisible) {
                this.sideRhythmicModalityWindow.Close();
                return;
            }

            this.sideRhythmicModalityWindow = WindowManager.OpenWindow("LargoSharedWindows", "SideRhythmicModalityWindow", null);
            if (SideRhythmicModalityWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        /// <summary> Open Window - Harmonies. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e">      Routed event information. </param>
        public void SideHarmonicStructures(object sender, RoutedEventArgs e)
        {
            if (this.sideHarmonicStructuresWindow != null && this.sideHarmonicStructuresWindow.IsVisible) {
                this.sideHarmonicStructuresWindow.Close();
                return;
            }

            this.sideHarmonicStructuresWindow = WindowManager.OpenWindow("LargoSharedWindows", "SideHarmonicStructuresWindow", null);
            if (SideHarmonicStructuresWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        /// <summary>
        /// Sides the rhythmic structures.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SideRhythmicStructures(object sender, RoutedEventArgs e)
        {
            if (this.sideRhythmicStructuresWindow != null && this.sideRhythmicStructuresWindow.IsVisible) {
                this.sideRhythmicStructuresWindow.Close();
                return;
            }

            this.sideRhythmicStructuresWindow = WindowManager.OpenWindow("LargoSharedWindows", "SideRhythmicStructuresWindow", null);
            if (SideRhythmicStructuresWindow.Singleton != null) {
                ////  DetailHarmonicWindow.Singleton.LoadData(this.HarmonicModel);
            }
        }

        #endregion

        #region Public methods - Tools
        /// <summary> Adds resource dictionary. </summary>
        /// <param name="directories">  The directories. </param>
        /// <param name="resourcePath"> Full pathname of the resource file. </param>
        public void AddResourceDictionary(Collection<ResourceDictionary> directories, string resourcePath)
        {
            directories.Add(new ResourceDictionary() { Source = new Uri(resourcePath, UriKind.Absolute) });
            //// Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(src, UriKind.Relative) });
        }

        /// <summary> Loads a theme. </summary>
        /// <param name="directories">  The directories. </param>
        public void LoadTheme(Collection<ResourceDictionary> directories)
        {
            //// string src = "pack://application:,,,/LargoSharedWindows;component/BlueTheme/Office2010Blue.MSControls.Core.Implicit.xaml";
            //// this.AddResourceDictionary(dirs, src);
            string resource = "pack://application:,,,/LargoSharedControls;component/SharedTheme.xaml";
            this.AddResourceDictionary(directories, resource);
        }
        #endregion

    }
}
