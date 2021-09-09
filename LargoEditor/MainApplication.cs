// <copyright file="MainApplication.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Localization;
using LargoSharedClasses.Music;
using LargoSharedClasses.Support;
using System;

namespace LargoEditor
{
    using LargoSharedClasses.Settings;
    using System.Windows;

    /// <summary> A main application. </summary>
    public class MainApplication : Application
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainApplication" /> class.
        /// Main single - instance application.
        /// </summary>
        public MainApplication() {
            MusicalBundle.MaxNumberOfBarsInBlock = 200;
        }
        #endregion

        #region Public static properties
        /// <summary>
        /// Gets or sets Parameter0.
        /// </summary>
        /// <value> Property description. </value>
        public static string Parameter0 { get; set; }

        /// <summary>
        /// Gets or sets Parameter1.
        /// </summary>
        /// <value> Property description. </value>
        public static string Parameter1 { get; set; }

        /// <summary>
        /// Gets or sets Parameter2.
        /// </summary>
        /// <value> Property description. </value>
        public static string Parameter2 { get; set; }
        #endregion

        #region Public properties

        /// <summary> Gets the window. </summary>
        /// <value> my window. </value>
        public EditorWindow EditorWindow { get; private set; }

        #endregion

        #region Dispatcher for Unhandled Exceptions
        /// <summary>
        /// Gets or sets a value indicating whether Boolean property to determine if we need to handle the exception or not.
        /// </summary>
        /// <value> Property description. </value>
        public bool DoHandle { get; set; }
        #endregion

        #region Static Methods
        /// <summary>
        /// Edits the file.
        /// </summary>
        /// <param name="givenFilePath">The given file path.</param>
        /// <param name="tectonicTemplatePath">The tectonic template path.</param>
        /// <param name="harmonicTemplatePath">The harmonic template path.</param>
        public static void EditFile(string givenFilePath, string tectonicTemplatePath, string harmonicTemplatePath)
        {
            Parameter0 = givenFilePath;
            Parameter1 = tectonicTemplatePath;
            Parameter2 = harmonicTemplatePath;
            LoadFiles();
        }

        /// <summary>
        /// Loads the files.
        /// </summary>
        public static void LoadFiles()
        {
            EditorWindow win;
            if (string.IsNullOrEmpty(Parameter0))
            {
                win = new EditorWindow();
                if (win.NewMusicFile(Parameter1, Parameter2))
                {
                    win.Show();
                }
            }
            else
            {
                win = new EditorWindow();
                if (win.LoadMusicFile())
                {
                    win.Show();
                }
            }
        }
        #endregion

        #region Methods
        /// <summary> Process the arguments. </summary>
        /// <param name="args"> The arguments. </param>
        /// <param name="firstInstance"> True to first instance. </param>
        public void ProcessArgs(string[] args, bool firstInstance) {
            //// Process Command Line Arguments Here
            if (args.Length > 0) {
                Parameter0 = args[0];
            }

            if (args.Length > 1) {
                Parameter1 = args[1];
            }

            if (args.Length > 2) {
                Parameter2 = args[2];
            }
        }

        /// <summary> Raises the <see cref="E:System.Windows.Application.Startup" /> event. </summary>
        /// <param name="e"> A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event
        /// data. </param> 
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            this.ProcessArgs(e.Args, true);

            if (!MusicalSettings.LoadSettingsStartup(SettingsApplication.ManufacturerName, SettingsApplication.ApplicationName)) {
                MessageBox.Show(LocalizedControls.String("Load of settings failed!"), SettingsApplication.ApplicationName);
                return;
            }

            UserFileLoader.Singleton.LoadWindowManager("LargoEditor", "EditorWindow", typeof(EditorWindow));
            //// WindowManager.Windows = Application.Current.Windows;

            var path = MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.InternalData);
            //// EditorSettings.Singleton.PathToMusicList
            PortCatalogs.Singleton.ReadXmlFiles(path);

            // define application exception handler
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

            LoadFiles();
        }
        #endregion

        #region Dispatcher for Unhandled Exceptions

        /// <summary>
        /// CurrentDomain UnhandledException.
        /// </summary>
        /// <param name="sender">Object -Sender.</param>
        /// <param name="e">Event Arguments.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Exception exception = e.ExceptionObject as Exception;
            //// var w = this.EditorWindow;
            //// if (w != null) {
                //// !?!? If you use a separate thread, it needs to be in a STA (single-threaded apartment), which is not the case for background worker threads. 
                //// You have to create the thread yourself, like this:
                ////     Thread t = new Thread(ThreadProc);
                ////     t.SetApartmentState(ApartmentState.STA);
                ////     t.Start();
                //// w.OpenMainException(exception);
            //// }

            if (exception != null) {
                MessageBox.Show(exception.Message, "Thread Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// The DispatcherUnhandledException is called whenever the UI thread of the application 
        /// generated an unhandled exception.
        /// </summary>
        /// <param name="sender">Method sender.</param>
        /// <param name="e">Caught Exception.</param>
        [UsedImplicitly]
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            this.DoHandle = true;
            if (this.DoHandle) {
                //// Handling the exception within the UnhandledException handler.
                //// Exception exception = e.Exception;
                /* UnhandledException !?
                 2016/10
                MainWindow w = App.LargoMainWindow;
                if (w != null) {
                    w.OpenMainException(exception);
                }
                else {
                    MessageBox.Show(e.Exception.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                */
                e.Handled = true;
            }
            else {
                //// If you do not set e.Handled to true, the application will close due to crash.
                MessageBox.Show("Application is going to close! ", "Uncaught Exception");
                e.Handled = false;
            }
        }

        #endregion
    }
}
