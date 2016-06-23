using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CitiesAndWays
{
    /// <summary>
    /// ребро в графе
    /// </summary>
    public class Edge
    {
        public int first { get; private set; }
        public int second { get; private set; }
        public bool isFict { get; private set; }
        /// <summary>
        /// какая компания приватизировала дорогу, 0 - никто
        /// </summary>
        public int Owner { get; set; }

        /// <summary>
        /// ребро графа
        /// </summary>
        /// <param name="first">первая вершина</param>
        /// <param name="second">вторая вершина</param>
        public Edge(int first, int second, bool fict)
        {
            this.first = first;
            this.second = second;
            this.isFict = fict;
            Owner = 0;
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
        public const int MAX_COUNT_OF_TOPS = 300;

        /// <summary>
        /// список ребер графа
        /// </summary>
        public List<Edge> Edges { get; set; }

        /// <summary>
        /// кол-во вершин графа
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// выполнялась ли приватизация графа уже
        /// </summary>
        public bool isPrivatizate { get; private set; }

        /// <summary>
        /// граф выстраивается сам, ПЕРЕПИСАТЬ ДЛЯ СЧЕТА ИЗ ФАЙЛА
        /// </summary>
        /// <param name="countOfTops">кол-во вершин</param>
        public Graph(int countOfTops)
        {
            if ((countOfTops % 2 != 0) || (countOfTops < COUNT_OF_EDGE + 1) || (countOfTops > MAX_COUNT_OF_TOPS))
                throw new Exception();
            this.Size = countOfTops;
            this.isPrivatizate = false;
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
                        Edges.Add(new Edge(row, column, false));

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


        public void Privatizate()
        {
            AddFictEdges();
            List<int> eiler = BuildEiler(0);
            GiveEdgesToCompanies(eiler);
            DeleteFictEdges();
            isPrivatizate = true;
        }


        private void GiveEdgesToCompanies(List<int> eiler)
        {
            int currentCompany = 1;
            
            //идем по эйлеровому циклу и отдаем компаниям по очереди пути
            for (int i = 0; i < eiler.Count - 1; i++)
            {
                foreach (Edge edge in Edges)
                {
                    if (((edge.first == eiler[i]) && (edge.second == eiler[i + 1]) ||
                        (edge.first == eiler[i + 1]) && (edge.second == eiler[i]))
                        && (edge.Owner == 0))
                    {
                        edge.Owner = currentCompany;
                        currentCompany = (currentCompany == 1) ? 2 : 1;
                        break;
                    }
                }
            }
        }

        private void AddFictEdges()
        {
            //соединим 0 и 1, 2 и 3 и т.д. Так у каждой будет по 4 ребра
            for (int top = 0; top < Size - 1; top+=2)
                Edges.Add(new Edge(top, top + 1, true));
        }

        private void DeleteFictEdges()
        {
            int curEdge = 0;
            while (curEdge < Edges.Count)
            {
                if (Edges[curEdge].isFict)
                    Edges.RemoveAt(curEdge);
                else
                    curEdge++;
            }
        }

        private List<int> Eiler(int startTop, List<int> mainCycle, List<Edge> edges)
        {
            //строим любой цикл
            List<int> newCycle;
            if (mainCycle!=null)
                newCycle = BuildCycle(mainCycle[startTop], edges);
            else
                newCycle = BuildCycle(0, edges);

            //добавляем этот цикл к основному
            if (mainCycle == null)
                mainCycle = newCycle;
            else
            {
                newCycle.RemoveAt(0);
                mainCycle.InsertRange(startTop+1, newCycle);
            }

            if (edges.Count != 0)
            {
                //еще не эйлеров, где-то затесалась вершина, инцидентная некому ребру, построим для нее цикл
                return Eiler(mainCycle.IndexOf(edges[0].first), mainCycle, edges);
            }
            return mainCycle;
        }

        public List<int> BuildEiler(int StartTop)
        {
            return Eiler(StartTop, null, Edges.ToList());
        }

        private List<int> BuildCycle(int startTop, List<Edge> edges)
        {
            List<int> cycle = new List<int>();
            cycle.Add(startTop);
            do
            {
                //ищем ребро инцидентное currentTop
                for (int edgeNumber = 0; edgeNumber < edges.Count; edgeNumber++)
                {
                    if ((edges[edgeNumber].first == cycle.Last()) || (edges[edgeNumber].second == cycle.Last()))
                    {
                        //добавляем вершину в список, удаляем ребро
                        if (edges[edgeNumber].first == cycle.Last())
                            cycle.Add(edges[edgeNumber].second);
                        else
                            cycle.Add(edges[edgeNumber].first);

                        edges.RemoveAt(edgeNumber);
                        break;
                    }
                }

            }//повторяем, пока не замкнем цикл
            while (cycle.Last() != startTop);

            return cycle;
        }

    }
}
