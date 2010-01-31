using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EvoImage.Models
{

    public interface IMutationChanges {

        double OldNegativeFitness { get; set; }
        double NewNegativeFitness { get; set; }
    };
    class ColorMutationChanges:IMutationChanges
    {
        public Brush OldBrush
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public Brush NewBrush { get; set; }


        public double OldNegativeFitness { get; set; }
        public double NewNegativeFitness { get; set; }
    }

    class PolygonMutationChanges : IMutationChanges
    {
        public Point[] OldPolygon
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }


        public Point[] NewPolygon { get; set; }

        public double OldNegativeFitness { get; set; }
        public double NewNegativeFitness { get; set; }
    }

    class PolygonAndColorMutationChanges : IMutationChanges
    {
        public Brush OldBrush
        {
            get;
            set;
        }

        public Point[] OldPolygon
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public Brush NewBrush { get; set; }

        public Point[] NewPolygon { get; set; }

        public double OldNegativeFitness { get; set; }
        public double NewNegativeFitness { get; set; }
    }

    class SubstractPolygonMutationChanges : IMutationChanges
    {
        public Point[] OldPolygon
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public Brush OldBrush { get; set; }

        public double OldNegativeFitness { get; set; }
        public double NewNegativeFitness { get; set; }
    }

    class AddPolygonMutationChanges : IMutationChanges
    {
        public int Index
        {
            get;
            set;
        }

        public Point[] NewPolygon { get; set; }

        public Brush NewBrush { get; set; }

        public double OldNegativeFitness { get; set; }
        public double NewNegativeFitness { get; set; }
    }
}
