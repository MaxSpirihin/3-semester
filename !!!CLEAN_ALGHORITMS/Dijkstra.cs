using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GraphStream
{
    class Graph
    {

        private class Top
        {
            public int Length { get; set; }//путь от начальной вершины
            public int Prev { get; set; }//пред. вершина
            public bool Const { get; set; }//постоянность

            public Top()
            {
                Const = false;
                Length = Int32.MaxValue;
                Prev = -1;
            }
        }


        const int MaxLength = 15;
        public int Size { get; private set; }
        private int[,] Mtx;


        public Graph()
        {
            try
            {
                StreamReader reader = new StreamReader("input.txt");//создаем поток ввода из файла
                //считаем количество строк в файле. Это будет Length
                int CountOfStrings = 0;
                while (!reader.EndOfStream)
                {
                    reader.ReadLine();
                    CountOfStrings++;
                }
                if (CountOfStrings > MaxLength) return;
                Size = CountOfStrings;
                reader.Close();


                //далее открываем файл заново и считываем саму матрицу
                reader = new StreamReader("input.txt");
                Mtx = new int[Size, Size];
                for (int i = 0; i < Size; i++)
                {
                    string[] Array = reader.ReadLine().Split(' ');                                 //считывается строка из файла и разрезается по пробелам
                    int j = 0;
                    foreach (string s in Array)
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            Mtx[i, j] = Convert.ToInt32(s);
                            j++;
                        }
                    }
                }
                this.Normalize();
            }
            catch (FileNotFoundException) { return; }
            catch (IndexOutOfRangeException) { return; }
        }




        //просто вывод матрицы смежности на консоль
        public void Output()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                    Console.Write(Mtx[i, j] + " ");
                Console.WriteLine();
            }
        }




        //нормализует граф (Mtx[i,j]=Mtx[j,i] and Mtx[i,i]=0)
        public void Normalize()
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    if (i == j) { Mtx[i, j] = 0; }
                    else if (j > i) { Mtx[j, i] = Mtx[i, j]; }
        }



        public List<int> Dijkstra(int from, int to) //поиск мин пути из вершины from в to возврат длины, заполнение массива пути
        {
            List<int> path = new List<int>();

            if (from == to)
            {
                path[0] = to;
                path[1] = to;
                return null;
            }
            //инициализируем массив вершин
            Top[] tops = new Top[Size];
            for (int i = 0; i < Size; i++)
            {
                tops[i] = new Top();
            }
            tops[from].Length = 0;//начальная вершина
            tops[from].Const = true;
            int currenttop;//текущая вершина
            currenttop = from;

            int amount = 0;//счетчик циклов(путь содержит не более n вершин. После каждй итерации цикла путь увеличивается на 1 вершину
            //Значит итераций циклов не может быть больше n. Считаем их и проверяем. Если счетчик перевалил за n,
            //значит что-то не так, а именно искомого пути не существует(связи нет). Выходим из цикла и возвращаем код ошибки


            while ((currenttop != to) && (amount < Size))//пока не дошли до конца (и счетчик не перепрыгнул предел)
            {

                for (int i = 0; i < Size; i++)//проходим по всем вершинам
                {
                    if ((Mtx[currenttop, i] != 0) && (tops[i].Const == false))//если currenttop и i связаны и i не постоянна
                    {
                        if (tops[i].Length > tops[currenttop].Length + Mtx[currenttop, i])//если новый способ дойти до i меньше делаем его
                        {
                            tops[i].Length = tops[currenttop].Length + Mtx[currenttop, i];
                            tops[i].Prev = currenttop;
                        }
                    }
                }

                int Min = Int32.MaxValue;//будем искать мин путь
                int Mintop = -1;
                for (int i = 0; i < Size; i++)//проходим по всем вершинам

                    if ((tops[i].Length < Min) && (tops[i].Const == false))//сравниваем с мин
                    {
                        Min = tops[i].Length;
                        Mintop = i;
                    }

                currenttop = Mintop;//минимальная вершина будет постоянной и новой текущей

                tops[currenttop].Const = true;
                amount++;
            }

            if (currenttop != to) return null;//если счетчик перепрыгнул предел возвращаем null

            //заполнение пути
            path.Add(to);
            while (currenttop != from)//заполняем массив пути
            {
                path.Add(tops[currenttop].Prev);
                currenttop = tops[currenttop].Prev;
            }
            return path;
        }



    }
}
