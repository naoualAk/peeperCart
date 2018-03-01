/*=============================================================================
  Copyright (C) 2012 Allied Vision Technologies.  All Rights Reserved.

  Redistribution of this file, in original or modified form, without
  prior written consent of Allied Vision Technologies is prohibited.

-------------------------------------------------------------------------------

  File:        MainForm.Designer.cs

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

namespace AsynchronousGrab
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.m_CameraListTable = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_conf = new System.Windows.Forms.TextBox();
            this.m_CameraList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bt_conf = new System.Windows.Forms.Button();
            this.m_AcquireButton = new System.Windows.Forms.Button();
            this.bt_doss = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_doss = new System.Windows.Forms.TextBox();
            this.m_LogList = new System.Windows.Forms.ListBox();
            this.m_PictureBox = new System.Windows.Forms.PictureBox();
            this.m_LogTable = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.m_DisplayPanel = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.tb_lotName = new System.Windows.Forms.TextBox();
            this.m_CameraListTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_PictureBox)).BeginInit();
            this.m_LogTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.m_DisplayPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_CameraListTable
            // 
            this.m_CameraListTable.ColumnCount = 1;
            this.m_CameraListTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_CameraListTable.Controls.Add(this.tb_lotName, 0, 9);
            this.m_CameraListTable.Controls.Add(this.label3, 0, 0);
            this.m_CameraListTable.Controls.Add(this.m_AcquireButton, 0, 10);
            this.m_CameraListTable.Controls.Add(this.tb_conf, 0, 1);
            this.m_CameraListTable.Controls.Add(this.m_CameraList, 0, 4);
            this.m_CameraListTable.Controls.Add(this.label1, 0, 3);
            this.m_CameraListTable.Controls.Add(this.bt_conf, 0, 2);
            this.m_CameraListTable.Controls.Add(this.bt_doss, 0, 7);
            this.m_CameraListTable.Controls.Add(this.label4, 0, 5);
            this.m_CameraListTable.Controls.Add(this.tb_doss, 0, 6);
            this.m_CameraListTable.Controls.Add(this.label5, 0, 8);
            this.m_CameraListTable.Location = new System.Drawing.Point(0, 0);
            this.m_CameraListTable.Name = "m_CameraListTable";
            this.m_CameraListTable.RowCount = 11;
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.m_CameraListTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.m_CameraListTable.Size = new System.Drawing.Size(198, 472);
            this.m_CameraListTable.TabIndex = 0;
            this.m_CameraListTable.Paint += new System.Windows.Forms.PaintEventHandler(this.m_CameraListTable_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(106)))), ((int)(((byte)(162)))));
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(198, 13);
            this.label3.TabIndex = 5;
            //this.label3.Text = "Configuration caméra";
            this.label3.Text = "Camera configuration";
            // 
            // tb_conf
            // 
            this.tb_conf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_conf.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tb_conf.ForeColor = System.Drawing.SystemColors.InfoText;
            this.tb_conf.Location = new System.Drawing.Point(3, 16);
            this.tb_conf.Name = "tb_conf";
            this.tb_conf.ReadOnly = true;
            this.tb_conf.Size = new System.Drawing.Size(192, 20);
            this.tb_conf.TabIndex = 1;
            // 
            // m_CameraList
            // 
            this.m_CameraList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_CameraList.FormattingEnabled = true;
            this.m_CameraList.IntegralHeight = false;
            this.m_CameraList.Location = new System.Drawing.Point(0, 98);
            this.m_CameraList.Margin = new System.Windows.Forms.Padding(0, 3, 0, 20);
            this.m_CameraList.Name = "m_CameraList";
            this.m_CameraList.Size = new System.Drawing.Size(198, 127);
            this.m_CameraList.TabIndex = 1;
            this.m_CameraList.Click += new System.EventHandler(this.m_CameraList_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(106)))), ((int)(((byte)(162)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(0, 82);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cameras:";
            // 
            // bt_conf
            // 
            this.bt_conf.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.bt_conf.Location = new System.Drawing.Point(61, 42);
            this.bt_conf.Name = "bt_conf";
            this.bt_conf.Size = new System.Drawing.Size(75, 23);
            this.bt_conf.TabIndex = 6;
            //this.bt_conf.Text = "Ouvrir";
            this.bt_conf.Text = "Open";
            this.bt_conf.UseVisualStyleBackColor = true;
            this.bt_conf.Click += new System.EventHandler(this.bt_conf_Click);
            // 
            // m_AcquireButton
            // 
            this.m_AcquireButton.AutoSize = true;
            this.m_AcquireButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.m_AcquireButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_AcquireButton.Enabled = false;
            this.m_AcquireButton.Location = new System.Drawing.Point(5, 408);
            this.m_AcquireButton.Margin = new System.Windows.Forms.Padding(5, 3, 5, 0);
            this.m_AcquireButton.Name = "m_AcquireButton";
            this.m_AcquireButton.Size = new System.Drawing.Size(188, 64);
            this.m_AcquireButton.TabIndex = 3;
            //this.m_AcquireButton.Text = "Commencer";
            this.m_AcquireButton.Text = "Start";
            this.m_AcquireButton.UseVisualStyleBackColor = true;
            this.m_AcquireButton.Click += new System.EventHandler(this.AcquireButton_Click);
            // 
            // bt_doss
            // 
            this.bt_doss.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.bt_doss.Location = new System.Drawing.Point(61, 287);
            this.bt_doss.Name = "bt_doss";
            this.bt_doss.Size = new System.Drawing.Size(75, 23);
            this.bt_doss.TabIndex = 7;
            //this.bt_doss.Text = "Ouvrir";
            this.bt_doss.Text = "Open";
            this.bt_doss.UseVisualStyleBackColor = true;
            this.bt_doss.Click += new System.EventHandler(this.bt_doss_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(106)))), ((int)(((byte)(162)))));
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(0, 245);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(198, 13);
            this.label4.TabIndex = 8;
            //this.label4.Text = "Dossier d\'enregistrement";
            this.label4.Text = "Registration folder";
            // 
            // tb_doss
            // 
            this.tb_doss.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_doss.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tb_doss.Location = new System.Drawing.Point(3, 261);
            this.tb_doss.Name = "tb_doss";
            this.tb_doss.ReadOnly = true;
            this.tb_doss.Size = new System.Drawing.Size(192, 20);
            this.tb_doss.TabIndex = 9;
            // 
            // m_LogList
            // 
            this.m_LogList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_LogList.FormattingEnabled = true;
            this.m_LogList.IntegralHeight = false;
            this.m_LogList.Location = new System.Drawing.Point(0, 13);
            this.m_LogList.Margin = new System.Windows.Forms.Padding(0);
            this.m_LogList.Name = "m_LogList";
            this.m_LogList.Size = new System.Drawing.Size(934, 136);
            this.m_LogList.TabIndex = 1;
            // 
            // m_PictureBox
            // 
            this.m_PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_PictureBox.Location = new System.Drawing.Point(0, 0);
            this.m_PictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.m_PictureBox.Name = "m_PictureBox";
            this.m_PictureBox.Size = new System.Drawing.Size(726, 550);
            this.m_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.m_PictureBox.TabIndex = 2;
            this.m_PictureBox.TabStop = false;
            this.m_PictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox_Paint);
            this.m_PictureBox.DoubleClick += new System.EventHandler(this.PictureBox_DoubleClick);
            // 
            // m_LogTable
            // 
            this.m_LogTable.ColumnCount = 1;
            this.m_LogTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_LogTable.Controls.Add(this.m_LogList, 0, 1);
            this.m_LogTable.Controls.Add(this.label2, 0, 0);
            this.m_LogTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_LogTable.Location = new System.Drawing.Point(0, 0);
            this.m_LogTable.Name = "m_LogTable";
            this.m_LogTable.RowCount = 2;
            this.m_LogTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.m_LogTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.m_LogTable.Size = new System.Drawing.Size(934, 149);
            this.m_LogTable.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Log messages:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.m_LogTable);
            this.splitContainer1.Panel2MinSize = 50;
            this.splitContainer1.Size = new System.Drawing.Size(934, 707);
            this.splitContainer1.SplitterDistance = 554;
            this.splitContainer1.TabIndex = 8;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.m_CameraListTable);
            this.splitContainer2.Panel1MinSize = 100;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.m_DisplayPanel);
            this.splitContainer2.Panel2MinSize = 100;
            this.splitContainer2.Size = new System.Drawing.Size(934, 554);
            this.splitContainer2.SplitterDistance = 200;
            this.splitContainer2.TabIndex = 0;
            // 
            // m_DisplayPanel
            // 
            this.m_DisplayPanel.AutoScroll = true;
            this.m_DisplayPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_DisplayPanel.Controls.Add(this.m_PictureBox);
            this.m_DisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_DisplayPanel.Location = new System.Drawing.Point(0, 0);
            this.m_DisplayPanel.Name = "m_DisplayPanel";
            this.m_DisplayPanel.Size = new System.Drawing.Size(730, 554);
            this.m_DisplayPanel.TabIndex = 3;
            this.m_DisplayPanel.DoubleClick += new System.EventHandler(this.DisplayPanel_DoubleClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(106)))), ((int)(((byte)(162)))));
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label5.Location = new System.Drawing.Point(0, 327);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(198, 13);
            this.label5.TabIndex = 10;
            //this.label5.Text = "Nom du lot";
            this.label5.Text = "Lot name";
            // 
            // tb_lotName
            // 
            this.tb_lotName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_lotName.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tb_lotName.Location = new System.Drawing.Point(3, 343);
            this.tb_lotName.Name = "tb_lotName";
            this.tb_lotName.Size = new System.Drawing.Size(192, 20);
            this.tb_lotName.TabIndex = 11;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(940, 713);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 350);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "PepperCam";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.m_CameraListTable.ResumeLayout(false);
            this.m_CameraListTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_PictureBox)).EndInit();
            this.m_LogTable.ResumeLayout(false);
            this.m_LogTable.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.m_DisplayPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel m_CameraListTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox m_CameraList;
        private System.Windows.Forms.ListBox m_LogList;
        private System.Windows.Forms.PictureBox m_PictureBox;
        private System.Windows.Forms.TableLayoutPanel m_LogTable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel m_DisplayPanel;
        private System.Windows.Forms.Button m_AcquireButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_conf;
        private System.Windows.Forms.Button bt_conf;
        private System.Windows.Forms.Button bt_doss;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_doss;
        private System.Windows.Forms.TextBox tb_lotName;
        private System.Windows.Forms.Label label5;
    }
}

