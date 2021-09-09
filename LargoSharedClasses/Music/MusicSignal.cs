// <copyright file="MusicSignal.cs" company="Largo">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author> vl </author>
// <email></email>
// <date>2009-01-01</date>
// <summary>Contains ...</summary>

using System.Xml.Linq;
using JetBrains.Annotations;

namespace LargoCommon.Music
{
    /// <summary>
    /// Music Signal.
    /// </summary>
    public class MusicSignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicSignal" /> class.
        /// </summary>
        /// <param name="givenFrequency">The given frequency.</param>
        /// <param name="givenAmplitude">The given amplitude.</param>
        public MusicSignal(double givenFrequency, double givenAmplitude)
        {
            this.F = givenFrequency;
            this.A = givenAmplitude;
        }

        /// <summary>
        /// Gets or sets the f.
        /// </summary>
        /// <value>
        /// The frequency f.
        /// </value>
        public double F { get; set; }

        /// <summary>
        /// Gets or sets a.
        /// </summary>
        /// <value>
        /// Amplitude a.
        /// </value>
        public double A { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"f={this.F} A={this.A}";
        }

        /// <summary>
        /// Saves to.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        /// <returns>Returns value.</returns>
        [UsedImplicitly]
        public bool SaveTo(string givenPath)
        {
            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var xsound = new XElement("Sound");
            var xf = new XAttribute("f", this.F);
            xsound.Add(xf);
            var xa = new XAttribute("A", this.A);
            xsound.Add(xa);
            xdoc.Add(xsound);
            xdoc.Save(givenPath);
            return true;
        }
    }
}
