// <copyright file="ExtendExceptions.cs" company="J.K.R.">
// Copyright (c) 2012 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Abstract {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;

    /// <summary>
    /// Exception Extensions.
    /// </summary>
    public static class ExtendExceptions {
        #region Exceptions
        /// <summary>
        /// Writes the exception to file.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="givenFilePath">The given file path.</param>
        [UsedImplicitly]
        public static void WriteToFile(this Exception exception, string givenFilePath) {
            var r = new string('-', 80);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(r);
            sb.AppendFormat(CultureInfo.CurrentCulture, "{0} {1}\t{2}\n", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(), exception.Message ?? string.Empty);
            sb.AppendLine(r);
            sb.AppendLine(exception.StackTrace ?? string.Empty);
            sb.AppendLine(r);
            var message = exception.GetExceptionMessages();
            sb.AppendLine(message ?? string.Empty);
            SupportFiles.StringToFile(sb.ToString(), givenFilePath);
        }

        /// <summary>
        /// List Error.
        /// </summary>
        /// <param name="exception">Given Exception.</param>
        /// <returns> Returns value. </returns>
        public static string ListError(this Exception exception) {
            if (exception?.StackTrace == null) {
                return string.Empty;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0} \n Stack: {1}", exception.Message.Trim(), exception.StackTrace.Trim());
        }

        /// <summary>
        /// Gets Messages from actual exception and all internal exceptions.
        /// </summary>
        /// <param name="ex">Reference to exception object.</param>
        /// <returns>Returns messages in rows.</returns>
        public static string GetExceptionMessages(this Exception ex) {
            if (ex == null) {
                return string.Empty;
            }

            // Find a message from the current exception.
            var messages = ex.Message;

            // Finding an internal exception report.
            if (ex.InnerException != null) {
                messages += Environment.NewLine +
                   GetExceptionMessages(ex.InnerException);
            }

            // Check if messages are repeated.
            var messageList = Regex.Split(messages, Environment.NewLine);
            for (var i = 0; i < messageList.Length; i++) {
                for (var j = i + 1; j < messageList.Length; j++) {
                    if (string.CompareOrdinal(messageList[j], messageList[i]) == 0) {
                        messageList[j] = null;
                    }
                }
            }

            // Rebuild the remaining messages into a string.
            messages = null;
            foreach (var t in messageList.Where(t => t != null)) {
                if (messages != null) {
                    messages += Environment.NewLine;
                }

                messages += t;
            }

            return messages;
        }
        #endregion   

        /// <summary>
        /// Inner exception set.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>Returns value.</returns>
        public static List<string> InnerExceptionsSet(this Exception ex) {
            var exceptionsSet = new List<string>();
            InnerEx(ref exceptionsSet, ex);
            return exceptionsSet;
        }

        /// <summary>
        /// To the debug message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>Returns value.</returns>
        [UsedImplicitly]
        public static string ToDebugMessage(this Exception exception) {
            var sb = new StringBuilder("Exception:");
            if (exception != null) {
                sb.AppendLine($"Type: {exception.GetType().FullName}");
                sb.AppendLine($"Message: {exception.Message}");
                if (exception is WebException webEx) {
                    sb.AppendLine($"WebException.Status: {webEx.Status}");
                    if (webEx.Status == WebExceptionStatus.ProtocolError) {
                        sb.AppendLine(
                            $"WebException.Response.Status Code: {((HttpWebResponse) webEx.Response).StatusCode}");
                        sb.AppendLine(
                            $"WebException.Response.Status Description: {((HttpWebResponse) webEx.Response).StatusDescription}");
                    }
                }

                var exceptionsSet = exception.InnerExceptionsSet();
                if (exceptionsSet.Count > 1) {
                    sb.AppendLine("Exception list: ");
                    exceptionsSet.ForEach(p => sb.AppendLine(p));
                }

                var newLineSeparator = new[] { Environment.NewLine };
                sb.AppendLine("Source: ");
                exception.Source.Split(newLineSeparator, StringSplitOptions.None).ToList().ForEach(p => sb.AppendLine(p));

                sb.AppendLine("StackTrace: ");
                exception.StackTrace.Split(newLineSeparator, StringSplitOptions.None).ToList().ForEach(p => sb.AppendLine(p));
            }
            else {
                sb.AppendLine("[exception is null ?]");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Inner exception set.
        /// </summary>
        /// <param name="exceptionsSet">The exceptions set.</param>
        /// <param name="ex">The ex.</param>
        private static void InnerEx(ref List<string> exceptionsSet, Exception ex) {
            if (ex != null) {
                exceptionsSet.Add(ex.Message);
                InnerEx(ref exceptionsSet, ex.InnerException);
            }
        }
    }
}
