using SegmentationCreator.Seed_Data;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentationCreator.Process
{
    static public class PRod
    {
        static public ProcessData Run(string path, bool AFF = false)
        {

            ProcessData p = new ProcessData();

            FileInfo fi = new FileInfo(path);
            p.Name = fi.Name;


            Image<Hsv, Byte> im = new Image<Hsv, byte>(path);

            p.Im = im.Convert<Bgr, Byte>();

            Image<Gray, Byte> histG = new Image<Gray, Byte>(256, 256);

            Image<Gray, Byte> imG = new Image<Gray, Byte>(im.Size);

            if (AFF) CvInvoke.cvShowImage("im Origin", im.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());

            for (int i = 0; i < histG.Rows; i++)
                for (int j = 0; j < histG.Cols; j++)
                    histG[i, j] = new Gray(0);

            for (int i = 0; i < im.Rows; i++)
            {
                for (int j = 0; j < im.Cols; j++)
                {
                    // if (im[i, j].Hue > 30 && im[i, j].Hue < 90 && im[i, j].Value < 70 && im[i, j].Value > 8 && im[i, j].Hue <= (im[i, j].Satuation * (-0.25F) + 105))
                    if (im[i, j].Hue > 24 && im[i, j].Hue < 90 && im[i, j].Value > 8 && i > 450)
                    {
                        histG[(int)im[i, j].Hue, (int)im[i, j].Satuation] = new Gray(255);
                        imG[i, j] = new Gray(255);
                    }
                    else im[i, j] = new Hsv(0, 0, 0);
                }
            }

            ImageViewer.Show(imG, "img");

            if (AFF) CvInvoke.cvShowImage("imG threshold", imG.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());
            int size = 1;
            StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            imG = imG.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 5);

            size = 2;
            elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            imG = imG.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 2);

            ImageViewer.Show(imG, "img");

            List<View> views = ProcessUtils.FindContour(imG, 250);
            if (AFF) CvInvoke.cvShowImage("imG contour", imG.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());


            int nbPx = 0;
            for (int i = 0; i < im.Rows; i++)
                for (int j = 0; j < im.Cols; j++)
                {
                    if (imG[i, j].Intensity == 0)
                        im[i, j] = new Hsv(0, 0, 0);
                    else nbPx++;
                }

            if (AFF) CvInvoke.cvShowImage("imG threshold morpho", imG.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());


         
            Debug.WriteLine("views.Count = " + views.Count);

            p.NbRod = views.Count;


           imG = ProcessUtils.Skeleton(imG);

            if (AFF) CvInvoke.cvShowImage("imG ske", imG.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());

            nbPx = 0;
            for (int i = 0; i < im.Rows; i++)
                for (int j = 0; j < im.Cols; j++)
                    if (imG[i, j].Intensity != 0)
                        nbPx++;

            p.PxSkLeaves = nbPx;

             size = 4;
             elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
             imG = imG.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 4);

            if (AFF) CvInvoke.cvShowImage("imG ske morpho", imG.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());

            imG._Dilate(1);

            if (AFF) CvInvoke.cvShowImage("imG ske dilate", imG.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());


            List<View> viewsSke = ProcessUtils.FindContour(imG);

            
            p.SkeMin = viewsSke.Min(v => v.Contour.Area);
            p.SkeMax = viewsSke.Max(v => v.Contour.Area);
            p.SkeMoy = viewsSke.Average(v => v.Contour.Area);
            p.SkeSTD = viewsSke.StdDev(v => v.Contour.Area);

            //ProcessUtils.DrawLine(histG, new PointF(0, 105F), new System.Numerics.Vector2(1F, -0.25F));
            Debug.WriteLine("vSke : " + viewsSke.Count );

            Image<Gray, Byte> mask = new Image<Gray, byte>(imG.Width, imG.Height, new Gray(0));
     
            for (int i = 0; i < viewsSke.Count; i++)
            {
                im.Draw(viewsSke[i].Contour, new Hsv(50, 200, 200), 2);
            }
            

            p.ImAlgo = im.Convert<Bgr, Byte>();

            if (AFF) CvInvoke.cvShowImage("histG avec morpho", histG.Convert<Bgr, byte>());


            // im = im.Copy(imG).Add(imG.Mul(255).Not().Convert<Rgb, byte>());


            //im.Convert<Bgr, Byte>().Save(@"C:\Users\damien.delhay\Documents\Travail\Biostimulant\campagne1\13-03\algo.bmp");

            if (AFF) CvInvoke.cvShowImage("imG", imG.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, byte>());

            if (AFF) ImageViewer.Show(im.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), String.Format("im"));

            p.ProcessIsEnded = true;
            return p;
        }

        static private Image<Gray, Byte> deleteViewsTouchBorder(List<View> views, Image<Gray, Byte> imG)
        {

            Image<Gray, Byte> im = imG.Copy();
            for (int i = 0; i < views.Count; i++)
            {
                if (views[i].Roi.Bottom > imG.ROI.Bottom - 5)
                {
                    im.FillConvexPoly(views[i].Contour.ToArray(), new Gray(0));
                    //  Debug.WriteLine("views:"  +views[i].Roi.ToString());
                    //Debug.WriteLine("roi:" + imG.ROI.ToString());
                }

            }
            return im;
        }
    }
}
