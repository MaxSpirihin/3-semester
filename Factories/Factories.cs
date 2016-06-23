using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Factorie
{
    class Factories
    {
        private int[,] Mtx;
        public bool[,] MatchingMtx { get; private set; }
        const int MaxLength = 15;
        public int LengthRow { get; private set; }
        public int LengthColumn { get; private set; }

        public Factories()
        {
            string NameOfFile = "input.txt";
            try
            {
                StreamReader reader = new StreamReader(NameOfFile);//создаем поток ввода из файла
                //считаем количество строк в файле. Это будет Length
                int CountOfStrings = 0;
                while (!reader.EndOfStream)
                {
                    string s = reader.ReadLine();
                    LengthColumn = s.Split('|')[1].Split(' ').Length;
                    CountOfStrings += Convert.ToInt32(s.Split('|')[0]);
                }
                if (CountOfStrings > MaxLength) return;
                LengthRow = CountOfStrings;
                reader.Close();


                //далее открываем файл заново и считываем саму матрицу
                reader = new StreamReader(NameOfFile);
                Mtx = new int[LengthRow, LengthColumn];
                int row = 0;
                while (row < LengthRow)
                {
                    string str = reader.ReadLine();
                    int count = Convert.ToInt32(str.Split('|')[0]);
                    string[] Array = str.Split('|')[1].Split(' '); //считывается строка из файла и разрезается по пробелам

                    for (int i = 0; i < count; i++)
                    {
                        int column = 0;
                        foreach (string s in Array)
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                Mtx[row, column] = Convert.ToInt32(s);
                                column++;
                            }
                        }
                        row++;
                    }
                }
            }
            catch (FileNotFoundException) { return; }
            catch (IndexOutOfRangeException) { return; }
        }


        //просто вывод матрицы смежности на консоль
        public void Output()
        {
            Console.WriteLine();
            Console.WriteLine("Matrix");

            for (int row = 0; row < LengthRow; row++)
            {
                for (int column = 0; column < LengthColumn; column++)
                    Console.Write(Mtx[row, column] + " ");
                Console.WriteLine();
            }
        }


        //просто вывод матрицы смежности на консоль
        public void OutputMatching()
        {
            Console.WriteLine();
            Console.WriteLine("Matching");

            for (int row = 0; row < LengthRow; row++)
            {
                for (int column = 0; column < LengthColumn; column++)
                    Console.Write(Convert.ToInt32(MatchingMtx[row, column]) + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.Write("Max time: ");
            int maxTime = 0;
            for (int row = 0; row < LengthRow; row++)
                for (int column = 0; column < LengthColumn; column++)
                    if ((MatchingMtx[row, column]) && (Mtx[row, column] > maxTime))
                        maxTime = Mtx[row, column];
            Console.WriteLine(maxTime);
        }


        public int TotalTime()
        {
            int maxTime = 0;
            for (int row = 0; row < LengthRow; row++)
                for (int column = 0; column < LengthColumn; column++)
                    if ((MatchingMtx[row, column]) && (Mtx[row, column] > maxTime))
                        maxTime = Mtx[row, column];
            return maxTime;
        }

        public void AssignDelivery()
        {
            int currentFringe = Int32.MaxValue;
            while (true)
            {
          
                //ищем новую грань
                int max = 0;
                for (int row = 0; row < LengthRow; row++)
                    for (int column = 0; column < LengthColumn; column++)
                        if ((Mtx[row, column] < currentFringe) && (Mtx[row, column] > max))
                            max = Mtx[row, column];
                currentFringe = max;

                //создаем матрицу
                int[,] eMtx = new int[LengthRow, LengthColumn];
                for (int row = 0; row < LengthRow; row++)
                    for (int column = 0; column < LengthColumn; column++)
                        if (Mtx[row, column] <= currentFringe)
                            eMtx[row, column] = 1;
                        else
                            eMtx[row, column] = 0;
      
                
                //создаем объект граф
                Graph graph = new Graph(eMtx, LengthRow, LengthColumn);

                //ищем паросочетания
                graph.FindSaturatedMatching();

                //проверяем все ли магазины заняты (МАГАЗИНЫ - СТОЛБЦЫ)
                bool AllMarketIsOk = true;
                for (int column = 0; column < LengthColumn; column++)
                {
                    bool marketIsOk = false;
                    for (int row = 0; row < LengthRow; row++)
                        if (graph.MatchingMtx[row, column])
                            marketIsOk = true;
                    if (!marketIsOk)
                        AllMarketIsOk = false;
                }



                //если нет
                if (!AllMarketIsOk)
                {
                   
                    //конец
                    return;
                } 
                
                //переносим матри
                this.MatchingMtx = new bool[LengthRow, LengthColumn];
                for (int row = 0; row < LengthRow; row++)
                    for (int column = 0; column < LengthColumn; column++)
                        MatchingMtx[row, column] = graph.MatchingMtx[row, column];
                // заново

            }
        }


    }
}
