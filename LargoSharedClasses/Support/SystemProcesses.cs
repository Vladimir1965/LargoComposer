// <copyright file="SystemProcesses.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security;
using JetBrains.Annotations;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// System Processes.
    /// </summary>
    public static class SystemProcesses {
        /// <summary>
        /// Run Executable File.
        /// </summary>
        /// <param name="executableFile">Executable file.</param>
        [UsedImplicitly]
        public static void RunExecutableFile(string executableFile) {
            Contract.Requires(executableFile != null);
            Process.Start(executableFile); 
        }

        /// <summary>
        /// Kills the application.
        /// </summary>
        /// <param name="givenName">Name of the given.</param>
        public static void KillApplication(string givenName)
        {
            Process[] process = null;

            try {
                process = Process.GetProcessesByName(givenName); 
                if (process.Length == 0) {
                    return;
                }

                Process paintProcess = process[0];

                if (!paintProcess.HasExited) {
                    paintProcess.Kill();
                }
            }
            finally {
                if (process != null) {
                    foreach (Process p in process) {
                        p.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Opens documents using Internet Explorer.
        /// </summary>
        /// <param name="argument">Web address or file path.</param>
        [UsedImplicitly]
        public static void OpenInternetExplorer(string argument) {
            //// urls and .html, urls are not considered documents. They can only be opened by passing them as arguments.
            Process.Start("IExplore.exe", argument);
        }

        /// <summary>
        /// Opens documents using Internet Explorer.
        /// </summary>
        /// <param name="argument">Web address or file path.</param>
        [UsedImplicitly]
        public static void OpenMozillaFirefox(string argument) {
            // urls are not considered documents. They can only be opened by passing them as arguments.
            Process.Start("firefox.exe", argument);
        }

        #region Processes
        /// <summary>
        /// Run Process Secure.
        /// </summary>
        /// <param name="command">Command to process.</param>
        /// <param name="arguments">Parameters of the command.</param>
        /// <param name="waitForExit">if set to <c>true</c> [wait for exit].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public static bool RunProcessSecure(string command, string arguments, bool waitForExit) {
            Contract.Requires(command != null);
            Contract.Requires(arguments != null);
            bool result;
            try {
                result = RunProcessSimple(command, arguments, waitForExit);
            }
            catch (SecurityException ex) {
                const string msg = "SecurityProblem"; //// LocalizedAbstract.String(
                Console.WriteLine(msg + ex.Message + ex.StackTrace);
                throw;
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Run Process Simple.
        /// </summary>
        /// <param name="command">Command to process.</param>
        /// <param name="arguments">Parameters of the command.</param>
        /// <param name="waitForExit">if set to <c>true</c> [wait for exit].</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        //// CA2122, CA2135  [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        [SecurityCritical]
        internal static bool RunProcessSimple(string command, string arguments, bool waitForExit) {
            Contract.Requires(command != null);
            Contract.Requires(arguments != null);
            using (var p = Process.Start(command, arguments)) {
                if (p == null) {
                    return false;
                }

                //// p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //// p.StartInfo.CreateNoWindow = true;
                if (waitForExit)
                {
                    p.WaitForExit();
                    return p.ExitCode == 0;
                }
            }

            return true;
        }
    }
}
