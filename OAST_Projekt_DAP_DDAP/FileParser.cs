using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OAST_Projekt_DAP_DDAP
{
    // Klasa do wczytywania pliku żeby nie śmiecić w main
    public class FileParser
    {
        public string filePath;

        public FileParser()
        {
            filePath = GetDesiredFile();
        }

        // Funkcja odpowiedzialna za znalezienie ścieżki do wybranego pliku
        public static string GetDesiredFile()
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
