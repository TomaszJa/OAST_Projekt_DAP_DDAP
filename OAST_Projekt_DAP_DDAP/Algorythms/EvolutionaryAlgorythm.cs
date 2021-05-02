using OAST_Projekt_DAP_DDAP.NetworkElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.Algorythms
{
    public class EvolutionaryAlgorythm
    {
        const int DEFAULT_POPULATION_SIZE = 5;
        const double DEFAULT_MUTATION_PROBABILITY = 0.05;
        const double DEFAULT_CROSSOVER_PROBABILITY = 0.2;
        const int DEFAULT_SEED = 1;

        Random random = new Random(DEFAULT_SEED);   // Domyślnie random będzie korzystać z tego samego ziarna za każdym razem

        public EvolutionaryAlgorythm()      // Konstruktor domyślny
        {

        }

        public EvolutionaryAlgorythm(int SEED)
        {
            random = new Random(SEED);          // W konstruktorze można podać wybraną wartość ziarna
        }

        public Chromosome GenerateChromosome(List<Demand> _demands)
        {
            var genes = new List<Gene>();       // Lista genów chromosomu, którą tworzymy na podstawie żądań

            foreach (var demand in _demands)    // Dla każdego żądania tworzymy losowy podział przepływu
            {
                var demandSize = demand.demandSize;     // Zmienna przechowująca przepływność do rozdysponowania po ścieżkach
                var numberOfPaths = demand.numberOfPaths;       // Zmienna przechowująca liczbę ścieżek

                // lista z allelami, czyli podziałem przepływu na poszczególne ścieżki.
                // Początkowo generowana jest jako lista 0, której długość jest równa ilości ścieżek.
                // Na każdej ścieżce początkowo przydzielona przepływność wynosi 0.
                var listOfAlleles = new List<int>(new int[numberOfPaths]);      

                var demandToAssign = demandSize;    // Zmienna pomocnicza która służy do sprawdzenia ile pozostało zasobów do przydzielenia

                while (demandToAssign > 0)
                {
                    var alleleToIncrement = random.Next(0, numberOfPaths);      // Wybieramy losowo ścieżkę, do której przydzielimy jednostkę przepływności
                    listOfAlleles[alleleToIncrement] += 1;      // i przydzielamy właśnie tę jednostkę

                    demandToAssign--;       // przydzieliliśmy 1 jednostkę przepływności, więc zmniejszamy pulę
                }

                genes.Add(new Gene(listOfAlleles, demandSize));     // Po rozdysponowaniu wszystkich przepływności tworzymy nowy gen i go dodajemy.
            }

            var chromosome = new Chromosome(genes, 0, 0);       // Po utworzeniu wszystkich genów tworzymy z nich chromosom

            return chromosome;
        }

        public Population GenerateStartingPopulation(List<Demand> _demands, int _populationSize = DEFAULT_POPULATION_SIZE)
        {
            var firstPopulation = new Population()
            {
                generationNumber = 1
            };

            for (int i = 0; i < _populationSize; i++)   // stwórz tyle chromosomów ile jest populacji
            {
                var chromosome = GenerateChromosome(_demands);      
                firstPopulation.Chromosomes.Add(chromosome);        // i dodaj do populacji każdy chromosom
            }

            return firstPopulation;
        }

        public Chromosome MutateChromosome(Chromosome _chromosome, double _mutationProbability = DEFAULT_MUTATION_PROBABILITY)
        {
            foreach (var gene in _chromosome.Genes)     // Algorytm mutacji wykonujemy na każdym genie...
            {
                if (EventProbability(_mutationProbability))     // ...Jeżeli prawdopodobieństwo wyniesie odpowiednią wartość
                {
                    var numberOfPaths = gene.listOfAlleles.Count;

                    // Mutacja polega na odebraniu jednostki przepływności z jednego allela (jednej ścieżki)
                    // i przekazaniu jej innemu allelowi, dlatego losuję dwa allele, między którymi dojdzie do zamiany.
                    // Oczywiście zakładam (i najpewniej tak jest, bo inaczej algorytm nie miałby to sensu), 
                    // że istnieją zawsze przynajmniej 2 ścieżki, między którymi można rozdzielić przepływy
                    var firstPath = random.Next(0, numberOfPaths);
                    var secondPath = random.Next(0, numberOfPaths);     // losujemy drugą
                    var success = false;

                    while (!success)
                    {
                        if (gene.listOfAlleles[firstPath] == 0)     // Z pierwszej ścieżki zabierzemy przepływ, więc musi być on niezerowy
                        {
                            firstPath = random.Next(0, numberOfPaths);
                        }
                        else if (gene.listOfAlleles[firstPath] > 0)
                        {
                            success = true;
                        }
                    }

                    success = false;

                    while (!success)     // Może się tak zdarzyć, że druga wylosowana ścieżka będzie taka sama co pierwsza a tego nie chcemy
                    {
                        if (secondPath != firstPath)        // jeżeli ścieżki są różne to można wyjść z pętli
                        {
                            success = true;
                        }
                        else
                        {
                            secondPath = random.Next(0, numberOfPaths);     // Jeżeli są takie same to losujemy inną
                        }
                    }

                    // na koniec z pierwszej odejmujemy jednostkę przepływu, a drugiej ją dodajemy
                    gene.listOfAlleles[firstPath]--;
                    gene.listOfAlleles[secondPath]++;

                    _chromosome.wasMutated = true;
                }
            }
            return _chromosome;
        }

        public List<Chromosome> CrossoverChromosomes(List<Chromosome> _chromosomes, double _crossoverProbability = DEFAULT_CROSSOVER_PROBABILITY)
        {
            if (EventProbability(_crossoverProbability))
            {
                var parentChromosomes = _chromosomes;   // Lista z rodzicami

            }
            return _chromosomes;
        }

        public Boolean EventProbability(double probability)     // funkcja do losowania na podstawie zadanego prawdopodobieństwa
        {
            bool result = random.NextDouble() < probability;    // zwraca true jeżeli wylosowana wartość jest mniejsza od prawdopodobieństwa
            return result;
        }
    }
}
