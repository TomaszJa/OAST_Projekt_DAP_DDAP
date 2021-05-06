using OAST_Projekt_DAP_DDAP.NetworkElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OAST_Projekt_DAP_DDAP
{
    // Drzewo genealogiczne najlepszych chromosomów
    public class Tree
    {
        public List<Chromosome> BestChromosomes = new List<Chromosome>();
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

            Console.WriteLine("Writing results to file, this might take a while...");
            foreach (var chromosome in BestChromosomes)
            {
                text += $"Best chromosome in {i} generation: \n";
                foreach (var gene in chromosome.Genes)
                {
                    text += "[ ";
                    foreach (var allele in gene.Alleles)
                    {
                        text += $"{allele} ";
                    }
                    text += "]\n";
                }

                text += "###############\n";
                text += $"DAP: {chromosome.DAPfitness} \n";
                text += $"DDAP: {chromosome.DDAPfitness} \n";
                text += "###############\n";
                i++;
            }

            text += $"Ziarno: {seed}\n";
            text += $"Liczba iteracji algorytmu: {i-1}\n";
            text += $"Czas optymalizacji: {simulationTime}\n";
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
            int i = 1;

            Console.WriteLine("Writing results to a file, this might take a while...");
            foreach (var chromosome in BestChromosomes)
            {
                text += $"Best chromosome in {i} generation: \n";
                foreach (var gene in chromosome.Genes)
                {
                    text += "[ ";
                    foreach (var allele in gene.Alleles)
                    {
                        text += $"{allele} ";
                    }
                    text += "]\n";
                }

                text += "###############\n";
                text += $"DAP: {chromosome.DAPfitness} \n";
                text += $"DDAP: {chromosome.DDAPfitness} \n";
                text += "###############\n";
                i++;
            }

            text += $"Ziarno: {seed}\n";
            text += $"Liczba iteracji algorytmu: {i-1}\n";
            text += $"Czas optymalizacji: {simulationTime} [s]\n";
            text += $"Ilość mutacji: {mutations}\n";
            text += $"Liczność populacji: {populationSize}\n"
                              + $"Prawdopodobieństwo krzyżowania: {crossoverProbability}\n"
                              + $"Prawdopodobieństwo mutacji: {mutationProbability}\n";

            OutputFileName("DDAP");
            File.WriteAllText($"Wyniki/DDAP/{outputFile}", text);
            Console.WriteLine("Writing Finished!");
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
    }
}
