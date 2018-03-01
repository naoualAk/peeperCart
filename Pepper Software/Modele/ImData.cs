using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pepperSoft.Modele
{
    public class ImData
    {
        public Image<Rgb, Byte> imOrigin { get; set; }

        public Image<Bgr, Byte> Mask { get; set; }

        public Image<Bgr, Byte> Origin { get; set; }

        public Image<Bgr, Byte> Kmean { get; set; }

        public Image<Bgr, Byte> Color { get; set; }
    }
}
