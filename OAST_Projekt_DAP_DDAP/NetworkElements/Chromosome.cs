using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    public class Chromosome
    {
        public List<Gene> listOfGenes = new List<Gene>();
        public int DAPfitness;
        public int DDAPfitness;

        public Chromosome()
        {
        }

        public Chromosome(List<Gene> _listOfGenes, int _DAPfitness, int _DDAPfitness)
        {
            listOfGenes = _listOfGenes;
            DAPfitness = _DAPfitness;
            DDAPfitness = _DDAPfitness;
        }
    }
}
