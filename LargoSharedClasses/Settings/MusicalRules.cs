// <copyright file="MusicalRules.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using JetBrains.Annotations;

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// Musical Engine.
    /// </summary>
    [Serializable]
    public sealed class MusicalRules {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [individualize melodic voices].
        /// </summary>
        /// <value>
        /// <c>True</c> if [individualize melodic voices]; otherwise, <c>false</c>.
        /// </value>
        public bool IndividualizeMelodicVoices { get; set; }

        /// <summary>
        /// Gets RuleHarmonicCover.
        /// </summary>
        /// <value> Property description. </value>
        public float RuleHarmonicCover { get; private set; }

        /// <summary>
        /// Gets RuleFreeBand.
        /// </summary>
        /// <value> Property description. </value>
        public float RuleFreeBand { get; private set; }

        /// <summary>
        /// Gets RuleImpulseCollisions.
        /// </summary>
        /// <value> Property description. </value>
        public float RuleImpulseCollisions { get; private set; }

        /// <summary>
        /// Gets RuleAmbitChange.
        /// </summary>
        /// <value> General musical property.</value>
        public float RuleAmbitChange { get; private set; }

        /// <summary>
        /// Gets RuleMelodicCollisions.
        /// </summary>
        /// <value> General musical property.</value>
        public float RuleMelodicCollisions { get; private set; }

        /// <summary>
        /// Gets RuleSimpleHarmony.
        /// </summary>
        /// <value> Property description. </value>
        public float RuleSimpleHarmony { get; private set; }
        #endregion 

        #region Private properties
        /// <summary>
        /// Gets or sets name of the rule-set.
        /// </summary>
        /// <value> Property description. </value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private string Name { [UsedImplicitly] get; set; }

        /// <summary>
        /// Gets or sets  RandomEffect.
        /// </summary>
        /// <value> Property description. </value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private float RandomEffect { [UsedImplicitly] get; set; }
        #endregion

        #region Public static methods
        /// <summary>
        /// Gets NewStandardEngine.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static MusicalRules NewStandardMusicalRules() {
            var me = new MusicalRules
                                  {
                                      Name = "Standard",
                                      RandomEffect = 0,
                                      RuleFreeBand = 1,
                                      RuleHarmonicCover = 1,
                                      RuleImpulseCollisions = 1,
                                      RuleMelodicCollisions = 1,
                                      RuleAmbitChange = 0,
                                      RuleSimpleHarmony = 0
                                  };
            return me;
       }

        /// <summary>
        /// Gets NewStandardEngine.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static MusicalRules NewSimpleHarmonicMusicalRules() {
            var me = new MusicalRules
                                  {
                                      Name = "Simple Harmony",
                                      RandomEffect = 0,
                                      RuleFreeBand = 1,
                                      RuleHarmonicCover = 0,
                                      RuleImpulseCollisions = 0,
                                      RuleMelodicCollisions = 0,
                                      RuleAmbitChange = 0,
                                      RuleSimpleHarmony = 1
                                  };

            return me;
        }

        /// <summary>
        /// Gets NewStandardEngine.
        /// </summary>
        /// <returns> Returns value. </returns>
        public static MusicalRules NewMusicalImpulseRules() {
            var me = new MusicalRules
                                  {
                                      Name = "Simple Harmony",
                                      RandomEffect = 0,
                                      RuleFreeBand = 1,
                                      RuleHarmonicCover = 1,
                                      RuleImpulseCollisions = 0,
                                      RuleMelodicCollisions = 0,
                                      RuleAmbitChange = 0,
                                      RuleSimpleHarmony = 0
                                  };
            return me;
        }
        #endregion
    }
}
