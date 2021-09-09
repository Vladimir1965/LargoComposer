// <copyright file="InstrumentChange.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using LargoSharedClasses.Abstract;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Instrument Change.
    /// </summary>
    public sealed class InstrumentChange : AbstractChange {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentChange"/> class.
        /// </summary>
        [UsedImplicitly]
        public InstrumentChange() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentChange"/> class.
        /// </summary>
        /// <param name="xchange">The given change.</param>
        public InstrumentChange(XElement xchange)
            : base(xchange) {
                Contract.Requires(xchange != null);
           if (xchange == null) {
                return;
           }

           ////201509!!!!! this.Channel = (MidiChannel)LibSupport.ReadStringAttribute(xchange.Attribute("Channel"));
           this.Channel = DataEnums.ReadAttributeMidiChannel(xchange.Attribute("Channel")); 
           var number = XmlSupport.ReadByteAttribute(xchange.Attribute("Instrument"));
           this.Instrument = new MusicalInstrument((MidiMelodicInstrument)number);
           this.ChangeType = MusicalChangeType.Instrument;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLine">The given line.</param>
        public InstrumentChange(int givenBar, int givenLine)
            : base(givenBar, givenLine, MusicalChangeType.Instrument) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentChange"/> class.
        /// </summary>
        /// <param name="givenBar">The given bar.</param>
        /// <param name="givenLine">The given line.</param>
        /// <param name="givenChannel">The given channel.</param>
        /// <param name="givenInstrument">The given instrument.</param>
        public InstrumentChange(int givenBar, int givenLine, MidiChannel givenChannel, MusicalInstrument givenInstrument)
            : base(givenBar, givenLine, MusicalChangeType.Instrument) {
            this.Channel = givenChannel;
            this.Instrument = givenInstrument;
        }

        #endregion

        #region Properties - Xml
        /// <summary>
        /// Gets Xml representation.
        /// </summary>
        /// <value>
        /// Property description.
        /// </value>
        public override XElement GetXElement {
            get {
                var change = base.GetXElement;
                change.Add(new XAttribute("Instrument", this.Instrument));
                change.Add(new XAttribute("Channel", this.Channel));
                return change;
            }
        }
        #endregion

        #region Properties
        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MusicalInstrument Instrument { get; set; }

        /// <summary> Gets or sets class of melodic part. </summary>
        /// <value> Property description. </value>
        public MidiChannel Channel { get; set; }

        /// <summary>
        /// Gets InstrumentString.
        /// </summary>
        /// <value> General musical property.</value>
        //// Do not make private!!! - It is used by DetailMusicalChanges.xaml.
        [UsedImplicitly]
        public string InstrumentString => this.Instrument.ToString();

        /// <summary>
        /// Gets the channel string.
        /// </summary>
        /// <value> Property description. </value>
        [UsedImplicitly]
        public string ChannelString => this.Channel.ToString();

        #endregion

        #region Public methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns> Returns object. </returns>
        public override object Clone() {
            var tmc = new InstrumentChange(this.BarNumber, this.LineIndex)
            {
                Instrument = this.Instrument, Channel = this.Channel
            };
            //// tmc.BlockModel = this.BlockModel;

            return tmc;
        }
        #endregion

        #region String representation

        /// <summary> String representation of the object. </summary>
        /// <returns> Returns value. </returns>
        public override string ToString() {
            var s = new StringBuilder();
            s.AppendFormat(CultureInfo.CurrentCulture, base.ToString());
            s.Append(", " + this.InstrumentString);
            return s.ToString();
        }
        #endregion
    }
}
