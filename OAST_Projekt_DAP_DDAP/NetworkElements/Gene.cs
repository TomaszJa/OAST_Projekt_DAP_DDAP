using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    public class Gene
    {
        public List<int> Alleles = new List<int>();
        public int demandSize;

        public Gene()
        {

        }
        public Gene(List<int> _listOfAlleles, int _demandSize)
        {
            Alleles = _listOfAlleles;
            demandSize = _demandSize;
        }

        public void PrintProperties()
        {
            Console.Write("[");
            Alleles.ForEach(Console.Write);
            Console.Write("]\n");
        }
    }
}
