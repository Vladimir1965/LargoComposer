// <copyright file="PortAnalysis.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml.Linq;
using LargoSharedClasses.Models;
using LargoSharedClasses.Music;
using LargoSharedClasses.Orchestra;

namespace LargoSharedClasses.Support
{
    /// <summary>
    /// Data Analysis.
    /// </summary>
    public class PortAnalysis
    {
        /// <summary>
        /// Singleton variable.
        /// </summary>
        private static readonly PortAnalysis InternalSingleton = new PortAnalysis();

        /// <summary>
        /// The harmonic stream list
        /// </summary>
        private List<HarmonicStream> harmonicStreamList;

        /// <summary>
        /// The rhythmic material list
        /// </summary>
        private List<RhythmicMaterial> rhythmicMaterialList;

        /// <summary>
        /// The orchestra block list
        /// </summary>
        private List<OrchestraBlock> orchestraBlockList;

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the PortAnalysis class from being created.
        /// </summary>
        private PortAnalysis() {
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Gets the ProcessLogger Singleton.
        /// </summary>
        /// <value> Property description. </value>
        public static PortAnalysis Singleton {
            get {
                Contract.Ensures(Contract.Result<PortAnalysis>() != null);
                if (InternalSingleton == null) {
                    throw new InvalidOperationException("Singleton PortAnalysis is null.");
                }

                return InternalSingleton;
            }
        }
        #endregion

        #region List Properties

        /// <summary>
        /// Gets or sets the internal path.
        /// </summary>
        /// <value>
        /// The internal path.
        /// </value>
        public string InternalPath { get; set; }

        /// <summary>
        /// Gets or sets the harmonic stream list.
        /// </summary>
        /// <value>
        /// The harmonic stream list.
        /// </value>
        public List<HarmonicStream> HarmonicStreamList { 
            get {
                if (this.harmonicStreamList == null) {
                    this.LoadHarmonicStreams(this.InternalPath);
                }

                return this.harmonicStreamList;
            } 

            set => this.harmonicStreamList = value;
        }

        /// <summary>
        /// Gets or sets the rhythmic material list.
        /// </summary>
        /// <value>
        /// The rhythmic material list.
        /// </value>
        public List<RhythmicMaterial> RhythmicMaterialList {
            get {
                if (this.rhythmicMaterialList == null) {
                    this.LoadRhythmicMaterials(this.InternalPath);
                }

                return this.rhythmicMaterialList;
            }

            set => this.rhythmicMaterialList = value;
        }

        /// <summary>
        /// Gets or sets the orchestra blocks list.
        /// </summary>
        /// <value>
        /// The orchestra blocks list.
        /// </value>
        public List<OrchestraBlock> OrchestraBlockList {
            get {
                if (this.orchestraBlockList == null) {
                    this.LoadOrchestraBlocks(this.InternalPath);
                }

                return this.orchestraBlockList;
            }

            set => this.orchestraBlockList = value;
        }

        #endregion

        #region Public methods - Loading
        /// <summary>
        /// Loads the harmonic streams.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public void LoadHarmonicStreams(string givenPath) {
            this.HarmonicStreamList = new List<HarmonicStream>();
            var fileName = "SavedHarmonicTemplates.xml";
            var filepath = Path.Combine(givenPath, fileName); 
            if (!File.Exists(filepath)) {
                return;
            }

            var xdoc = XDocument.Load(filepath);
            var root = xdoc.Root;
            if (root == null || root.Name != "HarmonicTemplates") {
                return;
            }

            var xlist = root;
            foreach (var xstream in xlist.Elements()) {
                HarmonicStream stream = new HarmonicStream(xstream, true);
                if (stream.HarmonicBars.Count > 0) {
                    this.HarmonicStreamList.Add(stream);
                }
            }
        }

        /// <summary>
        /// Loads the rhythmic materials.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public void LoadRhythmicMaterials(string givenPath) {
            this.RhythmicMaterialList = new List<RhythmicMaterial>();
            var fileName = "SavedRhythmicTemplates.xml";
            var filepath = Path.Combine(givenPath, fileName);  
            if (!File.Exists(filepath)) {
                return;
            }

            var xdoc = XDocument.Load(filepath);
            var root = xdoc.Root;
            if (root == null || root.Name != "RhythmicTemplates") {
                return;
            }

            var xlist = root;
            foreach (var xmaterial in xlist.Elements()) {
                RhythmicMaterial material = new RhythmicMaterial(xmaterial);
                if (material.Structures.Count > 0) {
                    this.RhythmicMaterialList.Add(material);
                }
            }
        }

        /// <summary>
        /// Loads the blocks.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        public void LoadOrchestraBlocks(string givenPath) {
            this.OrchestraBlockList = new List<OrchestraBlock>();
            var fileName = "SavedOrchestraTemplates.xml";
            var filepath = Path.Combine(givenPath, fileName); 
            if (!File.Exists(filepath)) {
                return;
            }

            var xdoc = XDocument.Load(filepath);
            var root = xdoc.Root;
            if (root == null || root.Name != "OrchestraTemplates") {
                return;
            }

            var xlist = root;
            foreach (var xblock in xlist.Elements()) {
                OrchestraBlock block = new OrchestraBlock(xblock);
                if (block.Strip.OrchestraVoices.Count > 0) {
                    this.OrchestraBlockList.Add(block);
                }
            }
        }

        /// <summary>
        /// Extracts the models.
        /// </summary>
        /// <param name="musicBundle">The music bundle.</param>
        /// <param name="givenPath">The given path.</param>
        public void ExtractModels(MusicalBundle musicBundle, string givenPath) {
            //// For first load (from MIDI or MXL do this staff ...] 

            foreach (var block in musicBundle.Blocks) {
                var blockWrap = new MusicalBlockWrap(block);
                ////  MusicalStyle --> MelodicPatterns, 
                ////  MusicalTectonic --> TectonicList
                ////  Body --> LineChunkList

                this.HarmonicStreamList.AddRange(blockWrap.HarmonicModel.HarmonicStreams);
                this.RhythmicMaterialList.Add(blockWrap.RhythmicModel.ExtractRhythmicMaterial());
                this.OrchestraBlockList.AddRange(blockWrap.Orchestration.OrchestraBlocks);
            }

            this.SaveHarmonicStreams(givenPath);
            this.SaveRhythmicMaterials(givenPath);
            this.SaveOrchestraBlocks(givenPath);
        }
        #endregion

        #region Private methods - Saving

        /// <summary>
        /// Saves the harmonic streams.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        private void SaveHarmonicStreams(string givenPath) {
            if (this.harmonicStreamList == null) {
                return;
            }
            
            XElement xlist = new XElement("HarmonicTemplates");
            foreach (var stream in this.HarmonicStreamList) {
                if (stream.HarmonicBars.Count == 0) {
                    continue;
                }

                var xstream = stream.GetXElement;
                xlist.Add(xstream);
            }

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xlist);
            var fileName = "SavedHarmonicTemplates.xml";
            var filepath = Path.Combine(givenPath, fileName); 
            if (File.Exists(filepath)) {
                xdoc.Save(filepath);
            }
        }

        /// <summary>
        /// Saves the rhythmic materials.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        private void SaveRhythmicMaterials(string givenPath) {
            if (this.rhythmicMaterialList == null) {
                return;
            }

            XElement xlist = new XElement("RhythmicTemplates");
            foreach (var stream in this.RhythmicMaterialList) {
                if (stream.Structures.Count == 0) {
                    continue;
                }

                var xstream = stream.GetXElement;
                xlist.Add(xstream);
            }

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xlist);
            var fileName = "SavedRhythmicTemplates.xml";
            var filepath = Path.Combine(givenPath, fileName); 
            if (File.Exists(filepath)) {
                xdoc.Save(filepath);
            }
        }

        /// <summary>
        /// Saves the blocks.
        /// </summary>
        /// <param name="givenPath">The given path.</param>
        private void SaveOrchestraBlocks(string givenPath) {
            if (this.orchestraBlockList == null) {
                return;
            }

            XElement xlist = new XElement("OrchestraTemplates");
            foreach (var block in this.OrchestraBlockList) {
                if (block.Strip.OrchestraVoices.Count == 0) {
                    continue;
                }

                var xblock = block.GetXElement;
                xlist.Add(xblock);
            }

            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", null), xlist);
            var fileName = "SavedOrchestraTemplates.xml";
            var filepath = Path.Combine(givenPath, fileName); 
            if (File.Exists(filepath)) {
                xdoc.Save(filepath);
            }
        }

        #endregion
    }
}
