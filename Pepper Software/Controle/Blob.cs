using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using pepperSoft.Modele;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using VisartLib;
using VisartLib.ColorExtension;
using VisartLib.Data;

namespace pepperSoft.Controle
{
    static public class Blob
    {

        static bool viewer = false;

        static public void BlobCount(this Image<Gray, Byte> imG, out Image<Rgb, Byte> maskCount, out List<Shape> shapes)
        {

            maskCount = new Image<Rgb, byte>(imG.Size);
            shapes = new List<Shape>();
            try
            {
                //  dataGrid.Items.Clear();


                Image<Gray, Byte> imDist = imG.DistanceMap().Convert<Gray, Byte>();



                int min = 30;
                imDist = imDist.ThresholdBinary(new Gray(min), new Gray(255));

                shapes = imDist.FindShapes();

                //correction des multiples blobs
                shapes = MultiBlobDetection(shapes, imG.Size);

                Debug.WriteLine("Number of shapes is " + shapes.Count);

                shapes.Sort((x, y) => ((double)x.Roi.X).CompareTo(y.Roi.X));
               
                //test _GIT

                for (int i = 0; i < shapes.Count; i++)
                {
                    float ellipseArea = (shapes[i].Ellipse.MCvBox2D.size.Height / 2) * (shapes[i].Ellipse.MCvBox2D.size.Width / 2) * (float)Math.PI;
                    float contourArea = (float)shapes[i].Contour.Area;


                    System.Drawing.SizeF size = shapes[i].Ellipse.MCvBox2D.size;
                    size.Height += min + 6;
                    size.Width += min + 6;

                    Ellipse ell = new Ellipse(shapes[i].Ellipse.MCvBox2D.center, size, shapes[i].Ellipse.MCvBox2D.angle);


                    maskCount.Draw(shapes[i].Contour, new Rgb(200, 20, 20), 1);
                    maskCount.Draw(shapes[i].Ellipse, new Rgb(255, 40, 50), 1);
                    maskCount.Draw(ell, new Rgb(255, 200, 200), 2);

                    shapes[i].Ellipse = ell;

                    MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX_SMALL, 2, 2);
                    maskCount.Draw(i.ToString(), ref font, new System.Drawing.Point(shapes[i].Roi.X - 5, shapes[i].Roi.Y - 5), new Rgb(200, 100, 50));
                }

            }
            catch { }
        }

        static public void BlobCount(this Image<Gray, Byte> imG, out Image<Rgb, Byte> maskCount, out List<Shape> shapes, IProgress<ProgressData> p)
        {

            maskCount = new Image<Rgb, byte>(imG.Size);
            shapes = new List<Shape>();
            try
            {
                //  dataGrid.Items.Clear();


                Image<Gray, Byte> imDist = imG.DistanceMap().Convert<Gray, Byte>();


                int min = 2;
                imDist = imDist.ThresholdBinary(new Gray(min), new Gray(255));

                shapes = imDist.FindShapes(1000);

                p.Report(new ProgressData(1, 3, null, false, "Find Shape finished"));

                //correction des multiples blobs
                //  shapes = MultiBlobDetection(shapes, imG.Size);
                p.Report(new ProgressData(2, 3, null, false, "MultiBlobDetection finished"));

                Debug.WriteLine("Number of shapes is " + shapes.Count);

                //  shapes.Sort((x, y) => ((double)x.Roi.X).CompareTo(y.Roi.X));


                for (int i = 0; i < shapes.Count; i++)
                {
                    float ellipseArea = (shapes[i].Ellipse.MCvBox2D.size.Height / 2) * (shapes[i].Ellipse.MCvBox2D.size.Width / 2) * (float)Math.PI;
                    float contourArea = (float)shapes[i].Contour.Area;


                    /* System.Drawing.SizeF size = shapes[i].Ellipse.MCvBox2D.size;
                     size.Height += min + 6;
                     size.Width += min + 6;

                     Ellipse ell = new Ellipse(shapes[i].Ellipse.MCvBox2D.center, size, shapes[i].Ellipse.MCvBox2D.angle);
                     */
                    Ellipse ell = shapes[i].Ellipse;

                    maskCount.Draw(shapes[i].Contour, new Rgb(200, 20, 20), 1);
                    maskCount.Draw(shapes[i].Ellipse, new Rgb(255, 40, 50), 1);
                    maskCount.Draw(ell, new Rgb(255, 200, 200), 2);

                    // shapes[i].Ellipse = ell;

                    MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX_SMALL, 2, 2);
                    maskCount.Draw(i.ToString(), ref font, new System.Drawing.Point(shapes[i].Roi.X - 5, shapes[i].Roi.Y - 5), new Rgb(200, 100, 50));
                }

                p.Report(new ProgressData(3, 3, null, false, "Drawing finished"));

            }
            catch { }
        }

        static public void BlobGlobalAnalyse(Item item, IProgress<ProgressData> p)
        {

            Image<Rgb, Byte> imOrigin = item.images.Origin.Convert<Rgb, Byte>().Copy();

           // ImageViewer.Show(imOrigin, "imOrigin");
            Rectangle[] rois = new Rectangle[1];
            rois[0] = new Rectangle(0, 0, imOrigin.Width, imOrigin.Height);

            item.images.Mask = imOrigin.Convert<Gray, Byte>().ThresholdBinary(new Gray(1), new Gray(255)).Convert<Bgr, Byte>();

            Image<Gray, Byte> threshIm = imOrigin.Convert<Gray, Byte>().ThresholdBinary(new Gray(1), new Gray(255)); ;


            //ImageViewer.Show(threshIm, "thresim");
            item.shapes = threshIm.FindShapes(500);
            List<Shape> shapes = item.shapes;


            if (shapes.Count < 3)
            {
                Debug.WriteLine("err 2 : Views.Count equals to 0 : return ");
                return;
            }
            else if (shapes.Count != 3)
            {
                Debug.WriteLine("err 1 : Views.Count not equals to contourMax : " + shapes.Count);
                //return;
                #region sorting views with 2 bigger area and left to right order
                shapes.Sort((x, y) => ((double)y.Contour.Area).CompareTo((double)x.Contour.Area)); // bigger
                #endregion

                int maxContour = 3;
                if (shapes.Count > maxContour)
                {
                    for (int i = maxContour; i < shapes.Count; i++)
                    {
                        // imOrigin.FillConvexPoly(views[i].Contour.ToArray(), new Rgb(0, 0, 0));
                        threshIm.FillConvexPoly(shapes[i].Contour.ToArray(), new Gray(0));
                    }
                    shapes.RemoveRange(maxContour, shapes.Count - maxContour);
                }
            }

            #region sorting views with left to right order
            shapes.Sort((x, y) => ((double)y.RectMinArea.center.X).CompareTo((double)x.RectMinArea.center.X)); // bigger
            #endregion

            double[] tempHeight, tempWidth;
            tempHeight = new double[shapes.Count];
            tempWidth = new double[shapes.Count];


            for (int i = 0; i < shapes.Count; i++)
            {
                tempHeight[i] = shapes[i].LengthRotatingCaliper();
                tempWidth[i] = shapes[i].WidthRotatingCaliper();
            }

            item.dimension1.Height = (float)tempHeight[0];
            item.dimension1.Area = (float)shapes[0].Contour.Area;
            item.dimension1.Width = (float)tempWidth[0];

            item.dimension2.Height = (float)tempHeight[1];
            item.dimension2.Area = (float)shapes[1].Contour.Area;
            item.dimension2.Width = (float)tempWidth[1];

            item.dimension3.Height = (float)tempHeight[2];
            item.dimension3.Area = (float)shapes[2].Contour.Area;
            item.dimension3.Width = (float)tempWidth[2];

            int nbPxSeed = 0, nbPxTreated = 0; // 4 s

            // multi threading part
            nbPxSeed = 0;
            Parallel.For(0, imOrigin.Rows, r =>
            {
                Parallel.For(0, imOrigin.Cols, c =>
                {
                    if (threshIm[r, c].Intensity == 0)
                        imOrigin[r, c] = new Rgb(0, 0, 0);
                    else Interlocked.Increment(ref nbPxSeed);
                });
            });

            Image<Hsv, Byte> imHSV = imOrigin.Convert<Hsv, Byte>();

            List<double> featKMeans;


            Image<Lab, Byte> imKmeans = SegmentationCreator.Process.HaploidUtils.Kmeans(imOrigin.Convert<Lab, Byte>(), threshIm, out featKMeans, 1E-06F, 4);
            item.images.Kmean = imKmeans.Convert<Bgr, Byte>();

            //ImageViewer.Show(imKmeans, "imKmeans");

            Lab[] labs = SearchColor(imKmeans, 4);

            for (int i = 0; i < labs.Count(); i++)
            {
                Debug.WriteLine(labs[i].ToString() + "|" + labs[i].ToRGB().ToString());
            }

            Rgb color = labs[1].ToRGB();
            item.statsColor.Color1 = color;
            item.statsColor.Color1Str = "#" + ((int)color.Red).ToString("X2") + ((int)color.Green).ToString("X2") + ((int)color.Blue).ToString("X2");

            color = labs[2].ToRGB();
            item.statsColor.Color2 = color;
            item.statsColor.Color2Str = "#" + ((int)color.Red).ToString("X2") + ((int)color.Green).ToString("X2") + ((int)color.Blue).ToString("X2");


            color = labs[3].ToRGB();
            item.statsColor.Color3 = color;
            item.statsColor.Color3Str = "#" + ((int)color.Red).ToString("X2") + ((int)color.Green).ToString("X2") + ((int)color.Blue).ToString("X2");



            // p.ImAlgo = imHSV.Convert<Bgr, Byte>();


            //  p.ProcessIsEnded = true;
            return;
        }

        static public void PepperBlob(Item item, bool AFF = false)
        {

            Image<Rgb, Byte> imOrigin = item.images.Origin.Convert<Rgb, Byte>().Copy();


            Rectangle[] rois = new Rectangle[1];
            rois[0] = new Rectangle(0, 0, imOrigin.Width, imOrigin.Height);

            Image<Gray, Byte> threshIm = item.images.Mask.Convert<Gray, Byte>();

            List<Shape> shapes = item.shapes;


            if (shapes.Count == 0)
            {
                Debug.WriteLine("err 2 : Views.Count equals to 0 : return ");
                return;
            }
            else if (shapes.Count != 3)
            {
                Debug.WriteLine("err 1 : Views.Count not equals to contourMax : " + shapes.Count);
                //return;
                #region sorting views with 2 bigger area and left to right order
                shapes.Sort((x, y) => ((double)y.Contour.Area).CompareTo((double)x.Contour.Area)); // bigger
                #endregion

                int maxContour = 3;
                if (shapes.Count > maxContour)
                {
                    for (int i = maxContour; i < shapes.Count; i++)
                    {
                        // imOrigin.FillConvexPoly(views[i].Contour.ToArray(), new Rgb(0, 0, 0));
                        threshIm.FillConvexPoly(shapes[i].Contour.ToArray(), new Gray(0));
                    }
                    shapes.RemoveRange(maxContour, shapes.Count - maxContour);
                }
            }

            #region sorting views with left to right order
            //   shapes.Sort((x, y) => ((double)y.RectMinArea.center.X).CompareTo((double)x.RectMinArea.center.X)); // bigger
            #endregion




            /*  double[] tempHeight, tempWidth;
              tempHeight = new double[shapes.Count];
              tempWidth = new double[shapes.Count];


              for (int i = 0; i < shapes.Count; i++)
              {
                  tempHeight[i] = shapes[i].LengthRotatingCaliper();
                  tempWidth[i] = shapes[i].WidthRotatingCaliper();  
              }



              p.Height1 = tempHeight[0];
              p.Area1 = shapes[0].Contour.Area;
              p.Width1 = tempWidth[0];

              p.Height2 = tempHeight[1];
              p.Area2 = shapes[1].Contour.Area;
              p.Width2 = tempWidth[1];

              p.Height3 = tempHeight[2];
              p.Area3 = shapes[2].Contour.Area;
              p.Width3 = tempWidth[2];*/

            int nbPxSeed = 0, nbPxTreated = 0; // 4 s

            // multi threading part
            nbPxSeed = 0;
            Parallel.For(0, imOrigin.Rows, r =>
            {
                Parallel.For(0, imOrigin.Cols, c =>
                {
                    if (threshIm[r, c].Intensity == 0)
                        imOrigin[r, c] = new Rgb(0, 0, 0);
                    else Interlocked.Increment(ref nbPxSeed);
                });
            });


            if (AFF) CvInvoke.cvShowImage("imOrigin", imOrigin.Convert<Bgr, Byte>());
            if (AFF) CvInvoke.cvShowImage("threshIm", threshIm.Convert<Bgr, Byte>());

            Image<Hsv, Byte> imHSV = imOrigin.Convert<Hsv, Byte>();


            List<double> featKMeans;


            Image<Lab, Byte> imKmeans = SegmentationCreator.Process.HaploidUtils.Kmeans(imOrigin.Convert<Lab, Byte>(), threshIm, out featKMeans, 4);
            item.images.Kmean = imKmeans.Convert<Bgr, Byte>();

            // CvInvoke.cvShowImage("imOrginLAB KMEANS", imKmeans.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Convert<Bgr, Byte>());

            Lab[] labs = SearchColor(imKmeans, 4);

            for (int i = 0; i < labs.Count(); i++)
            {
                Debug.WriteLine(labs[i].ToString() + "|" + labs[i].ToRGB().ToString());
            }

            Rgb color = labs[1].ToRGB();
            item.statsColor.Color1 = color;
            item.statsColor.Color1Str = "#" + ((int)color.Red).ToString("X2") + ((int)color.Green).ToString("X2") + ((int)color.Blue).ToString("X2");

            color = labs[2].ToRGB();
            item.statsColor.Color2 = color;
            item.statsColor.Color2Str = "#" + ((int)color.Red).ToString("X2") + ((int)color.Green).ToString("X2") + ((int)color.Blue).ToString("X2");


            color = labs[3].ToRGB();
            item.statsColor.Color3 = color;
            item.statsColor.Color3Str = "#" + ((int)color.Red).ToString("X2") + ((int)color.Green).ToString("X2") + ((int)color.Blue).ToString("X2");


            if (AFF) CvInvoke.cvShowImage("imHSV", imHSV.Convert<Bgr, Byte>());

            // p.ImAlgo = imHSV.Convert<Bgr, Byte>();


            //  p.ProcessIsEnded = true;
            return;
        }

        static Lab[] SearchColor(Image<Lab, Byte> im, int nbColor)
        {


            int nbColorsFound = 0;
            Lab[] labs = new Lab[nbColor];
            for (int rw = 0; rw < im.Rows; rw++)
            {
                for (int col = 0; col < im.Cols; col++)
                {
                    if (nbColorsFound < nbColor)
                    {
                        bool isInTab = false;
                        for (int i = 0; i < nbColorsFound; i++)
                        {
                            if (labs[i].Equals(im[rw, col]))
                            {
                                isInTab = true;
                            }
                        }
                        if (!isInTab)
                        {
                            labs[nbColorsFound] = im[rw, col];
                            nbColorsFound++;
                        }
                    }
                    else break;
                }
            }
            return labs;
        }


        static public void BlobAnalyse(this Image<Gray, Byte> imG, List<Shape> shapes, Item item, out Image<Rgb, Byte> maskRug, IProgress<ProgressData> p)
        {

            maskRug = new Image<Rgb, byte>(imG.Size);

            int offset = 0;
            try
            {
                Image<Gray, Byte> imShapes = imG.Copy();

                Image<Gray, float> sobel1 = imShapes.Sobel(0, 1, 3);
                sobel1 = sobel1.Add(imShapes.Sobel(1, 0, 3)).AbsDiff(new Gray(0));

                if (viewer) ImageViewer.Show(imShapes.Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_AREA), "imShapes");

                for (int i = 0; i < shapes.Count; i++)
                {

                    try
                    {
                        Image<Gray, Byte> mask = new Image<Gray, byte>(imG.Size);


                        System.Drawing.SizeF size = shapes[i].Ellipse.MCvBox2D.size;


                        mask = shapes[i].BinaryImage(imG.Size);

                        /* Ellipse ell = new Ellipse(shapes[i].Ellipse.MCvBox2D.center, size, shapes[i].Ellipse.MCvBox2D.angle);

                         mask.Draw(ell, new Gray(255), -1);*/

                        //  mask.Draw(shapes[i].Ellipse, new Gray(255), -1);

                        if (viewer) ImageViewer.Show(mask.Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_AREA), "mask");


                        Image<Gray, byte> imgGray = imG.Copy();
                        imgGray.Mask(mask);

                        Rectangle roi = new Rectangle(shapes[i].Roi.Left - offset, shapes[i].Roi.Top - offset, shapes[i].Roi.Width + offset * 2, shapes[i].Roi.Height + offset * 2);

                        Image<Gray, Byte> imT = imgGray.Copy(roi);

                        // ImageViewer.Show(mask, "mask");


                        List<float> imVal = new List<float>();

                        Image<Gray, float> sobel = sobel1.Mask(mask).Copy();
                        if (viewer) ImageViewer.Show(sobel.Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_AREA), "sobel");
                        // sobel1 = sobel1.Copy(shapes[i].Roi);

                        //  ImageViewer.Show(sobel, "sobel");


                        List<float> val = new List<float>();
                        int rws = sobel.Rows, cls = sobel.Cols;





                        for (int r = shapes[i].Roi.Top - offset; r < shapes[i].Roi.Bottom + offset; r++)
                        {
                            for (int c = shapes[i].Roi.Left - offset; c < shapes[i].Roi.Right + offset; c++)
                            {
                                if (sobel[r, c].Intensity != 0)
                                    val.Add((float)sobel[r, c].Intensity);
                                if (imgGray[r, c].Intensity != 0)
                                    imVal.Add((float)imgGray[r, c].Intensity);
                            }
                        }

                        float avg = 0;
                        float max = 0;
                        float mini = 0;
                        float std = 0;
                        try
                        {
                            avg = imVal.Average();
                            max = imVal.Max();
                            mini = imVal.Min();
                            std = imVal.StdDev();
                        }
                        catch
                        {

                        }



                        float avgSob = val.Average();
                        float maxSob = val.Max();
                        float minSob = val.Min();
                        float stdSob = val.StdDev();
                        float width = (float)shapes[i].WidthRotatingCaliper();
                        float height = (float)shapes[i].LengthMax;
                        float area = (float)shapes[i].Contour.Area;

                        float rugosite = (-0.136287F * avg + 0.2286872F * avgSob + 0.19534F * std);


                        float rugMod = rugosite < -6 ? -6 : rugosite;
                        rugMod = rugMod > 1 ? 1 : rugMod;

                        rugMod += 6;

                        Hls rugositeColor = new Hls(rugMod * 180 / 7, 120, 220);


                        Image<Rgb, Byte> imTemp = new Image<Rgb, byte>(maskRug.Size);

                        for (int r = shapes[i].Roi.Top - offset; r < shapes[i].Roi.Bottom + offset; r++)
                        {
                            for (int c = shapes[i].Roi.Left - offset; c < shapes[i].Roi.Right + offset; c++)
                            {
                                if (imgGray[r, c].Intensity != 0)
                                    imTemp[r, c] = rugositeColor.ToRGB();
                            }
                        }

                        // ImageViewer.Show(imTemp.Resize(0.25, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR), "imTemp");
                        maskRug = maskRug.Add(imTemp);


                        Bitmap bitmap = imT.Resize(80, 80, Emgu.CV.CvEnum.INTER.CV_INTER_LANCZOS4).ToBitmap();

                        //ImageViewer.Show(imT, "im");



                        //System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                        //image.Source = bitmapImage;



                        //  item.ID = i;
                        item.rugosite = rugosite;
                        item.avg = avg;
                        item.max = max;
                        item.min = mini;
                        item.std = std;
                        item.avgSob = avgSob;
                        item.maxSob = maxSob;
                        item.minSob = minSob;
                        item.stdSob = stdSob;
                        item.area = area;
                        item.height = height;
                        item.width = width;
                        item.roundness = size.Height / size.Width;
                        item.ellHeight = size.Height;
                        item.ellWidth = size.Width;
                        //  item.bitmap = bitmap,

                        /* contrast = haralicks[0],
                         correlation = haralicks[1],
                         variance = haralicks[2],
                         inverseDiffMoment = haralicks[3],
                         sumAvg = haralicks[4],
                         sumVar = haralicks[5],
                         sumEntropy = haralicks[6],
                         entropy = haralicks[7],
                         diffVar = haralicks[8],
                         diffEntropy = haralicks[9],
                         firstInfoMeasure = haralicks[10],
                         secondInfoMeasure = haralicks[11]*/



                    }
                    catch { Debug.WriteLine("error in for of blob analyse"); }

                    p.Report(new ProgressData(i, shapes.Count, maskRug));

                }
            }
            catch { Debug.WriteLine("error in blob analyse"); }
        }


        static public List<Shape> MultiBlobDetection(List<Shape> shapes, System.Drawing.Size size)
        {


            for (int i = 0; i < shapes.Count; i++)
            {

                float ellipseArea = (shapes[i].Ellipse.MCvBox2D.size.Height / 2) * (shapes[i].Ellipse.MCvBox2D.size.Width / 2) * (float)Math.PI;
                float contourArea = (float)shapes[i].Contour.Area;



                if ((ellipseArea * Math.Exp(1 - contourArea / ellipseArea) > 10000))
                {

                    // Image<Gray, Byte> mask = shapes[i].BinaryImage(size);
                    // ImageViewer.Show(mask, "mask");

                    List<Shape> addShape = ForceSepareBlob(shapes[i], size);

                    addShape = MultiBlobDetection(addShape, size);


                    shapes.Remove(shapes[i]);
                    i--;

                    shapes.AddRange(addShape);
                }
            }

            return shapes;
        }


        static public List<Shape> ForceSepareBlob(Shape shape, System.Drawing.Size imSize)
        {

            List<Shape> shapes;

            int erode = 2;
            do
            {
                Image<Gray, Byte> imShape = shape.BinaryImage(imSize);

                imShape = imShape.Erode(erode);
                erode += 2;


                shapes = imShape.FindShapes();


                /*if(shapes.Count != 1)
                    ImageViewer.Show(imShape, "imShape");*/
            } while (shapes != null && shapes.Count == 1);

            if (shapes == null || shapes.Count < 2)
                return new List<Shape>() { shape };

            for (int j = 0; j < shapes.Count; j++)
            {
                System.Drawing.SizeF sizeEll = shapes[j].Ellipse.MCvBox2D.size;
                sizeEll.Height = sizeEll.Height + erode;
                sizeEll.Width = sizeEll.Width + erode;


                shapes[j].Ellipse = new Ellipse(shapes[j].Ellipse.MCvBox2D.center, sizeEll, shapes[j].Ellipse.MCvBox2D.angle); ;
            }

            return shapes;
        }
    }
}
