using OAST_Projekt_DAP_DDAP.Algorythms;
using OAST_Projekt_DAP_DDAP.NetworkElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace OAST_Projekt_DAP_DDAP
{
    public class Simulation
    {
        ReadFromFile SimulationInfo = new ReadFromFile();
        public int seed;
        public int populationSize;
        public int time = 10;
        public int numberOfGenerations = 10;
        public int numberOfMutations = 100;
        public int iterationsWithoutBetterSolution = 10;
        public int iterations;
        public int stopCryterium = 1;
        public int bestDAP = int.MaxValue;
        public int bestDDAP = int.MaxValue;
        public double mutationProbability;
        public double crossoverProbability;
        public Network network = new Network();
        public bool DAP = true;  // jak true to algorytm liczy DAP, jak false to DDAP

        public Simulation()
        {
            SimulationInfo.GetSimulationInfo();
            stopCryterium = GetCryterium();
            DAP = GetProblem();

            network = SimulationInfo.network;
            seed = SimulationInfo.seed;
            populationSize = SimulationInfo.populationSize;
            mutationProbability = SimulationInfo.mutationProbability;
            crossoverProbability = SimulationInfo.crossoverProbability;
        }

        public void RunSimulation()
        {
            
            var algorythm = new EvolutionaryAlgorythm(seed);    // Tworzymy nowy algorytm na bazie pobranego ziarna

            var population = algorythm.GenerateStartingPopulation(network.Demands, populationSize); // Tworzymy pierwszą populację 

            population = algorythm.CalculateFitness(population, network.Links, network.Demands);    // Obliczamy DAP i DDAP dla pierwszej populacji

            if (DAP)
            {
                DAPSimulation(algorythm, population);
            }
            else
            {
                DDAPSimulation(algorythm, population);
            }
        }

        public void DAPSimulation(EvolutionaryAlgorythm algorythm, Population population)
        {
            // OrderBy ustawia obiekty z najniższą wartością fitness na początku listy
            // dzięki temu w algorytmie krzyżowania do krzyżowania będą wybierane najlepsze chromosomy ze sobą

            var DAPBestPopulation = new Population();
            DAPBestPopulation.Chromosomes.AddRange(population.Chromosomes.OrderBy(x => x.DAPfitness).ToList());

            bestDAP = DAPBestPopulation.Chromosomes[0].DAPfitness;
            PrintInfoAboutBestChromosome(DAPBestPopulation, "DAP");

            var timer = new Timer(1000);

            while (!StopAlgorythm())
            {
                //timer.Start();
                population.generationNumber++;      // kolejna generacja

                algorythm.CrossoverChromosomes(population.Chromosomes, crossoverProbability);   // Krzyżowanie chromosomów

                foreach (var chromosome in population.Chromosomes)
                {
                    algorythm.MutateChromosome(chromosome, mutationProbability);    // Mutowanie
                    if (chromosome.wasMutated)
                    {
                        numberOfMutations--;        // początkowa ilość mutacji jest określona w kryterium i gdy zmaleje do 0 to algorytm skończy działanie
                    }
                }

                algorythm.CalculateFitness(population, network.Links, network.Demands);  // Obliczamy DAP dla każdego chromosomu

                // Sortujemy chromosomy pod DAP i wybieramy tylko tyle najlepszych chromosomów ile wynosi populacja (po krzyżowaniu jest ich 2x więcej)
                DAPBestPopulation.Chromosomes.AddRange(population.Chromosomes);
                // Ze starej i nowej generacji sortujemy po DAP i wybieramy najlepsze
                DAPBestPopulation.Chromosomes = DAPBestPopulation.Chromosomes.OrderBy(x => x.DAPfitness).Take(populationSize).ToList();
                DAPBestPopulation.generationNumber = population.generationNumber;

                if (bestDAP <= DAPBestPopulation.Chromosomes[0].DAPfitness)    // Sprawdzamy, czy poprawiła się wartość F(x) dla najlepszego chromosomu i jak 
                {
                    iterations++;       // jak nie to zwiększamy ilość iteracji do kryterium
                }
                else                    // A jak tak to:
                {
                    bestDAP = DAPBestPopulation.Chromosomes[0].DAPfitness;     // Obliczamy nową najlepszą wartość DAP
                    iterations = 0;    // skoro poprawiła się najlepsza wartość to zerujemy iterację do kryterium ilości iteracji bez poprawy
                }

                PrintInfoAboutBestChromosome(DAPBestPopulation, "Current Best Chromosome For DAP");

                numberOfGenerations--;      // Kiedy spadnie do 0 to algorytm dla kryterium generacji skończy działanie

                population = new Population()   // Usuwamy starą populację i w jej miejsce tworzymy nową
                {
                    generationNumber = DAPBestPopulation.generationNumber
                };    
                population.Chromosomes.AddRange(DAPBestPopulation.Chromosomes); // I kopiujemy do niej nową najlepszą. Trzeba w ten sposób.
                //timer.Stop();
            }

            PrintInfoAboutBestChromosome(DAPBestPopulation, "Final Best Chromosome For DAP");
        }

        public void DDAPSimulation(EvolutionaryAlgorythm algorythm, Population population)
        {
            var DDAPBestPopulation = new Population();
            DDAPBestPopulation.Chromosomes.AddRange(population.Chromosomes.OrderBy(x => x.DDAPfitness).ToList());

            bestDDAP = DDAPBestPopulation.Chromosomes[0].DDAPfitness;

            PrintInfoAboutBestChromosome(DDAPBestPopulation, "Current Best Chromosome For DDAP");
        }

        public static void PrintInfoAboutBestChromosome(Population population, string message)
        {
            Console.WriteLine($"\n{message}:\n");
            population.Chromosomes[0].PrintProperties();
        }

        public int GetCryterium()
        {

            int Cryterium = 1;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Podaj kryterium: \n" + "[1] Czas\n" + "[2] Liczba Generacji\n" + "[3] Liczba Mutacji\n" + "[4] Liczba Iteracji bez poprawy\n");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && option > 0 && option < 5)
                {
                    Cryterium = option;

                    switch (Cryterium)
                    {
                        case 1:
                            time = GetTime();
                            break;
                        case 2:
                            numberOfGenerations = GetGenerations();
                            break;
                        case 3:
                            numberOfMutations = GetMutationsNumber();
                            break;
                        case 4:
                            iterationsWithoutBetterSolution = GetIterations();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return Cryterium;
        }

        public static int GetTime()
        {

            int time = 1;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Podaj Czas w sekundach: ");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && time > 0)
                {
                    time = option;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return time;
        }

        public Boolean GetProblem()
        {

            int problem = 1;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Wybierz problem do rozwiązania: \n" + "[1] DAP\n" + "[2] DDAP\n");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && problem > 0 && problem < 3)
                {
                    problem = option;

                    switch (problem)
                    {
                        case 1:
                            return true;
                        case 2:
                            return false;
                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return true;
        }

        public static int GetGenerations()
        {

            int generations = 1;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Podaj liczbę generacji: ");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && generations > 0)
                {
                    generations = option;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return generations;
        }

        public static int GetMutationsNumber()
        {

            int mutations = 1;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Podaj liczbę mutacji: ");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && mutations > 0)
                {
                    mutations = option;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return mutations;
        }

        public static int GetIterations()
        {

            int iterations = 1;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Podaj liczbę iteracji: ");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && iterations > 0)
                {
                    iterations = option;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return iterations;
        }

        public Boolean StopAlgorythm()
        {
            if (stopCryterium == 1)
            {
                return time == 0;
            }
            else if (stopCryterium == 2)
            {
                return numberOfGenerations == 0;
            }
            else if (stopCryterium == 3)
            {
                return numberOfMutations <= 0;
            }
            else if (stopCryterium == 4)
            {
                return iterations >= iterationsWithoutBetterSolution;
            }
            return true;
        }
    }
}
