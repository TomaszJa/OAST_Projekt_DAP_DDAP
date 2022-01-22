using OAST_Projekt_DAP_DDAP.NetworkElements;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Taka sama funkcja jak MutateChromosome tylko dla pojedynczego genu
        public Gene MutateGene(Gene gene)
        {
            var numberOfPaths = gene.Alleles.Count;

            if (numberOfPaths > 1)
            {
                var firstPath = random.Next(0, numberOfPaths);
                var secondPath = random.Next(0, numberOfPaths);     // losujemy drugą
                var success = false;

                while (!success)
                {
                    if (gene.Alleles[firstPath] == 0)     // Z pierwszej ścieżki zabierzemy przepływ, więc musi być on niezerowy
                    {
                        firstPath = random.Next(0, numberOfPaths);
                    }
                    else if (gene.Alleles[firstPath] > 0)
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
                gene.Alleles[firstPath]--;
                gene.Alleles[secondPath]++;
            }
            return gene;
        }

        public Chromosome MutateChromosome(Chromosome _chromosome, double _mutationProbability = DEFAULT_MUTATION_PROBABILITY)
        {
            Chromosome newChromosome = CopyChromosome(_chromosome);

            foreach (var gene in newChromosome.Genes)     // Algorytm mutacji wykonujemy na każdym genie...
            {
                if (EventProbability(_mutationProbability))     // ...Jeżeli prawdopodobieństwo wyniesie odpowiednią wartość
                {
                    var numberOfPaths = gene.Alleles.Count;

                    // Mutacja polega na odebraniu jednostki przepływności z jednego allela (jednej ścieżki)
                    // i przekazaniu jej innemu allelowi, dlatego losuję dwa allele, między którymi dojdzie do zamiany.
                    // Oczywiście zakładam (i najpewniej tak jest, bo inaczej algorytm nie miałby sensu), 
                    // że istnieją zawsze przynajmniej 2 ścieżki, między którymi można rozdzielić przepływy
                    // Update: Jednak tak nie jest i straciłem na tym 3h :|
                    if (numberOfPaths > 1)
                    {
                        var firstPath = random.Next(0, numberOfPaths);
                        var secondPath = random.Next(0, numberOfPaths);     // losujemy drugą
                        var success = false;

                        while (!success)
                        {
                            if (gene.Alleles[firstPath] == 0)     // Z pierwszej ścieżki zabierzemy przepływ, więc musi być on niezerowy
                            {
                                firstPath = random.Next(0, numberOfPaths);
                            }
                            else if (gene.Alleles[firstPath] > 0)
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
                        gene.Alleles[firstPath]--;
                        gene.Alleles[secondPath]++;

                        newChromosome.wasMutated = true;
                    }
                }
            }
            return newChromosome;
        }

        // Ta metoda jest bardzo istotna, bo inaczej generowane są bugi.
        // Wynika to z tego, że gdy kopiujemy obiekt, który ma pola to tak naprawdę
        // nie tworzymy nowych miejsc w pamięci, które przechowują te pola, tylko
        // referencje do starych pól. To jest spoko w teorii, bo oszczędza pamięć,
        // ale w praktyce sprawia, że gdy modyfikujemy elementy w nowym obiekcie,
        // to w rzeczywistości modyfikujemy też te same elementy w starym.
        // O ile "pojedyncze" pola po skopiowaniu od razu tworzą się jako nowe,
        // o tyle kopiowanie kolekcji to tak naprawdę kopiowanie referencji do starej kolekcji
        // dlatego trzeba przelecieć po wszystkich elementach w starej kolekcji i kopiować je
        // "ręcznie" do nowej.
        public Chromosome CopyChromosome(Chromosome _chromosome)
        {
            var newChromosome = new Chromosome()
            {
                DAPfitness = _chromosome.DAPfitness,
                DDAPfitness = _chromosome.DDAPfitness,
                wasMutated = _chromosome.wasMutated
            };

            foreach (var gene in _chromosome.Genes)
            {
                var newGene = new Gene()
                {
                    demandSize = gene.demandSize
                };
                foreach (var allele in gene.Alleles)
                {
                    newGene.Alleles.Add(allele);
                }
                newChromosome.Genes.Add(newGene);
            }

            return newChromosome;
        }

        public List<Chromosome> CrossoverChromosomes(List<Chromosome> _chromosomes, double _crossoverProbability = DEFAULT_CROSSOVER_PROBABILITY)
        {
            var parentChromosomes = new List<Chromosome>();  // Lista z rodzicami
            parentChromosomes.AddRange(_chromosomes);
            var childrenChromosomes = new List<Chromosome>();   // Lista z dziećmi

            while (parentChromosomes.Count >= 2)        // Wykonujemy krzyżowanie do momentu aż będą minumum 2 chromosmy do skrzyżowania ze sobą
            {
                var firstChromosome = parentChromosomes[0];     // Pobieranie dwóch rodziców
                var secondChromosome = parentChromosomes[1];

                if (EventProbability(_crossoverProbability))    // Jeżeli zachodzi krzyżowanie to...
                {
                    var firstChildrenChromosome = new Chromosome();     // ...tworzone są 2 dzieci...
                    var secondChildrenChromosome = new Chromosome();

                    for (int i = 0; i < firstChromosome.Genes.Count; i++)         // ...na podstawie genów rodziców
                    {
                        if (EventProbability(0.5))
                        {
                            firstChildrenChromosome.Genes.Add(firstChromosome.Genes[i]);    // Dodaj do pierwszego dziecka gen pierwszego rodzica
                            secondChildrenChromosome.Genes.Add(secondChromosome.Genes[i]);  // a do drugiego gen drugiego rodzica
                        }
                        else // lub odwrotnie
                        {
                            firstChildrenChromosome.Genes.Add(secondChromosome.Genes[i]);    // Dodaj do pierwszego dziecka gen drugiego rodzica
                            secondChildrenChromosome.Genes.Add(firstChromosome.Genes[i]);    // a do drugiego gen pierwszego rodzica
                        }
                    }

                    childrenChromosomes.Add(firstChildrenChromosome);       // utworzone chromosomy dodajemy do listy z dziećmi
                    childrenChromosomes.Add(secondChildrenChromosome);

                    parentChromosomes.RemoveAt(1);      // usuwamy z pomocniczej listy 1 i 2 rodzica, ponieważ potem na początku pętli
                    parentChromosomes.RemoveAt(0);      // wybieramy kolejne 2 chromosomy o indeksach 0 i 1 z tej listy i nie chcę żeby się powtarzały w nieskończoność
                }
            }
            _chromosomes.AddRange(childrenChromosomes);     // Na koniec dodajemy do rodziców ich dzieci, dzięki czemu uzyskujemy listę z oboma pokoleniami

            return _chromosomes;        // i ją zwracamy
        }

        public Boolean EventProbability(double probability)     // funkcja do losowania na podstawie zadanego prawdopodobieństwa
        {
            bool result = random.NextDouble() < probability;    // zwraca true jeżeli wylosowana wartość jest mniejsza od prawdopodobieństwa
            return result;
        }

        public Population CalculateFitness(Population _population, List<Link> _links, List<Demand> _demands)
        {
            // Celem DAP jest tak rozdysponować ruchem na łączach, by zminimalizować wartość F(x)
            // gdzie F(x) = max{l(e,x) - Ce}, czyli ruch na danym łączu - pojemność łącza.
            // W takim razie trzeba obliczyć F(x) dla każdego chromosomu.

            foreach (var chromosome in _population.Chromosomes)
            {
                // Upewniam się, że chromosom ma wyzerowane wartości fitness DAP i DDAP
                chromosome.DAPfitness = 0;
                chromosome.DDAPfitness = 0;

                int[] l = new int[_links.Count];    // tablica o długości odpowiadającej ilości łączy, przechwoująca wartość l(e,x) dla każdego łącza
                int[] F = new int[_links.Count];    // tablica przechowująca wartości F(x)
                int[] y = new int[_links.Count];    // tablica przechowująca wartości y dla każdego łącza (DDAP)

                for (int d = 0; d < chromosome.Genes.Count; d++)    // iterujemy po każdym genie w chromosomie (Wykład 1 slajd 13)
                {
                    for (int p = 0; p < chromosome.Genes[d].Alleles.Count; p++)   // po każdym Allelu (każdej ścieżce)
                    {
                        var allele = chromosome.Genes[d].Alleles[p];      // pobranie tablicy z allelami
                        var path = _demands[d].Paths[p];            // Pobranie konkretnej ścieżki, w której będziemy sprawdzać, czy znajdują się dane łącza

                        for (int e = 0; e < _links.Count; e++)      // e to numer łącza
                        {
                            if (path.LinksIds.Contains(e + 1))   // jako, że numerację łączy zaczynamy od 1 to dlatego e + 1
                            {
                                l[e] += allele;     // Pobieramy informację o obciążeniu dla danego łącza i dodajemy ją do sumy, z której wyjdzie l(e,x)
                            }

                        }
                    }
                }
                for (int i = 0; i < _links.Count; i++)
                {
                    double yValue = (double)l[i] / (double)_links[i].moduleSize;    // Obliczamy wartość y dla danego łącza czyli l(e,x)/rozmiar modułu
                    y[i] = (int)Math.Ceiling(yValue);       // i zaokrąglamy w górę, ponieważ jak będzie potrzeba przesłać 10,5 Mb to należy mieć 11 na łączu
                    F[i] = l[i] - _links[i].capacity;     // Obliczamy F(x) dla DAP
                    chromosome.DDAPfitness += y[i] * _links[i].moduleCost;      // Dodajemy do sumy DDAP, którą chcemy zminimalizować
                }
                chromosome.DAPfitness = F.Max();        // Obliczamy maksymalną wartość F(x) dla DAP i zapisujemy ją do zmiennej. Dążymy do tego, by ta wartość
                                                        // była jak najmniejsza
            }

            return _population;
        }

        public Population CalculateFitness(Population _population, List<Link> _links, List<Demand> _demands, List<Node> _nodes)
        {
            // Celem DAP jest tak rozdysponować ruchem na łączach, by zminimalizować wartość F(x)
            // gdzie F(x) = max{l(e,x) - Ce}, czyli ruch na danym łączu - pojemność łącza.
            // W takim razie trzeba obliczyć F(x) dla każdego chromosomu.

            foreach (var chromosome in _population.Chromosomes)
            {
                // Upewniam się, że każdy węzeł ma wyzerowane przepływy
                foreach (var node in _nodes)
                {
                    node.IncomingTraffic = 0;
                }
                int averageModuleCost = 0;  // średni koszt modułu potrzebny dla MPI i węzła

                // Upewniam się, że chromosom ma wyzerowane wartości fitness DAP i DDAP
                chromosome.DAPfitness = 0;
                chromosome.DDAPfitness = 0;

                int[] l = new int[_links.Count];    // tablica o długości odpowiadającej ilości łączy, przechwoująca wartość l(e,x) dla każdego łącza
                int[] F = new int[_links.Count];    // tablica przechowująca wartości F(x)
                int[] y = new int[_links.Count];    // tablica przechowująca wartości y dla każdego łącza (DDAP)

                for (int d = 0; d < chromosome.Genes.Count; d++)    // iterujemy po każdym genie w chromosomie (Wykład 1 slajd 13)
                {
                    for (int p = 0; p < chromosome.Genes[d].Alleles.Count; p++)   // po każdym Allelu (każdej ścieżce)
                    {
                        var allele = chromosome.Genes[d].Alleles[p];      // pobranie tablicy z allelami
                        var path = _demands[d].Paths[p];            // Pobranie konkretnej ścieżki, w której będziemy sprawdzać, czy znajdują się dane łącza

                        for (int e = 0; e < _links.Count; e++)      // e to numer łącza
                        {
                            if (path.LinksIds.Contains(e + 1))   // jako, że numerację łączy zaczynamy od 1 to dlatego e + 1
                            {
                                l[e] += allele;     // Pobieramy informację o obciążeniu dla danego łącza i dodajemy ją do sumy, z której wyjdzie l(e,x)
                            }

                        }
                    }
                }
                for (int i = 0; i < _links.Count; i++)
                {
                    double yValue = (double)l[i] / (double)_links[i].moduleSize;    // Obliczamy wartość y dla danego łącza czyli l(e,x)/rozmiar modułu
                    y[i] = (int)Math.Ceiling(yValue);       // i zaokrąglamy w górę, ponieważ jak będzie potrzeba przesłać 10,5 Mb to należy mieć 11 na łączu
                    F[i] = l[i] - _links[i].capacity;     // Obliczamy F(x) dla DAP
                    chromosome.DDAPfitness += y[i] * _links[i].moduleCost;      // Dodajemy do sumy DDAP, którą chcemy zminimalizować

                    foreach (var node in _nodes)
                    {
                        if (node.Index == _links[i].startingNode || node.Index == _links[i].endingNode)
                        {
                            node.IncomingTraffic += y[i];
                        }
                    }
                    averageModuleCost += _links[i].moduleCost;
                }
                averageModuleCost /= _links.Count;
                foreach (var node in _nodes)
                {
                    if (node.IncomingTraffic > node.Capacity)
                    {
                        chromosome.DDAPfitness += (node.IncomingTraffic - node.Capacity) * 100; // dodajemy do DDAP koszt wyjścia poza pojemność VM (koszt odrzucenia)
                        chromosome.DDAPfitness += node.Capacity * 1;
                    }
                    else
                    {
                        chromosome.DDAPfitness += node.IncomingTraffic * 1;
                    }
                }

                chromosome.DAPfitness = F.Max();        // Obliczamy maksymalną wartość F(x) dla DAP i zapisujemy ją do zmiennej. Dążymy do tego, by ta wartość
                                                        // była jak najmniejsza
            }

            return _population;
        }
    }
}
