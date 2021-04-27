using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    public class Network
    {
        public int numberOfLinks;
        public int numberOfDemands;
        public List<Link> links = new List<Link>();
    }
}
