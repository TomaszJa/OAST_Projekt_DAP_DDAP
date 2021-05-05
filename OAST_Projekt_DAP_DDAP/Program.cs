using System;
using System.IO;

namespace OAST_Projekt_DAP_DDAP
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var readFromFile = new ReadFromFile();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
