using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskAboutPrivatization
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph(8);
            graph.PrintToConsole();
            Console.ReadLine();
        }
    }
}
