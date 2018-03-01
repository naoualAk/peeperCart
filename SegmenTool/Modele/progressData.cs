using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmenTool.Modele
{
    public class ProgressData
    {

        public ProgressData(int currentImg, int totalImg, bool error=false, string message = "")
        {
            CurrentImg = currentImg;
            TotalImg = totalImg;
            Error = error;
            Message = message;
        }

        public ProgressData(bool ended)
        {
            Ended = ended;
        }


        public int CurrentImg { get; set; }
        public int TotalImg { get; set; }
        public string Message { get; set; }
        public bool Error { get; set; }

        public bool Ended { get; set; }
    }
}
