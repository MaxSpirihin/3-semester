using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Predator
{
    class Program
    {
        static void Main(string[] args)
        {
            for (; ; )
            {
                Console.Clear();
                Console.WriteLine("     ХИЩНИК - ЖЕРТВА.  ИИ против ИИ\n");
                Console.Write("Введите позицию выхода: ");
                int exit = Convert.ToInt32(Console.ReadLine());
                Console.Write("Введите позицию хищника: ");
                int predator = Convert.ToInt32(Console.ReadLine());
                Console.Write("Введите позицию жертвы: ");
                int victim = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                Game game = new Game(exit);
                game.Start(victim, predator);
                Console.WriteLine("\nЖмите Enter для рестарта");
                Console.ReadLine();
            }
        }
    }
}
