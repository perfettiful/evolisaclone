using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvoImage.Models;
using System.Drawing;
using EvoImage.Runners;
using System.IO;

namespace EvoImage.Util
{
    public class FileUtil
    {
        public static void SaveGeneToFile(String fileName,Gene gene,Image image,Image originalImage)
        {
            string guid = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(fileName) == true)
            {
                fileName = guid;
            }
            string originalImageFileName = fileName + "_original.jpg";
            Bitmap savedImage = new Bitmap(image);
            savedImage.Save(fileName+".jpg");

            Bitmap originalImageBmp = new Bitmap(originalImage);
            originalImageBmp.Save(originalImageFileName);
            StreamWriter sw = new StreamWriter(fileName + ".dat");
            sw.WriteLine("Polygons");

            foreach (Point[] poly in gene.Polygons)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Point point in poly)
                {
                    sb.Append("(" + point.X + "," + point.Y + ")");
                }
                sb.Append(";");
                sw.WriteLine(sb.ToString());
            }
            sw.WriteLine("Brushes");
            foreach (SolidBrush brush in gene.Brushes)
            {
                sw.WriteLine(brush.Color.A.ToString() + "," +
                             brush.Color.R.ToString() + "," +
                             brush.Color.G.ToString() + "," +
                             brush.Color.B.ToString() + ";");
            }
            sw.Close();
        }

        internal static GeneImageStruct LoadGene(string file,int width, int height)
        {
            Bitmap image = (Bitmap)Bitmap.FromFile(file.ToLower().Replace("dat", "jpg"));
            Bitmap workingImage = new Bitmap(image, width, height);

            Bitmap origFileImage = (Bitmap)Bitmap.FromFile(file.ToLower().Replace(".dat", "_original.jpg"));
            Bitmap originalImage = new Bitmap(origFileImage, width, height);

            String content = File.ReadAllText(file);
            String[] allContent = content.Split(new String[] { "Polygons", "Brushes" }, StringSplitOptions.RemoveEmptyEntries);
            String polygonsContent = allContent[0];
            String brushesContent = allContent[1];
            String[] pointSets = polygonsContent.Split(new char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            String[] brushesSets = brushesContent.Split(new char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            List<Point[]> polygons = new List<Point[]>();
            List<Brush> brushes = new List<Brush>();

            foreach (String pointSet in pointSets)
            {
                String[] points = pointSet.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
                Point[] polygon = new Point[points.Length / 2];
                int index = 0;
                for (int i = 0; i < points.Length; i = i + 2)
                {
                    int x = Convert.ToInt32(points[i]);
                    int y = Convert.ToInt32(points[i + 1]);
                    polygon[index] = new Point(x, y);
                    index++;
                }
                polygons.Add(polygon);
            }

            foreach (String brushDef in brushesSets)
            {
                String[] colours = brushDef.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int alfa = Convert.ToInt32(colours[0]);
                int red = Convert.ToInt32(colours[1]);
                int green = Convert.ToInt32(colours[2]);
                int blue = Convert.ToInt32(colours[3]);
                Color color = Color.FromArgb(alfa, red, green, blue);
                SolidBrush brush = new SolidBrush(color);
                brushes.Add(brush);
            }

            Gene gene = new Gene();
            gene.InitFromData(polygons, brushes, originalImage);

            GeneImageStruct imageStruct = new GeneImageStruct();
            imageStruct.WorkingGene = gene;
            imageStruct.OriginalImage = originalImage;
            imageStruct.WorkingImage = workingImage;
            return imageStruct;
        }
    }
}
