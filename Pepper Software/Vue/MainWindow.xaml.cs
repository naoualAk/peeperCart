using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



using pepperSoft.Modele;
using System.IO;
using Microsoft.Win32;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Forms;
using VisartLib.Data;
using VisartLib;
using System.Diagnostics;



using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;





using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using VisartLib.ColorExtension;
using Emgu.CV.UI;
using System.Windows.Threading;
using pepperSoft.Controle;
using pepperSoft.Vue;

namespace pepperSoft
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Features features;
        FilesData filesData;
        ImData imData;
        List<Shape> shapes;
        Image<Rgb, Byte> imCur;
        Image<Rgb, Byte> maskCount;
        Image<Rgb, Byte> maskRug;
        List<Item> items;
        StatsPresentation statsPresentation;

        // initial size of image
        int imgSize = 200;

        public MainWindow()
        {
            InitializeComponent();

            dataGrid.Visibility = Visibility.Hidden;

            features = new Features();
            filesData = new FilesData();
            shapes = new List<Shape>();
            imData = new ImData();


            timer.Tick += new EventHandler(timer_Tick);


            imgBox.SizeMode = PictureBoxSizeMode.Zoom;
            Init(dataGrid);

            dataGridStats.Visibility = Visibility.Hidden;

            labelImgBox.Visibility = Visibility.Hidden;
            labelStats.Visibility = Visibility.Hidden;
            labelStatsGlobale.Visibility = Visibility.Hidden;
            labelDoss.Visibility = Visibility.Hidden;
            listView.Visibility = Visibility.Hidden;
            splitter.Visibility = Visibility.Hidden;

            /* test d'affichage item dans datagrid
            items = new List<Modele.Item>();
             Item item = new Modele.Item();
             item.dimension1.Height = 3.20F;
             item.statsColor.Color1Str = "#FFFF22";


             items.Add( item );
             UpdateList();*/
        }


        /// <summary>
        /// Init le dataDrid en ajoutant des colonnes correspondants aux items
        /// </summary>
        /// <param name="dataDrig"></param>
        private void Init(System.Windows.Controls.DataGrid dataDrig)
        {


            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Header = "ID";
            textColumn.Binding = new System.Windows.Data.Binding("ID");
            dataGrid.Columns.Add(textColumn);

            // AddColumnsImage("imageSource");

            // AddColumns("rugosite");

            /*  textColumn = new DataGridTextColumn();
              textColumn.Header = "Avg";
              textColumn.Binding = new System.Windows.Data.Binding("avg");
              dataGrid.Columns.Add(textColumn);

              textColumn = new DataGridTextColumn();
              textColumn.Header = "Max";
              textColumn.Binding = new System.Windows.Data.Binding("max");
              dataGrid.Columns.Add(textColumn);

              textColumn = new DataGridTextColumn();
              textColumn.Header = "Min";
              textColumn.Binding = new System.Windows.Data.Binding("min");
              dataGrid.Columns.Add(textColumn);

              textColumn = new DataGridTextColumn();
              textColumn.Header = "Std";
              textColumn.Binding = new System.Windows.Data.Binding("std");
              dataGrid.Columns.Add(textColumn);

              textColumn = new DataGridTextColumn();
              textColumn.Header = "Area";
              textColumn.Binding = new System.Windows.Data.Binding("area");
              dataGrid.Columns.Add(textColumn);

              textColumn = new DataGridTextColumn();
              textColumn.Header = "Height";
              textColumn.Binding = new System.Windows.Data.Binding("height");
              dataGrid.Columns.Add(textColumn);

              textColumn = new DataGridTextColumn();
              textColumn.Header = "Width";
              textColumn.Binding = new System.Windows.Data.Binding("width");
              dataGrid.Columns.Add(textColumn);*/


            AddColumns("statsColor.Color1Str", "Color1", true);
            AddColumns("statsColor.Color2Str", "Color2", true);
            AddColumns("statsColor.Color3Str", "Color3", true);

            AddColumns("dimension1.Height", "F1 Longueur");
            AddColumns("dimension1.Width", "F1 Largeur");
           // AddColumns("dimension1.Area", "F1 Aire");

            AddColumns("dimension2.Height", "F2 Longueur");
            AddColumns("dimension2.Width", "F2 Largeur");
            //AddColumns("dimension2.Area", "F2 Aire");

            AddColumns("dimension3.Height", "F3 Longueur");
            AddColumns("dimension3.Width", "F3 Largeur");
           // AddColumns("dimension3.Area", "F3 Aire");

            /*    AddColumns("avgSob");
                AddColumns("minSob");
                AddColumns("maxSob");
                AddColumns("stdSob");

                AddColumns("roundness");
                AddColumns("ellHeight");
                AddColumns("ellWidth");

                AddColumns("haralick.contrast");
                AddColumns("haralick.correlation");
                AddColumns("haralick.variance");
                AddColumns("haralick.inverseDiffMoment");
                AddColumns("haralick.sumAvg");
                AddColumns("haralick.sumVar");
                AddColumns("haralick.sumEntropy");
                AddColumns("haralick.entropy");
                AddColumns("haralick.diffVar");
                AddColumns("haralick.diffEntropy");
                AddColumns("haralick.firstInfoMeasure");
                AddColumns("haralick.secondInfoMeasure");*/
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            info_mid.Text = "";
        }

        public DispatcherTimer timer = new DispatcherTimer();

        public void AddColumns(string bindingStr, string header = "", bool Color = false)
        {
            DataGridTextColumn textColumn = new DataGridTextColumn();
            if (header == "")
                textColumn.Header = bindingStr;
            else
                textColumn.Header = header;

            if (Color)
            {
                System.Windows.Data.Binding bd = new System.Windows.Data.Binding(bindingStr);
                bd.Converter = new ColorToBrushConverter();


                Style columnStyle = new Style(typeof(TextBlock));


                columnStyle.Setters.Add(new Setter(TextBlock.BackgroundProperty, bd));

                textColumn.ElementStyle = columnStyle;
                textColumn.Binding = bd;

            }
            else
            {
                System.Windows.Data.Binding bd = new System.Windows.Data.Binding(bindingStr);
                textColumn.Binding = bd;
                textColumn.Binding.StringFormat = "{0:F2}";
            }


            /* try
             {
                 textColumn.ElementStyle.Setters.Add(
                     new Setter(TextBlock.BackgroundProperty, bd)
                 );
             }
             catch
             {
                 Debug.WriteLine("failed background setter");

             }*/



            dataGrid.Columns.Add(textColumn);
        }


        public void AddColumnsImage(string binding, string header = "")
        {




            DataGridTemplateColumn imgColumn = new DataGridTemplateColumn();
            imgColumn.Header = header;
            FrameworkElementFactory factory1 = new FrameworkElementFactory(typeof(Image));
            System.Windows.Data.Binding b1 = new System.Windows.Data.Binding(binding);
            b1.Mode = BindingMode.TwoWay;
            factory1.SetValue(Image.SourceProperty, b1);
            DataTemplate cellTemplate1 = new DataTemplate();
            cellTemplate1.VisualTree = factory1;
            imgColumn.CellTemplate = cellTemplate1;
            dataGrid.Columns.Add(imgColumn);

            if (header == "")
                imgColumn.Header = binding;
            else
                imgColumn.Header = header;

            //dataGrid.Columns.Add(imgColumn);


        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {


            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image file (*.bmp)|*.bmp|All files (*.*)|*.*";
            openFileDialog.Title = "Open an image ";
            FileInfo fiSeg;
            try
            {
                fiSeg = new FileInfo(Properties.Settings.Default.imgPath);
                openFileDialog.InitialDirectory = fiSeg.DirectoryName;
            }
            catch
            {

            }

            if (openFileDialog.ShowDialog() == true)
            {
                filesData.imgFilePath = openFileDialog.FileName;
                FileInfo fi = new FileInfo(openFileDialog.FileName);
                Properties.Settings.Default.imgPath = fi.FullName;
                Properties.Settings.Default.Save();
                imCur = new Image<Rgb, Byte>(filesData.imgFilePath);
                //    imData.imOrigin = imCur;

                shapes = new List<Shape>();
                maskCount = null;
                maskRug = null;
                dataGrid.ItemsSource = null;
            }
        }

        async Task BlobCountT(Item item)
        {
            Progress<ProgressData> progress = new Progress<ProgressData>(i => UpdateProgress(i));

            await Task.Run(() =>
            {
                Image<Bgr, Byte> im = item.images.Origin.Copy();
                Image<Gray, Byte> imGCpy = im.Convert<Gray, Byte>().Copy();

                List<Shape> shapes;
                imGCpy.ThresholdBinary(new Gray(5), new Gray(255)).Copy().BlobCount(out maskCount, out shapes, progress);

                item.shapes = shapes;

                item.images.Mask = maskCount.Convert<Bgr, Byte>();
            });
        }


        async Task BlobAnalyseT(Item item)
        {
            Progress<ProgressData> progress = new Progress<ProgressData>(i => UpdateProgress(i));

            await Task.Run(() =>
            {

                Image<Bgr, Byte> im = item.images.Origin.Copy();
                Image<Gray, Byte> imGCpy = im.Convert<Gray, Byte>().Copy();


                imGCpy.BlobAnalyse(item.shapes, item, out maskRug, progress);

                item.images.Color = maskRug.Convert<Bgr, Byte>();

                //   ImageViewer.Show(maskRug.Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_AREA), "maskRug");
            });
        }




        private void UpdateProgress(ProgressData p)
        {
            if (!p.Ended)
                if (!p.Error)
                {
                    info_mid.Foreground = new SolidColorBrush(Color.FromArgb(255, 56, 90, 182));
                    info_mid.Text = p.Message;
                    progressBar.Maximum = p.TotalImg;
                    progressBar.Value = p.CurrentImg;

                }
                else
                {
                    info_mid.Foreground = Brushes.Red;
                    info_mid.Text = p.Message;


                    timer.Interval = new TimeSpan(0, 0, 4);
                    timer.Start();
                }
            else
            {
                info_mid.Foreground = new SolidColorBrush(Color.FromArgb(255, 56, 90, 182));
                info_mid.Text = "Process ended";


                timer.Interval = new TimeSpan(0, 0, 3);
                timer.Start();
            }
        }

        private async void Analyser_click(object sender, RoutedEventArgs e)
        {
            foreach (Item item in items)
            {

                shapes = new List<Shape>();


                await Count();

                // imGCpy.BlobAnalyse(shapes, out items, out maskRug);

                progressBar.Visibility = Visibility.Visible;



                await BlobAnalyseT(item);

                progressBar.Visibility = Visibility.Hidden;



                UpdateList();


            }
        }


        private void Compter_click(object sender, RoutedEventArgs e)
        {
            Count();
        }


        private async Task Count()
        {


            foreach (Item item in items)
            {


                try
                {

                    shapes = new List<Shape>();



                    //dataGrid.Items.Clear();

                    Image<Bgr, Byte> im = item.images.Origin.Copy();

                    Image<Gray, Byte> imG = im.Convert<Gray, Byte>().ThresholdBinary(new Gray(5), new Gray(255)).Copy();

                    progressBar.Visibility = Visibility.Visible;



                    //imG.BlobCount(out maskCount, out shapes);
                    await BlobCountT(item);

                    progressBar.Visibility = Visibility.Hidden;



                    //ImageViewer.Show(maskCount, "maskCount");
                    // item.SetImageSource(maskCount.ToBitmap());






                    UpdateList();

                    info_mid.Foreground = new SolidColorBrush(Color.FromArgb(255, 56, 90, 182));
                    info_mid.Text = "Nombre de grains : " + shapes.Count;


                    timer.Interval = new TimeSpan(0, 0, 5);
                    timer.Start();

                }
                catch { }
            }
        }




        private void SvgExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateImgBox()
        {
            /* if (imCur != null)
                 if (affRug.IsChecked && affCompte.IsChecked)
                 {

                 }
                 else if (affCompte.IsChecked)
                 {

                 }
                 else if (affRug.IsChecked)
                 {
                     if (maskRug != null)
                         imgBox.Image = imCur.Add(maskRug.Mul(0.5));
                 }
                 else imgBox.Image = imData.imOrigin;*/

            Item item = (Item)listView.SelectedItem;
            if (item != null)
            {
                if (affMask.IsChecked && item.images.Mask != null)
                {
                    imgBox.Image = item.images.Mask;
                    labelImgBox.Visibility = Visibility.Visible;
                }
                else if (affKmean.IsChecked && item.images.Kmean != null)
                {
                    imgBox.Image = item.images.Kmean;
                    labelImgBox.Visibility = Visibility.Visible;
                }
                else
                {
                    imgBox.Image = item.images.Origin;
                    labelImgBox.Visibility = Visibility.Visible;
                }
            }
        }

        private void AffRug_click(object sender, RoutedEventArgs e)
        {
            if (maskRug != null && imCur != null)
                try
                {
                    UpdateImgBox();
                }
                catch { Debug.WriteLine("affichage impossible"); }

        }

        private void AffRug_unclick(object sender, RoutedEventArgs e)
        {
            if (imCur != null)
                try
                {
                    UpdateImgBox();
                }
                catch { Debug.WriteLine("Affichage impossible"); }
        }

        private void AffCompte_unclick(object sender, RoutedEventArgs e)
        {
            if (imCur != null)
                try
                {
                    UpdateImgBox();
                }
                catch { Debug.WriteLine("Affichage impossible"); }
        }

        private void AffCompte_click(object sender, RoutedEventArgs e)
        {
            if (maskCount != null && imCur != null)
                try
                {
                    UpdateImgBox();
                }
                catch { Debug.WriteLine("Affichage impossible"); }
        }

        private void acquisition_images_Click(object sender, RoutedEventArgs e)
        {
            AsynchronousGrab.MainForm acqWindow = new AsynchronousGrab.MainForm();
            acqWindow.ShowDialog();
        }

        private void seg_creer_Click(object sender, RoutedEventArgs e)
        {
            SegmentationCreator.MainWindow segCreatorWindow = new SegmentationCreator.MainWindow();
            segCreatorWindow.ShowDialog();

        }

        private void seg_appliquer_Click(object sender, RoutedEventArgs e)
        {
            SegmenTool.MainWindow segToolWindow = new SegmenTool.MainWindow();
            segToolWindow.ShowDialog();
        }

        private void OpenDoss_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = Properties.Settings.Default.imgPath;
                dialog.Description = "Select output folder";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.imgPath = dialog.SelectedPath;
                    Properties.Settings.Default.Save();

                }

            }

            items = new List<Item>();

            DirectoryInfo d = new DirectoryInfo(Properties.Settings.Default.imgPath);//Assuming Test is your Folder
            if (d == null)
                return;
            FileInfo[] Files = d.GetFiles("*.bmp"); //Getting Text files
            if (Files.Length < 1) return;
            int id = 0;

            List<IconImage> icons = new List<IconImage>();
            foreach (FileInfo file in Files)
            {


                Image<Bgr, Byte> im = new Image<Bgr, byte>(file.FullName);
                Image<Bgr, Byte> imIcon = im.Copy(new System.Drawing.Rectangle(im.ROI.Width / 3, 0, im.ROI.Width / 3, im.ROI.Height));
                imIcon = imIcon.Resize(80, 80, Emgu.CV.CvEnum.INTER.CV_INTER_AREA);

                //ImageViewer.Show(imIcon, "imIcon");
                BitmapImage bitmapImageIcon = new BitmapImage();
                using (MemoryStream memory = new MemoryStream())
                {
                    imIcon.ToBitmap().Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImageIcon = new BitmapImage();
                    bitmapImageIcon.BeginInit();
                    bitmapImageIcon.StreamSource = memory;
                    bitmapImageIcon.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImageIcon.EndInit();
                }

                System.Drawing.Bitmap bitmap = im.ToBitmap();

                ImData imData = new ImData() { Origin = im };
                im = im.Resize((int)(imgSize * 2.5), imgSize, Emgu.CV.CvEnum.INTER.CV_INTER_AREA);

                BitmapImage bitmapImage = new BitmapImage();
                System.Drawing.Bitmap bitmapConversion = im.ToBitmap();

                using (MemoryStream memory = new MemoryStream())
                {
                    bitmapConversion.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }

                Image image = new Image();
                image.Source = bitmapImage;


                icons.Add(new IconImage()
                {
                    Name = "img" + id.ToString(),
                    ImageSource = bitmapImageIcon
                });


                items.Add(new Item() { Name = "img" + id.ToString(), ID = id, ImageSource = bitmapImageIcon, bitmap = bitmap, images = imData });
                id++;
            }

            DataContext = items;

            UpdateList();
            listView.Visibility = Visibility.Visible;
            splitter.Visibility = Visibility.Visible;
            labelDoss.Visibility = Visibility.Visible;
        }


        private void UpdateList()
        {

            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = items;
            dataGrid.Visibility = Visibility.Visible;

            dataGridStats.ItemsSource = null;
            statsPresentation = new StatsPresentation(items);
            dataGridStats.ItemsSource = statsPresentation;

            dataGrid.Visibility = Visibility.Visible;

            dataGrid.Visibility = Visibility.Visible;
            labelStats.Visibility = Visibility.Visible;
        }

        private async void AnalyserCouleur_click(object sender, RoutedEventArgs e)
        {
            foreach (Item item in items)
            {
                progressBar.Visibility = Visibility.Visible;

                await BlobAnalyseCouleurT(item);

                progressBar.Visibility = Visibility.Hidden;

                UpdateList();
            }
        }


        async Task BlobAnalyseCouleurT(Item item)
        {
            Progress<ProgressData> progress = new Progress<ProgressData>(i => UpdateProgress(i));

            await Task.Run(() =>
            {

                Image<Bgr, Byte> im = item.images.Origin.Copy();
                Image<Gray, Byte> imGCpy = im.Convert<Gray, Byte>().Copy();

                Blob.PepperBlob(item);

                // item.images.Color = maskRug.Convert<Bgr, Byte>();

                //   ImageViewer.Show(maskRug.Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_AREA), "maskRug");
            });
        }

        private void ImageGotFocus(object sender, RoutedEventArgs e)
        {
            //Item item = (Item)listView.SelectedItem;
            UpdateImgBox();

        }

        private async void Analyser2_click(object sender, RoutedEventArgs e)
        {

            if (items == null || items.Count < 1)
            {
                System.Windows.Forms.MessageBox.Show("Veuillez ouvrir un dossier d'images avant de lancer l'analyse");
                return;
            }

            int count = 0;
            foreach (Item item in items)
            {

                info_mid.Text = count.ToString() + " / " + items.Count();
                progressBar.Visibility = Visibility.Visible;

                await BlobGlobalAnalyse(item);

                progressBar.Visibility = Visibility.Hidden;

                labelStatsGlobale.Visibility = Visibility.Visible;
                dataGridStats.Visibility = Visibility.Visible;
                UpdateList();
                count++;
            }
            info_mid.Text = "Analyse terminée";
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Start();

        }

        private async Task BlobGlobalAnalyse(Item item)
        {
            Progress<ProgressData> progress = new Progress<ProgressData>(i => UpdateProgress(i));

            await Task.Run(() =>
            {

                Image<Bgr, Byte> im = item.images.Origin.Copy();
                Image<Gray, Byte> imGCpy = im.Convert<Gray, Byte>().Copy();


                Blob.BlobGlobalAnalyse(item, progress);

                //item.images.Color = maskRug.Convert<Bgr, Byte>();

                //ImageViewer.Show(maskRug.Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_AREA), "maskRug");
            });
        }

        private void AffKmean_click(object sender, RoutedEventArgs e)
        {
            UpdateImgBox();
        }

        private void AffKmean_unclick(object sender, RoutedEventArgs e)
        {
            UpdateImgBox();
        }

        private void affMask_click(object sender, RoutedEventArgs e)
        {
            UpdateImgBox();
        }

        private void affMask_unclick(object sender, RoutedEventArgs e)
        {
            UpdateImgBox();
        }
    }
}
