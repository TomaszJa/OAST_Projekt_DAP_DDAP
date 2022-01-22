using OAST_Projekt_DAP_DDAP.NetworkElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAST_Projekt_DAP_DDAP
{
    // Drzewo genealogiczne najlepszych chromosomów
    public class Tree
    {
        public List<Chromosome> BestChromosomes = new List<Chromosome>();
        public List<Link> Links = new List<Link>();
        public List<Demand> Demands = new List<Demand>();
        public List<Node> Nodes = new List<Node>();
        public int seed;
        public int populationSize;
        public int iterations;
        public int simulationTime;
        public int mutations;
        public double mutationProbability;
        public double crossoverProbability;
        public string inputFile;
        public string outputFile = "Wyniki.txt";

        public void WriteToDAPFile()
        {
            string text = null;
            int i = 1;

            Console.WriteLine("Writing results to a file, this might take a while...");
            foreach (var chromosome in BestChromosomes)
            {
                text += $"\t\t----------Best chromosome in {i} generation----------\n";
                for (int a = 0; a < chromosome.Genes.Count; a++)
                {
                    text += $"Węzeł początkowy: {Demands[a].startNode} | " +
                        $"Węzeł końcowy: {Demands[a].destinationNode} | " +
                        $"Rozmiar żądania: {Demands[a].demandSize} | " +
                        $"Podział na ścieżki:";
                    text += "[ ";
                    foreach (var allele in chromosome.Genes[a].Alleles)
                    {
                        text += $"{allele} ";
                    }
                    text += "]\n";
                }

                text += "\t\t\t\t###############\n";
                text += $"\t\t\t\t   DAP: {chromosome.DAPfitness} \n";
                text += $"\t\t\t\t   DDAP: {chromosome.DDAPfitness} \n";
                text += "\t\t\t\t###############\n\n";
                i++;
            }

            var bestChromosome = BestChromosomes.Last();

            text += CalculateFitnessAndPrintValues(bestChromosome, Links, Demands, Nodes);

            text += $"Ziarno: {seed}\n";
            text += $"Liczba iteracji algorytmu: {i-1}\n";
            text += $"Czas optymalizacji: {simulationTime} [s]\n";
            text += $"Ilość mutacji: {mutations}\n";
            text += $"Liczność populacji: {populationSize}\n"
                              + $"Prawdopodobieństwo krzyżowania: {crossoverProbability}\n"
                              + $"Prawdopodobieństwo mutacji: {mutationProbability}\n";

            OutputFileName("DAP");
            File.WriteAllText($"Wyniki/DAP/{outputFile}", text);
            Console.WriteLine("Writing Finished!");
        }

        public void WriteToDDAPFile()
        {
            string text = null;
            int i = BestChromosomes.Count;

            Console.Clear();

            var bestChromosome = BestChromosomes.Last();

            text += $"\t\t----------Best chromosome in {i} generation----------\n";
            for (int a = 0; a < bestChromosome.Genes.Count; a++)
            {
                text += $"Węzeł początkowy: {Demands[a].startNode} | " +
                    $"Węzeł końcowy: {Demands[a].destinationNode} | " +
                    $"Rozmiar żądania: {Demands[a].demandSize}[Mbit/s] | " +
                    $"Podział na ścieżki:";
                text += "[ ";
                foreach (var allele in bestChromosome.Genes[a].Alleles)
                {
                    text += $"{allele} ";
                }
                text += "]\n";
            }

            text += "\t\t\t\t###############\n";
            text += $"\t\t\t\t   Łączny koszt : {bestChromosome.DDAPfitness} \n";
            text += "\t\t\t\t###############\n\n";

            text += CalculateFitnessAndPrintValues(bestChromosome, Links, Demands, Nodes);

            text += $"Ziarno: {seed}\n";
            text += $"Liczba iteracji algorytmu: {i-1}\n";
            text += $"Czas optymalizacji: {simulationTime} [s]\n";
            text += $"Ilość mutacji: {mutations}\n";
            text += $"Liczność populacji: {populationSize}\n"
                              + $"Prawdopodobieństwo krzyżowania: {crossoverProbability}\n"
                              + $"Prawdopodobieństwo mutacji: {mutationProbability}\n";

            OutputFileName("DDAP");
            File.WriteAllText($"Wyniki/DDAP/{outputFile}", text);
            Process.Start("Notepad.exe", $"Wyniki/DDAP/{outputFile}");
            Console.WriteLine(text);
        }

        public void OutputFileName(string problem = "")
        {
            if (inputFile.Contains("net4"))
            {
                outputFile = $"net4_Population_{populationSize}_mutation_{mutationProbability}_Crossover_{crossoverProbability}_Wyniki_{problem}.txt";
            }
            else if (inputFile.Contains("net12_1"))
            {
                outputFile = $"net12_1_Population_{populationSize}_mutation_{mutationProbability}_Crossover_{crossoverProbability}_Wyniki_{problem}.txt";
            }
            else if (inputFile.Contains("net12_2"))
            {
                outputFile = $"net12_2_Population_{populationSize}_mutation_{mutationProbability}_Crossover_{crossoverProbability}_Wyniki_{problem}.txt";
            }
        }

        public string CalculateFitnessAndPrintValues(Chromosome chromosome, List<Link> links, List<Demand> demands, List<Node> nodes)
        {
            string results = "";

            foreach (var node in nodes)
            {
                node.IncomingTraffic = 0;
            }

            chromosome.DAPfitness = 0;
            chromosome.DDAPfitness = 0;

            int[] l = new int[links.Count];    // tablica o długości odpowiadającej ilości łączy, przechwoująca wartość l(e,x) dla każdego łącza
            int[] F = new int[links.Count];    // tablica przechowująca wartości F(x)
            int[] y = new int[links.Count];    // tablica przechowująca wartości y dla każdego łącza (DDAP)

            for (int d = 0; d < chromosome.Genes.Count; d++)    // iterujemy po każdym genie w chromosomie (Wykład 1 slajd 13)
            {
                for (int p = 0; p < chromosome.Genes[d].Alleles.Count; p++)   // po każdym Allelu (każdej ścieżce)
                {
                    var allele = chromosome.Genes[d].Alleles[p];      // pobranie tablicy z allelami
                    var path = demands[d].Paths[p];            // Pobranie konkretnej ścieżki, w której będziemy sprawdzać, czy znajdują się dane łącza

                    for (int e = 0; e < links.Count; e++)      // e to numer łącza
                    {
                        if (path.LinksIds.Contains(e + 1))   // jako, że numerację łączy zaczynamy od 1 to dlatego e + 1
                        {
                            l[e] += allele;     // Pobieramy informację o obciążeniu dla danego łącza i dodajemy ją do sumy, z której wyjdzie l(e,x)
                        }

                    }
                }
            }
            for (int i = 0; i < links.Count; i++)
            {
                double yValue = (double)l[i] / (double)links[i].moduleSize;    // Obliczamy wartość y dla danego łącza czyli l(e,x)/rozmiar modułu
                y[i] = (int)Math.Ceiling(yValue);       // i zaokrąglamy w górę, ponieważ jak będzie potrzeba przesłać 10,5 Mb to należy mieć 11 na łączu
                F[i] = l[i] - links[i].capacity;     // Obliczamy F(x) dla DAP

                foreach (var node in nodes)
                {
                    if (node.Index == links[i].startingNode || node.Index == links[i].endingNode)
                    {
                        node.IncomingTraffic += y[i];
                    }
                }

                //results += $"Obciążenie łącza {i + 1}: {F[i]}; ";
                //results += $"Wymiar łącza {i + 1}: {y[i]}\n";
                results += $"Przepływność łącza {i + 1}: {y[i]} [Mbit/s]\n";
            }
            results += "\n";
            foreach (var node in nodes)
            {
                results += $"Obciążenie węzła {node.Index}: {Math.Truncate(((double)node.IncomingTraffic/(double)node.Capacity)*100)}%; \n";
            }
            results += "\n";
            return results;
        }
    }
}
