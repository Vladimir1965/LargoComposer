// <copyright file="RegUtilities.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Class for register.
    /// </summary>
    public static class RegUtilities {
        /// <summary>
        /// Read register item.
        /// </summary>
        /// <param name="rootKey">Root, where to start search.</param>
        /// <param name="keyPath">Path to the key in register (without root).</param>
        /// <param name="valueName">Name of read values.</param>
        /// <param name="defaultValue">Implicit value.</param>
        /// <returns>Returns value of register item or otherwise implicit value.</returns>
        [System.Diagnostics.Contracts.Pure]
        [UsedImplicitly]
        public static object ReadValue(RegistryRootKey rootKey, string keyPath, string valueName, object defaultValue) {
            // Determine key
            using (var regKey = GetRegistryRootKey(rootKey).OpenSubKey(keyPath)) {
                // Read values if sub-key has been found
                var regValue = regKey != null ? regKey.GetValue(valueName) : defaultValue;

                return regValue;
            }
        }

        /// <summary>
        /// Writes register item.
        /// </summary>
        /// <param name="rootKey">Root, where to start search.</param>
        /// <param name="keyPath">Path to the key in register (without root).</param>
        /// <param name="valueName">Name of written value.</param>
        /// <param name="value">Written value.</param>
        /// <param name="createIfNotExist">Create item if not exist.</param>
        [UsedImplicitly]
        public static void WriteValue(RegistryRootKey rootKey, string keyPath, string valueName, object value, bool createIfNotExist) {
            // Root key
            var regKey = GetRegistryRootKey(rootKey);

            if (keyPath == null) {
                throw new InvalidOperationException("Empty registry key.");
            }

            var pathToken = keyPath.Split('\\');
            foreach (var t in pathToken) {
                if (regKey == null) {
                    continue;
                }

                var subKey = regKey.OpenSubKey(t, true);
                if (subKey == null && createIfNotExist) {
                    // sub-key not found
                    subKey = regKey.CreateSubKey(t);
                }

                regKey = subKey;
            }

            // Write value.
            if (regKey != null) {
                regKey.SetValue(valueName, value);
            }
            else {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Key {0}\\{1} not found.", rootKey, keyPath));
            }
        }

        /// <summary>
        /// Removes register item.
        /// </summary>
        /// <param name="rootKey">Root, where to start search.</param>
        /// <param name="keyPath">Path to the key in register (without root).</param>
        /// <param name="valueName">Name of deleted value.</param>
        [UsedImplicitly]
        public static void DeleteValue(RegistryRootKey rootKey, string keyPath, string valueName) {
            if (keyPath == null) {
                throw new InvalidOperationException("Empty registry key.");
            }

            using (var regKey = GetRegistryRootKey(rootKey).OpenSubKey(keyPath, true)) {
                if (regKey != null) {
                    regKey.DeleteValue(valueName, false);
                }
                else {
                    // Key not found.
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Key {0}\\{1} not found.", rootKey, keyPath));
                }
            }
        }

        /// <summary>
        /// Removes register key.
        /// </summary>
        /// <param name="rootKey">Root, where to start search.</param>
        /// <param name="keyPath">Path to the key in register (without root).</param>
        [UsedImplicitly]
        public static void DeleteKey(RegistryRootKey rootKey, string keyPath) {
            if (keyPath == null) {
                throw new InvalidOperationException("Empty registry key.");
            }

            // Determine path to deleted key.
            var i = keyPath.LastIndexOf("\\", StringComparison.Ordinal);
            if (i < 0) {
                return;
            }

            var parentKeyPath = keyPath.Substring(0, i);
            var keyName = keyPath.Substring(i + 1, keyPath.Length - i - 1);

            // Open parent key.
            using (var regKey = GetRegistryRootKey(rootKey).OpenSubKey(parentKeyPath, true)) {
                if (regKey != null) {
                    regKey.DeleteSubKey(keyName);
                }
                else {
                    // Key not found.
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Key {0}\\{1} not found.", rootKey, keyPath));
                }
            }
        }

        /// <summary>
        /// Returns root register key with given key.
        /// </summary>
        /// <param name="rootKey">Given root key.</param>
        /// <returns> Returns value.</returns>
        [System.Diagnostics.Contracts.Pure]
        private static RegistryKey GetRegistryRootKey(RegistryRootKey rootKey) {
            RegistryKey regKey = null;
            switch (rootKey) {
                case RegistryRootKey.RegistryKeyClassesRoot:
                    regKey = Registry.ClassesRoot;
                    break;
                case RegistryRootKey.RegistryKeyCurrentConfig:
                    regKey = Registry.CurrentConfig;
                    break;
                case RegistryRootKey.RegistryKeyCurrentUser:
                    regKey = Registry.CurrentUser;
                    break;
                case RegistryRootKey.RegistryKeyLocalMachine:
                    regKey = Registry.LocalMachine;
                    break;
                case RegistryRootKey.RegistryKeyPerformanceData:
                    regKey = Registry.PerformanceData;
                    break;
                case RegistryRootKey.RegistryKeyUsers:
                    regKey = Registry.Users;
                    break;
                case RegistryRootKey.RegistryKeyDynamicData:
                    break;
                //// resharper default: break;
                //// Warning 89'Microsoft.Win32.Registry.DynData' is obsolete: "The DynData registry 
                ////  key only works on Win9x,  which is no longer supported by the CLR.
                //// On NT-based operating systems, use the PerformanceData registry key instead.
                //// case RegistryRootKeys.RegistryDynamicData: regKey = Registry.DynData; break; 
            }

            return regKey;
        }
    }
}
