// <copyright file="MusicalRulesType.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Settings
{
    /// <summary> Sets of musical rules. </summary>
    public enum MusicalRulesType {
        /// <summary> Rules type. </summary>
        [UsedImplicitly] None = 0,

        /// <summary> Rules type. </summary>
        StandardMusicalRules = 1,

        /// <summary> Rules type. </summary>
        SimpleHarmonicRules = 2,

        /// <summary> Rules type. </summary>
        MusicalImpulseRules = 3
    }
}
