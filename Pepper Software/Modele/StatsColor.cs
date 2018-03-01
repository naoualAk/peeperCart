using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pepperSoft.Modele
{
    public class StatsColor
    {

        public int L { get; set; }
        public int a { get; set; }
        public int b { get; set; }
        public double LET { get; set; }
        public double aET { get; set; }
        public double bET { get; set; }


        public Rgb Color1 { get; set; }
        public string Color1Str { get; set; }
        public Rgb Color2 { get; internal set; }
        public string Color2Str { get; internal set; }
        public Rgb Color3 { get; internal set; }
        public string Color3Str { get; internal set; }
    }
}
