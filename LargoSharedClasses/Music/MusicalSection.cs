// <copyright file="MusicalSection.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Text;

namespace LargoSharedClasses.Music {
    /// <summary>
    /// Musical Area.
    /// </summary>
    public class MusicalSection {  //// struct
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MusicalSection class.
        /// </summary>
        /// <param name="barFrom">From bar number.</param>
        /// <param name="barTo">To bar number.</param>
        /// <param name="name">Name of the area.</param>
        public MusicalSection(int barFrom, int barTo, string name) : this() {
            this.Name = name;
            this.BarFrom = barFrom;
            this.BarTo = barTo; 
        }
        
        /// <summary>
        /// Prevents a default instance of the <see cref="MusicalSection" /> class from being created.
        /// </summary>
        private MusicalSection() {      
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the bar from.
        /// </summary>
        /// <value>
        /// The bar from.
        /// </value>
        public int BarFrom { get; }

        /// <summary>
        /// Gets or sets the bar to.
        /// </summary>
        /// <value>
        /// The bar to.
        /// </value>
        public int BarTo { get; set; }

        /// <summary>
        /// Gets the number of bars.
        /// </summary>
        /// <value> Property description. </value>
        public int Length => this.BarTo - this.BarFrom + 1;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of area.
        /// </value>
        public string Name { get; set; }
        #endregion

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat("{0,4} - {1,4}", this.BarFrom, this.BarTo);
            s.Append(this.Name);

            return s.ToString();
        }
        #endregion
    }
}
