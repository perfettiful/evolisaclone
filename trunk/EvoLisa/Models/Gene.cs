using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace EvoImage.Models
{

    struct Pixel
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;
    }

    public class Gene
    {
        private Bitmap originalImage;
        private Random random = new Random();
        private int RandSeed = 2;

        #region properties
        public List<Point[]> Polygons
        {
            get;
            set;
        }

        public List<Brush> Brushes
        {
            get;
            set;
        }
        
        public Bitmap OriginalImage
        {
            get
            {
                return originalImage;
            }
            set
            {
                this.originalImage = value;
            }
        }

        public double NegativeFitness
        {
            get;
            set;
        }

        public int NrOfPolygons
        {
            get;
            set;
        }
        #endregion

        public void SetOriginaImage(Bitmap originalImage)
        {
            this.originalImage = originalImage;
        }

        public void InitialFill(Bitmap image, int nrOfPolygons, int initialPolygonsCount)
        {
            NrOfPolygons = nrOfPolygons;
            Polygons = new List<Point[]>();
            Brushes = new List<Brush>();
            originalImage = image;
            for (int i = 0; i < initialPolygonsCount; i++)
            {
                Point[] polygon = getPoints(3);
                Brush brush = getRandomBrush();

                Polygons.Add(polygon);
                Brushes.Add(brush);
            }

            NegativeFitness = computeNegativeFittness();
            
        }

        #region clonegene
        public Gene Clone()
        {
            Gene gene = new Gene();
            gene.NegativeFitness = this.NegativeFitness;
            gene.Polygons = new List<Point[]>();
            gene.Brushes = new List<Brush>();
            gene.OriginalImage = (Bitmap)this.OriginalImage.Clone();

            for (int i = 0; i < Brushes.Count; i++)
            {
                gene.Brushes.Add((Brush)this.Brushes[i].Clone());
                Point[] polygon = new Point[this.Polygons[i].Length];
                this.Polygons[i].CopyTo(polygon, 0);
                gene.Polygons.Add(polygon);
            }

            return gene;
        }
        #endregion

        #region genereation part
        private Point[] getPoints(int nrOfPoints)
        {
            Point[] points = new Point[nrOfPoints];

            int width = originalImage.Width;
            int height = originalImage.Height;

            int i = 0;
            while (true)
            {
                int x = random.Next(width);
                int y = random.Next(height);

                if (0 <= x && x < width && 0 <= y && y < height)
                {
                    points[i] = new Point(x, y);
                    i++;
                    if (i == nrOfPoints) break;
                }
            }
            return points;
        }

        private Brush getRandomBrush()
        {
            int opacity = 50 + random.Next(200);
            Color color = getRandomColor();
            SolidBrush brush = new SolidBrush(Color.FromArgb(opacity, color));
            return brush;
        }

        private Color getRandomColor()
        {
            int red = random.Next(255);
            int green = random.Next(255);
            int blue = random.Next(255);
            Color color = Color.FromArgb(red, green, blue);
            return color;
        }
        #endregion

        #region computeNegativeFitness

        private double computeNegativeFittness()
        {
            Bitmap bmp = new Bitmap(OriginalImage.Width, OriginalImage.Height);
            Graphics gr = Graphics.FromImage(bmp);
            gr.Clear(Color.Black);
            gr.FillRectangle(new SolidBrush(Color.Black), 0, 0, OriginalImage.Width, OriginalImage.Height);
            for (int p = 0; p < Polygons.Count; p++)
            {
                Point[] polyPoints = Polygons[p];
                Brush br = Brushes[p];
                if (br != null && polyPoints != null)
                {
                    gr.DrawPolygon(new Pen(br), polyPoints);
                    gr.FillPolygon(br, polyPoints);
                }
            }
            return computeError(bmp);
        }

        private double computeError(Bitmap sourceImage)
        {
            double error = 0;

            BitmapData bd2 = OriginalImage.LockBits(
                new Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            Pixel[] sourcePixels = null;

            BitmapData bd = sourceImage.LockBits(
            new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);
            sourcePixels = new Pixel[sourceImage.Width * sourceImage.Height];
            unsafe
            {
                fixed (Pixel* psourcePixels = sourcePixels)
                {
                    Pixel* pSrc = (Pixel*)bd.Scan0.ToPointer();
                    Pixel* pDst = psourcePixels;
                    for (int i = sourcePixels.Length; i > 0; i--)
                        *(pDst++) = *(pSrc++);
                }
            }
            sourceImage.UnlockBits(bd);


            unchecked
            {
                unsafe
                {
                    fixed (Pixel* psourcePixels = sourcePixels)
                    {
                        Pixel* p1 = (Pixel*)bd2.Scan0.ToPointer();
                        Pixel* p2 = psourcePixels;
                        for (int i = sourcePixels.Length; i > 0; i--, p1++, p2++)
                        {
                            int r = p1->R - p2->R;
                            int g = p1->G - p2->G;
                            int b = p1->B - p2->B;
                            error += r * r + g * g + b * b;
                        }
                    }
                }
            }
            OriginalImage.UnlockBits(bd);

            return Math.Sqrt(error);
        }

        #endregion

        #region Apply mutation changes
        internal AddPolygonMutationChanges AddPolygon()
        {
            AddPolygonMutationChanges changes = new AddPolygonMutationChanges();
            changes.OldNegativeFitness = this.NegativeFitness;
            int numberOfPoints = 3 + random.Next(RandSeed);
            Point[] polygon = getPoints(numberOfPoints);

            Brush brush = getRandomBrush();

            changes.Index = Polygons.Count;

            Polygons.Add(polygon);
            Brushes.Add(brush);

            changes.NewPolygon = polygon;
            changes.NewBrush = brush;

            this.NegativeFitness = computeNegativeFittness();
            changes.NewNegativeFitness = this.NegativeFitness;
            return changes;
        }

        internal SubstractPolygonMutationChanges RemovePolygon()
        {
            if (this.Polygons.Count > 0)
            {
                SubstractPolygonMutationChanges changes = new SubstractPolygonMutationChanges();
                changes.OldNegativeFitness = this.NegativeFitness;
                int position = random.Next(this.Polygons.Count);

                Point[] polygon = this.Polygons[position];
                Brush brush = this.Brushes[position];

                changes.Index = position;
                changes.OldPolygon = polygon;
                changes.OldBrush = brush;

                this.Polygons.RemoveAt(position);
                this.Brushes.RemoveAt(position);

                this.NegativeFitness = computeNegativeFittness();
                changes.NewNegativeFitness = this.NegativeFitness;
                return changes;
            }
            return null;
        }

        internal SubstractPolygonMutationChanges RemovePolygonAtPosition(int position)
        {
            if (this.Polygons.Count > 0)
            {
                SubstractPolygonMutationChanges changes = new SubstractPolygonMutationChanges();
                changes.OldNegativeFitness = this.NegativeFitness;

                Point[] polygon = this.Polygons[position];
                Brush brush = this.Brushes[position];

                changes.Index = position;
                changes.OldPolygon = polygon;
                changes.OldBrush = brush;

                this.Polygons.RemoveAt(position);
                this.Brushes.RemoveAt(position);

                this.NegativeFitness = computeNegativeFittness();
                changes.NewNegativeFitness = this.NegativeFitness;
                return changes;
            }
            return null;
        }

        internal PolygonMutationChanges MutatePolygon()
        {
            if (this.Polygons.Count > 0)
            {
                int position = random.Next(this.Polygons.Count);
                PolygonMutationChanges changes = new PolygonMutationChanges();
                changes.OldNegativeFitness = this.NegativeFitness;
                changes.OldPolygon = this.Polygons[position];
                changes.Index = position;
                

                int nrOfPoints = 3 + random.Next(RandSeed);
                Point[] newPolygon = getPoints(nrOfPoints);

                this.Polygons[position] = newPolygon;
                this.NegativeFitness = computeNegativeFittness();
                changes.NewNegativeFitness = this.NegativeFitness;
                changes.NewPolygon = newPolygon;

                return changes;
            }
            return null;
        }

        internal ColorMutationChanges MutateColor()
        {
            if (this.Brushes.Count > 0)
            {
                ColorMutationChanges changes = new ColorMutationChanges();
                changes.OldNegativeFitness = this.NegativeFitness;
                int position = random.Next(this.Brushes.Count);
                changes.Index = position;
                changes.OldBrush = this.Brushes[position];

                Brush newBrush = getRandomBrush();

                this.Brushes[position] = newBrush;
                this.NegativeFitness = computeNegativeFittness();
                changes.NewNegativeFitness = this.NegativeFitness;
                changes.NewBrush = newBrush;
                return changes;
            }
            return null;
        }

        internal PolygonAndColorMutationChanges MutatePolygonAndColor()
        {
            if (this.Polygons.Count > 0)
            {
                int position = random.Next(this.Polygons.Count);
                PolygonAndColorMutationChanges changes = new PolygonAndColorMutationChanges();
                changes.OldNegativeFitness = this.NegativeFitness;
                changes.OldBrush = this.Brushes[position];
                changes.OldPolygon = this.Polygons[position];
                changes.Index = position;

                int nrOfPoints = 3 + random.Next(RandSeed);
                Point[] newPolygon = getPoints(nrOfPoints);
                Brush newBrush = getRandomBrush();

                this.Polygons[position] = newPolygon;
                this.Brushes[position] = newBrush;
                this.NegativeFitness = computeNegativeFittness();
                changes.NewNegativeFitness = this.NegativeFitness;

                changes.NewPolygon = newPolygon;
                changes.NewBrush = newBrush;

                return changes;
            }
            return null;
        }

        internal PolygonMutationChanges MutatePolygonPoint()
        {
            if (this.Polygons.Count > 0)
            {
                int position = random.Next(this.Polygons.Count);
                PolygonMutationChanges changes = new PolygonMutationChanges();
                changes.OldNegativeFitness = this.NegativeFitness;

                Point[] oldPolygon = new Point[this.Polygons[position].Length];
                Array.Copy(this.Polygons[position],oldPolygon,this.Polygons[position].Length);
                changes.OldPolygon = oldPolygon;
                changes.Index = position;

                int pointPosition = random.Next(oldPolygon.Length);

                int new_x = random.Next(originalImage.Width);
                int new_y = random.Next(originalImage.Height);
                Point newPoint = new Point(new_x, new_y);

                this.Polygons[position][pointPosition] = newPoint;
                this.NegativeFitness = computeNegativeFittness();
                changes.NewNegativeFitness = this.NegativeFitness;
                changes.NewPolygon = this.Polygons[position];

                return changes;
            }
            return null;
        }

        internal ColorMutationChanges MutateColorParam()
        {
            if (this.Brushes.Count > 0)
            {
                ColorMutationChanges changes = new ColorMutationChanges();
                changes.OldNegativeFitness = this.NegativeFitness;
                int position = random.Next(this.Brushes.Count);
                changes.Index = position;

                SolidBrush oldBrush = (SolidBrush)this.Brushes[position];
                changes.OldBrush = oldBrush;

                Color newColor = Color.FromArgb(oldBrush.Color.A,
                    oldBrush.Color.R,
                    oldBrush.Color.G,
                    oldBrush.Color.B);
                int colorParamPos = random.Next(3);
                int newColorParam = random.Next(255);
                if (colorParamPos == 0)
                {
                    newColor = Color.FromArgb(oldBrush.Color.A,
                    newColorParam,
                    oldBrush.Color.G,
                    oldBrush.Color.B);
                }
                else if (colorParamPos == 1)
                {
                   newColor = Color.FromArgb(oldBrush.Color.A,
                   oldBrush.Color.R,
                   newColorParam,
                   oldBrush.Color.B);
                }
                else if (colorParamPos == 2)
                {
                   newColor =  Color.FromArgb(oldBrush.Color.A,
                   oldBrush.Color.R,
                   oldBrush.Color.G,
                   newColorParam);
                }

                SolidBrush newBrush = new SolidBrush(newColor);

                this.Brushes[position] = newBrush;
                this.NegativeFitness = computeNegativeFittness();
                changes.NewNegativeFitness = this.NegativeFitness;
                changes.NewBrush = newBrush;
                return changes;
            }
            return null;
        }

        #endregion

        internal void RestoreMutation(IMutationChanges changes, double oldNegativeFitness)
        {
            if(changes == null) return;
            this.NegativeFitness  = oldNegativeFitness;
            if (changes is PolygonAndColorMutationChanges)
            {
                PolygonAndColorMutationChanges change = (PolygonAndColorMutationChanges)changes;
                int index = change.Index;
                Point[] polygon = change.OldPolygon;
                Brush brush = change.OldBrush;

                this.Polygons[index] = polygon;
                this.Brushes[index] = brush;
            }
            else if (changes is ColorMutationChanges)
            {
                ColorMutationChanges change = (ColorMutationChanges)changes;
                int index = change.Index;
                Brush brush = change.OldBrush;
                this.Brushes[index] = brush;
            }
            else if (changes is PolygonMutationChanges)
            {
                PolygonMutationChanges change = (PolygonMutationChanges)changes;
                int index = change.Index;
                Point[] polygon = change.OldPolygon;
                this.Polygons[index] = polygon;
            }
            else if (changes is SubstractPolygonMutationChanges)
            {
                SubstractPolygonMutationChanges change = (SubstractPolygonMutationChanges)changes;
                int index = change.Index;
                Point[] polygon = change.OldPolygon;
                Brush brush = change.OldBrush;
                this.Polygons.Insert(index,polygon);
                this.Brushes.Insert(index, brush);
            }
            else if (changes is AddPolygonMutationChanges)
            {
                AddPolygonMutationChanges change = (AddPolygonMutationChanges)changes;
                int index = change.Index;
                this.Brushes.RemoveAt(index);
                this.Polygons.RemoveAt(index);
            }
        }

        internal void ApplyMutation(IMutationChanges changes)
        {
            if (changes == null) return;
            this.NegativeFitness = changes.NewNegativeFitness;
            if (changes is PolygonAndColorMutationChanges)
            {
                PolygonAndColorMutationChanges change = (PolygonAndColorMutationChanges)changes;
                int index = change.Index;
                Point[] polygon = change.NewPolygon;
                Brush brush = change.NewBrush;

                this.Polygons[index] = polygon;
                this.Brushes[index] = brush;
            }
            else if (changes is ColorMutationChanges)
            {
                ColorMutationChanges change = (ColorMutationChanges)changes;
                int index = change.Index;
                Brush brush = change.NewBrush;
                this.Brushes[index] = brush;
            }
            else if (changes is PolygonMutationChanges)
            {
                PolygonMutationChanges change = (PolygonMutationChanges)changes;
                int index = change.Index;
                Point[] polygon = change.NewPolygon;
                this.Polygons[index] = polygon;
            }
            else if (changes is SubstractPolygonMutationChanges)
            {
                SubstractPolygonMutationChanges change = (SubstractPolygonMutationChanges)changes;
                int index = change.Index;
                Point[] polygon = change.OldPolygon;
                Brush brush = change.OldBrush;
                //this.Polygons.Insert(index, polygon);
                //this.GeneBrush.Insert(index, brush);
                this.Brushes.RemoveAt(index);
                this.Polygons.RemoveAt(index);
            }
            else if (changes is AddPolygonMutationChanges)
            {
                AddPolygonMutationChanges change = (AddPolygonMutationChanges)changes;
                int index = change.Index;
                Point[] polygon = change.NewPolygon;
                Brush brush = change.NewBrush;
                this.Brushes.Insert(index, brush);
                this.Polygons.Insert(index, polygon);
            }


        }

        internal void InitFromData(List<Point[]> polygons, List<Brush> brushes, Bitmap origImage)
        {
            Polygons = polygons;
            Brushes = brushes;
            OriginalImage = (Bitmap)origImage.Clone();
            NrOfPolygons = polygons.Count;
            this.NegativeFitness = computeNegativeFittness();
        }

       
    }
}
