// <copyright file="RhythmicContainer.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;
using LargoSharedClasses.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Rhythmic Container.
    /// </summary>
    public class RhythmicContainer
    {
        #region Fields
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly RhythmicContainer InternalSingleton = new RhythmicContainer();
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the RhythmicContainer class from being created.
        /// </summary>
        private RhythmicContainer() {
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>/// 
        public static RhythmicContainer Singleton {
            get {
                Contract.Ensures(Contract.Result<RhythmicContainer>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton Rhythmic Container is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the rhythmic structures.
        /// </summary>
        /// <value>
        /// The rhythmic structures.
        /// </value>
        public List<RhythmicStructure> RhythmicStructures { get; set; }

        #endregion

        #region Public methods - Find rhythmic structures

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() {
            this.RhythmicStructures = new List<RhythmicStructure>();
        }

        /// <summary>
        /// Adds the rhythmic structures.
        /// </summary>
        /// <param name="givenStructures">The given structures.</param>
        public void AddRhythmicStructures(IEnumerable<RhythmicStructure> givenStructures) {
            Contract.Requires(givenStructures != null);
            this.RhythmicStructures.AddRange(givenStructures);
        }

        /// <summary>
        /// Rhythmic structure.
        /// </summary>
        /// <param name="energyChange">The energy change.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public RhythmicStructure FindRhythmicStructure(EnergyChange energyChange) {
            RhythmicStructure optimalStructure = null;
            float bestValue = +10000;
            foreach (var structure in this.RhythmicStructures) {
                var value = Math.Abs(structure.Level - energyChange.BeatLevel)
                              + Math.Abs(structure.ToneLevel - energyChange.ToneLevel)
                              + Math.Abs(structure.FormalBehavior.Variance - energyChange.RhythmicTension);
                if (value >= bestValue) {
                    continue;
                }

                bestValue = value;
                optimalStructure = structure;
            }

            return optimalStructure;
        }

        /// <summary>
        /// Finds the similar structure.
        /// </summary>
        /// <param name="givenStruct">The given struct.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        [UsedImplicitly]
        public RhythmicStructure FindSimilarStructure(RhythmicStructure givenStruct) {
            Contract.Requires(givenStruct != null);

            RhythmicStructure optimalStruct = null;
            float minimumResult = 1000;

            //// rstruct.DetermineBehavior();
            foreach (var rstruct in this.RhythmicStructures) { //// 1000
                if (rstruct.ElementSchema == givenStruct.ElementSchema) {
                    continue;
                }

                var s = rstruct.SimilarityValue(givenStruct);
                if (s > 90) {
                    continue;
                }

                var level = givenStruct.Level + rstruct.Level > 0 ?
                            100.0f * 2 * Math.Abs(givenStruct.Level - rstruct.Level) / (givenStruct.Level + rstruct.Level)
                            : 0;
                var toneLevel = givenStruct.ToneLevel + rstruct.ToneLevel > 0 ?
                            100.0f * 2 * Math.Abs(givenStruct.ToneLevel - rstruct.ToneLevel) / (givenStruct.ToneLevel + rstruct.ToneLevel)
                            : 0;

                var variance = Math.Abs(givenStruct.FormalBehavior.Variance - rstruct.FormalBehavior.Variance);
                var balance = Math.Abs(givenStruct.FormalBehavior.Balance - rstruct.FormalBehavior.Balance);
                //// float beat = Math.Abs(givenStruct.Beat - rstruct.Beat);
                //// float filling = Math.Abs(givenStruct.Filling - rstruct.Filling);
                //// float complexity = Math.Abs(givenStruct.Complexity - rstruct.Complexity);

                var result = (4 * toneLevel) + (2 * level) + variance + balance;
                if (result >= minimumResult) {
                    continue;
                }

                optimalStruct = rstruct;
                minimumResult = result;
            }

            return optimalStruct;
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            return "RhythmicContainer";
        }
        #endregion
    }
}