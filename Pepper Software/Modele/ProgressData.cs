using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pepperSoft.Modele
{
    public class ProgressData
    {

        public ProgressData(int currentImg, int totalImg, Image<Rgb,Byte> image=null, bool error = false, string message = "")
        {
            CurrentImg = currentImg;
            TotalImg = totalImg;
            Error = error;
            Message = message;
            Image = image;
        }



        public ProgressData(bool ended)
        {
            Ended = ended;
        }

        public Image<Rgb, Byte> Image { get; set; }
        public int CurrentImg { get; set; }
        public int TotalImg { get; set; }
        public string Message { get; set; }
        public bool Error { get; set; }

        public bool Ended { get; set; }
    }
}

