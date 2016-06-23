using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskAboutPrivatization
{

    /// <summary>
    /// ребро в графе
    /// </summary>
    public class Edge
    {
        public int first { get; private set; }
        public int second { get; private set; }

        /// <summary>
        /// ребро графа
        /// </summary>
        /// <param name="first">первая вершина</param>
        /// <param name="second">вторая вершина</param>
        public Edge(int first, int second)
        {
            this.first = first;
            this.second = second;
        }


        public void Print()
        {
            Console.WriteLine(first + "-" + second);
        }
    }


    /// <summary>
    /// Наш граф, вершины - города, ребра - дороги между ними
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// кол-во ребер из каждой вершины
        /// </summary>
        public const int COUNT_OF_EDGE = 3;

        /// <summary>
        /// список ребер графа
        /// </summary>
        public List<Edge> Edges { get; private set; }

        /// <summary>
        /// кол-во вершин графа
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// граф выстраивается сам, по правилу, от нас требуется лишь кол-во вершин
        /// </summary>
        /// <param name="countOfTops">кол-во вершин</param>
        public Graph(int countOfTops)
        {
            if (countOfTops % 2 != 0)
                throw new Exception();
            this.Size = countOfTops;
            Edges = new List<Edge>();

            //сначала заполним матрицу смежности, так удобнее
            int[,] mtx = new int[Size, Size];
            for (int row = 0; row < Size; row++)
                for (int column = 0; column < Size; column++)
                    mtx[row, column] = 0;

            for (int currentTop = 0; currentTop < Size; currentTop++)
            {
                int countOfAdjacentTop = CountOfAdjacentTop(currentTop, mtx);

                while (countOfAdjacentTop < COUNT_OF_EDGE)
                {
                    //нужно добавить еще смежных вершин, чтобы было COUNT_OF_EDGE
                    //соединять будем с первой вершиной, у которой кол-во смежных вершин мало
                    int minTop = -1;
                    int min = Int32.MaxValue;
                    for (int top = 0; top < Size; top++)
                        if ((top != currentTop) && (CountOfAdjacentTop(top, mtx) < min))
                        {
                            minTop = top;
                            min = CountOfAdjacentTop(top, mtx);
                        }

                    //соединим 
                    mtx[currentTop, minTop] = 1;
                    mtx[minTop, currentTop] = 1;
                    countOfAdjacentTop = CountOfAdjacentTop(currentTop, mtx);
                }

            }

            //у нас есть матрица смежности, переведем в матрицу инциндентности
            for (int row = 0; row < Size; row++)
                for (int column = row; column < Size; column++)
                    if (mtx[row, column] == 1)
                        Edges.Add(new Edge(row, column));

        }


        private int CountOfAdjacentTop(int currentTop, int[,] mtx)
        {
            //посчитаем кол-во смежных этой вершине ребер
            int countOfAdjacentTop = 0;
            for (int adjacentTop = 0; adjacentTop < Size; adjacentTop++)
                if ((mtx[currentTop, adjacentTop] == 1) && (adjacentTop != currentTop))
                    countOfAdjacentTop++;
            return countOfAdjacentTop;
        }


        public void PrintToConsole()
        {
            foreach (Edge e in Edges)
                e.Print();
        }


    }
}
