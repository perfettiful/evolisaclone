using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EvoImage.Runners;
using EvoImage.Models;
using System.Threading;
using System.IO;
using System.Reflection;
using EvoImage.Threads;
using EvoImage.Util;

namespace EvoImage
{
    public partial class Form1 : Form
    {

        private Rectangle PlotArea;
        private Pen marginsPen;
        Random random = null;

        private Bitmap originalImage;
        private GeneEnvironment env;

        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.ResizeRedraw, true);

            drawingImagePanel.SetBounds(drawingImagePanel.Bounds.X, drawingImagePanel.Bounds.Y, pictureBox1.Width, pictureBox1.Height);
            workingImagePanel.SetBounds(workingImagePanel.Bounds.X, workingImagePanel.Bounds.Y, pictureBox1.Width, pictureBox1.Height);

            marginsPen = new Pen(Brushes.Blue, 2);
            random = new Random(10);

            Rectangle rect = drawingImagePanel.ClientRectangle;
            Size size = new System.Drawing.Size(rect.Size.Width, rect.Height - 50);
            PlotArea = new Rectangle(rect.Location, size);
            var g = drawingImagePanel.CreateGraphics();
            g.DrawRectangle(marginsPen, PlotArea);
            originalImage = new Bitmap(pictureBox1.Image);
            label1.Size = new Size(100, 13);

            timer1.Interval = 10000;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();
            env = new GeneEnvironment();
        }

        private double oldNegativeFitness = 0;
        private double fileNameCounter = 0;
        private double intFileNameCounter = 0;
        void timer1_Tick(object sender, EventArgs e)
        {
            if (CommandChannel.GetCommand() == 2)
            {
                timer1.Stop();
                return;
            }
            Gene gene = GeneContainer.Instance.GetGene();
            if (gene != null)
            {
                Graphics graphics = drawingImagePanel.CreateGraphics();
                graphics.Clear(Color.Black);
                Bitmap bmp = new Bitmap(originalImage.Width, originalImage.Height);
                Graphics drawingGraphics = Graphics.FromImage(bmp);
                graphics.Clear(Color.Black);
                drawingGraphics.Clear(Color.Black);
                for (int ip = 0; ip < gene.Polygons.Count; ip++)
                {
                    if (gene.Polygons[ip] != null && gene.Brushes[ip] != null)
                    {
                        drawingGraphics.DrawPolygon(new Pen(gene.Brushes[ip]), gene.Polygons[ip]);
                        drawingGraphics.FillPolygon(gene.Brushes[ip], gene.Polygons[ip]);
                        graphics.DrawImage(bmp, new Point(0, 0)); 
                    }
                    Thread.Sleep(100);
                }
                
                if (oldNegativeFitness != gene.NegativeFitness)
                {
                    Console.WriteLine("[MainThread],NF: {0}", gene.NegativeFitness);
                    oldNegativeFitness = gene.NegativeFitness;
                    ((Bitmap)bmp.Clone()).Save("_"+gene.Polygons.Count.ToString() + ".bmp");
                    fileNameCounter++;
                    if (fileNameCounter % 10 == 0)
                    {
                        intFileNameCounter++;
                        ((Bitmap)bmp.Clone()).Save(intFileNameCounter.ToString() + ".bmp");
                    }
                }

                graphics.Clear(Color.Black);
                Bitmap demoBmp = (Bitmap)bmp.Clone();
                workingImagePanel.BackColor = Color.Black;
                workingImagePanel.BackgroundImage = demoBmp;
                
                label1.Text = "Error: " + gene.NegativeFitness.ToString()+"   " + gene.Polygons.Count +" polygons used";
            }
        }

        private void startProcessing(object sender, EventArgs e)
        {
            if (env.WorkingGene == null)
            {
                env.OriginalImage = ((Bitmap)this.originalImage.Clone());
                env.NrOfMutationsTries = 8;
                env.NrOfPolygons =8;
                env.GenerateOriginalGene();
            }

            ParameterizedThreadStart pts = new ParameterizedThreadStart(env.Run);
            Thread tr = new Thread(pts);
            tr.Name = "Thread1";
            int nrOfIterations = Convert.ToInt32(numericUpDown1.Value);
            tr.Start(nrOfIterations);

            stopButton.Enabled = true;
            saveResultButton.Enabled = false;
            timer1.Start();
        }

        private void loadImageOrProject(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Project file (*.dat)|*.dat|All files (*.*)|*.*";
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;

                if (File.Exists(file) == true)
                {
                    if (file.ToLower().Trim().EndsWith("dat") == true)
                    {
                        loadGene(file);
                    }
                }
            }
        }

        private void loadGene(string file)
        {
            GeneImageStruct info = FileUtil.LoadGene(file, pictureBox1.Width, pictureBox1.Height);
            env.LoadGene(info.WorkingGene,info.OriginalImage);

            Graphics graphics = drawingImagePanel.CreateGraphics();
            graphics.Clear(Color.Black);
            Bitmap bmp = new Bitmap(originalImage.Width, originalImage.Height);
            Graphics drawingGraphics = Graphics.FromImage(bmp);
            graphics.Clear(Color.Black);
            for (int ip = 0; ip < info.WorkingGene.Polygons.Count; ip++)
            {
                if (info.WorkingGene.Polygons[ip] != null && info.WorkingGene.Brushes[ip] != null)
                {
                    drawingGraphics.DrawPolygon(new Pen(info.WorkingGene.Brushes[ip]), info.WorkingGene.Polygons[ip]);
                    drawingGraphics.FillPolygon(info.WorkingGene.Brushes[ip], info.WorkingGene.Polygons[ip]);
                    graphics.DrawImage(bmp, new Point(0, 0));
                }
            }
            graphics.Clear(Color.Black);
            Bitmap demoBmp = (Bitmap)bmp.Clone();
            workingImagePanel.BackColor = Color.Black;
            workingImagePanel.BackgroundImage = demoBmp;
            pictureBox1.Image = info.OriginalImage;
        }

        private void stopProcessing(object sender, EventArgs e)
        {
            CommandChannel.SetCommand(1);
            stopButton.Enabled = false;
            saveResultButton.Enabled = true;
            timer1.Stop();
        }

        private void quickSaveImage(object sender, EventArgs e)
        {
            if (stopButton.Enabled == false)
            {
                string path = Assembly.GetExecutingAssembly().Location;
                string fileName = Guid.NewGuid().ToString() + ".jpg";
                Bitmap savedImage = new Bitmap(workingImagePanel.BackgroundImage);
                savedImage.Save(fileName);
                MessageBox.Show("File was saved: " + fileName);
            }
        }

        #region save image and data
        private void saveImageAndData(object sender, EventArgs e)
        {
            if (stopButton.Enabled == true) return;

            SaveFileDialog dialog = new SaveFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Gene gene = GeneContainer.Instance.GetGene();
                String fileName = dialog.FileName;
                if (gene != null)
                {
                    FileUtil.SaveGeneToFile(fileName,gene, workingImagePanel.BackgroundImage, pictureBox1.Image);
                    MessageBox.Show("File was saved!");
                }
                else
                {
                    MessageBox.Show("No gene in container");
                }
            }
        }
        #endregion

        private void loadOriginalImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg)|*.jpg|All files (*.*)|*.*";
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(dialog.FileName);
                Bitmap origImage = new Bitmap(bmp,pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = origImage;
                this.originalImage = (Bitmap)origImage.Clone();
                env.OriginalImage = ((Bitmap)origImage.Clone());
                env.NrOfMutationsTries = 8;
                env.NrOfPolygons = 8;
                env.GenerateOriginalGene();
            }
        }
    }
}
