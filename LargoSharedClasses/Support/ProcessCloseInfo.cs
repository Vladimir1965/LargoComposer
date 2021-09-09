// <copyright file="ProcessCloseInfo.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Support {
    /// <summary>
    /// Information about closing and closed processes.
    /// </summary>
    public struct ProcessCloseInfo {
        /// <summary>
        /// Gets or sets the process count, that will be closed.
        /// </summary>
        /// <value>
        /// The process count.
        /// </value>
        [UsedImplicitly]
        public int ProcessCount { get; set; }

        /// <summary>
        /// Gets or sets the process count.
        /// Number of processes, that can be closed.
        /// </summary>
        /// <value>
        /// The closed processes.
        /// </value>       
        [UsedImplicitly]
        public int ClosedProcesses { get; set; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="info1">The info1.</param>
        /// <param name="info2">The info2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ProcessCloseInfo info1, ProcessCloseInfo info2) {
            return object.Equals(info1, info2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="info1">The info1.</param>
        /// <param name="info2">The info2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ProcessCloseInfo info1, ProcessCloseInfo info2) {
            return !object.Equals(info1, info2);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() {
            return 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) {
            return false;
        }
    }
}
