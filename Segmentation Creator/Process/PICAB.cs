using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Diagnostics;
using System.Drawing;
using SegmentationCreator.Seed_Data;
using System.IO;


namespace SegmentationCreator.Process
{
    public static class PICAB
    {
        public static bool SAVE = true;

        static public ProcessData Run(string path, bool AFF = false)
        {
            ProcessData p = new ProcessData();

            FileInfo fi = new FileInfo(path);
            p.Name = fi.Name;

            string save_path = fi.Directory + @"\" + Path.GetFileNameWithoutExtension(fi.Name) + "-algo.bmp";

            Image<Hsv, Byte> im = new Image<Hsv, byte>(path);

            p.Im = im.Convert<Bgr, Byte>();

            Image<Gray, Byte> histG = new Image<Gray, Byte>(256, 256);

            Image<Gray, Byte> imG = new Image<Gray, Byte>(im.Size);

          //  ImageViewer.Show(im);

            if (AFF) CvInvoke.cvShowImage("im Origin", im.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());
            

         /*  for (int i = 0; i < histG.Rows; i++)
                for (int j = 0; j < histG.Cols; j++)
                    histG[i, j] = new Gray(0);*/

            for (int i = 0; i < im.Rows; i++)
            {
                for (int j = 0; j < im.Cols; j++)
                {
                    // if (im[i, j].Hue > 30 && im[i, j].Hue < 90 && im[i, j].Value < 70 && im[i, j].Value > 8 && im[i, j].Hue <= (im[i, j].Satuation * (-0.25F) + 105))
                    if (im[i, j].Value > 20 && im[i, j].Satuation > 60)
                    //if (im[i, j].Value > 58 && im[i, j].Satuation > 150)
                    {
                        histG[(int)im[i, j].Hue, (int)im[i, j].Satuation] = new Gray(255);
                        imG[i, j] = new Gray(255);
                    }
                    else im[i, j] = new Hsv(0, 0, 0);
                }
            }


            //  ImageViewer.Show(imG, "avt");

            // bio
            int size = 3;
            StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            imG = imG.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 2);

            size = 4;
            elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            imG = imG.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 3);

          //     ImageViewer.Show(imG, "avt");

            im.Mask(imG);

            ImageViewer.Show(im, "final");


           if(SAVE) im.Convert<Bgr, Byte>().Save(save_path);

            /*   int nbPx = 0;
               for (int i = 0; i < im.Rows; i++)
                   for (int j = 0; j < im.Cols; j++)
                   {
                       if (imG[i, j].Intensity == 0)
                           im[i, j] = new Hsv(0, 0, 0);
                       else nbPx++;
                   }

               p.PxLeaves = nbPx;
               // ImageViewer.Show(imG, "apres");

               if (SAVE) imG.Convert<Bgr, byte>().Save(@"E:\Biostimulant\imGray.bmp");


               //  List<View> views = ProcessUtils.FindContour(imG);
               List<View> views = ProcessUtils.FindContour(imG);

               if (views.Count > 0)
               {
                   p.LeavesAreaMin = views.Min(v => v.Contour.Area);
                   p.LeavesAreaMax = views.Max(v => v.Contour.Area);
                   p.LeavesAreaMoy = views.Average(v => v.Contour.Area);
                   p.LeavesAreaStd = views.StdDev(v => v.Contour.Area);

                   p.LeavesLengthCalipMin = views.Min(v => v.LengthCalip);
                   p.LeavesLengthCalipMax = views.Max(v => v.LengthCalip);
                   p.LeavesLengthCalipMoy = views.Average(v => v.LengthCalip);
                   p.LeavesLengthCalipStd = views.StdDev(v => v.LengthCalip);

                   p.LeavesWidthCalipMin = views.Min(v => v.WidthCalip);
                   p.LeavesWidthCalipMax = views.Max(v => v.WidthCalip);
                   p.LeavesWidthCalipMoy = views.Average(v => v.WidthCalip);
                   p.LeavesWidthCalipStd = views.StdDev(v => v.WidthCalip);
               }


               #region Distance Map
               Image<Gray, float> distT = new Image<Gray, float>(imG.Size);
               IntPtr lb = new IntPtr();
               CvInvoke.cvDistTransform(imG, distT, Emgu.CV.CvEnum.DIST_TYPE.CV_DIST_L2, 5, null, lb);
               Image<Gray, byte> distAFF = distT.Convert<Gray, Byte>();


               //distance map data
               List<float> distMapData = distT.Data.OfType<float>().ToList();

               distMapData.RemoveAll(i => i == 0);

               p.DistMapMin = distMapData.Min();
               p.DistMapMax = distMapData.Max();
               p.DistMapMoy = distMapData.Average();
               p.DistMapSTD = distMapData.StdDev();

               distAFF.Convert<Bgr, byte>().Save(@"E:\Biostimulant\distAFF.bmp");
               #endregion

               #region ZhangSuen Skeleton !

               Image<Gray, byte> SuenSkelet = ZhangSuen.Skeleton(imG);
               // if (AFF) ImageViewer.Show(imG, String.Format("imG"));
               //if (AFF) ImageViewer.Show(SuenSkelet, "SuenSkelet");
               if (SAVE) SuenSkelet.Convert<Bgr, byte>().Save(@"E:\Biostimulant\SuenSkelet.bmp");

               #endregion

               #region suen dist map
               Image<Gray, float> suenDistMap = distT.Copy();
               suenDistMap = ProcessUtils.Mask(suenDistMap, SuenSkelet);
               // if (AFF) ImageViewer.Show(suenDistMap, "imEp SuenSkelet");
               if (SAVE) suenDistMap.Convert<Bgr, byte>().Save(@"E:\Biostimulant\DistMapSuenSkelet.bmp");


               List<float> listSuenDistMap = suenDistMap.Data.OfType<float>().ToList();

               listSuenDistMap.RemoveAll(i => i == 0);

               p.SuenDistMapMin = listSuenDistMap.Min();
               p.SuenDistMapMax = listSuenDistMap.Max();
               p.SuenDistMapMoy = listSuenDistMap.Average();
               p.SuenDistMapSTD = listSuenDistMap.StdDev();

               #endregion

               imG = SuenSkelet.Copy();
               nbPx = 0;
               for (int i = 0; i < im.Rows; i++)
                   for (int j = 0; j < im.Cols; j++)
                       if (imG[i, j].Intensity != 0)
                           nbPx++;
               p.PxSkLeaves = nbPx;


               /*imG = ProcessUtils.Skeleton(imG);

               nbPx = 0;
               for (int i = 0; i < im.Rows; i++)
                   for (int j = 0; j < im.Cols; j++)
                       if (imG[i, j].Intensity != 0)
                           nbPx++;

               p.PxSkLeaves = nbPx;

               size = 3;
               elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
               imG = imG.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 3);*/
            /*
            List<View> viewsSke = ProcessUtils.FindContour(imG);

            p.SkeMin = viewsSke.Min(v => v.Contour.Area);
            p.SkeMax = viewsSke.Max(v => v.Contour.Area);
            p.SkeMoy = viewsSke.Average(v => v.Contour.Area);
            p.SkeSTD = viewsSke.StdDev(v => v.Contour.Area);

            //ProcessUtils.DrawLine(histG, new PointF(0, 105F), new System.Numerics.Vector2(1F, -0.25F));
            Debug.WriteLine("vSke : " + viewsSke.Count + ";v : " + views.Count);

            im.Convert<Bgr, Byte>().Save(@"C:\Users\damien.delhay\Documents\Travail\PNMOV\IMG_0186-algo.bmp");

            Image<Gray, Byte> mask = new Image<Gray, byte>(imG.Width, imG.Height, new Gray(0));
            for (int i = 0; i < views.Count; i++)
            {
                im.Draw(views[i].Contour, new Hsv(50, 200, 200), 2);
            }

            for (int i = 0; i < viewsSke.Count; i++)
            {
                im.Draw(viewsSke[i].Contour, new Hsv(50, 200, 200), 2);
            }

            p.ImAlgo = im.Convert<Bgr, Byte>();
            // p.ImAlgo = suenDistMap.Convert<Bgr, Byte>();

            if (AFF) CvInvoke.cvShowImage("histG avec morpho", histG.Convert<Bgr, byte>());

            // im = im.Copy(imG).Add(imG.Mul(255).Not().Convert<Rgb, byte>());


            //  im.Convert<Bgr, Byte>().Save(@"C:\Users\damien.delhay\Documents\Travail\Biostimulant\campagne1\16-03\F5_algo.bmp");

            if (AFF) ImageViewer.Show(imG.Convert<Bgr, byte>(), "imG");
            if (SAVE) imG.Convert<Bgr, byte>().Save(@"E:\Biostimulant\imG.bmp");

            if (AFF) ImageViewer.Show(im.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("im"));
            if (SAVE) im.Convert<Bgr, byte>().Save(@"E:\Biostimulant\im.bmp");

            p.ProcessIsEnded = true;*/
            return p;
        }




        static int CountNoBlackPixel(Image<Gray, Byte> im)
        {
            return 0;
        }

        /// obsolete
        static public ProcessData Run2(string path, bool AFF = false)
        {
            ProcessData p = new ProcessData();

            FileInfo fi = new FileInfo(path);
            p.Name = fi.Name;


            Image<Hsv, Byte> im = new Image<Hsv, byte>(path);

            p.Im = im.Convert<Bgr, Byte>();

            Image<Gray, Byte> histHS = new Image<Gray, Byte>(256, 256);
            Image<Gray, double> histHSD = new Image<Gray, double>(256, 256);
            Image<Hsv, double> histHSColor = new Image<Hsv, double>(256, 256);

            Image<Gray, Byte> histHV = new Image<Gray, Byte>(256, 256);
            Image<Gray, Byte> histSV = new Image<Gray, Byte>(256, 256);

            Image<Gray, Byte> imG = new Image<Gray, Byte>(im.Size);

            if (AFF) CvInvoke.cvShowImage("im Origin", im.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());

            for (int i = 0; i < histHS.Rows; i++)
                for (int j = 0; j < histHS.Cols; j++)
                {
                    histHS[i, j] = new Gray(0);
                    histHV[i, j] = new Gray(0);
                    histSV[i, j] = new Gray(0);
                    histHSD[i, j] = new Gray(0);
                }

            double max = 0;
            for (int i = 0; i < im.Rows; i++)
            {
                for (int j = 0; j < im.Cols; j++)
                {

                    if (im[i, j].Hue > 30 && im[i, j].Hue < 90 && im[i, j].Value > 8 && im[i, j].Satuation > 30)
                    {
                        histHS[(int)im[i, j].Hue, (int)im[i, j].Satuation] = new Gray(255);
                        histHSD[(int)im[i, j].Hue, (int)im[i, j].Satuation] = new Gray(histHSD[(int)im[i, j].Hue, (int)im[i, j].Satuation].Intensity + 1);
                        if (histHSD[(int)im[i, j].Hue, (int)im[i, j].Satuation].Intensity > max)
                            max = histHSD[(int)im[i, j].Hue, (int)im[i, j].Satuation].Intensity;


                        histHV[(int)im[i, j].Hue, (int)im[i, j].Value] = new Gray(255);
                        histSV[(int)im[i, j].Satuation, (int)im[i, j].Value] = new Gray(255);

                        imG[i, j] = new Gray(255);
                    }
                    else im[i, j] = new Hsv(0, 0, 0);
                }
            }

            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    histHSColor[i, j] = new Hsv(120 - (Math.Sqrt(histHSD[i, j].Intensity) * 120 / Math.Sqrt(max)), 150, 150);
                    // histHSColor[i, j] = new Hsv(360-(((j * 180 / 256)+130)%180), 200, 200);

                }
            }

            int size = 1;
            StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            imG = imG.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 5);

            size = 2;
            elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            imG = imG.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 2);

            int nbPx = 0;
            for (int i = 0; i < im.Rows; i++)
                for (int j = 0; j < im.Cols; j++)
                {
                    if (imG[i, j].Intensity == 0)
                        im[i, j] = new Hsv(0, 0, 0);
                    else nbPx++;

                }

            p.PxLeaves = nbPx;

            imG = ProcessUtils.Skeleton(imG);

            nbPx = 0;
            for (int i = 0; i < im.Rows; i++)
                for (int j = 0; j < im.Cols; j++)
                    if (imG[i, j].Intensity != 0)
                        nbPx++;

            p.PxSkLeaves = nbPx;


            size = 3;
            elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            imG = imG.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 3);

            List<View> viewsSke = ProcessUtils.FindContour(imG);

            p.SkeMin = viewsSke.Min(v => v.Contour.Area);
            p.SkeMax = viewsSke.Max(v => v.Contour.Area);
            p.SkeMoy = viewsSke.Average(v => v.Contour.Area);
            p.SkeSTD = viewsSke.StdDev(v => v.Contour.Area);

            //ProcessUtils.DrawLine(histG, new PointF(0, 105F), new System.Numerics.Vector2(1F, -0.25F));
            Debug.WriteLine("vSke : " + viewsSke.Count);

            Image<Gray, Byte> mask = new Image<Gray, byte>(imG.Width, imG.Height, new Gray(0));

            for (int i = 0; i < viewsSke.Count; i++)
            {
                im.Draw(viewsSke[i].Contour, new Hsv(50, 200, 200), 2);
            }


            p.ImAlgo = im.Convert<Bgr, Byte>();

            if (AFF) CvInvoke.cvShowImage("histHS", histHS.Convert<Bgr, byte>());
            if (AFF) CvInvoke.cvShowImage("histHSD", histHSD.Convert<Bgr, byte>());
            if (AFF) CvInvoke.cvShowImage("histHSColor", histHSColor.Convert<Bgr, byte>());
            if (AFF) CvInvoke.cvShowImage("histHV", histHV.Convert<Bgr, byte>());
            if (AFF) CvInvoke.cvShowImage("histSV", histSV.Convert<Bgr, byte>());


            // im = im.Copy(imG).Add(imG.Mul(255).Not().Convert<Rgb, byte>());


            //  im.Convert<Bgr, Byte>().Save(@"C:\Users\damien.delhay\Documents\Travail\Biostimulant\campagne1\16-03\F5_algo.bmp");

            if (AFF) CvInvoke.cvShowImage("imG", imG.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());

            if (AFF) ImageViewer.Show(im.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("im"));

            p.ProcessIsEnded = true;
            return p;
        }

    }
}
