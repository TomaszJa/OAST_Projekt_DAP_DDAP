using System;
using System.Collections.Generic;
using System.Text;

namespace OAST_Projekt_DAP_DDAP.NetworkElements
{
    public class Network
    {
        public int numberOfLinks;
        public int numberOfDemands;
        public List<Link> Links = new List<Link>();
        public List<Demand> Demands = new List<Demand>();
        public List<Node> Nodes = new List<Node>();
    }
}
