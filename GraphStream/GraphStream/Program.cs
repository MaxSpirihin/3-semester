using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphStream
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph g = new Graph();
            g.Output();
            g.ComputeStream() ;
            g.OutputStream();

            Console.ReadLine();
        }
    }
}
