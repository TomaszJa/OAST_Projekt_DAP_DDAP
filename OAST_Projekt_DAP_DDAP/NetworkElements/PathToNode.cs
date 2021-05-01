using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    // Klasa opisująca ścieżkę do węzła
    public class PathToNode
    {
        public int pathNumber;
        public List<int> LinksIds = new List<int>();

        public void PrintProperties()
        {
            Console.WriteLine($"Numer sciezki: {pathNumber}, Krawedzie sciezki: ");
            foreach (var link in LinksIds)
            {
                Console.Write(link.ToString() + " ");
            }
            Console.WriteLine();
        }
    }
}
