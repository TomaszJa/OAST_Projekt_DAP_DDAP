using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    // Klasa opisująca żądanie
    public class Demand
    {
        public int startNode;
        public int destinationNode;
        public int demandSize;
        public int numberOfPaths;
        public List<PathToNode> Paths = new List<PathToNode>();

        public void PrintProperties()
        {
            Console.WriteLine($"Wezel poczatkowy: {startNode}, Wezel koncowy: {destinationNode} " +
                $"Rozmiar zadania: {demandSize}, Liczba sciezek: {numberOfPaths}, \nSciezki:\n");

            foreach (var path in Paths)
            {
                path.PrintProperties();
            }
        }
    }
}
