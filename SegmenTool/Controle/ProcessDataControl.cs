using SegmenTool.Modele;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmenTool.Controle
{
    class ProcessDataControl : ProcessData
    {

        public bool SegmentationFilePath(string value)
        {
            FileInfo fi = new FileInfo(value);
            if (fi.Exists)
            {
                segmentationFilePath = value;
                return true;
            }
            else
            {
                segmentationFilePath = "";
                return false;
            }
        }

        public bool InputFolderPath(string value)
        {
            if (Directory.Exists(value))
            {
                inputFolderPath = value;
                return true;
            }
            else
            {
                inputFolderPath = "";
                return false;
            }
        }

        public void ForceOutputFolderPath(string value)
        {
            outputFolderPath = value;
        }

        public bool OutputFolderPath(string value)
        {
            if (Directory.Exists(value) )
            {
                outputFolderPath = value;
                return true;
            }
            else
            {
                    outputFolderPath = "";
                    return false;   
            }
        }
    }
}
