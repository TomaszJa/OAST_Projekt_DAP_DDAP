using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    public class Chromosome
    {
        public List<Gene> listOfGenes = new List<Gene>();
        public int fitnessDAP;
        public int fitnessDDAP;
    }
}
