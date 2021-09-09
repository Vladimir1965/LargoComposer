// <copyright file="WindowManager.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// File Loader.
    /// </summary>
    public sealed class WindowManager {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly WindowManager InternalSingleton = new WindowManager();
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the <see cref="WindowManager"/> class from being created.
        /// </summary>
        private WindowManager() {
            this.Status = new Dictionary<string, WindowStatus>();
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>/// 
        [UsedImplicitly]
        public static WindowManager Singleton {
            get {
                Contract.Ensures(Contract.Result<WindowManager>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Event Sender is null.");
                }

                return InternalSingleton;
            }
        }

        /// <summary>
        /// Gets or sets the name of the largo shared assembly.
        /// </summary>
        /// <value>
        /// The name of the largo shared assembly.
        /// </value>
        public static string LargoSharedAssemblyName { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets the name of the editor windows assembly.
        /// </summary>
        /// <value>
        /// The name of the editor windows assembly.
        /// </value>
        public static string EditorWindowsAssemblyName { get; set; }   //// CA1044 (FxCop)

        /// <summary>
        /// Gets or sets the name of the x editor qualified assembly.
        /// </summary>
        /// <value>
        /// The name of the x editor qualified assembly.
        /// </value>
        public static string QualifiedAssemblyName { get; set; }   //// CA1044 (FxCop)

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the manager.
        /// </summary>
        /// <value>
        /// The name of the manager.
        /// </value>
        public string ManagerName { get; set; }
        
        /// <summary>
        /// Gets the manager path.
        /// </summary>
        /// <value>
        /// The manager path.
        /// </value>
        public string ManagerPath {
            get {
                var settings = MusicalSettings.Singleton;
                var folder = settings.Folders.GetFolder(MusicalFolder.InternalSettings);
                return Path.Combine(folder, (this.ManagerName ?? string.Empty) + "Windows.xml");
            }
        }      

        /// <summary>
        /// Gets or sets The status dictionary.
        /// </summary>
        public Dictionary<string, WindowStatus> Status { get; set; }
        #endregion

        #region Send Events

        /*
        /// <summary>
        /// Open Modal Window.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="windowTypeName">Window Type Name.</param>
        /// <param name="identifier">Given identifier.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static WinAbstract OpenModalWindow(string assemblyName, string windowTypeName, string identifier) {
            var qualifiedAssemblyName = (assemblyName == "LargoSharedWindows") ? CommonQualifiedAssemblyName : QualifiedAssemblyName;
            qualifiedAssemblyName = qualifiedAssemblyName.Replace("#WCLASSNAME", windowTypeName);
            //// qualifiedAssemblyName = qualifiedAssemblyName.Replace("#WASSEMBLYNAME", assemblyName);
            //// Type t1 = Type.GetType(string.Format("{0}.{1}", WindowManager.WindowNamespace, windowTypeName));
            var t1 = Type.GetType(qualifiedAssemblyName);
            var w = OpenModalWindow(t1, identifier);
            return w;
        }*/

        #endregion

        #region Public static factory methods
        /// <summary>
        /// Loads the musical settings.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns> Returns value. </returns>
        public static Dictionary<string, WindowStatus> LoadWindowManager(string path) {
            var root = XmlSupport.GetXDocRoot(path);
            if (root == null || root.Name != "WindowManager") {
                return null;
            }

            var xmanager = root;
            var manager = ReadWindowManager(xmanager);
            return manager;
        }

        /// <summary>
        /// Saves the musical settings.
        /// </summary>
        /// <param name="givenManager">The given manager.</param>
        /// <param name="path">The path.</param>
        public static void SaveWindowManager(Dictionary<string, WindowStatus> givenManager, string path) {
            var xmanager = WriteWindowManager(givenManager);
            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xmanager);
            xdoc.Save(path);
        }

        /// <summary>
        /// Reads the musical settings.
        /// </summary>
        /// <param name="markWindowManager">The mark window manager.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static Dictionary<string, WindowStatus> ReadWindowManager(XContainer markWindowManager) { //// XElement
            Contract.Requires(markWindowManager != null);
            if (markWindowManager == null) {
                return null;
            }

            Dictionary<string, WindowStatus> windowManager = new Dictionary<string, WindowStatus>();
            var xwindows = markWindowManager.Element("Windows");
            if (xwindows != null) {
                foreach (var xstatus in xwindows.Elements()) {
                    WindowStatus status = new WindowStatus {
                        Name = XmlSupport.ReadStringAttribute(xstatus.Attribute("Name")),
                        Top = XmlSupport.ReadDoubleAttribute(xstatus.Attribute("Top")),
                        Left = XmlSupport.ReadDoubleAttribute(xstatus.Attribute("Left")),
                        Height = XmlSupport.ReadDoubleAttribute(xstatus.Attribute("Height")),
                        Width = XmlSupport.ReadDoubleAttribute(xstatus.Attribute("Width"))
                    };
                    windowManager[status.Name] = status;
                }
            }

            return windowManager;
        }

        /// <summary>
        /// Writes the musical settings.
        /// </summary>
        /// <param name="givenManager">The given manager.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static XElement WriteWindowManager(Dictionary<string, WindowStatus> givenManager) {
            var xmanager = new XElement("WindowManager");

            //// Windows
            var xwindows = new XElement("Windows");
            var keys = givenManager.Keys;
            foreach (string key in keys) {
                var status = givenManager[key];
                var xstatus = new XElement("Window");
                xstatus.Add(new XAttribute("Name", status.Name));
                xstatus.Add(new XAttribute("Top", status.Top));
                xstatus.Add(new XAttribute("Left", status.Left));
                xstatus.Add(new XAttribute("Height", status.Height));
                xstatus.Add(new XAttribute("Width", status.Width));
                xwindows.Add(xstatus);
            }

            xmanager.Add(xwindows);
            return xmanager;
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Open Window.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="windowTypeName">Window class type name.</param>
        /// <param name="identifier">Window identifier.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public static WinAbstract OpenWindow(string assemblyName, string windowTypeName, string identifier)
        {
            string qualifiedAssemblyName;
            switch (assemblyName) {
                case "LargoSharedWindows": {
                        qualifiedAssemblyName = LargoSharedAssemblyName;
                        break;
                    }

                case "EditorWindows": {
                        qualifiedAssemblyName = EditorWindowsAssemblyName;
                        break;
                    }

                default: {
                        qualifiedAssemblyName = QualifiedAssemblyName;
                        break;
                    }
            }

            qualifiedAssemblyName = qualifiedAssemblyName.Replace("#WCLASSNAME", windowTypeName);
            //// Type t1 = Type.GetType(string.Format("{0}.{1}", WindowManager.WindowNamespace, windowTypeName));
            var t1 = Type.GetType(qualifiedAssemblyName);
            var w = OpenWindow(t1, identifier);
            return w;
        }

        /// <summary>
        /// Exists Window.
        /// </summary>
        /// <param name="windowType">Window class type.</param>
        /// <param name="ident">Window identifier.</param>
        /// <returns> Returns value.</returns>
        public static Window ExistsWindow(Type windowType, string ident) {
            if (Application.Current == null) {
                return null;
            }

            var ws = Application.Current.Windows;
            for (var intCounter = ws.Count - 1; intCounter >= 0; intCounter--) {
                var win = ws[intCounter];
                if (win == null) {
                    continue;
                }

                if (win.GetType() != windowType || (ident != null && string.CompareOrdinal(win.Title, ident) != 0)) {
                    continue;
                }

                //// 7.10.2019
                if (!win.IsActive) {
                    continue;
                }

                return win;
            }

            return null;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Changes the state to.
        /// </summary>
        /// <param name="state">The state.</param>
        public void ChangeStateTo(WindowState state)
        {
            if (state != WindowState.Minimized && state != WindowState.Normal)
            {
                return;
            }

            if (Application.Current == null)
            {
                return;
            }

            var ws = Application.Current.Windows;
            for (var intCounter = ws.Count - 1; intCounter >= 0; intCounter--)
            {
                var win = ws[intCounter];
                if (win == null)
                {
                    continue;
                }

                //// if (!win.IsActive) { continue; }

                win.WindowState = state;
            }
        }
        #endregion

        #region Positions
        /// <summary>
        /// Loads the position.
        /// </summary>
        /// <param name="givenWindow">The given window.</param>
        public void LoadPosition(Window givenWindow) {
            const double tolerance = 0.01;
            var t = givenWindow.GetType();
            if (!this.Status.ContainsKey(t.FullName ?? throw new InvalidOperationException())) {
                return;
            }

            var windowStatus = this.Status[t.FullName];
            if (windowStatus == null || Math.Abs(windowStatus.Height) < tolerance || Math.Abs(windowStatus.Width) < tolerance) {
                return;
            }

            givenWindow.Top = windowStatus.Top;
            givenWindow.Left = windowStatus.Left;
            givenWindow.Height = windowStatus.Height;
            givenWindow.Width = windowStatus.Width;
        }

        /// <summary> Loads position only. </summary>
        /// <param name="givenWindow"> The given window. </param>
        [UsedImplicitly]
        public void LoadPositionOnly(Window givenWindow) {
            var t = givenWindow.GetType();
            if (!this.Status.ContainsKey(t.FullName ?? throw new InvalidOperationException())) {
                return;
            }

            var windowStatus = this.Status[t.FullName];
            if (windowStatus == null) {
                return;
            }

            givenWindow.Top = windowStatus.Top;
            givenWindow.Left = windowStatus.Left;
        }

        /// <summary>
        /// Saves the position.
        /// </summary>
        /// <param name="givenWindow">The given window.</param>
        public void SavePosition(Window givenWindow) {
            //// Minimized windows have zero coordinates?!
            if (givenWindow.WindowState == WindowState.Minimized) {
                return;
            }

            var t = givenWindow.GetType();
            var windowStatus = new WindowStatus {
                Name = t.FullName,
                Top = givenWindow.Top,
                Left = givenWindow.Left,
                Width = givenWindow.Width,
                Height = givenWindow.Height
            };

            this.Status[t.FullName ?? throw new InvalidOperationException()] = windowStatus;
        }

        #endregion

        #region Private static methods

        /// <summary>
        /// Gets the public key token from assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        private static string GetPublicKeyTokenFromAssembly(Assembly assembly) {
            var bytes = assembly.GetName().GetPublicKeyToken();
            if (bytes == null || bytes.Length == 0) {
                return "None";
            }

            var publicKeyToken = string.Empty;
            for (int i = 0; i < bytes.GetLength(0); i++) {
                publicKeyToken += $"{bytes[i]:x2}";
            }

            return publicKeyToken;
        }

        /// <summary>
        /// Gets the name of the public key token from assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns> Returns value. </returns>
        [UsedImplicitly]
        private static string GetPublicKeyTokenFromAssemblyName(AssemblyName assemblyName) {
            var bytes = assemblyName.GetPublicKeyToken();
            if (bytes == null || bytes.Length == 0) {
                return "None";
            }

            var publicKeyToken = string.Empty;
            for (int i = 0; i < bytes.GetLength(0); i++) {
                publicKeyToken += $"{bytes[i]:x2}";
            }

            return publicKeyToken;
        }

        /// <summary>
        /// Open Window.
        /// </summary>
        /// <param name="windowType">Type of the window.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static WinAbstract OpenWindow(Type windowType, string identifier) {
            if (windowType == null) {
                return null;
            }

            var w = (WinAbstract)ExistsWindow(windowType, identifier);
            if (w == null) {
                w = OpenNewWindow(windowType, identifier, false);
            }
            else {
                w.Activate();
            }

            return w;
        }

        /// <summary>
        /// Opens the modal window.
        /// </summary>
        /// <param name="windowType">Type of the window.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static WinAbstract OpenModalWindow(Type windowType, string identifier) {
            if (windowType == null) {
                return null;
            }

            var w = (WinAbstract)ExistsWindow(windowType, identifier);
            if (w == null) {
                w = OpenNewWindow(windowType, identifier, true);
            }
            else {
                w.Activate();
            }

            return w;
        }

        /// <summary>
        /// Opens the new window.
        /// </summary>
        /// <param name="windowType">Type of the window.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="modal">If set to <c>true</c> [modal].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static WinAbstract OpenNewWindow(Type windowType, string identifier, bool modal) { ////
            Contract.Requires(windowType != null);
            var w = (WinAbstract)Activator.CreateInstance(windowType);
            w.Identifier = identifier;  //// w.Title = identifier; 
            w.LoadData();
            if (modal) {
                w.ShowDialog();
            }
            else {
                w.Topmost = true; //// 19.10.2013
                w.ShowActivated = true;
                w.ShowInTaskbar = true;
                w.Show();
            }

            w.Activate();
            return w;
        }
        #endregion
    }
}
