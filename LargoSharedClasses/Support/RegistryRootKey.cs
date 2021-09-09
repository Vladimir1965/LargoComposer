// <copyright file="RegistryRootKey.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Support {
    /// <summary>
    /// Enumeration for root keys of register.
    /// </summary>
    public enum RegistryRootKey {
        /// <summary>
        /// Registry key HKEY_CLASSES_ROOT.
        /// </summary>
        RegistryKeyClassesRoot,

        /// <summary>
        /// Registry key HKEY_CURRENT_CONFIG.
        /// </summary>
        RegistryKeyCurrentConfig,

        /// <summary>
        /// Registry key HKEY_CURRENT_USER.
        /// </summary>
        RegistryKeyCurrentUser,

        /// <summary>
        /// Registry key HKEY_DYN_DATA (Obsolete).
        /// </summary>
        [UsedImplicitly] RegistryKeyDynamicData,

        /// <summary>
        /// Registry key HKEY_LOCAL_MACHINE.
        /// </summary>
        RegistryKeyLocalMachine,

        /// <summary>
        /// Registry key HKEY_PERFORMANCE_DATA.
        /// </summary>
        RegistryKeyPerformanceData,

        /// <summary>
        /// Registry key HKEY_USERS.
        /// </summary>
        RegistryKeyUsers
    }
}
