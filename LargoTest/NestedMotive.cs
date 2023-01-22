using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargoTest
{
    public class NestedMotive
    {        
        public NestedMotive(string givenLine, NestedMotive givenLeaves)
        {
            this.Line = givenLine;
            this.Leaves = givenLeaves;
        }

        public string Line { get; set; }
        public NestedMotive Leaves { get; set; }
        public string Result(bool rotateLeaves) {
            var sb = new StringBuilder();
            foreach (var c in this.Line) {
                sb.Append(c);
                if (this.Leaves == null) {
                    sb.Append(",");
                }

                var leavesResult = this.Leaves != null ? this.Leaves.Result(rotateLeaves) + " | " : string.Empty;
                if (!string.IsNullOrEmpty(leavesResult)) {
                    sb.Append(leavesResult.Substring(1));
                }

                if (this.Leaves != null && rotateLeaves) {
                    this.Leaves.RotateLine();
                }
            }

            return sb.ToString();
        }

        public List<char> ResultList(bool rotateLeaves)
        {
            var sb = new List<char>();
            foreach (var c in this.Line) {
                sb.Add(c);
                if (this.Leaves == null) {
                    sb.Add(',');
                }

                var leavesResult = new List<char>();
                if (this.Leaves != null) {
                    leavesResult = this.Leaves.ResultList(rotateLeaves);
                    leavesResult.Add('|');
                }

                if (leavesResult.Any()) {
                    for (int k = 1; k < leavesResult.Count; k++) {
                        sb.Add(leavesResult[k]);
                    }
                }

                if (this.Leaves != null && rotateLeaves) {
                    this.Leaves.RotateLine();
                }
            }

            return sb;
        }

        public void RotateLine()
        {
            var s = this.Line.Substring(1) + this.Line[0];
            this.Line = s;
        }
    }
}
