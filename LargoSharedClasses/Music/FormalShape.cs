// <copyright file="FormalShape.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Interfaces;
using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Xml.Serialization;

namespace LargoSharedClasses.Music {
    /// <summary>  Formal shape.  </summary>
    /// <remarks> Formal shape represents simplified formal distribution. </remarks>
 [Serializable] 
    [XmlRoot]
    public sealed class FormalShape : RhythmicShape {
        #region Constructors
        /// <summary> Initializes a new instance of the FormalShape class.  Serializable. </summary>
        public FormalShape()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FormalShape class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="structuralCode">Structural code.</param>
        public FormalShape(GeneralSystem givenSystem, string structuralCode)
            : base(givenSystem, structuralCode) {
                Contract.Requires(givenSystem != null);
        }

        /// <summary>
        /// Initializes a new instance of the FormalShape class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="bitArray">Bit array.</param>
        public FormalShape(GeneralSystem givenSystem, BitArray bitArray)
            : base(givenSystem, bitArray) {
                Contract.Requires(givenSystem != null); 
        }

        /// <summary>
        /// Initializes a new instance of the FormalShape class.
        /// </summary>
        /// <param name="shape">Rhythmic shape.</param>
        public FormalShape(BinaryStructure shape)
            : base(shape) {
                Contract.Requires(shape != null);
        }

        /// <summary>
        /// Initializes a new instance of the FormalShape class. 
        /// </summary>
        /// <param name="sysOrder">System order.</param>
        /// <param name="structure">Rhythmical structure.</param>
        public FormalShape(byte sysOrder, IGeneralStruct structure)
            : base(sysOrder, structure) {
                Contract.Requires(structure != null);
        }

        /// <summary>
        /// Initializes a new instance of the FormalShape class.
        /// </summary>
        /// <param name="givenSystem">The given system.</param>
        /// <param name="number">Number of structure.</param>
        public FormalShape(GeneralSystem givenSystem, long number)
            : base(givenSystem, number) {
        }

        /// <summary> Makes a deep copy of the FormalShape object. </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            return new FormalShape(this.GSystem, this.GetStructuralCode);
        }
        #endregion

        #region Public methods
        /// <summary> Validity test. </summary>
        /// <returns> Returns value. </returns>
        public override bool IsValidStruct() {
            var ok = true;
            for (byte e = 0; e < this.Level; e++) {
                if (e >= this.Distances.Count)
                {
                    continue;
                }

                var num = this.Distances[e];

                // ReSharper disable once InvertIf
                if (num < 2) { // length 1 in sequence
                    ok = false;
                    break;
                }
            }

            return ok;
        }
        #endregion
    }
}
