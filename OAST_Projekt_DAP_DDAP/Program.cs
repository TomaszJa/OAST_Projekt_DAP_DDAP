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
                string filePath = FileParser.GetDesiredFile();

                using (StreamReader streamR = new StreamReader(filePath))
                {
                    string line;

                    while (!streamR.EndOfStream)
                    {
                        line = streamR.ReadLine();
                        Console.WriteLine(line);

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
