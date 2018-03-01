using SegmentationCreator;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using SegmenTool.Modele;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;


namespace SegmenTool.Controle
{
    static public class SegmentationProcess
    {
        static public void Process(string segmentationPath, string inputPath, string outputPath, IProgress<ProgressData> progress)
        {

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            List<Segmentation> segmentations = LoadSegmentationsFromXMLFile(segmentationPath);
            if (segmentations == null || segmentations.Count < 1)
            {
                progress.Report(new ProgressData(0, 0, true, "Segmentation file is wrong."));
                Debug.WriteLine("Segmentations failed");
                return;
            }


            string[] fileEntries = Directory.GetFiles(inputPath, "*.bmp");
            Array.Sort(fileEntries, (s1, s2) => s1.CompareTo(s2));

            if (fileEntries.Length < 1)
            {
                progress.Report(new ProgressData(0, 0, true, "Input folder not contains BMP files."));
                Debug.WriteLine("Input folder not contains BMP files.");
                return;
            }

            int currentFile = 0;
            progress.Report(new ProgressData(currentFile, fileEntries.Length));


            foreach (string fileName in fileEntries)
            {
                try
                {
                    processFile(segmentations, fileName, outputPath);
                    currentFile++;
                    progress.Report(new ProgressData(currentFile, fileEntries.Length));
                }
                catch
                {

                    FileInfo fi = new FileInfo(fileName);
                    progress.Report(new ProgressData(0, 0, true, "Process of " + fi.Name + " failed"));

                    Debug.WriteLine("processFile failed");
                    //return;
                }
            }

            progress.Report(new ProgressData(true));
            return;

        }

        private static void processFile(List<Segmentation> segmentations, string fileName, string outputPath)
        {
            Image<Rgb, Byte> im = new Image<Rgb, Byte>(fileName);
            Image<Rgb, Byte> imOrigin = im.Copy();

            foreach (Segmentation segmentation in segmentations)
            {
                if (segmentation.segmentationType == Segmentation.SegmentationType.Soustraction)
                {
                    Image<Rgb, Byte> imBg = new Image<Rgb, byte>(segmentation.backgroundPath);
                    if (imBg == null) Debug.WriteLine("Background segmentation failed.");

                    Image<Gray, Byte> imGray = im.AbsDiff(imBg).Convert<Gray, Byte>().ThresholdBinary(new Gray(segmentation.backgroundThreshold), new Gray(255));

                    Parallel.For(0, im.Rows, r =>
                    {
                        Parallel.For(0, im.Cols, c =>
                        {
                            if (imGray[r, c].Intensity == 0)
                                im[r, c] = new Rgb(0, 0, 0);
                        });
                    });
                }
                else if (segmentation.segmentationType == Segmentation.SegmentationType.Seuillage)
                {
                    Image<Hsv, Byte> imHSV = im.Convert<Hsv, Byte>();
                    Image<Lab, Byte> imLab = im.Convert<Lab, Byte>();

                    if (!segmentation.inverseHue)
                        Parallel.For(0, imHSV.Rows, r =>
                        {
                            Parallel.For(0, imHSV.Cols, c =>
                            {
                                if (imLab[r, c].X <= segmentation.tHighLightness && (
                                imLab[r, c].X <= segmentation.tLowLightness ||
                                imHSV[r, c].Satuation < (segmentation.tSaturation * 255 / 100) ||
                                imHSV[r, c].Hue < segmentation.tHueLow ||
                                imHSV[r, c].Hue > segmentation.tHueHigh))
                                    im[r, c] = new Rgb(0, 0, 0);
                            });
                        });
                   else
                        Parallel.For(0, imHSV.Rows, r =>
                        {
                            Parallel.For(0, imHSV.Cols, c =>
                            {
                                if (imLab[r, c].X <= segmentation.tHighLightness && (
                                imLab[r, c].X <= segmentation.tLowLightness ||
                                imHSV[r, c].Satuation < (segmentation.tSaturation * 255 / 100) ||
                                (imHSV[r, c].Hue >= segmentation.tHueLow &&
                                imHSV[r, c].Hue <= segmentation.tHueHigh)))
                                    im[r, c] = new Rgb(0, 0, 0);
                            });
                        });
                }
                else if (segmentation.segmentationType == Segmentation.SegmentationType.Fermeture)
                {
                    Image<Gray, Byte> imGray = im.Convert<Gray, Byte>();
                    imGray = imGray.ThresholdBinary(new Gray(1), new Gray(255));

                    int size = segmentation.closingSize;
                    StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    imGray = imGray.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, segmentation.closingIteration);

                    im = imOrigin.Copy();

                    Parallel.For(0, im.Rows, r =>
                    {
                        Parallel.For(0, im.Cols, c =>
                        {
                            if (imGray[r, c].Intensity == 0)
                                im[r, c] = new Rgb(0, 0, 0);
                        });
                    });
                }
                else if (segmentation.segmentationType == Segmentation.SegmentationType.Ouverture)
                {
                    Image<Gray, Byte> imGray = im.Convert<Gray, Byte>();
                    imGray = imGray.ThresholdBinary(new Gray(1), new Gray(255));

                    int size = segmentation.openningSize;
                    StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                    imGray = imGray.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, segmentation.openningIteration);
                    im = imOrigin.Copy();

                    Parallel.For(0, im.Rows, r =>
                    {
                        Parallel.For(0, im.Cols, c =>
                        {
                            if (imGray[r, c].Intensity == 0)
                                im[r, c] = new Rgb(0, 0, 0);
                        });
                    });
                }

              //  ImageViewer.Show(im, "result");
            }
            Save(im, fileName, outputPath);
        }

        private static void Save(Image<Rgb, byte> im, string fileName, string outputPath)
        {
            FileInfo fi = new FileInfo(fileName);
            string outputFileName = outputPath + @"\" + Path.GetFileNameWithoutExtension(fi.Name);
            if (File.Exists(outputFileName + ".bmp"))
            {
                outputFileName = outputPath + @"\" + Path.GetFileNameWithoutExtension(fi.Name);
                bool fileSaved = false;
                for (int i = 1; i <= 1000; i++)
                {
                    if (!File.Exists(outputFileName + "-" + i + ".bmp"))
                    {
                        im.Convert<Bgr, Byte>().Save(outputFileName + "-" + i + ".bmp");
                        fileSaved = true;
                        break;
                    }
                }
                if (!fileSaved)
                    Debug.WriteLine("File not saved");
            }
            else
            {
                im.Convert<Bgr, Byte>().Save(outputFileName + ".bmp");
            }
        }

        static private List<Segmentation> LoadSegmentationsFromXMLFile(string segmentationPath)
        {
            List<Segmentation> segmentations = new List<Segmentation>();

            XmlSerializer xs = new XmlSerializer(typeof(List<Segmentation>));
            try
            {
                using (StreamReader rd = new StreamReader(segmentationPath))
                {
                    segmentations.AddRange(xs.Deserialize(rd) as List<Segmentation>);
                }
            }
            catch
            {
                return null;
            }
            return segmentations;
        }



    }
}
