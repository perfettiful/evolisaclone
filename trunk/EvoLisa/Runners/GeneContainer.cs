using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvoImage.Models;

namespace EvoImage.Runners
{
    public class GeneContainer
    {
        private GeneContainer() { }
        private static GeneContainer _instance = new GeneContainer();

        private static readonly object padLock = new object();

        public static GeneContainer Instance
        {
            get
            {
                return _instance;
            }
        }

        private Gene localGene;

        public void AddGene(Gene gene)
        {
            lock (padLock)
            {
                localGene = null;
                this.localGene = gene;
            }
        }

        public Gene GetGene()
        {
            if (localGene != null)
            {
                lock (padLock)
                {
                    return this.localGene.Clone();
                }
            }
            return null;
        }


    }
}
