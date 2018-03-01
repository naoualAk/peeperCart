using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
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
using SegmenTool.Modele;
using SegmenTool.Controle;
using System.Diagnostics;
using System.Windows.Threading;

namespace SegmenTool
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        ProcessDataControl p;

        public MainWindow()
        {
            p = new ProcessDataControl();
            InitializeComponent();

            FileInfo fi=null;

            //Properties.Settings.Default.Reset();

            if (Properties.Settings.Default.segmentationPath != "" && File.Exists(Properties.Settings.Default.segmentationPath))
                fi = new FileInfo(Properties.Settings.Default.segmentationPath);
            else
            {
                FileInfo fiExe = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);

                string path= fiExe.Directory + @"\Segmentations\blackSeg-base.xml";

               // MessageBox.Show(path);

                if (File.Exists(path))
                {
                    fi = new FileInfo(path);

                    Properties.Settings.Default.segmentationPath = path;
                    Properties.Settings.Default.Save();
                }
            }


            if (fi!=null && fi.Exists && fi.Extension == ".xml")
            {
                if (p.SegmentationFilePath(Properties.Settings.Default.segmentationPath))
                    textBox_segmentation.Text = Properties.Settings.Default.segmentationPath;
            }

            timer.Tick += new EventHandler(timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            info_mid.Text = "";
        }

        public DispatcherTimer timer = new DispatcherTimer();
        

        private async void button_process_Click(object sender, RoutedEventArgs e)
        {
            bool processEnabled = true;
            if (p.SegmentationFilePath() == "")
            {
                SetWrongTextBoxDesign(textBox_segmentation);
                processEnabled = false;
            }
            if (p.InputFolderPath() == "")
            {
                SetWrongTextBoxDesign(textBox_input_folder);
                processEnabled = false;
            }
            if (p.OutputFolderPath() == "")
            {
                SetWrongTextBoxDesign(textBox_output_folder);
                processEnabled = false;
            }
            if (processEnabled)
            {

                stackPanel.IsEnabled = false;
                progressBar.Visibility = Visibility.Visible;

                await RunT();

                
                progressBar.Visibility = Visibility.Hidden;
                stackPanel.IsEnabled = true;
               
            }

        }

        private void UpdateProgress(ProgressData p)
        {
            if(!p.Ended)
                if (!p.Error)
                {
                    info_mid.Foreground = new SolidColorBrush(Color.FromArgb(255, 56, 90, 182));
                    info_mid.Text = p.CurrentImg.ToString() + "/" + p.TotalImg.ToString();
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
                

                timer.Interval = new TimeSpan(0,0,3);
                timer.Start();
            }
        }

        async Task RunT()
        {
            Progress<ProgressData> progress = new Progress<ProgressData>(i => UpdateProgress(i));

            await Task.Run(() =>
            {
                Run(progress);
            });
        }

        void Run(IProgress<ProgressData> progress)
        {
            //progress.Report("ok");
            SegmentationProcess.Process(p.SegmentationFilePath(), p.InputFolderPath(), p.OutputFolderPath(), progress);
        }


        void SetWrongTextBoxDesign(TextBox t)
        {
            t.BorderBrush = Brushes.Red;
            t.BorderThickness = new Thickness(3);
        }

        void SetGoodTextBoxDesign(TextBox t)
        {
            t.BorderBrush = Brushes.LightGray;
            t.BorderThickness = new Thickness(1);
        }

        private void button_output_folder_Click(object sender, RoutedEventArgs e)
        {

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = Properties.Settings.Default.currentPath;
                dialog.Description = "Select output folder";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {

                    if (p.OutputFolderPath(dialog.SelectedPath))
                        SetGoodTextBoxDesign(textBox_output_folder);

                    textBox_output_folder.Text = p.OutputFolderPath();
                }
            }
        }

        private void button_input_folder_Click(object sender, RoutedEventArgs e)
        {

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = Properties.Settings.Default.currentPath;
                dialog.Description = "Select input folder";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {

                    if (p.InputFolderPath(dialog.SelectedPath))
                    {
                        SetGoodTextBoxDesign(textBox_input_folder);

                        p.ForceOutputFolderPath(dialog.SelectedPath + @"\output");
                        textBox_output_folder.Text = p.OutputFolderPath();
                        SetGoodTextBoxDesign(textBox_output_folder);

                        Properties.Settings.Default.currentPath = dialog.SelectedPath;
                        Properties.Settings.Default.Save();
                    }

                    textBox_input_folder.Text = p.InputFolderPath();
                }
            }

        }

        private void button_segmentation_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Segmentations files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.Title = "Open segmentation file";

            if(Properties.Settings.Default.segmentationPath != "")
            {
                FileInfo fiSeg = new FileInfo(Properties.Settings.Default.segmentationPath);
                openFileDialog.InitialDirectory = fiSeg.DirectoryName;
            }
         
            if (openFileDialog.ShowDialog() == true)
            {
                textBox_segmentation.Text = openFileDialog.FileName;
                if (p.SegmentationFilePath(openFileDialog.FileName))
                {
                    FileInfo fi = new FileInfo(openFileDialog.FileName);
                    Properties.Settings.Default.segmentationPath = fi.FullName;
                    Properties.Settings.Default.Save();
                    SetGoodTextBoxDesign(textBox_segmentation);
                }
            }
        }
    }
}
