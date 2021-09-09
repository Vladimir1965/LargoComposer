// <copyright file="MusicalChanges.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace LargoSharedClasses.Models
{
    /// <summary>
    /// Musical Block Changes.
    /// </summary>
    public sealed class MusicalChanges {
        #region Fields
        /// <summary>
        /// Musical Changes.
        /// </summary>
        private Collection<AbstractChange> changes;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalChanges"/> class.
        /// </summary>
        public MusicalChanges() {
            this.Changes = new Collection<AbstractChange>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicalChanges" /> class.
        /// </summary>
        /// <param name="markBlockModel">The mark block model.</param>
        public MusicalChanges(XContainer markBlockModel) { //// XElement
            Contract.Requires(markBlockModel != null);
            if (markBlockModel == null) {
                return;
            }

            //// EnergyChanges
            XElement xchanges = markBlockModel.Element("EnergyChanges");
            if (xchanges != null) {
                foreach (var xchange in xchanges.Elements()) {
                    EnergyChange change = new EnergyChange(xchange);
                    this.Changes.Add(change);
                }
            }

            //// HarmonicChanges
            xchanges = markBlockModel.Element("HarmonicChanges");
            if (xchanges != null) {
                foreach (var xchange in xchanges.Elements()) {
                    var change = new HarmonicChange(xchange);
                    this.Changes.Add(change);
                }
            }

            //// InstrumentChanges
            xchanges = markBlockModel.Element("InstrumentChanges");
            if (xchanges != null) {
                foreach (var xchange in xchanges.Elements()) {
                    var change = new InstrumentChange(xchange);
                    this.Changes.Add(change);
                }
            }

            //// LoudnessChanges
            xchanges = markBlockModel.Element("LoudnessChanges");
            if (xchanges != null) {
                foreach (var xchange in xchanges.Elements()) {
                    var change = new LoudnessChange(xchange);
                    this.Changes.Add(change);
                }
            }

            //// OctaveChanges
            xchanges = markBlockModel.Element("OctaveChanges");
            if (xchanges != null) {
                foreach (var xchange in xchanges.Elements()) {
                    var change = new OctaveChange(xchange);
                    this.Changes.Add(change);
                }
            }

            //// StaffChanges
            xchanges = markBlockModel.Element("StaffChanges");
            if (xchanges != null) {
                foreach (var xchange in xchanges.Elements()) {
                    var change = new StaffChange(xchange);
                    this.Changes.Add(change);
                }
            }

            //// TempoChanges
            xchanges = markBlockModel.Element("TempoChanges");
            if (xchanges != null) {
                foreach (var xchange in xchanges.Elements()) {
                    var change = new TempoChange(xchange);
                    this.Changes.Add(change);
                }
            }

            //// TonalityChanges
            xchanges = markBlockModel.Element("TonalityChanges");
            if (xchanges != null) {
                foreach (var xchange in xchanges.Elements()) {
                    var change = new TonalityChange(xchange);
                    this.Changes.Add(change);
                }
            }
        }

        #endregion

        #region Properties - Xml
        /// <summary> Gets Xml representation. </summary>
        /// <value> Property description. </value>
        public XElement GetXElement {
            get {
                XElement xlist = new XElement("Changes");

                //// EnergyChanges
                XElement xchanges = new XElement("EnergyChanges");
                foreach (var change in this.EnergyChanges.Where(change => change != null)) {
                    var xchange = change.GetXElement;
                    xchanges.Add(xchange);
                }

                xlist.Add(xchanges);

                //// HarmonicChanges
                xchanges = new XElement("HarmonicChanges");
                foreach (var change in this.HarmonicChanges.Where(change => change != null)) {
                    var xchange = change.GetXElement;
                    xchanges.Add(xchange);
                }

                xlist.Add(xchanges);

                //// InstrumentChanges
                xchanges = new XElement("InstrumentChanges");
                foreach (var change in this.InstrumentChanges.Where(change => change != null)) {
                    var xchange = change.GetXElement;
                    xchanges.Add(xchange);
                }

                xlist.Add(xchanges);

                //// LoudnessChanges
                xchanges = new XElement("LoudnessChanges");
                foreach (var change in this.LoudnessChanges.Where(change => change != null)) {
                    var xchange = change.GetXElement;
                    xchanges.Add(xchange);
                }

                xlist.Add(xchanges);

                //// OctaveChanges
                xchanges = new XElement("OctaveChanges");
                foreach (var change in this.OctaveChanges.Where(change => change != null)) {
                    var xchange = change.GetXElement;
                    xchanges.Add(xchange);
                }

                xlist.Add(xchanges);

                //// StaffChanges
                xchanges = new XElement("StaffChanges");
                foreach (var change in this.StaffChanges.Where(change => change != null)) {
                    var xchange = change.GetXElement;
                    xchanges.Add(xchange);
                }

                xlist.Add(xchanges);

                //// TempoChanges
                xchanges = new XElement("TempoChanges");
                foreach (var change in this.TempoChanges.Where(change => change != null)) {
                    var xchange = change.GetXElement;
                    xchanges.Add(xchange);
                }

                xlist.Add(xchanges);

                //// TonalityChanges
                xchanges = new XElement("TonalityChanges");
                foreach (var change in this.TonalityChanges.Where(change => change != null)) {
                    var xchange = change.GetXElement;
                    xchanges.Add(xchange);
                }

                xlist.Add(xchanges);

                return xlist;
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the musical changes.
        /// </summary>
        /// <value>
        /// The musical changes.
        /// </value>
        public Collection<AbstractChange> Changes {
            get {
                Contract.Ensures(Contract.Result<Collection<AbstractChange>>() != null);
                if (this.changes == null) {
                    throw new InvalidOperationException(Localization.LocalizedMusic.String("Musical model has no changes."));
                }

                return this.changes;
            }

            private set => this.changes = value ?? throw new ArgumentException(Localization.LocalizedMusic.String("Argument cannot be empty."), nameof(value));
        }

        /// <summary>
        /// Gets the harmonic changes.
        /// </summary>
        /// <value> Property description. </value>
        public IEnumerable<HarmonicChange> HarmonicChanges {
            get {
                if (this.changes == null) {
                    return null;
                }

                var cs = (from c in this.changes
                               where c.ChangeType == MusicalChangeType.Harmonic
                               orderby c.BarNumber
                               select c as HarmonicChange).ToList();
                return cs;
            }
        }

        /// <summary>
        /// Gets the tempo changes.
        /// </summary>
        /// <value> Property description. </value>
        public IEnumerable<TempoChange> TempoChanges {
            get {
                if (this.changes == null) {
                    return null;
                }

                var cs = (from c in this.changes
                               where c.ChangeType == MusicalChangeType.Tempo
                               orderby c.BarNumber
                               select c as TempoChange).ToList();
                return cs;
            }
        }

        /// <summary>
        /// Gets the instrument changes.
        /// </summary>
        /// <value> Property description. </value>
        public IEnumerable<InstrumentChange> InstrumentChanges {
            get {
                if (this.changes == null) {
                    return null;
                }

                var cs = (from c in this.changes
                               where c.ChangeType == MusicalChangeType.Instrument
                               orderby c.BarNumber
                               select c as InstrumentChange).ToList();
                return cs;
            }
        }

        /// <summary>
        /// Gets the staff changes.
        /// </summary>
        /// <value> Property description. </value>
        public IEnumerable<StaffChange> StaffChanges {
            get {
                if (this.changes == null) {
                    return null;
                }

                var cs = (from c in this.changes
                               where c.ChangeType == MusicalChangeType.Staff
                               orderby c.BarNumber
                               select c as StaffChange).ToList();
                return cs;
            }
        }

        /// <summary>
        /// Gets the staff changes.
        /// </summary>
        /// <value> Property description. </value>
        public IEnumerable<OctaveChange> OctaveChanges {
            get {
                if (this.changes == null) {
                    return null;
                }

                var cs = (from c in this.changes
                               where c.ChangeType == MusicalChangeType.Octave
                               orderby c.BarNumber
                               select c as OctaveChange).ToList();
                return cs;
            }
        }

        /// <summary>
        /// Gets the staff changes.
        /// </summary>
        /// <value> Property description. </value>
        public IEnumerable<LoudnessChange> LoudnessChanges {
            get {
                if (this.changes == null) {
                    return null;
                }

                var cs = (from c in this.changes
                               where c.ChangeType == MusicalChangeType.Loudness
                               orderby c.BarNumber
                               select c as LoudnessChange).ToList();
                return cs;
            }
        }

        /// <summary>
        /// Gets the energy changes.
        /// </summary>
        /// <value> Property description. </value>
        public IEnumerable<EnergyChange> EnergyChanges {
            get {
                if (this.changes == null) {
                    return null;
                }

                var cs = (from c in this.changes
                               where c.ChangeType == MusicalChangeType.Energy
                               orderby c.BarNumber
                               select c as EnergyChange).ToList();
                return cs;
            }
        }

        /// <summary>
        /// Gets the tonality changes.
        /// </summary>
        /// <value> Property description. </value>
        public IEnumerable<TonalityChange> TonalityChanges {
            get {
                if (this.changes == null) {
                    return null;
                }

                var cs = (from c in this.changes
                               where c.ChangeType == MusicalChangeType.Tonality
                               orderby c.BarNumber
                               select c as TonalityChange).ToList();
                return cs;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Appends the copy of changes.
        /// </summary>
        /// <param name="givenChanges">The given changes.</param>
        public void AppendCopyOfChanges(IEnumerable<AbstractChange> givenChanges) { //// BlockModel givenModel
            Contract.Requires(givenChanges != null);
            this.AppendChanges(givenChanges.Select(tmc => (AbstractChange)tmc.Clone()));
        }

        /// <summary>
        /// Removes the changes.
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        [UsedImplicitly]
        public void RemoveChanges(MusicalChangeType changeType) {
            var cs = (from mc in this.Changes where mc.ChangeType == changeType select mc).ToList();
            cs.ForEach(mc => this.Changes.Remove(mc));
        }

        /// <summary>
        /// Find the last change in given line.
        /// </summary>
        /// <param name="lineIndex">The line index.</param>
        /// <param name="barNumber">Bar number.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <returns>
        /// Returns value.
        /// </returns>
        public AbstractChange CurrentChange(int lineIndex, int barNumber, MusicalChangeType changeType) {
            IEnumerable<AbstractChange> musicChanges = (from c in this.Changes
                                                    where c.LineIndex == lineIndex && c.BarNumber == barNumber && c.ChangeType == changeType
                                                    orderby c.IsStop descending, c.LineType
                                                    select c).ToList();
            //// ReSharper disable PossibleMultipleEnumeration
            return !musicChanges.Any() ? null : musicChanges.Last();
            //// ReSharper restore PossibleMultipleEnumeration
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Appends the changes.
        /// </summary>
        /// <param name="givenChanges">The given changes.</param>
        private void AppendChanges(IEnumerable<AbstractChange> givenChanges) { //// , BlockModel givenModel
            Contract.Requires(givenChanges != null);
            foreach (var change in givenChanges) {
                //// change.BlockModel = givenModel;
                this.Changes.Add(change);
            }
        }

        #endregion
    }
}
