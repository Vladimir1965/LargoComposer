using LargoSharedClasses.Composer;
using LargoSharedClasses.Interfaces;
using LargoSharedClasses.Melody;
using LargoSharedClasses.Music;
using LargoSharedClasses.Player;
using System.Collections.Generic;
using System.Windows;

namespace LargoTest
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
        var m = new NestedMotive("abcd", null);
        var M = new NestedMotive("ABCD", m);
        var T = new NestedMotive("PQRS", M);
        var r = T.Result(true);
        this.BigTextBox.Text = r;
        */

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var m = new NestedMotive("0123", null);
            var M = new NestedMotive("abcd", m);
            var T = new NestedMotive("ABCD", M);
            var b = this.SimpleBlock(T);

            BlockComposer blockComposer = new BlockComposer();
            MusicalBundle result = blockComposer.ComposeMusic(b);
            var bx = result.Blocks[0];
            this.MusicPlayMp3(bx);
        }

        public MusicalBlock SimpleBlock(NestedMotive melMotive)
        {
            var x = melMotive.ResultList(true);
            var nlist = new List<int>();
            foreach (var c in x) {
                int s = 999;
                switch (c) {
                    case 'a': s = 2; break;
                    case 'b': s = 4; break;
                    case 'c': s = 6; break;
                    case 'd': s = 8; break;
                    case 'A': s = 0; break;
                    case 'B': s = 3; break;
                    case 'C': s = 1; break;
                    case 'D': s = 4; break;
                    default: break;
                }

                if (s != 999) {
                    nlist.Add(s);
                }
            }
            //// this.BigTextBox.Text = r;

            var block = new MusicalBlock();
            var lineStatus = new LineStatus {
                LocalPurpose = LinePurpose.Fixed,
                Instrument = new MusicalInstrument(MidiMelodicInstrument.AcousticGrandPiano)
            };

            var newline = block.AddContentLine(lineStatus);
            var voice = new MusicalVoice {
                    Instrument = lineStatus.Instrument,
                    Octave = lineStatus.Octave,
                    Loudness = lineStatus.Loudness,
                    Line = newline
            };

            newline.Voices = new List<IAbstractVoice> { voice };
            newline.MainVoice = voice;

            var line = (MusicalLine)newline;
            int barNumber = 0;
            byte bit = 0;
            var tonenum = 0;
            foreach (var n in nlist) {
                if (tonenum % 4 == 0 || barNumber == 0) {
                    barNumber++;
                    var bar = block.AddContentBar(barNumber, null);
                    if (barNumber % 2 == 0) {
                        bar.HarmonicBar.SetStructure(new HarmonicStructure(block.Header.System.HarmonicSystem, "0,4,7"));
                    } else {
                        bar.HarmonicBar.SetStructure(new HarmonicStructure(block.Header.System.HarmonicSystem, "7,11,2"));
                    }
                    bit = 0;
                }

                var pitch = new MusicalPitch((byte) (64 + n));
                var mt = new MusicalTone(pitch, new BitRange(12, bit, 3), MusicalLoudness.MeanLoudness, barNumber);
                bit += 3;
                line.AddMusicalTone(mt);
                tonenum++;
            }

            ////--------------
            lineStatus = new LineStatus {
                LocalPurpose = LinePurpose.Composed,
                Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1)
            };

            lineStatus.LineType = MusicalLineType.Melodic;
            lineStatus.MelodicGenus = MelodicGenus.Harmonic;
            lineStatus.RhythmicStructure = RhythmicStructure.GetFullStructure(12);
            lineStatus.Octave = MusicalOctave.TwoLine;
            lineStatus.LocalPurpose = LinePurpose.Composed;
            newline = block.AddContentLine(lineStatus);
            ////--------------
            lineStatus = new LineStatus {
                LocalPurpose = LinePurpose.Composed,
                Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1)
            };

            lineStatus.LineType = MusicalLineType.Melodic;
            lineStatus.MelodicGenus = MelodicGenus.Harmonic;
            lineStatus.RhythmicStructure = RhythmicStructure.GetFullStructure(12);
            newline = block.AddContentLine(lineStatus);
            ////--------------
            lineStatus = new LineStatus {
                LocalPurpose = LinePurpose.Composed,
                Instrument = new MusicalInstrument(MidiMelodicInstrument.StringEnsemble1)
            };

            lineStatus.LineType = MusicalLineType.Melodic;
            lineStatus.MelodicGenus = MelodicGenus.Harmonic;
            lineStatus.RhythmicStructure = RhythmicStructure.GetFullStructure(12);
            newline = block.AddContentLine(lineStatus);

            //// Convert current body back to strip!?! (Status is in elements)
            //// block.ConvertBodyToStrip(false, true);
            
            //// block.Body = null;
            block.ConvertStripToBody(true);
            block.RefreshHeader();

            return block;
        }

        /// <summary>
        /// Music play MP3.
        /// </summary>
        /// <param name="givenBlock">The given block.</param>
        public void MusicPlayMp3(MusicalBlock givenBlock)
        {
            PlayCentrum.Singleton.UserMusicFolder = @"c:\temp"; //// MusicalSettings.Singleton.Folders.GetFolder(MusicalFolder.UserMusic);
            PlayCentrum.Singleton.PathToInternalConverter = @"c:\Users\Krakonoš\Documents\Indefinite Software\Largo 2022\InternalMusic\Converter";
            MusicalPlayer.Singleton.UserMusicFolder = @"c:\temp";
            //// PlayCentrum.Singleton.SoundFontName = soundFontName;
            PlayCentrum.Singleton.MusicPlayMp3(givenBlock);
        }
    }
}
