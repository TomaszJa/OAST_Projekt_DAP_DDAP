using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    public class Link
    {
        public int startingNode;
        public int endingNode;
        public int capacity;
        public int linkNumber;      // Numer łącza jest potrzebny do ścieżki
    }
}
