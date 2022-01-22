using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    public class Chromosome
    {
        public List<Gene> Genes = new List<Gene>();
        public int DAPfitness;
        public int DDAPfitness;
        public bool wasMutated = false;

        public Chromosome()
        {
        }

        public Chromosome(List<Gene> _genes, int _DAPfitness, int _DDAPfitness)
        {
            Genes = _genes;
            DAPfitness = _DAPfitness;
            DDAPfitness = _DDAPfitness;
        }

        public void PrintProperties()
        {
            foreach (var gene in Genes)
            {
                gene.PrintProperties();
            }
            Console.WriteLine("####################");
            Console.WriteLine($"Łączny koszt = {DDAPfitness}");
            Console.WriteLine("####################");
        }
    }
}
