using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pepperSoft.Modele
{
    public class Haralick
    {
        public float contrast { get; set; }
        public float correlation { get; set; }
        public float variance { get; set; }
        public float inverseDiffMoment { get; set; }
        public float sumAvg { get; set; }
        public float sumVar { get; set; }
        public float sumEntropy { get; set; }
        public float entropy { get; set; }
        public float diffVar { get; set; }
        public float diffEntropy { get; set; }
        public float firstInfoMeasure { get; set; }
        public float secondInfoMeasure { get; set; }

    }
}
