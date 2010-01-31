using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EvoImage.Models;
using EvoImage.Threads;

namespace EvoImage.Runners
{
    public class GeneEnvironment
    {
        private Random random = new Random();
        private Gene workingGene;

        #region Properties
        public int NrOfPolygons
        {
            get;
            set;
        }

        public Bitmap OriginalImage
        {
            get;
            set;
        }
        public int NrOfMutationsTries
        {
            get;
            set;
        }

        public Gene WorkingGene
        {
            get
            {
                return this.workingGene;
            }
        }
        #endregion

        public void Run(object obj_iterations)
        {
            int nrOfIterations = (int)obj_iterations;

            double referenceNegativeIndex = double.MaxValue;
            double referenceComeback = NrOfPolygons;
            bool nrOfMutationsTryChanged = false;

            int limit = 0;
            int shakeTheImageCounter = 0;
            double currentNegativeFitness = workingGene.NegativeFitness;

            for (int i = 0; i < nrOfIterations; i++)
            {
                if (CommandChannel.GetCommand() == 1)
                {
                    break;
                }
                List<int> mutationsList = retrieveMutationsList();
                for (int mi = 0; mi < mutationsList.Count; mi++)
                {
                    int mutationStep = mutationsList[mi];
                    switch (mutationStep)
                    {
                        case 1: performAddPolygon(workingGene); break;
                        case 2: performMutateColor(workingGene); break;
                        case 3: performMutatePolygon(workingGene); break;
                        case 4: performMutatePolygonAndColor(workingGene); break;
                        case 5: performMutateColorParam(workingGene); break;
                        case 6: performMutatePolygonPoint(workingGene); break;
                    }
                }

                
                #region check if the gene had evolved in the last 250 iterations; comment this region if you don't one to have momentum in your app
                if (currentNegativeFitness != workingGene.NegativeFitness)
                {
                    currentNegativeFitness = workingGene.NegativeFitness;
                    shakeTheImageCounter = 0;
                }
                else
                {
                    //that means the in the last 250 iterations nothing changed in the gene fitness.
                    if (shakeTheImageCounter > 250)
                    {
                        int nrOfPolysToBeRemoved = workingGene.NrOfPolygons / 10;
                        for (int ix = 0; ix < nrOfPolysToBeRemoved; ix++)
                        {
                            randomlyRemovePolygons(workingGene);
                        }
                        referenceComeback = workingGene.Polygons.Count + 1;
                    }
                    shakeTheImageCounter++;                    
                }
                #endregion
                
                //if gene is more fit, we added to the container;
                Console.WriteLine("Negative fitness: {0} ,     {1}", workingGene.NegativeFitness, workingGene.Polygons.Count);
                if (workingGene.NegativeFitness < referenceNegativeIndex)
                {
                    GeneContainer.Instance.AddGene(workingGene.Clone());
                    referenceNegativeIndex = workingGene.NegativeFitness;
                }

                #region every 100 iterations we increase the number of polygons, till we reach 50
                if (workingGene.Polygons.Count < 50 && this.NrOfPolygons < 50)
                {
                    if (i % 100 == 0 && i != 0)
                    {
                        this.NrOfPolygons++;
                        workingGene.NrOfPolygons++;
                    }
                }
                #endregion

                //if every time the number of polygons in the gene reaches the max number of polygons, then we increase the nr of polygons and 
                //we remove the less fit individuals
                if (workingGene.Polygons.Count < 50) 
                {
                    if (workingGene.Polygons.Count == referenceComeback)
                    {
                        limit =  workingGene.Polygons.Count / 10; 
                        for (int ix = 0; ix < limit; ix++)
                        {
                            removeLessFitPolys(workingGene);

                        }
                        Console.WriteLine("-------- [ReferenceComeback] :" + referenceComeback);
                        referenceComeback++;
                    }
                }
                // when 50 poly are reached, we double the nr of mutations tries;
                // and we remove a larger number of less fit individuals;
                else if (workingGene.Polygons.Count >= 50) 
                {
                        if (nrOfMutationsTryChanged == false)
                        {
                            NrOfMutationsTries = NrOfMutationsTries * 2;
                            nrOfMutationsTryChanged = true;
                        }

                        limit = workingGene.Polygons.Count / 8; 
                        for (int ix = 0; ix < limit; ix++)
                        {
                            removeLessFitPolys(workingGene);

                        }
                }

            }

            CommandChannel.SetCommand(2); //tell timer to stop
        }

        internal void GenerateOriginalGene()
        {
            workingGene = new Gene();
            int initialPolygonsCount = NrOfPolygons / 2 + random.Next(NrOfPolygons / 2);
            workingGene.InitialFill((Bitmap)OriginalImage.Clone(), NrOfPolygons, initialPolygonsCount);
        }

        #region perform mutations
        private void randomlyRemovePolygons(Gene gene)
        {
            int position = random.Next(gene.Polygons.Count);
            gene.RemovePolygonAtPosition(position);
        }

        private void performMutatePolygonAndColor(Gene gene)
        {
            double oldNegativeFitness = gene.NegativeFitness;
            PolygonAndColorMutationChanges bigChange = null;
            for (int i = 0; i < NrOfMutationsTries; i++)
            {
                PolygonAndColorMutationChanges changes = gene.MutatePolygonAndColor();
                if (changes != null && gene.NegativeFitness > oldNegativeFitness)
                {
                    gene.RestoreMutation(changes, oldNegativeFitness);
                }
                else if(changes != null)
                {
                    oldNegativeFitness = gene.NegativeFitness;
                    bigChange = changes;
                    gene.RestoreMutation(changes, changes.OldNegativeFitness);
                }
            }
            gene.ApplyMutation(bigChange);
        }

        private void performMutatePolygon(Gene gene)
        {
            double oldNegativeFitness = gene.NegativeFitness;
            PolygonMutationChanges bigChange = null;
            for (int i = 0; i < NrOfMutationsTries; i++)
            {
                PolygonMutationChanges changes = gene.MutatePolygon();
                if (changes != null && gene.NegativeFitness > oldNegativeFitness)
                {
                    gene.RestoreMutation(changes, oldNegativeFitness);
                }
                else if (changes != null)
                {
                    oldNegativeFitness = gene.NegativeFitness;
                    bigChange = changes;
                    gene.RestoreMutation(changes, changes.OldNegativeFitness);
                }

            }
            gene.ApplyMutation(bigChange);
        }

        private void performMutateColor(Gene gene)
        {
            double oldNegativeFitness = gene.NegativeFitness;
            ColorMutationChanges bigChange = null;
            for (int i = 0; i < NrOfMutationsTries; i++)
            {

                ColorMutationChanges changes = gene.MutateColor();
                if (changes != null && gene.NegativeFitness > oldNegativeFitness)
                {
                    gene.RestoreMutation(changes, oldNegativeFitness);
                }
                else if (changes != null)
                {
                    oldNegativeFitness = gene.NegativeFitness;
                    bigChange = changes;
                    gene.RestoreMutation(changes, changes.OldNegativeFitness);
                }

            }
            gene.ApplyMutation(bigChange);
        }

        private void performAddPolygon(Gene gene)
        {
            if (gene.Polygons.Count >= this.NrOfPolygons) return;
            double oldNegativeFitness = gene.NegativeFitness;
            AddPolygonMutationChanges bigChange = null;
            for (int i = 0; i < NrOfMutationsTries; i++)
            {
                AddPolygonMutationChanges changes = gene.AddPolygon();
                if (changes != null && gene.NegativeFitness > oldNegativeFitness)
                {
                    gene.RestoreMutation(changes, oldNegativeFitness);
                }
                else if (changes != null)
                {
                    oldNegativeFitness = gene.NegativeFitness;
                    bigChange = changes;
                    gene.RestoreMutation(changes, changes.OldNegativeFitness);
                }
            }
            gene.ApplyMutation(bigChange);
        }

        private void performMutatePolygonPoint(Gene gene)
        {
            double oldNegativeFitness = gene.NegativeFitness;
            PolygonMutationChanges bigChange = null;
            for (int i = 0; i < NrOfMutationsTries; i++)
            {
                PolygonMutationChanges changes = gene.MutatePolygonPoint();
                if (changes != null && gene.NegativeFitness > oldNegativeFitness)
                {
                    gene.RestoreMutation(changes, oldNegativeFitness);
                }
                else if (changes != null)
                {
                    oldNegativeFitness = gene.NegativeFitness;
                    bigChange = changes;
                    gene.RestoreMutation(changes, changes.OldNegativeFitness);
                }

            }
            gene.ApplyMutation(bigChange);
        }

        private void performMutateColorParam(Gene gene)
        {
            double oldNegativeFitness = gene.NegativeFitness;
            ColorMutationChanges bigChange = null;
            for (int i = 0; i < NrOfMutationsTries; i++)
            {

                ColorMutationChanges changes = gene.MutateColorParam();
                if (changes != null && gene.NegativeFitness > oldNegativeFitness)
                {
                    gene.RestoreMutation(changes, oldNegativeFitness);
                }
                else if (changes != null)
                {
                    oldNegativeFitness = gene.NegativeFitness;
                    bigChange = changes;
                    gene.RestoreMutation(changes, changes.OldNegativeFitness);
                }

            }
            gene.ApplyMutation(bigChange);
        }

        private void removeLessFitPolys(Gene gene)
        {
            if (gene.Polygons.Count < this.NrOfPolygons / 2 +1 ) return;

            double referenceNegativeFitness = double.MaxValue;
            SubstractPolygonMutationChanges bigChange = null;
            for (int i = 0; i < gene.Polygons.Count; i++)
            {
                double oldNegativeFitness = gene.NegativeFitness;
                SubstractPolygonMutationChanges changes = gene.RemovePolygonAtPosition(i);

                if (changes != null && gene.NegativeFitness < referenceNegativeFitness)
                {
                    bigChange = changes;
                    referenceNegativeFitness = gene.NegativeFitness;
                }
                gene.RestoreMutation(changes, oldNegativeFitness);
            }
            gene.ApplyMutation(bigChange);
        }


        /// <summary>
        /// This version of substracts poly performs just a 'from time to time' substract;
        /// </summary>
        /// <param name="gene"></param>
        private void performSubstract(Gene gene)
        {
            if (gene.Polygons.Count < this.NrOfPolygons / 2+1) return;
            int doIt = random.Next(100);
            if (doIt % 2 == 0) return;

            double referenceNegativeFitness = double.MaxValue;
            SubstractPolygonMutationChanges bigChange = null;
            for (int i = 0; i < gene.Polygons.Count; i++)
            {
                double oldNegativeFitness = gene.NegativeFitness;
                SubstractPolygonMutationChanges changes = gene.RemovePolygonAtPosition(i);

                if (changes != null && gene.NegativeFitness < referenceNegativeFitness)
                {
                    bigChange = changes;
                    referenceNegativeFitness = gene.NegativeFitness;
                }
                gene.RestoreMutation(changes, oldNegativeFitness);
            }
            gene.ApplyMutation(bigChange);
        }

        #endregion

        #region retrieve mutations List
        private List<int> retrieveMutationsList()
        {
            //1 - add polygon;
            //2 - mutateColor;
            //3 - mutatePolygon;
            //4 - mutatePolygonAndColor;
            //5 - mutateColorParam;
            //6 - mutatePolygonPoint;
            List<int> mutationsList = new List<int>();

            int mutationSetCount = 6;
            for (int i = 0; i < NrOfMutationsTries; i++)
            {
                int mutationStep = 1 + random.Next(mutationSetCount);
                mutationsList.Add(mutationStep);
            }

            return mutationsList;
        }
        #endregion

        #region load gene
        internal void LoadGene(Gene gene, Bitmap origImage)
        {
            workingGene = gene;
            this.OriginalImage = origImage;
            this.NrOfPolygons = workingGene.Polygons.Count;
            if (NrOfPolygons < 50)
            {
                this.NrOfMutationsTries = 5;
            }
            else
            {
                this.NrOfMutationsTries = 10;
            }
        }
        #endregion
    }
}
