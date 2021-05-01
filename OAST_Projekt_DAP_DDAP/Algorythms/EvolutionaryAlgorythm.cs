using OAST_Projekt_DAP_DDAP.NetworkElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.Algorythms
{
    public class EvolutionaryAlgorythm
    {
        static int DEFAULT_POPULATION_SIZE = 5;
        static double DEFAULT_MUTATION_PROBABILITY = 0.05;
        static double DEFAULT_CROSSOVER_PROBABILITY = 0.2;
        static int DEFAULT_SEED = 1;

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
    }
}
