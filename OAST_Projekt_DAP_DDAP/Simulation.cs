using OAST_Projekt_DAP_DDAP.Algorythms;
using OAST_Projekt_DAP_DDAP.NetworkElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OAST_Projekt_DAP_DDAP
{
    public class Simulation
    {
        public int numberOfMutations;
        ReadFromFile SimulationInfo = new ReadFromFile();
        public int seed;
        public int populationSize;
        public double mutationProbability;
        public double crossoverProbability;
        public Network network = new Network();

        public Simulation()
        {
            SimulationInfo.GetSimulationInfo();

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



            foreach (var chromosome in population.Chromosomes)
            {
                algorythm.MutateChromosome(chromosome, mutationProbability);
                if (chromosome.wasMutated)
                {
                    numberOfMutations++;
                }
            }
            algorythm.CrossoverChromosomes(population.Chromosomes, crossoverProbability);

        }


    }
}
