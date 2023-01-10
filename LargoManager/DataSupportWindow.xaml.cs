// <copyright file="DataSupportWindow.xaml.cs" company="Traced-Ideas, Czech republic">
// Copyright (c) 1990-2021 All Right Reserved
// </copyright>
// <author>vl</author>
// <email></email>
// <date>2021-09-01</date>
// <summary>Part of Largo Composer</summary>

using LargoSharedClasses.Music;
using LargoSharedClasses.Support;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace LargoManager
{
    /// <summary>
    /// Data Support Window.
    /// </summary>
    public partial class DataSupportWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSupportWindow"/> class.
        /// </summary>
        public DataSupportWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Generates the rhythmic patterns.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void GenerateRhythmicPatterns(object sender, RoutedEventArgs e)
        {
            MusicDocument document = PortDocuments.Singleton.SelectedDocument;
            if (document == null) {
                return;
            }

            //// CommonActions.Singleton.SelectedBlock = resultBlock;
            PortDocuments.Singleton.LoadDocument(document, true);
            var block = PortDocuments.Singleton.MusicalBlock;
            if (block == null) {
                return;
            }

            var style = new MusicalStyle(block);
            var rp = style.RhythmicPatterns;

            XElement rplist = new XElement("RhythmicPatterns");
            var xdocrp = new XDocument(new XDeclaration("1.0", "utf-8", null), rplist);
            foreach (var rp1 in rp) {
                rplist.Add(rp1.GetXElement);
            }

            xdocrp.Save(Path.Combine(@"C:\Temp", @"RhythmicPatterns.xml"));
        }

        /// <summary>
        /// Generates the melodic patterns.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void GenerateMelodicPatterns(object sender, RoutedEventArgs e)
        {
            MusicDocument document = PortDocuments.Singleton.SelectedDocument;
            if (document == null) {
                return;
            }

            //// CommonActions.Singleton.SelectedBlock = resultBlock;
            PortDocuments.Singleton.LoadDocument(document, true);
            var block = PortDocuments.Singleton.MusicalBlock;
            if (block == null) {
                return;
            }

            var style = new MusicalStyle(block);
            var mp = style.MelodicPatterns;

            XElement mplist = new XElement("MelodicPatterns");
            var xdocmp = new XDocument(new XDeclaration("1.0", "utf-8", null), mplist);
            foreach (var mp1 in mp) {
                mplist.Add(mp1.GetXElement);
            }

            xdocmp.Save(Path.Combine(@"C:\Temp", @"MelodicPatterns.xml"));

            /*
            ///// foreach (var bar in block.Body.Bars) {
            var x = from element in bar.Elements
                        where element?.Status?.RhythmicStructure?.ToneLevel > 0
                        group element by element?.Status?.RhythmicStructure?.ToShortString() ?? string.Empty
                            into g
                        select new { RhythmicSchema = g.Key, Elements = g }; 
                -----------------
                foreach (var t in x) {
                    if (t.Elements.Count() > 5) {
                        sb.AppendLine("* " + t.RhythmicSchema + " *");
                        int j = 0;
                        var elems = from v in t.Elements orderby v.Status.Octave descending select v;
                        foreach (var elem in elems) {
                            sb.Append(j + "/ ");
                            sb.Append(elem.Status.Instrument.MelodicInstrument.ToString());
                            sb.Append(@" >> " + elem.Status.Octave.ToString());
                            sb.AppendLine(@" >> " + elem.Status.Loudness.ToString());
                        }
                    }
                }
            */
        }
    }
}
