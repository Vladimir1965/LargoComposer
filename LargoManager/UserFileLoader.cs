// <copyright file="UserFileLoader.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using LargoSharedWindows;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;

namespace LargoManager
{
    /// <summary>User File Loader</summary>
    public class UserFileLoader
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static UserFileLoader singleton = new UserFileLoader();

        /// <summary>
        /// The saved harmony window.
        /// </summary>
        private WinAbstract sideHarmonicModalityWindow;

        /// <summary>
        /// The side rhythmic modality window
        /// </summary>
        private WinAbstract sideRhythmicModalityWindow;

        #endregion

        #region Static properties
        /// <summary>
        /// Gets the EditorLine Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static UserFileLoader Singleton {
            get {
                Contract.Ensures(Contract.Result<UserFileLoader>() != null);
                return singleton;
            }

            private set => singleton = value;
        }
        #endregion

        #region Public methods
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

        /// <summary>
        /// Loads the window manager.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="mainClassName">Name of the main class.</param>
        /// <param name="mainObjType">Type of the main object.</param>
        public void LoadWindowManager(string moduleName, string mainClassName, Type mainObjType) {
            string folder = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalSettings);
            //// folder = Path.Combine(folder, moduleName);
            string filepath = Path.Combine(folder, moduleName + @"Windows.xml");
            var winManagerStatus = WindowManager.LoadWindowManager(filepath);
            if (winManagerStatus == null) {
                MessageBox.Show(string.Format("Window Manager failed to load file: {0}\n\n", filepath), SettingsApplication.ApplicationName);
                return;
            }

            WindowManager.Singleton.ManagerName = moduleName;
            WindowManager.Singleton.Status = winManagerStatus;

            //// 2019/05
            var objType = typeof(InherentException);
            if (objType.AssemblyQualifiedName != null) {
                var assemblyQualifiedName = objType.AssemblyQualifiedName.Replace("InherentException", "#WCLASSNAME");
                WindowManager.LargoSharedAssemblyName = assemblyQualifiedName;
            } 

            if (mainObjType.AssemblyQualifiedName != null) {
                var assemblyQualifiedName = mainObjType.AssemblyQualifiedName.Replace(mainClassName, "#WCLASSNAME");
                WindowManager.QualifiedAssemblyName = assemblyQualifiedName;
            }
        }

        #endregion

        #region Public methods - Tools

        /// <summary> Open Window - Harmonies. </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> Routed event information. </param>
        public void SideHarmonicModality(object sender, RoutedEventArgs e)
        {
            if (this.sideHarmonicModalityWindow != null && this.sideHarmonicModalityWindow.IsVisible) {
                return;
            }

            this.sideHarmonicModalityWindow = WindowManager.OpenWindow("LargoSharedWindows", "SideHarmonicModalityWindow", null);
        }

        /// <summary>
        /// Sides the rhythmic modality.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void SideRhythmicModality(object sender, RoutedEventArgs e)
        {
            if (this.sideRhythmicModalityWindow != null && this.sideRhythmicModalityWindow.IsVisible) {
                return;
            }

            this.sideRhythmicModalityWindow = WindowManager.OpenWindow("LargoSharedWindows", "SideRhythmicModalityWindow", null);
        }

        #endregion
    }
}