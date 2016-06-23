using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Parket
{
    /// <summary>
    /// Паркет
    ///Комнату размером N*M единиц требуется покрыть 
    ///одинаковыми паркетными плитками размером 2*1 единицу без пропусков 
    ///и наложений. Требуется определить количество всех возможных
    ///способов укладки паркета для конкретных значений N и M.
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Enter M:");
            int M = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter N:");
            int N = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("\nResult: "+GetNumber(M,N).ToString());
            Console.ReadLine();
        }



        /// <summary>
        /// считает нужное кол-во, используя дин. программироание
        /// </summary>
        /// <param name="M"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        static long GetNumber(int M, int N)
        {
            //в таблице все возможные укладки - это двоичное переведенное число
            long[] last = new long[Convert.ToInt32(Math.Pow(2, M))];
            long[] next = new long[Convert.ToInt32(Math.Pow(2, M))];
            for (int i = 0; i < last.Length; i++)
            {
                last[i] = 0;
                next[i] = 0;
            }
            last[0] = 1;
            for (int currentN = 1; currentN <= N; currentN++)
            {
                //пересчет ряда
                for (int numberInNextArray = 0; numberInNextArray < next.Length; numberInNextArray++)
                {
                    next[numberInNextArray] = 0;
                    for (int numberOfLastArray = 0; numberOfLastArray < last.Length; numberOfLastArray++)
                        //если от старого можно перейти к новому, то это можно только 1 способом
                        if (LayingCompatible(numberOfLastArray, numberInNextArray, M))
                            next[numberInNextArray] += last[numberOfLastArray];
                }

                //суем новый в старый
                for (int i = 0; i < last.Length; i++)
                    last[i] = next[i];
            }


            return next[0];
        }


        /// <summary>
        /// проверка связности укладок
        /// </summary>
        /// <param name="last">укладка слева</param>
        /// <param name="next">укладка справа</param>
        /// <param name="WIDTH">ширина</param>
        /// <returns>true если можно перейти</returns>
        static bool LayingCompatible(int last, int next, int WIDTH)
        {
            bool b = false; //показыкает нецелое ли кол-во плиток уложено на данном этапе
            for (int i = 0; i < WIDTH; i++) // клетки, начиная сверху
            {
                //заняты ли данная клетка
                bool lastIs = (last / Convert.ToInt32(Math.Pow(2, i))) % 2 == 1;
                bool nextIs = (next / Convert.ToInt32(Math.Pow(2, i))) % 2 == 1;

                if ((lastIs) && (!nextIs) && (!b))
                {
                    //все уложено, ничего не меняем
                    b = false;
                }
                else if ((lastIs) && (!nextIs) && (b))
                {
                    //все уложено, ничего не меняем, остается висеть полплитки, все плохо
                    return false;
                }
                else if ((!lastIs) && (nextIs) && (!b))
                {
                    //ложим целую плитку
                    b = false;
                }
                else if ((!lastIs) && (nextIs) && (b))
                {
                    //ложим целую плитку, остается висеть полплитки, все плохо
                    return false;
                }
                else if ((!lastIs) && (!nextIs))
                {
                    //ложим слева полплитки
                    b = !b;
                }
                else
                    //если слева и справа есть, то плитку положить не можеи,
                    //а случай с вертикальной плиткой справа учтется потом
                    return false; 

            }

            //кол-во положенных плиток должно быть целое
            return !b;
        }


    }






}

