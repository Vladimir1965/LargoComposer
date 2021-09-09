// <copyright file="FileAssociation.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

namespace LargoSharedClasses.Settings
{
    /// <summary>
    /// File Associations.
    /// </summary>
    public class FileAssociation
    {
        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the program identifier.
        /// </summary>
        /// <value>
        /// The program identifier.
        /// </value>
        public string ProgId { get; set; }

        /// <summary>
        /// Gets or sets the file type description.
        /// </summary>
        /// <value>
        /// The file type description.
        /// </value>
        public string FileTypeDescription { get; set; }

        /// <summary>
        /// Gets or sets the executable file path.
        /// </summary>
        /// <value>
        /// The executable file path.
        /// </value>
        public string ExecutableFilePath { get; set; }
    }
}
