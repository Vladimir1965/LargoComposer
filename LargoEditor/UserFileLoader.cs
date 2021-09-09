// <copyright file="UserFileLoader.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows;
using EditorWindows;
using LargoSharedClasses.Settings;
using LargoSharedClasses.Support;
using LargoSharedWindows;

namespace LargoEditor
{
    /// <summary>User File Loader</summary>
    public class UserFileLoader
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static UserFileLoader singleton = new UserFileLoader();

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

        /// <summary>
        /// Loads the window manager.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="mainClassName">Name of the main class.</param>
        /// <param name="mainObjType">Type of the main object.</param>
        public void LoadWindowManager(string moduleName, string mainClassName, Type mainObjType)
        {
            string folder = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalSettings);
            //// folder = Path.Combine(folder, moduleName);
            string filepath = Path.Combine(folder, moduleName + @"Windows.xml");
            var winManagerStatus = WindowManager.LoadWindowManager(filepath);
            if (winManagerStatus == null) {
                MessageBox.Show(string.Format("Window Manager failed to load file: {0}", filepath), SettingsApplication.ApplicationName);
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

            objType = typeof(SideInstrumentWindow);
            if (objType.AssemblyQualifiedName != null) {
                var assemblyQualifiedName = objType.AssemblyQualifiedName.Replace("SideInstrumentWindow", "#WCLASSNAME");
                WindowManager.EditorWindowsAssemblyName = assemblyQualifiedName;
            }

            if (mainObjType.AssemblyQualifiedName != null) {
                var assemblyQualifiedName = mainObjType.AssemblyQualifiedName.Replace(mainClassName, "#WCLASSNAME");
                WindowManager.QualifiedAssemblyName = assemblyQualifiedName;
            }
        }

        #endregion
    }
}