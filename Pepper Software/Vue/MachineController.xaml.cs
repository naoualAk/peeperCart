using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace pepperSoft
{
    /// <summary>
    /// Logique d'interaction pour MachineController.xaml
    /// </summary>
    public partial class MachineController : Window
    {

      
        public MachineController()
        {
            InitializeComponent();
            label_path.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label_path.VerticalContentAlignment = VerticalAlignment.Center;



            try
            {
                if ((File.GetAttributes(Properties.Settings.Default.savePath) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    label_path.Content = "Dossier de sauvegarde : " + "\n" + Properties.Settings.Default.savePath;
                    path = Properties.Settings.Default.savePath;
                }  
            }
            catch { }


        

            imgBox.SizeMode = PictureBoxSizeMode.Zoom;

           // this.Closed += CloseDevices;
        }



        private void combobox_belt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox cb = (System.Windows.Controls.ComboBox)sender;
           
            
        }


        private bool enabled = false;
        private int IDSCounter = 0;
        private int imgCounter = 0;
        private bool hasTakeIDSBackground;
        private static int NB_IMAGE_FOR_BG = 20;

      


        private void button_go_Click(object sender, RoutedEventArgs e)
        {
            if (!enabled && path != null && path != "")
            {
                try
                {

                    if (!Directory.Exists(path + @"\segmented"))
                        try
                        {
                            System.IO.Directory.CreateDirectory(path + @"\segmented");
                        }
                        catch
                        {
                            info.Text = "Impossible de creer le dossier fond.";
                        }

                  //  cameraIDS.Start();
                    enabled = true;
                    button_go.Content = "Arreter la machine";
                    info.Text = "Acquisition des images de fond";
                    info_mid.Text = "";
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                //    cameraIDS.Stop();
                    IDSCounter = 0;
                    imgCounter = 0;
                    enabled = false;
                    hasTakeIDSBackground = false;
                    button_go.Content = "Démarrer la machine";
                    progressBar.Visibility = Visibility.Hidden;
                    info.Text = "";
                   if (path == null || path == "") info_mid.Text = "Veuillez entrer un chemin.";
                }
                catch
                {

                }
            }
        }
        string path;

        private void button_path_Click(object sender, RoutedEventArgs e)
        {


            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = path;
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    label_path.Content = "Dossier de sauvegarde : " + "\n" + fbd.SelectedPath;
                    path = fbd.SelectedPath;
                    Properties.Settings.Default.savePath = path;
                    Properties.Settings.Default.Save();
                }
                info_mid.Text = "";
            }

        }

        private float size = 0;

        private void textBox_length_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (sender == null)
                return;
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
            if (!IsTextAllowed(tb.Text))
                tb.Text = "";

            if (!float.TryParse(tb.Text.Replace('.', ','), out size)) size = 0;
        }

        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        bool saveBrute = false;

        private void checkBox_save_Checked(object sender, RoutedEventArgs e)
        {
            saveBrute = true;
        }

        private void checkBox_save_Unchecked(object sender, RoutedEventArgs e)
        {
            saveBrute = false;
        }
    }
}
