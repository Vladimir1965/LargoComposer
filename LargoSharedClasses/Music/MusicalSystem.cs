// <copyright file="MusicalSystem.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Rhythm;

namespace LargoSharedClasses.Music
{
    /// <summary>
    /// Musical System.
    /// </summary>
    public sealed class MusicalSystem {
        #region Fields
        /// <summary>
        /// Harmonic order.
        /// </summary>
        private byte harmonicOrder;

        /// <summary>
        /// Rhythmic order.
        /// </summary>
        private byte rhythmicOrder;

        /// <summary> Harmonic system. </summary>
        private HarmonicSystem harmonicSystem;

        /// <summary> Rhythmical system. </summary>
        private RhythmicSystem rhythmicSystem;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalSystem"/> class.
        /// </summary>
        /// <param name="xsystem">The system element.</param>
        public MusicalSystem(XElement xsystem) {
            Contract.Requires(xsystem != null);
            if (xsystem == null)
            {
                return;
            }

            this.HarmonicOrder = XmlSupport.ReadByteAttribute(xsystem.Attribute("HarmonicOrder"));
            this.RhythmicOrder = XmlSupport.ReadByteAttribute(xsystem.Attribute("RhythmicOrder"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalSystem"/> class.
        /// </summary>
        public MusicalSystem()
        {
        }
        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets the get x element.
        /// </summary>
        /// <value>
        /// The get x element.
        /// </value>
        public XElement GetXElement {
            get {
                XElement xsystem = new XElement("System", null);
                xsystem.Add(new XAttribute("HarmonicOrder", this.HarmonicOrder));
                xsystem.Add(new XAttribute("RhythmicOrder", this.RhythmicOrder));

                return xsystem;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets harmonic order.
        /// </summary>
        /// <value> Property description. </value>
        public byte HarmonicOrder {
            get {
                Contract.Ensures(Contract.Result<byte>() != 0);
                if (this.harmonicOrder == 0) {
                    throw new InvalidOperationException("Harmonic order of system must not be 0.");
                }

                return this.harmonicOrder;
            }

            set => this.harmonicOrder = value;
        }

        /// <summary>
        /// Gets or sets rhythmic order.
        /// </summary>
        /// <value> Property description. </value>
        public byte RhythmicOrder {
            get {
                Contract.Ensures(Contract.Result<byte>() != 0);
                if (this.rhythmicOrder == 0) {
                    throw new InvalidOperationException("Rhythmic order of system must not be 0.");
                }

                return this.rhythmicOrder;
            }

            set => this.rhythmicOrder = value;
        }

        /// <summary>
        /// Gets the order value.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string OrderValue => MusicalProperties.GetOrderValue(this.HarmonicOrder, this.RhythmicOrder);

        /// <summary>  Gets harmonic system. </summary>
        /// <value> Property description. </value>
        public HarmonicSystem HarmonicSystem {
            get {
                Contract.Ensures(Contract.Result<HarmonicSystem>() != null);
                if (this.harmonicSystem != null && this.harmonicSystem.Order == this.HarmonicOrder) {
                    return this.harmonicSystem;
                }

                if (this.harmonicSystem == null) {
                    this.harmonicSystem = HarmonicSystem.GetHarmonicSystem(this.HarmonicOrder);
                }

                if (this.harmonicSystem == null) {
                    throw new InvalidOperationException("Harmonic system is null.");
                }

                this.harmonicSystem = HarmonicSystem.GetHarmonicSystem(this.HarmonicOrder);
                return this.harmonicSystem;
            }
        }

        /// <summary> Gets rhythmical system. </summary>
        /// <value> Property description. </value>
        public RhythmicSystem RhythmicSystem {
            get {
                Contract.Ensures(Contract.Result<RhythmicSystem>() != null);
                if (this.rhythmicSystem != null && this.rhythmicSystem.Order == this.RhythmicOrder) {
                    return this.rhythmicSystem;
                }

                if (this.rhythmicSystem == null) {
                    this.rhythmicSystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, this.RhythmicOrder);
                }

                if (this.rhythmicSystem == null) {
                    throw new InvalidOperationException("Rhythmical system is null.");
                }

                this.rhythmicSystem = RhythmicSystem.GetRhythmicSystem(RhythmicDegree.Structure, this.RhythmicOrder);
                return this.rhythmicSystem;
            }
        }
        #endregion

        #region String representation
        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendFormat("H{0} R{1}", this.HarmonicOrder, this.RhythmicOrder); //// System

            return s.ToString();
        }
        #endregion
    }
}
