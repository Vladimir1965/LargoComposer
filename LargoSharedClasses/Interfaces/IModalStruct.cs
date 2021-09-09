// <copyright file="IModalStruct.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using JetBrains.Annotations;

namespace LargoSharedClasses.Interfaces
{
    /// <summary> Interface for all structures. </summary>
    public interface IModalStruct {
        /// <summary> Gets or sets modality of the structure.</summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        IGeneralStruct Modality { get; set; }
    }
}
