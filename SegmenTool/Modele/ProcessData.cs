using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmenTool.Modele
{
    class ProcessData
    {
        protected string segmentationFilePath = "";

        protected string inputFolderPath = "";

        protected string outputFolderPath = "";

        public string SegmentationFilePath() { return segmentationFilePath; }

        public string InputFolderPath() { return inputFolderPath; }

        public string OutputFolderPath() { return outputFolderPath; }

    }
}
