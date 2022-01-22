using OAST_Projekt_DAP_DDAP.Algorythms;
using OAST_Projekt_DAP_DDAP.NetworkElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OAST_Projekt_DAP_DDAP
{
    // Klasa do wyszukiwania ścieżki do pliku żeby nie śmiecić w main
    public class ReadFromFile
    {
        public string filePath = null;
        public int seed;
        public int populationSize;
        public double mutationProbability;
        public double crossoverProbability;
        public Network network = new Network();

        public void GetSimulationInfo()
        {
            filePath = GetDesiredFilePath();
            seed = GetSeed();
            populationSize = GetPopulationSize();
            mutationProbability = GetMutationProbability();
            crossoverProbability = GetCrossoverProbability();
            network = ReadFile(filePath);
        }

        public static int GetSeed()
        {

            int seed = 1;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Podaj Ziarno do generatora: ");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success)              
                {
                    seed = option;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return seed;
        }

        public static int GetPopulationSize()
        {

            int populationSize = 1;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Podaj wielkosc populacji poczatkowej: ");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success)
                {
                    populationSize = option;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return populationSize;
        }

        public static double GetMutationProbability()
        {
            double mutationProbability = 0.05;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Podaj prawdopodobienstwo mutacji: ");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                double option;
                success = double.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && option <= 1 && option > 0)
                {
                    mutationProbability = option;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return mutationProbability;
        }

        public static double GetCrossoverProbability()
        {
            double crossoverProbability = 0.05;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Podaj prawdopodobienstwo krzyzowania: ");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                double option;
                success = double.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && option <= 1)
                {
                    crossoverProbability = option;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;
                }
            }

            return crossoverProbability;
        }

        // Funkcja pobierająca wszystkie dane z pliku
        public static Network ReadFile(string _filePath)
        {
            // Potrzebne zmienne: 
            // węzeł początkowy
            // węzeł końcowy
            // ilość modułów
            // wielkość modułów
            // ilość zapotrzebowań
            // ilość dostępnych ścieżek
            // 

            var network = new Network()
            {
                numberOfLinks = 0,
                numberOfDemands = 0,
                Links = new List<Link>(),
                Demands = new List<Demand>()
            };

            var basicDemand = new Demand()
            {
                startNode = 0,
                destinationNode = 0,
                demandSize = 0,
                numberOfPaths = 0,
                Paths = new List<PathToNode>()
            };

            bool demandSaved = false;       // Dzięki tej zmiennej na pewno wczytają się wszystkie żądania

            try
            {
                // Wczytywanie z pliku (do zrobienia)
                using (StreamReader streamR = new StreamReader(_filePath))
                {
                    string line;                    // Zmienna przechowująca wartość linijki wczytanej z pliku
                    int lineNumber = 0;             // Zmienna potrzebna do przechwywania numeru linijki we wczytywanym pliku
                    bool collectDemandInfo = false;      // Zmienna do warunku czy należy zacząć zbierać info o żądaniu
                    bool collectPaths = false;          // Zmienna do warunku czy należy pobierać info o ścieżkach żądania
                    bool collectNodes = false;
                    int routeLinesNumber = 0;           // Zmienna do numeru linijki z ścieżkami


                    while (!streamR.EndOfStream)
                    {
                        line = streamR.ReadLine();      // Wczytywanie linijki i przypisanie jej wartości do zmiennej
                        lineNumber++;

                        // Info o łączach w pliku danych kończy się na -1.
                        // -1 znajduje się w linijce o numerze = (liczba łączy + 2) {taki format pliku}
                        if (lineNumber <= network.numberOfLinks + 1)
                        {
                            // W pierwszej linijce jest informacja o liczbie łączy w sieci
                            if (lineNumber == 1)
                            {
                                network.numberOfLinks = int.Parse(line);        // Pobranie informacji o liczbie łączy
                            }
                            else
                            {
                                var values = line.Split(" ");       // Wartości w liniach oddzielone są spacjami, więc zapisujemy je do tablicy

                                // values:
                                // [0] - węzeł początkowy
                                // [1] - węzeł końcowy
                                // [2] - ilość modułów
                                // [3] - koszt modułów  {niepotrzebny w DAP}
                                // [4] - wielkość modułu 

                                int _startingNode = int.Parse(values[0]);
                                int _endingNode = int.Parse(values[1]);
                                int _numberOfModules = int.Parse(values[2]);
                                int _moduleCost = int.Parse(values[3]);
                                int _moduleSize = int.Parse(values[4]);


                                var link = new Link()               // na podstawie pobranych wartości tworzymy nowe łącze...
                                {
                                    linkNumber = lineNumber - 1,                  // numer łącza odpowiada numerowi linii - 1
                                    startingNode = _startingNode,
                                    endingNode = _endingNode,
                                    capacity = _numberOfModules * _moduleSize,   // tak kazał to liczyć
                                    numberOfModules = _numberOfModules,
                                    moduleCost = _moduleCost,
                                    moduleSize = _moduleSize
                                };

                                network.Links.Add(link);            // ...i dodajemy je do sieci
                            }
                        }

                        if (collectNodes)
                        {
                            var values = line.Split(" ");       // Wartości w liniach oddzielone są spacjami, więc zapisujemy je do tablicy

                            // values:
                            // [0] - indeks węzła
                            // [1] - pojemność węzła

                            Node node = new Node()
                            {
                                Index = int.Parse(values[0]),
                                Capacity = int.Parse(values[1]),
                                IncomingTraffic = 0
                            };

                            network.Nodes.Add(node);
                        }

                        if (line == "Nodes")
                        {
                            collectNodes = true;
                            collectDemandInfo = false;
                            collectPaths = false;
                        }

                        if (lineNumber == network.numberOfLinks + 4)    // numer linijki, w której w pliku jest info o liczbie demand
                        {
                            network.numberOfDemands = int.Parse(line);
                            routeLinesNumber = lineNumber + 4;
                        }

                        if (line == "" && collectPaths && lineNumber > routeLinesNumber)
                        {
                            network.Demands.Add(basicDemand);       // Zapisanie żądania

                            demandSaved = true;         // Zmienna pomocnicza

                            basicDemand = new Demand()      // po dodaniu żądania resetujemy jego wartości
                            {
                                startNode = 0,
                                destinationNode = 0,
                                demandSize = 0,
                                numberOfPaths = 0,
                                Paths = new List<PathToNode>()
                            };
                        }

                        if (lineNumber > network.numberOfLinks + 4 && line == "")   // pobieranie info o nowym żądaniu
                        {
                            routeLinesNumber = lineNumber + 2;
                            collectDemandInfo = true;
                            collectPaths = false;
                        }

                        if (line != "" && collectDemandInfo && lineNumber <= routeLinesNumber)    // pobieranie info o żądaniu...
                        {
                            var values = line.Split(" ");

                            if (values.Length == 1)
                            {
                                basicDemand.numberOfPaths = int.Parse(values[0]);   // ...ile jest w nim ścieżek
                                collectPaths = true;
                            }
                            else
                            {
                                basicDemand.startNode = int.Parse(values[0]);           // i pozostałe dane
                                basicDemand.destinationNode = int.Parse(values[1]);
                                basicDemand.demandSize = int.Parse(values[2]);
                            }                           

                        }

                        if (line != "" && collectPaths && lineNumber > routeLinesNumber)    // Wczytywanie Ścieżek żądania
                        {
                            var values = line.Split(" ");
                            var pathToNode = new PathToNode()
                            {
                                pathNumber = int.Parse(values[0])       // Pobranie numeru ścieżki
                            };

                            for (int i = 1; i < values.Length; i++)     // wpisanie indeksów łączy do ścieżki
                            {
                                int linkId;

                                if (int.TryParse(values[i], out linkId))
                                {
                                    pathToNode.LinksIds.Add(linkId);
                                }                              
                            }

                            basicDemand.Paths.Add(pathToNode);      // Po pobraniu wszystkich wartości dodajemy ścieżkę

                            demandSaved = false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Poniższy if jest wymagany, ponieważ gdy plik z danymi ma za mało pustych linijek na końcu to ostatnie żądanie się
            // nie dodaje w głównej pętli i trzeba je dodać po.
            if (!demandSaved)
            {
                network.Demands.Add(basicDemand);
            }

            return network; // zwracamy obiekt klasy Network, w którym są wszystkie potrzebne dane wczytane w ładny sposób.
        }

        // Funkcja odpowiedzialna za znalezienie ścieżki do wybranego pliku
        public static string GetDesiredFilePath()
        {
            string filePath = null;                 // zmienna przechowująca ścieżkę do pliku
            bool success = false;                   // udany, poprawny wybór

            while (!success)
            {
                var directoryPath = Path.GetFullPath("data/");
                if (Directory.Exists(directoryPath))
                {
                    string[] files = Directory.GetFiles(directoryPath);
                    Console.WriteLine("Wybierz plik z danymi:");
                    for (int i = 0; i < files.Length; i++)
                    {
                        int found = files[i].IndexOf("data");
                        Console.WriteLine($"[{i}] {files[i].Substring(found + 5)}");
                    }

                    string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                    int option;
                    success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                    if (success && option >= 0 && option < files.Length)          // Jeżeli udało się to zrobić, i wartość jest z przedziału 1-3 to przypisz odpowiednią wartość         
                    {
                        // Wyznaczenie ścieżki do pliku z danymi (bo jest różne w zależności od kompa)
                        // katalog z danymi umieszczam w folderze data, w folderze, w którym znajduje się plik .exe
                        // funkcja Path.GetFullPath() zwraca pełną ścieżkę do szukanego pliku.
                        filePath = files[option];
                    }
                    else
                    {
                        Console.WriteLine("Niepoprawna wartosc!\n===================");
                        success = false;
                    }
                }
            }

            return filePath;
        }
    }
}
