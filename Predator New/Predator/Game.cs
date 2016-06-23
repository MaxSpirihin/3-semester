using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Predator
{
    public class Game
    {
        private int INFINITE = 1000000;
        private int exit;
        public int Length { get; private set; }
        //1 параметр - позиция жертвы, 2 хищника
        public int[,] PredatorMoves { get; private set; }
        public int[,] VictimMoves { get; private set; }
        private Field field;

        public Game(int exit)
        {
            this.field = new Field(exit);
           // field.Output();
            this.Length = field.Length;
            this.PredatorMoves = new int[Length, Length];
            this.VictimMoves = new int[Length, Length];
            this.exit = exit;
            

            InitiateStartMoves();

            bool matrixInverted;

            do
            {
                matrixInverted = false;
                
                for (int victimPos = 0; victimPos < Length; victimPos++)
                    for (int predatorPos = 0; predatorPos < Length; predatorPos++)
                        if (PredatorMoves[victimPos, predatorPos] == INFINITE)
                            if (PredatorMakeBestMove(predatorPos, victimPos))
                                matrixInverted = true;

                for (int victimPos = 0; victimPos < Length; victimPos++)
                    for (int predatorPos = 0; predatorPos < Length; predatorPos++)
                        if (VictimMoves[victimPos, predatorPos] == INFINITE)
                            VictimMakeBestMove(predatorPos, victimPos);

               // this.Output();

            } while (matrixInverted);

        }


        public void Output()
        {
            for (int row = 0; row < Length; row++)
            {
                for (int column = 0; column < Length; column++)
                    if (PredatorMoves[row, column] == INFINITE)
                        Console.Write("&" + " ");
                    else
                        Console.Write(Convert.ToString(PredatorMoves[row, column]) + " ");

                Console.Write("     ");


                for (int column = 0; column < Length; column++)
                    if (VictimMoves[row, column] == INFINITE)
                        Console.Write("&" + " ");
                    else
                        Console.Write(Convert.ToString(VictimMoves[row, column]) + " ");

                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();

        }


        public void Start(int victimPos,int predatorPos)
        {


            Console.WriteLine("Игра началась.");
            Console.WriteLine("Стартовые позиции - ["+Convert.ToString(predatorPos) + 
                                " ; " + Convert.ToString(victimPos)+" ]\n");
                

            if (victimPos == exit)
            {
                Console.WriteLine("Жертва в выходе.");
                return;
            }
            if (PredatorMoves[victimPos, predatorPos] != INFINITE)
                EmulatePredatorWin(victimPos, predatorPos);
            else
                EmulateVictimWin(victimPos, predatorPos);
        }


        private void EmulatePredatorWin(int victimPos, int predatorPos)
        {
            //первым ходит хищник
            int moves = PredatorMoves[victimPos, predatorPos];
            bool isPredator = true;
            string finalNote = "Хищник съел жертву. Количество ходов - " + Convert.ToString(PredatorMoves[victimPos, predatorPos] + ".");

            while (moves > 0)
            {
                if (isPredator)
                {
                    //ход хищника
                    for (int nextMove = 0; nextMove < Length; nextMove++)
                        if ((VictimMoves[victimPos, nextMove] == moves - 1) && (field.Mtx[predatorPos, nextMove] == 1))
                        {
                            Console.WriteLine("Хищник ходит " + Convert.ToString(predatorPos) + " -> " + 
                                Convert.ToString(nextMove) + "    [" + Convert.ToString(nextMove) + 
                                " ; " + Convert.ToString(victimPos)+" ]");
                            predatorPos = nextMove;
                            break;
                        }
                }
                else
                {
                    //ход жертвы
                    for (int nextMove = 0; nextMove < Length; nextMove++)
                        if ((PredatorMoves[nextMove, predatorPos] == moves - 1) && (field.Mtx[victimPos, nextMove] == 1))
                        {
                            Console.WriteLine("Жертва ходит " + Convert.ToString(victimPos) + " -> " + Convert.ToString(nextMove) 
                                + "    [" + Convert.ToString(predatorPos) +
                                " ; " + Convert.ToString(nextMove)+" ]");
                            victimPos = nextMove;
                            break;
                        }
                }
                isPredator = !isPredator;
                moves--;
            }

            Console.WriteLine("\n"+finalNote+"\n");
        }

        private void EmulateVictimWin(int victimPos, int predatorPos)
        {
            bool isPredator = true;
            List<int> predatorPositions = new List<int>();
            predatorPositions.Add(predatorPos);
            List<int> victimPositions = new List<int>();
            victimPositions.Add(victimPos);

            bool stop;
            do
            {
                stop = false;
                if (isPredator)
                {
                    //ход хищника
                    for (int nextMove = 0; nextMove < Length; nextMove++)
                        if ((VictimMoves[victimPos, nextMove] == INFINITE) && (field.Mtx[predatorPos, nextMove] == 1))
                        {
                            Console.WriteLine("Хищник ходит " + Convert.ToString(predatorPos) + " -> " +
                                Convert.ToString(nextMove) + "    [" + Convert.ToString(nextMove) +
                                " ; " + Convert.ToString(victimPos) + " ]"); predatorPos = nextMove;
                            break;
                        }
                }
                else
                {
                    //ход жертвы
                    for (int nextMove = 0; nextMove < Length; nextMove++)
                        if ((PredatorMoves[nextMove, predatorPos] == INFINITE) && (field.Mtx[victimPos, nextMove] >= 1))
                        {
                            Console.WriteLine("Жертва ходит " + Convert.ToString(victimPos) + " -> " + Convert.ToString(nextMove)
                                + "    [" + Convert.ToString(predatorPos) +
                                " ; " + Convert.ToString(nextMove) + " ]");
                            if ((nextMove >= Length - 2)||(nextMove == exit))
                            {
                                //жертва свалила в спец вершину
                                Console.WriteLine("\nЖертва добралась до выхода и свалила.\n");
                                return;
                            }
                            victimPos = nextMove;
                            break;
                        }
                }
                isPredator = !isPredator;

                for (int i = 0; i < predatorPositions.Count; i++)
                {
                    if ((predatorPositions[i] == predatorPos) && (victimPositions[i] == victimPos))
                        stop = true;
                }

                if (!stop)
                {
                    predatorPositions.Add(predatorPos);
                    victimPositions.Add(victimPos);
                }
            }
            while (!stop);

            Console.WriteLine("\nИгроки уже были в этих позициях. Хищник никогда не догонит жертву и умрет от голода.\n");


        }


        private void InitiateStartMoves()
        {
            for (int row = 0; row < Length; row++)
                for (int column = 0; column < Length; column++)
                    if (row == column)
                    {
                        PredatorMoves[row, column] = 0;
                        VictimMoves[row, column] = 0;
                    }
                    else
                    {
                        PredatorMoves[row, column] = INFINITE;
                        VictimMoves[row, column] = INFINITE;
                    }
        }


        private bool PredatorMakeBestMove(int predatorPosition, int victimPosition)
        {
            //мин. кол-во ходов в одной из вершин, куда можно сходить
            int minCount = PredatorMoves[victimPosition, predatorPosition];
            //вершина, куда надо сходить
            int minPosition = -1;

            //перебираем все возможные позиции, ищем лучший ход
            for (int nextMove = 0; nextMove < Length; nextMove++)
                if ((field.Mtx[predatorPosition, nextMove] == 1) && (VictimMoves[victimPosition, nextMove] + 1 < minCount))
                {
                    minPosition = nextMove;
                    minCount = VictimMoves[victimPosition, nextMove] + 1;
                }


            //если лучшая позиция не изменилась, ничего не меняем, иначе меняем
            if (minPosition >= 0)
            {
                PredatorMoves[victimPosition, predatorPosition] = minCount;
                return true;
            }
            return false;
        }


        private bool VictimMakeBestMove(int predatorPosition, int victimPosition)
        {
            //макс. кол-во ходов в одной из вершин, куда можно сходить
            int maxCount = -INFINITE;
            //вершина, куда надо сходить
            int maxPosition = -1;

            //перебираем все возможные позиции, ищем лучший ход
            for (int nextMove = 0; nextMove < Length; nextMove++)
                if ( ((field.Mtx[victimPosition, nextMove] == 1) ||((field.Mtx[victimPosition, nextMove] == 2) && (predatorPosition!=exit))) //если идем по фиктивному ребру, то хищника не должно быть в выходе
                    && (PredatorMoves[nextMove, predatorPosition] + 1 > maxCount)) 
                {
                    maxPosition = nextMove;
                    maxCount = PredatorMoves[nextMove, predatorPosition] + 1;
                }

            if (maxPosition >= 0)
            {
                if (maxCount > INFINITE)
                    VictimMoves[victimPosition, predatorPosition] = INFINITE;
                else
                    VictimMoves[victimPosition, predatorPosition] = maxCount;
                return true;
            }
            return false;

        }


    }





}
