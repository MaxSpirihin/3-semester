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
            public int TopStream { get; set; }//путь от начальной вершины
            public int Prev { get; set; }//пред. вершина
            public bool Const { get; set; }//постоянность

            public Top()
            {
                Const = false;
                TopStream = -Int32.MaxValue;
                Prev = -1;
            }
        }


        const int MaxLength = 15;
        public int Size { get; private set; }
        private int[,] Mtx;
        private int[,] MtxStream;


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

            }
            catch (FileNotFoundException) { return; }
            catch (IndexOutOfRangeException) { return; }
            finally
            {
                MtxStream = new int[Size, Size];
                for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                        MtxStream[i, j] = 0;

                this.Normalize();
            }
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
            Console.WriteLine();
        }

        public void OutputStream()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                    Console.Write(MtxStream[i, j] + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
        }


        public void ComputeStream(int source, int runoff)
        {
            bool complete;
            do
            {
                List<int> path = new List<int>();
                int stream = Dijkstra(source, runoff,path);
                if (stream != 0)
                {
                    //добавляем найденный доп поток к основному
                    for (int i = path.Count - 1; i > 0; i--)
                    {
                        if (path[i - 1] < path[i])
                            MtxStream[path[i - 1], path[i]] += -stream;
                        else
                            MtxStream[path[i], path[i-1]] += stream;
                    }
                    Normalize();

                    complete = false;
                }
                else
                    complete = true;
            }
            while (!complete);

        }




        //нормализует граф (Mtx[i,j]=Mtx[j,i] and Mtx[i,i]=0)
        public void Normalize()
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    if (i == j) { Mtx[i, j] = 0; }
                    else if (j > i) { Mtx[j, i] = Mtx[i, j]; }

            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    if (i == j) { MtxStream[i, j] = 0; }
                    else if (j > i) { MtxStream[j, i] = MtxStream[i, j]; }
        }






        public int Dijkstra(int from, int to, List<int> path) //поиск мин пути из вершины from в to возврат длины, заполнение массива пути
        {
            
            if (from == to)
            {
                path[0] = to;
                path[1] = to;
                return 0;
            }
            //инициализируем массив вершин
            Top[] tops = new Top[Size];
            for (int i = 0; i < Size; i++)
            {
                tops[i] = new Top();
            }
            tops[from].TopStream = Int32.MaxValue;//начальная вершина
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
                        int stream1 = currenttop < i ? 
                            Min(tops[currenttop].TopStream, Mtx[currenttop, i], Mtx[currenttop, i] - MtxStream[currenttop, i]) :
                            Min(tops[currenttop].TopStream, Mtx[currenttop, i], Mtx[currenttop, i] + MtxStream[currenttop, i]);
                        int stream = currenttop < i ? stream1 : -stream1;

                        //если новый способ дойти до i меньше делаем его
                        if ((tops[i].TopStream <
                            stream1) && (tops[i].TopStream < Mtx[currenttop, i]) && (Math.Abs(stream + MtxStream[currenttop, i]) <= Mtx[currenttop, i]))
                        {
                            tops[i].TopStream = stream1;
                            tops[i].Prev = currenttop;
                        }
                    }
                }

                int Max = -Int32.MaxValue;//будем искать мин путь
                int Mintop = -1;
                for (int i = 0; i < Size; i++)//проходим по всем вершинам

                    if ((tops[i].TopStream > Max) && (tops[i].Const == false))//сравниваем с мин
                    {
                        Max = tops[i].TopStream;
                        Mintop = i;
                    }

                currenttop = Mintop;//минимальная вершина будет постоянной и новой текущей

                tops[currenttop].Const = true;
                amount++;
            }

            if (currenttop != to) return 0;//если счетчик перепрыгнул предел возвращаем null

            //заполнение пути
            path.Add(to);
            while (currenttop != from)//заполняем массив пути
            {
                path.Add(tops[currenttop].Prev);
                currenttop = tops[currenttop].Prev;
            }
            return tops[to].TopStream;
        }


        private int Min(int a, int b, int c)
        {
            if ((a <= b) && (a <= c))
                return a;
            if ((b <= a) && (b <= c))
                return b;
            return c;

        }



    }
}
