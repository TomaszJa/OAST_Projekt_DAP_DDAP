using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    // Klasa opisująca łącze
    public class Link
    {
        public int linkNumber;      // Numer łącza jest potrzebny do ścieżki
        public int startingNode;
        public int endingNode;
        public int numberOfModules;
        public int moduleCost;
        public int moduleSize;
        public int capacity;        // Pojemność do DAP

        public void PrintProperties()
        {
            Console.WriteLine($"Numer lacza: {linkNumber}, Wezel poczatkowy: {startingNode}, " +
                $"Wezel koncowy: {endingNode}, Ilosc modulow: {numberOfModules} " +
                $"Koszt modulu: {moduleCost}, Rozmiar modulu: {moduleSize}, Pojemnosc lacza: {capacity}");
        }
    }
}
