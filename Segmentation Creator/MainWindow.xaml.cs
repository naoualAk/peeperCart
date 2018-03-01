using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using SegmentationCreator.Properties;

using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SegmentationCreator.Process;
using System.Threading;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Emgu.CV.UI;

namespace SegmentationCreator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        string postNameFile = "resultats";

        MetaProcessData mData;

        Image<Rgb, Byte> currentImg;

        int tLow, tHigh;

        int tHueHigh = 0;
        int tHueLow = 0;

        int idxCur = -1;

        List<Segmentation> segmentations;

        int nbImage = 0, nbImageCur = 0;

        Image<Rgb, Byte> imPath;
        private int openningSize;
        private int closingSize;

        private int openningIteration;
        private int closingIteration;
        private int backgroundThreshold;

        public MainWindow()
        {
            InitializeComponent();
            tLow = 3;
            tHigh = 3;

            mData = new MetaProcessData();
            UpdateIHM();
            imgBox.SizeMode = PictureBoxSizeMode.Zoom;
            segmentations = new List<Segmentation>();
        }





        public Image<Rgb, byte> imBg { get; private set; }
        public int tSaturation { get; private set; }

        public int tHighLightness { get; private set; }

        public int tLowLightness { get; private set; }

        private void btPath_Click(object sender, RoutedEventArgs e)
        {
            //var folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            // DialogResult result = folderBrowserDialog1.ShowDialog();


            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Open image exemple";
            openFileDialog.Filter = "Images files (*.bmp)|*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                buttonPath.Content = "\"" + openFileDialog.FileName + "\"";

                //save in settings
                Settings.Default.savePath = openFileDialog.FileName;
                Settings.Default.Save();
            }



            if (File.Exists(Settings.Default.savePath))
            {
                try
                {
                    imPath = new Image<Rgb, Byte>(Settings.Default.savePath);
                    currentImg = imPath;
                    imgBox.Image = imPath;
                    segmentations = new List<Segmentation>();
                }
                catch
                {
                    System.Windows.MessageBox.Show("Opening of image file is failed.");
                }
            }

        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Ficher segmentation xml | *.xml";
            saveFileDialog1.Title = "Sauvegarder un fichier segmentation XML";
            saveFileDialog1.ShowDialog();

            if (segmentations == null || segmentations.Count == 0)
            {
                System.Windows.MessageBox.Show("Segmentations are not started.");
                return;
            }

            if (saveFileDialog1.FileName != "")
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Segmentation>));
                using (StreamWriter wr = new StreamWriter(saveFileDialog1.FileName))
                {
                    xs.Serialize(wr, segmentations);
                }
                System.Windows.MessageBox.Show("Segmentation file saved.");
            }




        }

        private void UpdateIHM()
        {



        }

        void AddFeatures(List<GridData> d, string featuresName, object value)
        {
            if (value == null ||
                 value.GetType() == typeof(int) && Convert.ToInt32(value) == 0 ||
                 value.GetType() == typeof(double) && Convert.ToDouble(value) == 0 ||
                 value.GetType() == typeof(float) && Convert.ToDouble(value) == 0)
                return;

            d.Add(new GridData() { FeaturesName = featuresName, Value = value });
        }

        private void btPrev_Click(object sender, RoutedEventArgs e)
        {
            if (idxCur > 0 && idxCur < mData.datas.Count)
            {
                idxCur--;

                UpdateIHM();


            }
        }

        private void label_saturation_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                tSaturation = Convert.ToInt32(textBox_saturation.Text);
                if (tSaturation >= 0 && tSaturation <= 100)
                    slider_saturation.Value = tSaturation;
            }
            catch
            {

            }
        }

        private void label_hue_sup_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                tHueHigh = Convert.ToInt32(textBox_hue_sup_value.Text);
                if (tHueHigh >= 0 && tHueHigh <= 179)
                    slider_tUp.Value = 179 - tHueHigh;
            }
            catch
            {

            }
        }

        private void label_hue_inf_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                tHueLow = Convert.ToInt32(textBox_hue_inf_value.Text);
                if (tHueLow >= 0 && tHueLow < 256)
                    slider_tDown.Value = tHueLow;
            }
            catch
            {

            }
        }

        public class GridData
        {
            public string FeaturesName { get; set; }
            public object Value { get; set; }
        }

        private async void btProcess_Click(object sender, RoutedEventArgs e)
        {
            /*  Image<Bgr, Byte> image = new Image<Bgr, Byte>(@"D:\Users\damien.delhay\Documents\Travail\Biostimulant\campagne 1\21-03\F1.bmp");

              imgBox.Image = image;
              ;*/

            mData.processIsEnded = false;

            StepStr();

        }

        private string StepStr()
        {
            return nbImageCur + "/" + nbImage;
        }

        private void ShowStatsFolder()
        {
        }



        async Task RunT()
        {
            /*   Progress<string> progress = new Progress<string>(i => lbProcess.Content = i);

                await Task.Run(() =>
                {
                    Run(progress);
                });
                */
        }


        void Run(IProgress<string> progress)
        {

            string rootPath = Settings.Default.savePath;

            if (File.Exists(rootPath))
            {
                nbImage = 1;
                nbImageCur = 0;
                progress.Report(StepStr());

                System.IO.FileInfo FIRootPath = new FileInfo(rootPath);
                Debug.WriteLine(FIRootPath.DirectoryName);
                string excelPath = FIRootPath.DirectoryName + @"\" + postNameFile + @"_" + DateTime.Now.ToString("dd-MM-yy_HH'h'mm'm'ss's'") + ".xlsx";

                mData.datas.Add(processFile(rootPath));

            }
            else if (Directory.Exists(rootPath))
            {
                mData = processDirectory(rootPath, progress);
                if (mData.datas.Count > 1)
                    mData.processIsEnded = true;
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", rootPath);

                return;
            }

            // progressRing.IsActive = false;  
        }


        MetaProcessData processDirectory(string path, IProgress<string> progress)
        {
            // string[] fileEntries = Directory.GetFiles(path, "RecordedImage_BB-500GE_00-0C-DF-04-12-90_*.*");
            string[] fileEntries = Directory.GetFiles(path, "*.bmp");

            try
            {
                Array.Sort(fileEntries, (a, b) => int.Parse(Regex.Replace(a, "[^00-99]", "")) - int.Parse(Regex.Replace(b, "[^00-99]", "")));
            }
            catch
            {
                try
                {
                    Array.Sort(fileEntries, (s1, s2) => s1.CompareTo(s2));
                }
                catch { }
            }


            nbImage = fileEntries.Count();
            nbImageCur = 0;
            progress.Report(StepStr());
            foreach (string fileName in fileEntries)
            {
                mData.datas.Add(processFile(fileName));

                nbImageCur++;
                progress.Report(StepStr());
            }


            // Recurse into subdirectories of this directory.

            /*   string[] subdirectoryEntries = Directory.GetDirectories(path);
               foreach (string subdirectory in subdirectoryEntries)
                   processDirectory(subdirectory, progress);*/

            return mData;
        }

        public ProcessData processFile(string fileName)
        {
            try
            {
                /* Debug.WriteLine(fileName);
                 FileInfo fi = new FileInfo(fileName);
                 Image<Rgb, Byte> imOrigin = new Image<Rgb, Byte>(fileName);

                 ProcessData p;
                 if (pType == ProcessType.Root) // root
                     p = PRoot.Run(fileName);
                 else if (pType == ProcessType.Rod) //tige
                     p = PRod.Run(fileName);
                 else p = PLeaves.Run(fileName);

                 imgBox.Image = p.Im;

                 return p;*/
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }



        private void buttonAlgo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (segmentations != null && segmentations.Count > 0 && segmentations.Last().imThreshold != null)
                imgBox.Image = segmentations.Last().imThreshold;
        }

        private void buttonAlgo_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (segmentations != null && segmentations.Count > 0 && segmentations.Last().imAlgo != null)
                imgBox.Image = segmentations.Last().imAlgo;
        }



        private void imgBox_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            /*  if (imPath == null)
                  return;

             int offsetX = (int)(e.Location.X * imgBox.ZoomScale);
              int offsetY = (int)(e.Location.Y * imgBox.ZoomScale);
              int horizontalScrollBarValue = imgBox.HorizontalScrollBar.Visible ? (int)imgBox.HorizontalScrollBar.Value : 0;
              int verticalScrollBarValue = imgBox.VerticalScrollBar.Visible ? (int)imgBox.VerticalScrollBar.Value : 0;
              string text = Convert.ToString(offsetX + horizontalScrollBarValue) + "." + Convert.ToString(offsetY + verticalScrollBarValue);

              int x, y;
              ConvertCoordinates(imgBox, out x, out y, e.X, e.Y);
              x += horizontalScrollBarValue;
              y += verticalScrollBarValue;

              Debug.WriteLine(text);
              System.Drawing.PointF center = new System.Drawing.PointF((float)x, (float)y);


              Image<Gray, byte> mask = new Image<Gray, byte>(imPath.Width + 2, imPath.Height + 2);
              MCvConnectedComp comp = new MCvConnectedComp();


              Debug.WriteLine("tLow : " + tLow + "; thigh:" + tHigh);
              try
              {
                  CvInvoke.cvFloodFill(imPath, new System.Drawing.Point(x, y), new MCvScalar(255, 0, 0), new MCvScalar(tLow, tLow, tLow), new MCvScalar(tHigh, tHigh, tHigh), out comp, 8, mask);
              }
              catch { }
              imPath.Draw(new CircleF(center, 20), new Rgb(0, 0, 255), 20);
              imgBox.Image = imPath;*/
        }

        private void slider_Saturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tSaturation = (int)slider_saturation.Value;
            textBox_saturation.Text = ((int)slider_saturation.Value).ToString();
        }


        private void slider_tDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tHueLow = (int)slider_tDown.Value;
            textBox_hue_inf_value.Text = ((int)slider_tDown.Value).ToString();
        }

        private void slider_tUp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            tHueHigh = (int)(179 - slider_tUp.Value);
            textBox_hue_sup_value.Text = ((int)(179 - slider_tUp.Value)).ToString();

        }

        private static bool isValue(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }


        private void button_background_Click(object sender, RoutedEventArgs e)
        {
            //var folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            // DialogResult result = folderBrowserDialog1.ShowDialog();



            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Open background image";
            openFileDialog.Filter = "Images files (*.bmp)|*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                textBox_background.Text = openFileDialog.FileName;
            }


            if (File.Exists(openFileDialog.FileName))
            {
                imBg = new Image<Rgb, Byte>(openFileDialog.FileName);
                //  imgBox.Image = imBg;
            }
        }



        private void button_ok_background_Click(object sender, RoutedEventArgs e)
        {

            //System.Windows.Forms.MessageBox.Show("10");
            try
            {


                Segmentation seg = new Segmentation();
                seg.segmentationType = Segmentation.SegmentationType.Soustraction;
                seg.backgroundPath = textBox_background.Text;
                seg.backgroundThreshold = backgroundThreshold;

                //ImageViewer.Show(imPath, "imPath");
                //ImageViewer.Show(imBg, "imBg");

                // seg.imThreshold = imPath.AbsDiff(imBg).Convert<Gray, Byte>().ThresholdBinary(new Gray(10), new Gray(255));
                // seg.imAlgo = imPath.AbsDiff(imBg);


                seg.imThreshold = imPath.AbsDiff(imBg).Convert<Gray, Byte>().ThresholdBinary(new Gray(backgroundThreshold), new Gray(255));
                //  Debug.WriteLine("10");

                seg.imAlgo = imPath.Copy();

                Parallel.For(0, seg.imAlgo.Rows, r =>
                {
                    Parallel.For(0, seg.imAlgo.Cols, c =>
                    {
                        if (seg.imThreshold[r, c].Intensity == 0)
                            seg.imAlgo[r, c] = new Rgb(0, 0, 0);
                    });
                });



                segmentations.Add(seg);



                currentImg = seg.imAlgo;
                imgBox.Image = seg.imAlgo;
            }
            catch
            {
                System.Windows.MessageBox.Show("Problem of background soustraction");
            }
        }



        private void textBox_tlow_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null)
                return;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
            if (!isValue(tb.Text))
                tb.Text = "";

            if (!int.TryParse(tb.Text, out tLow)) tLow = 0;
        }

        private void button_ok_seuillage_Click(object sender, RoutedEventArgs e)
        {


            if (currentImg == null)
                return;

            try
            {
                Image<Hls, Byte> im = currentImg.Convert<Hls, Byte>();
                Image<Hsv, Byte> imHSV = currentImg.Convert<Hsv, Byte>();
                Image<Lab, Byte> imLab = currentImg.Convert<Lab, Byte>();

                Debug.WriteLine(tHueLow);
                Debug.WriteLine(tHueHigh);
                Debug.WriteLine(tSaturation * 255 / 100);
                Debug.WriteLine(tLowLightness);
                Debug.WriteLine(tHighLightness);


                if (!hueInversed)
                    Parallel.For(0, im.Rows, r =>
                    {
                        Parallel.For(0, im.Cols, c =>
                        {
                            if (imLab[r, c].X <= tHighLightness && (
                                imLab[r, c].X <= tLowLightness ||
                                imHSV[r, c].Satuation < (tSaturation * 255 / 100) ||
                                im[r, c].Hue < tHueLow ||
                                im[r, c].Hue > tHueHigh))
                                im[r, c] = new Hls(0, 0, 0);
                        });
                    });
                else
                    Parallel.For(0, im.Rows, r =>
                    {
                        Parallel.For(0, im.Cols, c =>
                        {
                            if (imLab[r, c].X <= tHighLightness && (
                            imLab[r, c].X <= tLowLightness ||
                            imHSV[r, c].Satuation < (tSaturation * 255 / 100) ||
                            (im[r, c].Hue >= tHueLow &&
                            im[r, c].Hue <= tHueHigh)))
                                im[r, c] = new Hls(0, 0, 0);
                        });
                    });


                Segmentation seg = new Segmentation();
                seg.segmentationType = Segmentation.SegmentationType.Seuillage;
                seg.tHueHigh = tHueHigh;
                seg.tHueLow = tHueLow;
                seg.inverseHue = hueInversed;
                seg.tSaturation = tSaturation;
                seg.tHighLightness = tHighLightness;
                seg.tLowLightness = tLowLightness;
                seg.imAlgo = im.Convert<Rgb, Byte>();
                seg.imThreshold = im.Convert<Gray, Byte>().ThresholdBinary(new Gray(1), new Gray(255));

                segmentations.Add(seg);

                currentImg = im.Convert<Rgb, Byte>();
                imgBox.Image = im.Convert<Rgb, Byte>();
            }
            catch
            {
                System.Windows.MessageBox.Show("Problem of color segmentation");
            }
        }

        private void UNDO_Click(object sender, RoutedEventArgs e)
        {
            if (segmentations != null && segmentations.Count > 0)
            {
                segmentations.Remove(segmentations.Last());
                if (segmentations.Count > 0)
                {
                    currentImg = segmentations.Last().imAlgo;
                    imgBox.Image = segmentations.Last().imAlgo;
                }
                else
                {
                    currentImg = imPath;
                    imgBox.Image = imPath;
                }
            }
        }

        private void WindowKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                UNDO_Click(this, null);
            }
        }

        private void textBox_closingSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null)
                return;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
            if (!isValue(tb.Text))
                tb.Text = "";

            if (!int.TryParse(tb.Text, out closingSize)) closingSize = 0;
        }

        private void textBox_openningSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null)
                return;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
            if (!isValue(tb.Text))
                tb.Text = "";

            if (!int.TryParse(tb.Text, out openningSize)) openningSize = 0;


        }


        private void textBox_closingIteration_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null)
                return;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
            if (!isValue(tb.Text))
                tb.Text = "";

            if (!int.TryParse(tb.Text, out closingIteration)) closingIteration = 0;
        }

        private void textBox_openningIteration_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null)
                return;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
            if (!isValue(tb.Text))
                tb.Text = "";

            if (!int.TryParse(tb.Text, out openningIteration)) openningIteration = 0;


        }

        private void button_closing_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (segmentations == null || segmentations.Count < 1)
                    return;
                Image<Gray, Byte> imThreshold = segmentations.Last().imThreshold.Copy();
                Image<Rgb, Byte> im = segmentations.Last().imAlgo.Copy();

                int size = closingSize;
                StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                imThreshold = imThreshold.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, closingIteration);

                Parallel.For(0, imThreshold.Rows, r =>
                {
                    Parallel.For(0, imThreshold.Cols, c =>
                    {
                        if (imThreshold[r, c].Intensity == 0)
                            im[r, c] = new Rgb(0, 0, 0);
                    });
                });


                Segmentation seg = new Segmentation();
                seg.segmentationType = Segmentation.SegmentationType.Fermeture;
                seg.closingSize = closingSize;
                seg.imAlgo = im;
                seg.imThreshold = imThreshold;
                seg.imAlgo = im;
                seg.closingIteration = closingIteration;


                segmentations.Add(seg);

                currentImg = im;
                imgBox.Image = im;
            }
            catch
            {
                System.Windows.MessageBox.Show("Problem of opening morphology");
            }
        }

        private void button_openning_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (segmentations == null || segmentations.Count < 1)
                    return;
                Image<Gray, Byte> imThreshold = segmentations.Last().imThreshold.Copy();
                Image<Rgb, Byte> im = imPath.Copy();

                int size = openningSize;
                StructuringElementEx elementOpen = new StructuringElementEx(size * 2 + 1, size * 2 + 1, size, size, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                imThreshold = imThreshold.MorphologyEx(elementOpen, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, openningIteration);

                Parallel.For(0, imThreshold.Rows, r =>
                {
                    Parallel.For(0, imThreshold.Cols, c =>
                    {
                        if (imThreshold[r, c].Intensity == 0)
                            im[r, c] = new Rgb(0, 0, 0);
                    });
                });


                Segmentation seg = new Segmentation();
                seg.segmentationType = Segmentation.SegmentationType.Ouverture;
                seg.openningIteration = openningIteration;
                seg.imAlgo = im;
                seg.imThreshold = imThreshold;
                seg.openningSize = openningSize;

                segmentations.Add(seg);

                currentImg = im;
                imgBox.Image = im;
            }
            catch
            {
                System.Windows.MessageBox.Show("Problem of opening morphology");
            }
        }

        bool hueInversed = false;

        private void checkbox_hue_Checked(object sender, RoutedEventArgs e)
        {
            hueInversed = true;
        }

        private void checkbox_hue_Unchecked(object sender, RoutedEventArgs e)
        {
            hueInversed = false;
        }

        private void textBox_threshold_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null)
                return;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
            if (!isValue(tb.Text))
                tb.Text = "";

            if (!int.TryParse(tb.Text, out backgroundThreshold)) backgroundThreshold = 0;
        }


        private void slider_hightLightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            try
            {
                tHighLightness = (int)slider_hightLightness.Value;
                textBox_hightLightness.Text = ((int)slider_hightLightness.Value).ToString();
            }
            catch { }
        }

        private void label_hightLightness_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                tHighLightness = Convert.ToInt32(textBox_hightLightness.Text);
                if (tHighLightness >= 0 && tHighLightness <= 255)
                    slider_hightLightness.Value = tHighLightness;
            }
            catch
            {

            }
        }

        private void slider_lowLightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                tLowLightness = (int)slider_lowLightness.Value;
                textBox_lowLightness.Text = ((int)slider_lowLightness.Value).ToString();
            }
            catch { }
        }

        private void label_lowLightness_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                tLowLightness = Convert.ToInt32(textBox_lowLightness.Text);
                if (tLowLightness >= 0 && tLowLightness <= 255)
                    slider_lowLightness.Value = tLowLightness;
            }
            catch
            {

            }
        }

        private void textBox_thigh_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null)
                return;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
            if (!isValue(tb.Text))
                tb.Text = "";

            if (!int.TryParse(tb.Text, out tHigh)) tHigh = 0;
        }

        private void SaveExcel(MetaProcessData mData, string fileName)
        {
            ExcelWriter ex = new ExcelWriter(fileName);
            List<object> l = new List<object>();
            l.Add("Nom");
            l.Add("Densité racinaire (t=70)");
            l.Add("Nombre px racine (t=70)");
            l.Add("Nombre px squelette (t=70)");
            l.Add("Densité racinaire (t=80)");
            l.Add("Nombre px racine (t=80)");
            l.Add("Nombre px squelette (t=80)");
            l.Add("Densité racinaire (t=90)");
            l.Add("Nombre px racine (t=90)");
            l.Add("Nombre px squelette (t=90)");
            l.Add("Nombre px Suen Ske");
            l.Add("Suen Dist Map [moy]");
            l.Add("Suen Dist Map [ET]");
            l.Add("Suen Dist Map [min]");
            l.Add("Suen Dist Map [max]");

            l.Add("Dist Map [moy]");
            l.Add("Dist Map [ET]");
            l.Add("Dist Map [min]");
            l.Add("Dist Map [max]");

            l.Add("Dist Map Dirt [moy]");
            l.Add("Dist Map Dirt [ET]");
            l.Add("Dist Map Dirt [min]");
            l.Add("Dist Map Dirt [max]");

            l.Add("Nombre px");
            l.Add("Nombre px squelette");
            l.Add("Aire foliaire [moy]");
            l.Add("Aire foliaire [min]");
            l.Add("Aire foliaire [max]");
            l.Add("Aire foliaire [ET]");
            l.Add("Envergure foliaire [moy]");
            l.Add("Envergure foliaire [min]");
            l.Add("Envergure foliaire [max]");
            l.Add("Envergure foliaire [ET]");
            l.Add("Largeur foliaire [moy]");
            l.Add("Largeur foliaire [min]");
            l.Add("Largeur foliaire [max]");
            l.Add("Largeur foliaire [ET]");
            l.Add("Longueur foliaire [moy]");
            l.Add("Longueur foliaire [min]");
            l.Add("Longueur foliaire [max]");
            l.Add("Longueur foliaire [ET]");

            l.Add("Nb Tiges");

            string haraStr = "Haralick Root";
            for (int i = 0; i <= 135; i += 45)
            {
                l.Add(haraStr + "01 " + i + "° Angular Second Momentum");
                l.Add(haraStr + "02 " + i + "° Contrast");
                l.Add(haraStr + "03 " + i + "° Correlation");
                l.Add(haraStr + "04 " + i + "° Sum of Squares: Variance");
                l.Add(haraStr + "05 " + i + "° Inverse Difference Moment");
                l.Add(haraStr + "06 " + i + "° Sum Average");
                l.Add(haraStr + "07 " + i + "° Sum Variance");
                l.Add(haraStr + "08 " + i + "° Sum Entropy");
                l.Add(haraStr + "09 " + i + "° Entropy");
                l.Add(haraStr + "10 " + i + "° Difference Variance");
                l.Add(haraStr + "11 " + i + "° Difference Entropy");
                l.Add(haraStr + "12 " + i + "° First Information Measure");
                l.Add(haraStr + "13 " + i + "° Second Information Measure");
            }

            haraStr = "Haralick DistMapRoot";
            for (int i = 0; i <= 135; i += 45)
            {
                l.Add(haraStr + "01 " + i + "° Angular Second Momentum");
                l.Add(haraStr + "02 " + i + "° Contrast");
                l.Add(haraStr + "03 " + i + "° Correlation");
                l.Add(haraStr + "04 " + i + "° Sum of Squares: Variance");
                l.Add(haraStr + "05 " + i + "° Inverse Difference Moment");
                l.Add(haraStr + "06 " + i + "° Sum Average");
                l.Add(haraStr + "07 " + i + "° Sum Variance");
                l.Add(haraStr + "08 " + i + "° Sum Entropy");
                l.Add(haraStr + "09 " + i + "° Entropy");
                l.Add(haraStr + "10 " + i + "° Difference Variance");
                l.Add(haraStr + "11 " + i + "° Difference Entropy");
                l.Add(haraStr + "12 " + i + "° First Information Measure");
                l.Add(haraStr + "13 " + i + "° Second Information Measure");
            }

            haraStr = "Haralick DistMapDirt";
            for (int i = 0; i <= 135; i += 45)
            {
                l.Add(haraStr + "01 " + i + "° Angular Second Momentum");
                l.Add(haraStr + "02 " + i + "° Contrast");
                l.Add(haraStr + "03 " + i + "° Correlation");
                l.Add(haraStr + "04 " + i + "° Sum of Squares: Variance");
                l.Add(haraStr + "05 " + i + "° Inverse Difference Moment");
                l.Add(haraStr + "06 " + i + "° Sum Average");
                l.Add(haraStr + "07 " + i + "° Sum Variance");
                l.Add(haraStr + "08 " + i + "° Sum Entropy");
                l.Add(haraStr + "09 " + i + "° Entropy");
                l.Add(haraStr + "10 " + i + "° Difference Variance");
                l.Add(haraStr + "11 " + i + "° Difference Entropy");
                l.Add(haraStr + "12 " + i + "° First Information Measure");
                l.Add(haraStr + "13 " + i + "° Second Information Measure");
            }


            //
            /*  res.Add(haraDesc.F02);  //Contrast
            res.Add(haraDesc.F03);  //
            res.Add(haraDesc.F04);  //.
            res.Add(haraDesc.F05);  //. 
            res.Add(haraDesc.F06);  //. 
            res.Add(haraDesc.F07);  // 
            res.Add(haraDesc.F08);  // .
            res.Add(haraDesc.F09);  //. 
            res.Add(haraDesc.F10);  //. 
            res.Add(haraDesc.F11);  //. 
            res.Add(haraDesc.F12);  // . 
            res.Add(haraDesc.F13);  //. 
            res.Add(haraDesc.F14);  //. */
            ex.WriteStats(l);

            for (int i = 0; i < mData.datas.Count; i++)
            {
                List<object> list = new List<object>();
                list.Add(mData.datas[i].Name);
                list.Add(mData.datas[i].DensityRoot);
                list.Add(mData.datas[i].NbPxRoot);
                list.Add(mData.datas[i].PxSkelet);
                list.Add(mData.datas[i].DensityRoot2);
                list.Add(mData.datas[i].NbPxRoot2);
                list.Add(mData.datas[i].PxSkelet2);
                list.Add(mData.datas[i].DensityRoot3);
                list.Add(mData.datas[i].NbPxRoot3);
                list.Add(mData.datas[i].PxSkelet3);
                list.Add(mData.datas[i].SuenSke);
                list.Add(mData.datas[i].SuenDistMapMoy);
                list.Add(mData.datas[i].SuenDistMapSTD);
                list.Add(mData.datas[i].SuenDistMapMin);
                list.Add(mData.datas[i].SuenDistMapMax);

                list.Add(mData.datas[i].DistMapMoy);
                list.Add(mData.datas[i].DistMapSTD);
                list.Add(mData.datas[i].DistMapMin);
                list.Add(mData.datas[i].DistMapMax);
                list.Add(mData.datas[i].DistMapDirtMoy);
                list.Add(mData.datas[i].DistMapDirtSTD);
                list.Add(mData.datas[i].DistMapDirtMin);
                list.Add(mData.datas[i].DistMapDirtMax);

                list.Add(mData.datas[i].PxLeaves);
                list.Add(mData.datas[i].PxSkLeaves);
                list.Add(mData.datas[i].LeavesAreaMoy);
                list.Add(mData.datas[i].LeavesAreaMin);
                list.Add(mData.datas[i].LeavesAreaMax);
                list.Add(mData.datas[i].LeavesAreaStd);
                list.Add(mData.datas[i].LeavesLengthCalipMoy);
                list.Add(mData.datas[i].LeavesLengthCalipMin);
                list.Add(mData.datas[i].LeavesLengthCalipMax);
                list.Add(mData.datas[i].LeavesLengthCalipStd);
                list.Add(mData.datas[i].LeavesWidthCalipMoy);
                list.Add(mData.datas[i].LeavesWidthCalipMin);
                list.Add(mData.datas[i].LeavesWidthCalipMax);
                list.Add(mData.datas[i].LeavesWidthCalipStd);
                list.Add(mData.datas[i].SkeMoy);
                list.Add(mData.datas[i].SkeMin);
                list.Add(mData.datas[i].SkeMax);
                list.Add(mData.datas[i].SkeSTD);
                list.Add(mData.datas[i].NbRod);


                for (int k = 0; k < 4; k++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        list.Add(mData.datas[i].haraRootList[k][j]);
                    }
                }

                for (int k = 0; k < 4; k++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        list.Add(mData.datas[i].haraDistMapRootList[k][j]);
                    }
                }

                for (int k = 0; k < 4; k++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        list.Add(mData.datas[i].haraDistMapDirtList[k][j]);
                    }
                }

                ex.WriteStats(list);
            }

            ex.Save();

            //ouverture fichier excel
            System.Diagnostics.Process.Start(fileName);
        }
    }
}
