/*=============================================================================
  Copyright (C) 2012 Allied Vision Technologies.  All Rights Reserved.

  Redistribution of this file, in original or modified form, without
  prior written consent of Allied Vision Technologies is prohibited.

-------------------------------------------------------------------------------

  File:        MainForm.cs

  Description: Forms class for the GUI of the AsynchronousGrab example of
               VimbaNET.

-------------------------------------------------------------------------------

  THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR IMPLIED
  WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF TITLE,
  NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS FOR A PARTICULAR  PURPOSE ARE
  DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT,
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
  AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

=============================================================================*/
using Emgu.CV;
using Emgu.CV.Structure;


namespace AsynchronousGrab
{

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// The MainForm (GUI) Class implementation
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The VimbaHelper (see VimbaHelper Class)
        /// </summary>
        private VimbaHelper m_VimbaHelper = null;

        /// <summary>
        /// Flag to indicate if the camera is acquiring images
        /// </summary>
        private bool m_Acquiring = false;

        /// <summary>
        /// Initializes a new instance of the MainForm class.
        /// </summary>
        public MainForm()
        {
            

            InitializeComponent();


            //tb_lotName.Text = "Entrer le nom du lot ici...";
            tb_lotName.Text = "Enter the lot name here...";
            tb_lotName.ForeColor = Color.Gray;
           
            tb_lotName.GotFocus += RemoveText;
            tb_lotName.LostFocus += AddText;
            tb_lotName.TextChanged += SaveNameLot;

            Settings.Default.lotName = "";
        }

        private void SaveNameLot(object sender, EventArgs e)
        {
            //if (tb_lotName.Text == "Entrer le nom du lot ici..." || tb_lotName.Text == "")
            if (tb_lotName.Text == "Enter the lot name here..." || tb_lotName.Text == "")
                Settings.Default.lotName = "";
            else Settings.Default.lotName = tb_lotName.Text;

            Settings.Default.Save();
        }

        public void  RemoveText(object sender, EventArgs e)
        {
            tb_lotName.Text = "";
            tb_lotName.ForeColor = Color.Black;
        }

        public void AddText(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tb_lotName.Text))
            {
                tb_lotName.ForeColor = Color.Gray;
                tb_lotName.Text = "Enter the lot name here...";
                //tb_lotName.Text = "Entrer le nom du lot ici...";
            }
                
        }

        /// <summary>
        /// Add log message to logging list box
        /// </summary>
        /// <param name="message">The message</param>
        private void LogMessage(string message)
        {
            if (null == message)
            {
                throw new ArgumentNullException("message");
            }

            int index = m_LogList.Items.Add(string.Format("{0:dd-MM-yyyy HH:mm:ss.fff}: {1}", DateTime.Now, message));
            m_LogList.TopIndex = index;
        }

        /// <summary>
        /// Add an error log message and show an error message box
        /// </summary>
        /// <param name="message">The message</param>
        private void LogError(string message)
        {
            LogMessage(message);

            MessageBox.Show(message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Update the camera List in the UI
        /// </summary>
        private void UpdateCameraList()
        {
            // Remember the old selection (if there was any)y
            CameraInfo oldSelectedItem = m_CameraList.SelectedItem as CameraInfo;
            m_CameraList.Items.Clear();

            List<CameraInfo> cameras = m_VimbaHelper.CameraList;

            CameraInfo newSelectedItem = null;
            foreach (CameraInfo cameraInfo in cameras)
            {
                m_CameraList.Items.Add(cameraInfo);

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
                m_CameraList.SelectedItem = newSelectedItem;
                m_AcquireButton.Enabled = true;
            }
            else
            {
                m_AcquireButton.Enabled = false;
            }
        }

        /// <summary>
        /// Handles the CameraListChanged event
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The EventArgs</param>
        private void OnCameraListChanged(object sender, EventArgs args)
        {
            // Start an asynchronous invoke in case this method was not
            // called by the GUI thread.
            if (InvokeRequired == true)
            {
                BeginInvoke(new CameraListChangedHandler(this.OnCameraListChanged), sender, args);
                return;
            }

            if (null != m_VimbaHelper)
            {
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

        int imgCount=0;

        /// <summary>
        /// Handles the FrameReceived event
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The FrameEventArgs</param>
        private void OnFrameReceived(object sender, FrameEventArgs args)
        {
            // Start an async invoke in case this method was not
            // called by the GUI thread.
            if (InvokeRequired == true)
            {
                BeginInvoke(new FrameReceivedHandler(this.OnFrameReceived), sender, args);
                return;
            }

            if (true == m_Acquiring)
            {
                // Display image
                Image image = args.Image;
                if (null != image)
                {
                   // m_PictureBox.Image = image;

                    Image<Bgr, Byte> imB = new Image<Bgr, byte>(new System.Drawing.Bitmap(image));
                    m_PictureBox.Image = imB.ToBitmap();
                    imgCount++;

                    string imgName = "img_" + string.Format("{0:dd-MM-yyyy_HH-mm-ss-fff}", DateTime.Now);

                    string path = Settings.Default.savePath + @"\" + Settings.Default.lotName + @"\";
                    imB.Save(path + imgName + ".bmp");
                    //LogMessage("Image N°" + imgCount + " sauvegardée :\n" + path + imgName + ".bmp");
                    LogMessage("Image N°" + imgCount + " saved :\n" + path + imgName + ".bmp");
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
                            m_CameraList.Enabled = true;
                        }

                        LogMessage("Asynchronous image acquisition stopped.");
                    }
                    catch (Exception exception)
                    {
                        LogError("Error while stopping asynchronous image acquisition. Reason: " + exception.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Form_Load event of this (MainForm)
        /// </summary>
        /// <param name="sender">The Sender (not used)</param>
        /// <param name="e">The EventArgs (not used)</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Start up Vimba SDK
                VimbaHelper vimbaHelper = new VimbaHelper();
                vimbaHelper.Startup(this.OnCameraListChanged);
                m_VimbaHelper = vimbaHelper;

                Text = String.Format("{0} (Vimba Version V{1})", Text, m_VimbaHelper.GetVersion());
                try
                {
                     m_CameraList.Select();
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
        }

        /// <summary>
        /// Handles the FormClosing event of the MainForm
        /// </summary>
        /// <param name="sender">The Sender (not used)</param>
        /// <param name="e">The EventArgs (not used)</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
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

        /// <summary>
        /// Updates the state of the Acquisition and Trigger controls
        /// </summary>
        private void UpdateControls()
        {
            if (true == m_Acquiring)
            {
                //m_AcquireButton.Text = "Arreter les acquisitions";
                m_AcquireButton.Text = "Stop acquisitions";
                m_AcquireButton.Enabled = true;
            }
            else
            {
                //m_AcquireButton.Text = "Commencer les acquisitions";
                m_AcquireButton.Text = "Start acquisitions";

                CameraInfo cameraInfo = m_CameraList.SelectedItem as CameraInfo;
                if (null != cameraInfo)
                {
                    // Enable button if a camera is selected
                    m_AcquireButton.Enabled = true;
                }
                else
                {
                    // Disable button if no camera is selected
                    m_AcquireButton.Enabled = false;
                }
            }

            tb_doss.Text = Settings.Default.savePath;
            tb_conf.Text = Settings.Default.cfgPath;

        }

        /// <summary>
        /// Handles the click event of the m_AcquireButton
        /// </summary>
        /// <param name="sender">The Sender (not used)</param>
        /// <param name="e">The EventArgs (not used)</param>
        private void AcquireButton_Click(object sender, EventArgs e)
        {
            if (false == m_Acquiring)
            {
                try
                {

                   

                    // Determine selected camera
                    CameraInfo selectedItem = m_CameraList.SelectedItem as CameraInfo;
                    if (null == selectedItem)
                    {
                        throw new NullReferenceException("No camera selected.");
                    }

 
                    // Open the camera if it was not opened before
                    m_VimbaHelper.OpenCamera(selectedItem.ID);
                    LogMessage("Camera is opened");

                    // Load parameters on camera
                    m_VimbaHelper.LoadCameraSettings(Settings.Default.cfgPath);
                    LogMessage("Camera settings is loaded");

                    if (CreateFolder())
                        LogMessage("Folder " + Settings.Default.savePath + @"\" + Settings.Default.lotName + " created");
                    else
                    {
                        LogMessage("Folder creation failed : " + Settings.Default.savePath + @"\" + Settings.Default.lotName);
                        return;
                    }


                    // Start asynchronous image acquisition (grab) in selected camera
                    m_VimbaHelper.StartContinuousImageAcquisition(this.OnFrameReceived);

                    m_Acquiring = true;
                    UpdateControls();

                    // Disable the camera list to inhibit changing the camera
                    m_CameraList.Enabled = false;

                    LogMessage("Asynchronous image acquisition started.");
                    imgCount = 0;
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
                        UpdateControls();
                    }

                    LogMessage("Asynchronous image acquisition stopped.");
                }
                catch (Exception exception)
                {
                    LogError("Error while stopping asynchronous image acquisition. Reason: " + exception.Message);
                }

                // Re-enable the camera list
                m_CameraList.Enabled = true;
            }
        }

        private bool CreateFolder()
        {
            string path;
            if (Directory.Exists(Settings.Default.savePath + @"\" + Settings.Default.lotName))
                return true;

            if (Settings.Default.savePath == "")
                return false;
            if(Settings.Default.lotName == "")
                return false;

            System.IO.Directory.CreateDirectory(Settings.Default.savePath + @"\" + Settings.Default.lotName);

            return true;
        }

        /// <summary>
        /// Toggle mode between zoomed and 1:1 image display
        /// </summary>
        private void ToogleDisplayMode()
        {
            if (PictureBoxSizeMode.Zoom == m_PictureBox.SizeMode)
            {
                m_PictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                m_PictureBox.Dock = DockStyle.None;
            }
            else
            {
                m_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                m_PictureBox.Dock = DockStyle.Fill;
            }
        }

        /// <summary>
        /// Handles the DoubleClick event of the m_PictureBox
        /// </summary>
        /// <param name="sender">The Sender (not used)</param>
        /// <param name="e">The EventArgs (not used)</param>
        private void PictureBox_DoubleClick(object sender, EventArgs e)
        {
            ToogleDisplayMode();
        }

        /// <summary>
        /// Handles the DoubleClick event of the m_DisplayPanel
        /// </summary>
        /// <param name="sender">The Sender (not used)</param>
        /// <param name="e">The EventArgs (not used)</param>
        private void DisplayPanel_DoubleClick(object sender, EventArgs e)
        {
            ToogleDisplayMode();
        }

        /// <summary>
        /// Handles the Paint event of the m_PictureBox
        /// </summary>
        /// <param name="sender">The Sender (not used)</param>
        /// <param name="e">The EventArgs (not used)</param>
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            VimbaHelper.ImageInUse = true;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the m_SoftwareTriggerCheckbox
        /// </summary>
        /// <param name="sender">The Sender (not used)</param>
        /// <param name="e">The EventArgs (not used)</param>
        private void SoftwareTriggerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_VimbaHelper.IsTriggerAvailable)
            {
               // m_VimbaHelper.EnableSoftwareTrigger(m_SoftwareTriggerCheckbox.Checked);
            }
            UpdateControls();
        }

        /// <summary>
        /// Handles the Click event of the  m_SoftwareTriggerButton
        /// </summary>
        /// <param name="sender">The Sender (not used)</param>
        /// <param name="e">The EventArgs (not used)</param>
        private void SoftwareTriggerButton_Click(object sender, EventArgs e)
        {
            m_VimbaHelper.TriggerSoftwareTrigger();
        }

        /// <summary>
        /// Handles the selection of an entry in the camera list. Open the camera to find
        /// out about the presence of software trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_CameraList_Click(object sender, EventArgs e)
        {
            // Close the camera if it was opened
            m_VimbaHelper.CloseCamera();
          

            // Determine selected camera
            CameraInfo selectedItem = m_CameraList.SelectedItem as CameraInfo;
            if (null == selectedItem)
            {
                LogMessage("No camera selected. ");
                return;
            }

            // Open selected camera
            m_VimbaHelper.OpenCamera(selectedItem.ID);

            UpdateControls();

            // In case that the check box is still checked, enable the software trigger
            if (m_VimbaHelper.IsTriggerAvailable)
            {
                //m_VimbaHelper.EnableSoftwareTrigger(m_SoftwareTriggerCheckbox.Checked);
            }
        }

        private void m_CameraListTable_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bt_conf_Click(object sender, EventArgs e)
        {


            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "configuration file (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.Title = "Open a configuration file ";

            FileInfo fiSeg;

            try
            {
                fiSeg = new FileInfo(Settings.Default.cfgPath);
                openFileDialog.InitialDirectory = fiSeg.DirectoryName;
            }
            catch
            {

            }

            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                FileInfo fi = new FileInfo(openFileDialog.FileName);
                Settings.Default.cfgPath = fi.FullName;
                Settings.Default.Save();
                tb_conf.Text = Settings.Default.cfgPath;
            }
            Console.WriteLine(result); // <-- For debugging use.

        }

        private void bt_doss_Click(object sender, EventArgs e)
        {


            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Settings.Default.savePath;
                fbd.Description = "Select output folder";
                DialogResult result = fbd.ShowDialog();


                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Settings.Default.savePath = fbd.SelectedPath;
                    Settings.Default.Save();

                    tb_doss.Text = Settings.Default.savePath;
                }
            }
        }
    }
}