using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Factorie
{
    class Graph
    {
        private int[,] Mtx;
        public bool[,] MatchingMtx { get; private set; }
        const int MaxLength = 15;
        public int LengthRow { get; private set; }
        public int LengthColumn { get; private set; }


        public Graph()
        {
            string NameOfFile = "input2.txt";
            try
            {
                StreamReader reader = new StreamReader(NameOfFile);//создаем поток ввода из файла
                //считаем количество строк в файле. Это будет Length
                int CountOfStrings = 0;
                while (!reader.EndOfStream)
                {
                    LengthColumn = reader.ReadLine().Split(' ').Length;
                    CountOfStrings++;
                }
                if (CountOfStrings > MaxLength) return;
                LengthRow = CountOfStrings;
                reader.Close();


                //далее открываем файл заново и считываем саму матрицу
                reader = new StreamReader(NameOfFile);
                Mtx = new int[LengthRow, LengthColumn];
                for (int row = 0; row < LengthRow; row++)
                {
                    string[] Array = reader.ReadLine().Split(' '); //считывается строка из файла и разрезается по пробелам
                    int column = 0;
                    foreach (string s in Array)
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            Mtx[row, column] = Convert.ToInt32(s);
                            column++;
                        }
                    }
                }
            }
            catch (FileNotFoundException) { return; }
            catch (IndexOutOfRangeException) { return; }
        }

        public Graph(int[,] Mtx, int LengthRow, int LengthColumn)
        {
            this.LengthColumn = LengthColumn;
            this.LengthRow = LengthRow;
            this.Mtx = new int[LengthRow, LengthColumn];
            for (int row = 0; row < LengthRow; row++)
                for (int column = 0; column < LengthColumn; column++)
                    this.Mtx[row, column] = Mtx[row, column];
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
            Console.WriteLine();
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
        }



        public void FindSaturatedMatching()
        {
            #region Строим насыщенное паросочетание

            //Создаем массив первых вершин и вторых, а также обнуляем матрицу паросочетаний
            bool[] topsRow = new bool[LengthRow];//этим вершинам соответствуют строки
            for (int row = 0; row < LengthRow; row++)
                topsRow[row] = false;

            bool[] topsColumn = new bool[LengthColumn];//этим вершинам соответствуют столбцы
            for (int column = 0; column < LengthColumn; column++)
                topsColumn[column] = false;

            MatchingMtx = new bool[LengthRow, LengthColumn];




            //Идем по матрице
            for (int row = 0; row < LengthRow; row++)
                for (int column = 0; column < LengthColumn; column++)
                {
                    //если есть ребро и его вершины не в паросочетании
                    if ((Mtx[row, column] == 1) && (!topsRow[row]) && (!topsColumn[column]))
                    {
                        //добавляем реьро к паросочетанию, а вершины помечаем
                        MatchingMtx[row, column] = true;
                        topsRow[row] = true;
                        topsColumn[column] = true;
                    }
                }

            #endregion

            bool complete;

            do
            {

                #region ВОЛНА
                //у нас есть насыщенное паросочетание
                //Берем вершину не в паросочетании
                int startRow = -1;
                for (int row = 0; row < LengthRow; row++)
                    if (!topsRow[row])
                        startRow = row;

                if (startRow == -1)
                    break;

                //создаем 2 массива меток (-1 - непомечена)
                int[] markerRow = new int[LengthRow];
                for (int row = 0; row < LengthRow; row++)
                    markerRow[row] = -1;
                int[] markerColumn = new int[LengthColumn];
                for (int column = 0; column < LengthColumn; column++)
                    markerColumn[column] = -1;

                //присваиваем старту d = 0
                int d = 0;
                markerRow[startRow] = d;

                //показывает не стоит ли прервать цикл
                bool stop;
                bool isRow = true;
                do
                {
                    stop = true;
                    //идем по строкам, где d
                    if (isRow)
                    {
                        for (int row = 0; row < LengthRow; row++)
                            if (markerRow[row] == d)
                                for (int column = 0; column < LengthColumn; column++)
                                    //непомеченные столбцы помечаем d+1
                                    if ((Mtx[row, column] == 1) && (markerColumn[column] == -1) && (!MatchingMtx[row, column]))
                                    {
                                        stop = false;
                                        markerColumn[column] = d + 1;
                                    }
                    }
                    else
                    {
                        //аналогично для столбцов
                        for (int column = 0; column < LengthColumn; column++)
                            if (markerColumn[column] == d)
                                for (int row = 0; row < LengthRow; row++)
                                    if ((Mtx[row, column] == 1) && (markerRow[row] == -1) && (MatchingMtx[row, column]))
                                    {
                                        stop = false;
                                        markerRow[row] = d + 1;
                                    }
                    }
                    isRow = !isRow;
                    d++;
                }
                //пока (есть вершины с меткой d)
                while (!stop);

                /*  Console.WriteLine("ROW:");
                  for (int row = 0; row < LengthRow; row++)
                      Console.Write(markerRow[row] + " ");
                  Console.WriteLine("\nCOL:");
                  for (int column = 0; column < LengthColumn; column++)
                      Console.Write(markerColumn[column] + " ");

                  */

                #endregion


                #region ПЕРЕСТРОЕНИЕ ГРАФА ИЛИ ПРОВЕРКА НА ВЫХОД
                //Цифры присвоены, ищем вершины в столбцах, не входящие в поросочетания (нечетная длина автоматом)
                int needColumn = -1;
                for (int column = 0; column < LengthColumn; column++)
                    if ((!topsColumn[column]) && (markerColumn[column] != -1))
                        needColumn = column;
                //Если нет, то все
                if (needColumn == -1)
                    complete = true;
                else
                {
                    //иначе перестраиваем граф паросочетаний
                    complete = false;
                    //добавляем конечную к паросочетанию
                    topsColumn[needColumn] = true;
                    int currentMarker = markerColumn[needColumn];
                    int currentNumber = needColumn;
                    bool isTheRow = false;

                    //пока не в начальной
                    while (currentMarker != 0)
                    {
                        //ищем в другой доле связанную вершину с маркером на 1 меньше
                        if (isTheRow)
                        {
                            int nextNumber = -1;
                            for (int column = 0; column < LengthColumn; column++)
                                if ((Mtx[currentNumber, column] == 1) && (markerColumn[column] == currentMarker - 1) && (MatchingMtx[currentNumber, column]))
                                    nextNumber = column;
                            //меняем вхождение ребра
                            MatchingMtx[currentNumber, nextNumber] = !MatchingMtx[currentNumber, nextNumber];
                            currentNumber = nextNumber;
                            currentMarker--;
                        }
                        else
                        {
                            int nextNumber = -1;
                            for (int row = 0; row < LengthRow; row++)
                                if ((Mtx[row, currentNumber] == 1) && (markerRow[row] == currentMarker - 1) && (!MatchingMtx[row, currentNumber]))
                                    nextNumber = row;
                            //меняем вхождение ребра
                            MatchingMtx[nextNumber, currentNumber] = !MatchingMtx[nextNumber, currentNumber];
                            currentNumber = nextNumber;
                            currentMarker--;
                        }
                        isTheRow = !isTheRow;
                    }

                    //добавляем первую к паросочетанию
                    topsRow[currentNumber] = true;

                }
                #endregion


            }
            while (!complete);


        }
    }
}
