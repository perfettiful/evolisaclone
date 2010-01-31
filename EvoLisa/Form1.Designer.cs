using EvoImage.Threads;
namespace EvoImage
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

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

            CommandChannel.SetCommand(1);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.originalImagePanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.drawingImagePanel = new System.Windows.Forms.Panel();
            this.workingImagePanel = new System.Windows.Forms.Panel();
            this.startButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadImageAndDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveResultButton = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.stopButton = new System.Windows.Forms.Button();
            this.loadOriginalImageButton = new System.Windows.Forms.Button();
            this.originalImagePanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // originalImagePanel
            // 
            this.originalImagePanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.originalImagePanel.Controls.Add(this.pictureBox1);
            this.originalImagePanel.Location = new System.Drawing.Point(432, 32);
            this.originalImagePanel.Name = "originalImagePanel";
            this.originalImagePanel.Size = new System.Drawing.Size(204, 206);
            this.originalImagePanel.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::EvoImage.ImageResources.ml;
            this.pictureBox1.Location = new System.Drawing.Point(2, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(202, 202);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // drawingImagePanel
            // 
            this.drawingImagePanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.drawingImagePanel.Location = new System.Drawing.Point(12, 32);
            this.drawingImagePanel.Name = "drawingImagePanel";
            this.drawingImagePanel.Size = new System.Drawing.Size(204, 206);
            this.drawingImagePanel.TabIndex = 1;
            // 
            // workingImagePanel
            // 
            this.workingImagePanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.workingImagePanel.Location = new System.Drawing.Point(222, 32);
            this.workingImagePanel.Name = "workingImagePanel";
            this.workingImagePanel.Size = new System.Drawing.Size(204, 206);
            this.workingImagePanel.TabIndex = 2;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(12, 280);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 3;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startProcessing);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 245);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Error:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(648, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadImageToolStripMenuItem,
            this.loadImageAndDataToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadImageToolStripMenuItem
            // 
            this.loadImageToolStripMenuItem.Name = "loadImageToolStripMenuItem";
            this.loadImageToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.loadImageToolStripMenuItem.Text = "Load Project";
            this.loadImageToolStripMenuItem.Click += new System.EventHandler(this.loadImageOrProject);
            // 
            // loadImageAndDataToolStripMenuItem
            // 
            this.loadImageAndDataToolStripMenuItem.Name = "loadImageAndDataToolStripMenuItem";
            this.loadImageAndDataToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.loadImageAndDataToolStripMenuItem.Text = "Save Project";
            this.loadImageAndDataToolStripMenuItem.Click += new System.EventHandler(this.saveImageAndData);
            // 
            // saveResultButton
            // 
            this.saveResultButton.Location = new System.Drawing.Point(350, 245);
            this.saveResultButton.Name = "saveResultButton";
            this.saveResultButton.Size = new System.Drawing.Size(75, 23);
            this.saveResultButton.TabIndex = 6;
            this.saveResultButton.Text = "Save result";
            this.saveResultButton.UseVisualStyleBackColor = true;
            this.saveResultButton.Click += new System.EventHandler(this.quickSaveImage);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(12, 245);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(75, 20);
            this.numericUpDown1.TabIndex = 7;
            this.numericUpDown1.Value = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(141, 280);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 8;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopProcessing);
            // 
            // loadOriginalImageButton
            // 
            this.loadOriginalImageButton.Location = new System.Drawing.Point(603, 245);
            this.loadOriginalImageButton.Name = "loadOriginalImageButton";
            this.loadOriginalImageButton.Size = new System.Drawing.Size(33, 23);
            this.loadOriginalImageButton.TabIndex = 9;
            this.loadOriginalImageButton.Text = "...";
            this.loadOriginalImageButton.UseVisualStyleBackColor = true;
            this.loadOriginalImageButton.Click += new System.EventHandler(this.loadOriginalImageButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(648, 315);
            this.Controls.Add(this.loadOriginalImageButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.saveResultButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.workingImagePanel);
            this.Controls.Add(this.drawingImagePanel);
            this.Controls.Add(this.originalImagePanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "EvoImage";
            this.originalImagePanel.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel originalImagePanel;
        private System.Windows.Forms.Panel drawingImagePanel;
        private System.Windows.Forms.Panel workingImagePanel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadImageToolStripMenuItem;
        private System.Windows.Forms.Button saveResultButton;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.ToolStripMenuItem loadImageAndDataToolStripMenuItem;
        private System.Windows.Forms.Button loadOriginalImageButton;
    }
}

