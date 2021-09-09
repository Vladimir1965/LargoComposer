// <copyright file="LocalizationManager.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Globalization;
using System.Resources;
using System.Threading;
using JetBrains.Annotations;

namespace LargoSharedClasses.Localization
{
    /// <summary>
    /// Represents a class that manages the localization features.
    /// </summary>
    public static class LocalizationManager {
        /// <summary>
        /// The <see cref="ResourceManager"/> by which resources as accessed.
        /// </summary>
        private static ResourceManager resourceManager;

        /// <summary>
        /// True if an attempt to load the resource manager has been made.
        /// </summary>
        private static bool resourceManagerLoaded;

        /// <summary>
        /// Gets or sets the resource manager to use to access the resources.
        /// </summary>
        public static ResourceManager ResourceManager {
            get {
                if (resourceManager == null && !resourceManagerLoaded) {
                    resourceManager = GetResourceManager();
                    resourceManagerLoaded = true;
                }

                return resourceManager;
            }

            set => resourceManager = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the current UI culture.
        /// </summary>
        /// <remarks>
        /// This property changes the UI culture of the current thread to the specified value
        /// and updates all localized property to reflect values of the new culture.
        /// </remarks>
        [UsedImplicitly]
        public static CultureInfo UiCulture {
            get => Thread.CurrentThread.CurrentUICulture;

            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                Thread.CurrentThread.CurrentUICulture = value;
            }
        }

        /// <summary>
        /// Returns resource manager to access the resources the application's main assembly.
        /// </summary>
        /// <returns>
        /// Returns value.
        /// </returns>
        private static ResourceManager GetResourceManager() {
            return Localized.ResourceManager;
        }
    }
}
