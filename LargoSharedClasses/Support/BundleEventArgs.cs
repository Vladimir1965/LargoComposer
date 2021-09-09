// <copyright file="BundleEventArgs.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Process Logger Event Args.
    /// </summary>
    public sealed class BundleEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the BundleEventArgs class.
        /// </summary>
        /// <param name="givenBundle">The given bundle.</param>
        /// <param name="givenOperation">The given operation.</param>
        /// <param name="analyze">If set to <c>true</c> [analyze].</param>
        public BundleEventArgs(MusicalBundle givenBundle, ObjectOperation givenOperation, bool analyze)
        {
            this.MusicalBundle = givenBundle;
            this.Operation = givenOperation;
            this.Analyze = analyze;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BundleEventArgs"/> class.
        /// </summary>
        [UsedImplicitly]
        public BundleEventArgs() {    
        }

        #region Properties
        /// <summary>
        /// Gets the musical block.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public MusicalBundle MusicalBundle { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BundleEventArgs"/> is analyze.
        /// </summary>
        /// <value>
        ///   <c>true</c> if analyze; otherwise, <c>false</c>.
        /// </value>
        public bool Analyze { get; }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private ObjectOperation Operation { [UsedImplicitly] get; }
        #endregion
    }
}
