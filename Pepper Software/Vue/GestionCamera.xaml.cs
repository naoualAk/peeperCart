using AVT.VmbAPINET;
using Emgu.CV;
using Emgu.CV.Structure;
using pepperSoft.Modele.AlliedVision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace pepperSoft.Vue
{
    /// <summary>
    /// Logique d'interaction pour GestionCamera.xaml
    /// </summary>
    public partial class GestionCamera : Window
    {

        /// <summary>
        /// The VimbaHelper (see VimbaHelper Class)
        /// </summary>
        private VimbaHelper m_VimbaHelper = null;
        private bool m_Acquiring = false;
        int imgCount = 0;

        public GestionCamera()
        {
            InitializeComponent();


            Init();
        }

        public void Init()
        {
            try
            {
                // Start up Vimba SDK
                VimbaHelper vimbaHelper = new VimbaHelper();
                vimbaHelper.Startup(this.OnCameraListChanged);
                m_VimbaHelper = vimbaHelper;

                LogMessage("Vimba Version V" + m_VimbaHelper.GetVersion());
                try
                {
                    UpdateCameraList();
                    UpdateControls();
                }
                catch (Exception exception)
                {
                    LogError("Could not update camera list. Reason: " + exception.Message);
                }
            }
            catch (Exception exception)
            {
                LogError("Could not startup Vimba API. Reason: " + exception.Message);
            }

            tb_cameraConf.Text = Properties.Settings.Default.cfgPath;
            imgBox.SizeMode = PictureBoxSizeMode.Zoom;
            imgBox.HorizontalScrollBar.Visible = false;
            imgBox.VerticalScrollBar.Visible = false;
            imgBox.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;

            this.Closed += Clear;
        }

        private void Clear(object sender, EventArgs e)
        {
            if (null != m_VimbaHelper)
            {
                try
                {
                    try
                    {
                        // Shutdown Vimba SDK when application exits
                        m_VimbaHelper.Shutdown();
                    }
                    finally
                    {
                        m_VimbaHelper = null;
                    }
                }
                catch (Exception exception)
                {
                    LogError("Could not shutdown Vimba API. Reason: " + exception.Message);
                }
            }
        }

        private void UpdateControls()
        {
        }

        private void OnCameraListChanged(object sender, EventArgs args)
        {
            // Start an asynchronous invoke in case this method was not
            // called by the GUI thread.
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                //LogMessage("Access is closed");
                Dispatcher.Invoke(new CameraListChangedHandler(this.OnCameraListChanged), sender, args);
                return;
            }

            if (null != m_VimbaHelper)
            {
                //LogMessage("Access is open");
                try
                {
                    UpdateCameraList();
                    LogMessage("Camera list updated.");
                }
                catch (Exception exception)
                {
                    LogError("Could not update camera list. Reason: " + exception.Message);
                }
            }
        }

        private void LogMessage(string message)
        {


            if (null == message)
            {
                throw new ArgumentNullException("message");
            }

            tb_cameraInfo.AppendText("\n" + string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}: {1}", DateTime.Now, message));

            if (tb_cameraInfo.LineCount != -1)
            {
                tb_cameraInfo.ScrollToLine(tb_cameraInfo.LineCount - 1);
            }


        }

        /// <summary>
        /// Add an error log message and show an error message box
        /// </summary>
        /// <param name="message">The message</param>
        private void LogError(string message)
        {
            LogMessage(message);

            System.Windows.Forms.MessageBox.Show(message, "Message", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
        }

        private void UpdateCameraList()
        {
            // Remember the old selection (if there was any)y
            CameraInfo oldSelectedItem = listBox_camera.SelectedItem as CameraInfo;
            listBox_camera.Items.Clear();

            List<CameraInfo> cameras = m_VimbaHelper.CameraList;

            CameraInfo newSelectedItem = null;
            foreach (CameraInfo cameraInfo in cameras)
            {
                listBox_camera.Items.Add(cameraInfo);

                if (null == newSelectedItem)
                {
                    // At least select the first camera
                    newSelectedItem = cameraInfo;
                }
                else if (null != oldSelectedItem)
                {
                    // If the previous selected camera is still available
                    // then prefer this camera.
                    if (string.Compare(newSelectedItem.ID, cameraInfo.ID, StringComparison.Ordinal) == 0)
                    {
                        newSelectedItem = cameraInfo;
                    }
                }
            }

            // If available select a camera and adjust the status of the "Start acquisition" button
            if (null != newSelectedItem)
            {
                listBox_camera.SelectedItem = newSelectedItem;
                bt_cameraConnect.IsEnabled = true;
            }
            else
            {
                bt_cameraConnect.IsEnabled = false;
            }
        }

        private void bt_cameraConf_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "configuration file (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.Title = "Open a configuration file ";
            FileInfo fiSeg;

            try
            {
                fiSeg = new FileInfo(Properties.Settings.Default.cfgPath);
                openFileDialog.InitialDirectory = fiSeg.DirectoryName;
            }
            catch
            {

            }

            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fi = new FileInfo(openFileDialog.FileName);
                Properties.Settings.Default.cfgPath = fi.FullName;
                Properties.Settings.Default.Save();
                tb_cameraConf.Text = Properties.Settings.Default.cfgPath;
            }
        }

        private void bt_cameraConnect_Click(object sender, RoutedEventArgs e)
        {
            // Close the camera if it was opened
            /*   m_VimbaHelper.CloseCamera();

               // Determine selected camera
               CameraInfo selectedItem = listBox_camera.SelectedItem as CameraInfo;
               if (null == selectedItem)
               {
                   throw new NullReferenceException("No camera selected.");
               }

               // Open selected camera
               m_VimbaHelper.OpenCamera(selectedItem.ID);


               LogMessage("Caméra " + m_VimbaHelper.cameraName() + " connectée");
               UpdateControls();

               m_VimbaHelper.LoadCameraSettings(Properties.Settings.Default.cfgPath);

               FileInfo fi = new FileInfo(Properties.Settings.Default.cfgPath);

               LogMessage("Configuration " + fi.Name + " chargée");*/
        }


        private void bt_saveDoss_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = Properties.Settings.Default.savePath;
                dialog.Description = "Select output folder";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.savePath = dialog.SelectedPath;
                    Properties.Settings.Default.Save();

                    tb_saveDoss.Text = Properties.Settings.Default.savePath;
                }
            }
        }

        /// <summary>
        /// Handles the FrameReceived event
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The FrameEventArgs</param>
        private void OnFrameReceived(object sender, FrameEventArgs args)
        {
            // Start an async invoke in case this method was not
            // called by the GUI thread.
            /* if (!CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
             {
                 Dispatcher.Invoke(new FrameReceivedHandler(this.OnFrameReceived), sender, args);
                 return;
             }*/
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                if (true == m_Acquiring)
                {
                    // Display image
                    System.Drawing.Image image = args.Image;

                    if (null != image)
                    {
                        Image<Bgr, Byte> imB = new Image<Bgr, byte>(new System.Drawing.Bitmap(image));
                        imgBox.Image = imB;
                        imgCount++;
                        //imB.Save(Properties.Settings.Default.savePath + @"\img" + imgCount + ".bmp");
                        LogMessage("Image N°" + imgCount + " sauvegardée dans " + Properties.Settings.Default.savePath);

                    }
                    else
                    {
                        LogMessage("An acquisition error occurred. Reason: " + args.Exception.Message);

                        try
                        {
                            try
                            {
                                // Start asynchronous image acquisition (grab) in selected camera
                                m_VimbaHelper.StopContinuousImageAcquisition();
                            }
                            finally
                            {
                                m_Acquiring = false;
                                UpdateControls();
                                listBox_camera.IsEnabled = true;
                            }

                            LogMessage("Asynchronous image acquisition stopped.");
                        }
                        catch (Exception exception)
                        {
                            LogError("Error while stopping asynchronous image acquisition. Reason: " + exception.Message);
                        }
                    }
                }
            }));
        }


        private void bt_start_Click(object sender, RoutedEventArgs e)
        {
            /*if (false == m_Acquiring)
           {
               try
               {
                   imgCount = 0;

                   // Determine selected camera
                   CameraInfo selectedItem = listBox_camera.SelectedItem as CameraInfo;
                   if (null == selectedItem)
                   {
                       throw new NullReferenceException("No camera selected.");
                   }

                   // Open the camera if it was not opened before
                   m_VimbaHelper.OpenCamera(selectedItem.ID);

                   // Start asynchronous image acquisition (grab) in selected camera
                   m_VimbaHelper.StartContinuousImageAcquisition(this.OnFrameReceived);

                   m_Acquiring = true;
                   UpdateControls();

                   // Disable the camera list to inhibit changing the camera
                   listBox_camera.IsEnabled = false;
                   bt_start.Content = "Arreter";

                   LogMessage("Asynchronous image acquisition started.");
               }
               catch (Exception exception)
               {
                   LogError("Could not start asynchronous image acquisition. Reason: " + exception.Message);
               }
           }
           else
           {
               try
               {
                   try
                   {
                       // Start asynchronous image acquisition (grab) in selected camera
                       m_VimbaHelper.StopContinuousImageAcquisition();
                   }
                   finally
                   {
                       m_Acquiring = false;
                       bt_start.Content = "Commencer";
                       UpdateControls();
                   }

                   LogMessage("Asynchronous image acquisition stopped.");
               }
               catch (Exception exception)
               {
                   LogError("Error while stopping asynchronous image acquisition. Reason: " + exception.Message);
               }
               // Re-enable the camera list
               listBox_camera.IsEnabled = true;
           }*/

            try
            {
                //Determine selected camera
                CameraInfo selectedItem = listBox_camera.SelectedItem as CameraInfo;
                if (null == selectedItem)
                {
                    throw new NullReferenceException("No camera selected.");
                }

                //Acquire an image synchronously (snap) from selected camera
                System.Drawing.Image image = m_VimbaHelper.AcquireSingleImage(selectedItem.ID);


                //Display image
                // Image<Bgr, Byte> imB = new Image<Bgr, byte>(new System.Drawing.Bitmap(image));
                //imgBox.Image = imB;
                imgCount++;
                LogMessage("Image N°" + imgCount + " sauvegardée dans " + Properties.Settings.Default.savePath);

                LogMessage("Image acquired synchronously.");

            }
            catch (Exception exception)
            {
                LogError("Could not acquire image. Reason: " + exception.Message);
            }
        }
    }
}
