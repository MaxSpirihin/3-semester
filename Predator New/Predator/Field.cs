using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Predator
{
    public class Field
    {
        public int Length { get; private set; }
        public int[,] Mtx { get; private set; }

        public Field(int exit)
        {
            string NameOfFile = "input.txt";
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
                Length = CountOfStrings+2;
                reader.Close();


                //далее открываем файл заново и считываем саму матрицу
                reader = new StreamReader(NameOfFile);
                Mtx = new int[Length, Length];
                for (int i = 0; i < Length - 2; i++)
                {
                    string[] Array = reader.ReadLine().Split(' ');  //считывается строка из файла и разрезается по пробелам
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

                //добавляем 2 фиктивные вершины (L-1,L-2) - по ним ходит только жертва
                Mtx[exit, Length - 2] = 2;
                Mtx[Length - 2, Length - 1] = 2;

                //соединим ближние к выходы вершины к ближним к выходу фикт. вершинам для жертвы
                for (int i = 0; i < Length - 2; i++)
                    if (Mtx[exit, i] == 1)
                    {
                        Mtx[i, Length - 2] = 2;
                    }

                this.Normalize();
            }
            catch (FileNotFoundException) { return; }
            catch (IndexOutOfRangeException) { return; }
        }

        //нормализует граф (Mtx[i,j]=Mtx[j,i] and Mtx[i,i]=0)
        public void Normalize()
        {
            for (int i = 0; i < Length; i++)
                for (int j = 0; j < Length; j++)
                    if (i == j) { Mtx[i, j] = 0; }
                    else if (j > i) { Mtx[j, i] = Mtx[i, j]; }
        }

        public void Output()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                    Console.Write(Mtx[i, j] + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }



}
