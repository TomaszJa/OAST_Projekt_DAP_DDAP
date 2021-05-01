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
        // Funkcja, która na podstawie wybranego problemu wybiera odpowiednią funkcję do wczytania pliku
        public static void ReadFile()
        {
            string problem = GetProblem();
            string filePath = GetDesiredFilePath();
            var network = new Network();

            switch(problem)
            {
                case "DAP":
                    network = ReadFileForDAP(filePath);

                    var algor = new EvolutionaryAlgorythm();

                    algor.GenerateChromosome(network.Demands);

                    break;
                case "DDAP":
                    ReadFileForDDAP(filePath);
                    break;
                default:
                    break;
            }
        }

        public static Network ReadFileForDAP(string _filePath)
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
                    int routeLinesNumber = 0;           // Zmienna do numeru linijki z ścieżkami


                    while (!streamR.EndOfStream)
                    {
                        line = streamR.ReadLine();      // Wczytywanie linijki i przypisanie jej wartości do zmiennej
                        lineNumber++;
                        Console.WriteLine(line);

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

        public static void ReadFileForDDAP(string _filePath)
        {
            // Wczytywanie z pliku (do zrobienia)
            using (StreamReader streamR = new StreamReader(_filePath))
            {
                string line;

                while (!streamR.EndOfStream)
                {
                    line = streamR.ReadLine();
                    Console.WriteLine(line);

                }
            }
        }

        // Funkcja do wyboru rozwiązywanego problemu
        public static string GetProblem()
        {
            string problem = null;
            bool success = false;

            while (!success)
            {
                Console.WriteLine("Wybierz problem do rozwiazania:\n" + "[1] DAP\n" + "[2] DDAP\n");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && option > 0 && option < 3)          // Jeżeli udało się to zrobić, i wartość jest z przedziału 1-2 to przypisz odpowiednią wartość         
                {
                    switch (option)
                    {
                        case 1:
                            problem = "DAP";
                            break;
                        case 2:
                            problem = "DDAP";
                            break;
                        default:
                            Console.WriteLine("Niepoprawna wartosc!");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;            // Jeżeli wartość uda się zamienić na int, ale będzie spoza przedziału to nie wyjdź z pętli while
                }
            }

            return problem;
        }
        // Funkcja odpowiedzialna za znalezienie ścieżki do wybranego pliku
        public static string GetDesiredFilePath()
        {
            string filePath = null;                 // zmienna przechowująca ścieżkę do pliku
            bool success = false;                   // udany, poprawny wybór

            while (!success)
            {
                Console.WriteLine("Wybierz plik z danymi:\n" + "[1] net12_1.txt\n" + "[2] net12_2.txt\n" + "[3] net4.txt\n");
                string value = Console.ReadLine();             // Zmienna przechowująca wybór użytkownika

                int option;
                success = int.TryParse(value, out option);      // "Spróbuj zamienić string wpisany przez użytkownika na int i zapisać go do zmiennej option"

                if (success && option > 0 && option < 4)          // Jeżeli udało się to zrobić, i wartość jest z przedziału 1-3 to przypisz odpowiednią wartość         
                {
                    // Wyznaczenie ścieżki do pliku z danymi (bo jest różne w zależności od kompa)
                    // katalog z danymi umieszczam w folderze data, w folderze, w którym znajduje się plik .exe
                    // funkcja Path.GetFullPath() zwraca pełną ścieżkę do szukanego pliku.
                    switch (option)
                    {
                        case 1:
                            filePath = Path.GetFullPath("data/net12_1.txt");
                            break;
                        case 2:
                            filePath = Path.GetFullPath("data/net12_2.txt");
                            break;
                        case 3:
                            filePath = Path.GetFullPath("data/net4.txt");
                            break;
                        default:
                            Console.WriteLine("Niepoprawna wartosc!");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartosc!\n===================");
                    success = false;            // Jeżeli wartość uda się zamienić na int, ale będzie spoza przedziału to nie wyjdź z pętli while
                }
            }

            return filePath;
        }
    }
}
