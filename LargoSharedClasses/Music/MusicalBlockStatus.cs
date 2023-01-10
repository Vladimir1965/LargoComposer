using LargoSharedClasses.Composer;
using LargoSharedClasses.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargoSharedClasses.Music
{
    public partial class MusicalBlock //// Status
    {
        /// <summary>
        /// Gets or sets a value indicating whether this object is strip status ok.
        /// </summary>
        /// <value> True if this object is strip status ok, false if not. </value>
        public bool IsStripStatusOk { get; set; }

        /// <summary>
        /// Loads the instruments to lines.
        /// </summary>
        public void LoadFirstStatusToLines()
        {
            foreach (var line in this.Strip.Lines) {
                foreach (var bar in this.Body.Bars) {
                    var point = new MusicalPoint(line.LineIndex, bar.BarNumber);
                    var element = this.Body.GetElement(point);
                    if (element?.Status?.Instrument != null && !element.Status.Instrument.IsEmpty) {
                        line.FirstStatus.Instrument = element.Status.Instrument; //// FixedInstrument
                        line.CurrentInstrument = line.FirstStatus.Instrument.Number; //// FixedInstrument
                        line.FirstStatus.LineType = element.Status.LineType;
                        line.FirstStatus.LocalPurpose = element.Status.LocalPurpose;  //// 2010/12
                        //// line.FirstStatus.Instrument = new MusicalInstrument(element.Status.InstrumentNumber, element.Status.LineType);
                        break;
                    }
                }

                line.FirstStatus.MelodicVariety = new MusicalVariety(MusicalSettings.Singleton);
            }
        }

        #region Body-Strip Status Conversion -- see also Strip.WriteBody

        /// <summary>
        /// Gets the line status list.
        /// This methods probably do not work properly !?!?
        /// Now used before composition in combination with SendStatusToTones!?
        /// </summary>
        public void ConvertStripStatusToBody()
        {
            if (this.Body?.Bars == null) {
                return;
            }

            foreach (MusicalLine line in this.Strip.Lines.Where(ml => ml != null)) {
                line.CurrentStatus = line.FirstStatus;

                foreach (var bar in this.Body.Bars) {
                    var element =
                        (from elem in bar.Elements where elem.Line.LineIdent == line.LineIdent select elem)
                        .FirstOrDefault();
                    if (element == null) {
                        continue;
                    }

                    //// Status from tones only in case where there is no status defined in tracks
                    if (line.StatusList != null && line.StatusList.Any()) {
                        var status = (from ts in line.StatusList
                                      where ts.BarNumber == bar.BarNumber
                                      select ts).FirstOrDefault();
                        if (status != null) {
                            //// 2018/10 && lineStatus.Purpose != LinePurpose.None !?
                            line.CurrentStatus = status;
                        }

                        var newStatus = (LineStatus)line.CurrentStatus.Clone();
                        newStatus.BarNumber = bar.BarNumber;
                        element.Status =
                            newStatus; //// !!!! otherwise there are 2 different statuses for one element ...??
                    } else {
                        element.SetElementStatusFromTones();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the line status list i.e. list of bar status in each given line. 
        /// </summary>
        public void ConvertBodyStatusToStrip()
        {
            foreach (MusicalLine line in this.Strip.Lines.Where(ml => ml != null)) {
                line.StatusList = new List<LineStatus>();
            }

            foreach (var bar in this.Body.Bars) {
                foreach (var element in bar.Elements) {
                    var line = element.MusicalLine;
                    line.StatusList?.Add(element.Status);
                }
            }
        }

        #endregion
    }
}

/*
/// <summary> Gets or sets a value indicating whether this object is body status ok. </summary>
/// <value> True if this object is body status ok, false if not. </value>
[UsedImplicitly]
public bool IsBodyStatusOk { get; set; } */