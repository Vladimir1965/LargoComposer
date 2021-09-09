// <copyright file="IGeneralStruct.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;

namespace LargoSharedClasses.Interfaces
{
    /// <summary> Interface for all structures. </summary>
    public interface IGeneralStruct {
        /// <summary> Gets system of the structure.</summary>
        /// <value> Property description. </value>
        GeneralSystem GSystem { get; }

        /// <summary>
        /// Gets level, i.e. number of ones in the structure. 
        /// </summary>
        /// <value> Property description. </value>
        byte Level { get; }

        /// <summary> Gets number of the structure.</summary>
        /// <value> Property description. </value>
        decimal DecimalNumber { get; }

        /// <summary> Tests emptiness of the structure.</summary>
        /// <returns> Returns value. </returns>
        bool IsEmptyStruct();

        /// <summary> Tests validity of the structure.</summary>
        /// <returns> Returns value. </returns>
        bool IsValidStruct();

        /// <summary>
        /// Determines whether the specified element is on.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>true</c> if the specified element is on; otherwise, <c>false</c>.
        /// </returns>
        bool IsOn(byte element);

        /// <summary> Evaluate properties of the structure. </summary>
        void DetermineBehavior();

        /// <summary> Returns requested property. </summary>
        /// <param name="generalProperty">General musical property.</param>
        /// <returns> Returns value. </returns>
        float GetProperty(GenProperty generalProperty);

        /// <summary>
        /// Writes the behavior to properties.
        /// </summary>
        void WriteBehaviorToProperties();

        /// <summary> Sum of property values multiplied by requested weights. </summary>
        /// <param name="request">General musical request.</param>
        /// <returns> Returns value. </returns>
        float SumForRequest(GeneralRequest request);
    }
}
