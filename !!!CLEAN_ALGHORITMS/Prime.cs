using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Models
{

    class Tops//вспомогательный класс для алгоритма Прима
    {
        public int Length { get; private set; }
        private bool[] isInTree;
        public const int InfinityLength = 1000000;

        public Tops(int length)//конструктор. Поначалу ни одна вершина не входит в дерево
        {
            Length = length;
            isInTree = new bool[Length];
            for (int i = 0; i < Length; i++)
                isInTree[i] = false;
        }

        public bool TreeIsBuilded()//возвращает true если все вершины включены в дерево
        {
            bool label = true;
            for (int i = 0; i < Length; i++)
                if (isInTree[i] == false)
                    label = false;
            return label;
        }

        public bool IsInTree(int i)//возвращает входит ли данная вершина в дерево
        {
            return isInTree[i];
        }

        public void AddToTree(int i)//добавляет данную вершину в дерево
        {
            isInTree[i] = true;
        }

    }

    public class Graph
    {
        const int MaxLength = 15;
        public int Length { get; private set; }
        private int[,] Mtx;


        //КОНСТРУКТОР///////////////////////////////////////////////
        public Graph()
        {
            this.Length = 0;
        }

        public Graph(int length)
        {
            if ((length > MaxLength) || (length < 1))
                return;
            this.Length = length;
            Mtx = new int[Length, Length];
            for (int i = 0; i < Length; i++)
                for (int j = 0; j < Length; j++)
                    Mtx[i, j] = 0;
        }
        ///////////////////////////////////////////////////////////




        //ФУНКЦИИ ВВОДА, ВЫВОДА И ПРОВЕРКИ/////////////////////////////////

        //нормализует граф (Mtx[i,j]=Mtx[j,i] and Mtx[i,i]=0)
        public void Normalize()
        {
            for (int i = 0; i < Length; i++)
                for (int j = 0; j < Length; j++)
                    if (i == j) { Mtx[i, j] = 0; }
                    else if (j > i) { Mtx[j, i] = Mtx[i, j]; }
        }

        //показывает связанный ли граф
        public bool IsConnected()
        {
            Normalize();
            int[] marker = new int[Length];
            for (int i = 0; i < Length; i++)
                marker[i] = 0;
            marker[0] = 1;

            while (true)
            {
                bool flag = false;
                for (int CurNum = 0; CurNum < Length; CurNum++)
                {

                    if (marker[CurNum] == 1)
                    {
                        flag = true;
                        marker[CurNum] = 2;
                        for (int i = 0; i < Length; i++)
                            if ((Mtx[CurNum, i] != 0) && (marker[i] == 0))
                                marker[i] = 1;
                        break;
                    }
                }
                if (!flag)
                    break;
            }

            foreach (int i in marker)
                if (i == 0)
                    return false;

            return true;
        }

        //Читает граф из файла
        public void InputFromFile(string NameOfFile)
        {
            try
            {
                StreamReader reader = new StreamReader(NameOfFile);//создаем поток ввода из файла
                //считаем количество строк в файле. Это будет Length
                int CountOfStrings = 0;
                while (!reader.EndOfStream)
                {
                    reader.ReadLine();
                    CountOfStrings++;
                }
                if (CountOfStrings > MaxLength) return;
                Length = CountOfStrings;
                reader.Close();


                //далее открываем файл заново и считываем саму матрицу
                reader = new StreamReader(NameOfFile);
                Mtx = new int[Length, Length];
                for (int i = 0; i < Length; i++)
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

        //вывод с красивой отрисовкой на консоль
        public void NiceOutput()
        {
            Console.Write("   |");
            char a = 'a';
            for (int i = 0; i < Length; i++)
            {
                Console.Write("{0,3}|",a);
                a++;
            }
            a = 'a';
            Console.WriteLine();

            for (int i = 0; i < Length; i++)
            {
                Console.Write("{0,3}|", a);
                a++;
                for (int j = 0; j < Length; j++)
                {
                    if (j > i)
                        if (Mtx[i,j] == 0)
                            Console.Write(" - |");
                        else
                            Console.Write("{0,3:D}|", Mtx[i,j]);
                    else
                    {
                        if (i == j)
                            Console.Write(" - |");
                        else
                            Console.Write ("   |");
                    }
                }
                Console.WriteLine();
            }
        }

        //просто вывод матрицы смежности на консоль
        public void Output()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                    Console.Write(Mtx[i, j] + " ");
                Console.WriteLine();
            }
        }

        //вывод с красивой отрисовкой в некий поток
        public void NiceOutput(StreamWriter writer)
        {
            try
            {
                writer.Write("   |");
                char a = 'a';
                for (int i = 0; i < Length; i++)
                {
                    writer.Write("{0,3}|", a);
                    a++;
                }
                a = 'a';
                writer.WriteLine();

                for (int i = 0; i < Length; i++)
                {
                    writer.Write("{0,3}|", a);
                    a++;
                    for (int j = 0; j < Length; j++)
                    {
                        if (j > i)
                            if (Mtx[i, j] == 0)
                                writer.Write(" - |");
                            else
                                writer.Write("{0,3:D}|", Mtx[i, j]);
                        else
                        {
                            if (i == j)
                                writer.Write(" - |");
                            else
                                writer.Write("   |");
                        }
                    }
                    writer.WriteLine();
                }
            }
            catch (IOException) { return; }
        }

        //просто вывод матрицы смежности в некий поток
        public void Output(StreamWriter writer)
        {
            try
            {
                for (int i = 0; i < Length; i++)
                {
                    for (int j = 0; j < Length; j++)
                        writer.Write(Mtx[i, j] + " ");
                    writer.WriteLine();
                }
            }
            catch (IOException) { return; }
        }

        ////////////////////////////////////////////////////////



        //НАХОЖДЕНИЕ МИНИМАЛЬНОГО ОСТОВОГО ДЕРЕВА ПО АЛГОРИТМУ ПРИМА/////////////////////////////////////
        public Graph MinSpanningTree()
        {
            Graph Tree = new Graph(Length);
            if (!this.IsConnected())
                return Tree;
            Tops tops = new Tops(Length);
            int CurrentTop = 0;                                     //текущая вершина берется нулевая(первая)
            tops.AddToTree(CurrentTop);                             //она добавляется в дерево

            while (!tops.TreeIsBuilded()) 
            {
                int MinEdge = Tops.InfinityLength;              //будем искать минимальное ребро между вершиной из дерева и не из дерева
                int NextTop = CurrentTop;                       //в процессе значение гарантированно сменится
                for (int i=0;i<Length;i++)                     //идем по всем вершинам в дереве
                {
                    if (tops.IsInTree(i))
                    {
                        for (int j=0;j<Length;j++)              
                            //для вершины i из дерева идем п всем другим вершинам
                            //если j не в дереве, соединена с i-й, причем вес ребра меньше минимума
                            if ((Mtx[i,j]!=0)&&(Mtx[i,j]<MinEdge)&&(!tops.IsInTree(j)))  
                            {
                                MinEdge = Mtx[i, j];           //имеем новый минимум
                                NextTop = j;                  //имеем новую следующкю вершину
                                CurrentTop = i;               //имеем новую вершину в дереве, которая соединится с NextTop
                            }
                    }
                }

                tops.AddToTree(NextTop);                                                                
                Tree.Mtx[CurrentTop, NextTop] = Mtx[CurrentTop, NextTop];
                Tree.Mtx[NextTop, CurrentTop] = Mtx[NextTop, CurrentTop];
                CurrentTop = NextTop;
            }
            return Tree;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
    }



}
