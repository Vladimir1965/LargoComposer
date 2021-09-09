// <copyright file="TupleNumber.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Notation
{
    /// <summary> Tuple Number. </summary>
    public enum TupleNumber {
        /// <summary> Tuple number. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Tuple number. </summary>
        Single = 1,

        /// <summary> Tuple number. </summary>
        [UsedImplicitly] Duple = 2,

        /// <summary> Tuple number. </summary>
        [UsedImplicitly] Triplet = 3,
        
        /// <summary> Tuple number. </summary>
        [UsedImplicitly] Quadruple = 4,
        
        /// <summary> Tuple number. </summary>
        [UsedImplicitly] Quintuple = 5,
        
        /// <summary> Tuple number. </summary>
        [UsedImplicitly] Sextuple = 6,
        
        /// <summary> Tuple number. </summary>
        [UsedImplicitly] Septuple = 7,
        
        /// <summary> Tuple number. </summary>
        [UsedImplicitly] Octuple = 8
    }
}
