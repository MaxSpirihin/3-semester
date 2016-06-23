using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;

namespace Reversi
{
    //доска игровая
    class Game       
    {
        public const int SIZE = 8; //ширина доски 
        public const int RectWidth = 40;
        public const int PLAYER = 1;
        public const int COMPUTER = -1;
        private const int MAX_DEEP = 3;

        private int[,] field;
        private int playerScore = 0;
        private int computerScore = 0;
  

        public Game()
        {
            InitMat();
        }


        public int ComputerScore
        {
            get { return computerScore; }
        }



        public int PlayerScore
        {
            get { return playerScore; }
        }




        //создает копию доски
        public Game Copy()
        {
            //копируем матрицу
            Game extraBoard = new Game();
            for (int column = 0; column < SIZE; column++)
            {
                for (int row = 0; row < SIZE; row++)
                {
                    extraBoard.field[column, row] = field[column, row];
                }
            }

            //копируем очки
            extraBoard.computerScore = this.ComputerScore;
            extraBoard.playerScore = this.PlayerScore;
            return extraBoard;
        }




        //вычисляет очки игрока и ИИ
        private void ComputeScores()
        {
            playerScore = 0;
            computerScore = 0;
            for (int column = 0; column < SIZE; column++)
            {
                for (int row = 0; row < SIZE; row++)
                {
                    if (field[column, row] < 0) computerScore++;
                    else if (field[column, row] > 0) playerScore++;
                }
            }
        }

        public bool IsEnd
        {
            get { return (FigCount == SIZE * SIZE); }
        }

        public int FigCount
        {
            get { return ComputerScore+PlayerScore; }
        }




        //инициализирует начало игры
        private void InitMat()
        {
            computerScore = 0;
            playerScore = 0;

            //инициализируем поле нулями
            field = new int[SIZE, SIZE];
            for (int column = 0; column < SIZE; column++)
            {
                for (int row = 0; row < SIZE; row++)
                {
                    field[column, row] = 0;
                }
            }

            //добавляем начальные позиции
            field[SIZE / 2 - 1, SIZE / 2 - 1] = COMPUTER;
            field[SIZE / 2, SIZE / 2 - 1] = PLAYER;

            field[SIZE / 2 - 1, SIZE / 2] = PLAYER;
            field[SIZE / 2, SIZE / 2] = COMPUTER;

            ComputeScores();
        }


        public int GetCell(int row, int column)
        {
            return field[column, row];
        }


        //вычисляет оценку стола для комрьютера
        public int ComputeRating()
        {
            //конец игры, комп либо выиграл, либо проиграл
            if (ComputerScore + PlayerScore == SIZE * SIZE)
                return (ComputerScore > PlayerScore) ? Int32.MaxValue : -Int32.MaxValue;

            int endLineScores = 0;
            int angleScores = 0;
            int scoresDiff = ComputerScore - PlayerScore;
            int constFiguresScores = 0;

            //очки за углы
            angleScores = field[0, 0] + field[0, SIZE - 1] + field[SIZE - 1, SIZE - 1] + field[SIZE - 1, 0];

            //очки за нахождение в крайних линиях
            for (int i = 0; i < SIZE; i++)
                endLineScores += (field[i, 0] + field[i, SIZE - 1] + field[0, i] + field[SIZE - 1, i]);


            //считаем очки за ряды одинаковых фигур около углов
            int first = field[0, 0];
            if (first != 0)
            {
                for (int i = 1; i < SIZE && field[0, i] == first; i++)
                    constFiguresScores += first;
                for (int i = 1; i < SIZE && field[i, 0] == first; i++)
                    constFiguresScores += first;
            }


            first = field[0, SIZE - 1];
            if (first != 0)
            {
                for (int i = SIZE - 2; i >= 0 && field[0, i] == first; i--)
                    constFiguresScores += first;
                for (int i = 1; i < SIZE && field[i, SIZE - 1] == first; i++)
                    constFiguresScores += first;
            }

            first = field[SIZE - 1, 0];
            if (first != 0)
            {
                for (int i = 1; i < SIZE && field[SIZE - 1, i] == first; i++)
                    constFiguresScores += first;
                for (int i = SIZE - 2; i >= 0 && field[i, SIZE - 1] == first; i--)
                    constFiguresScores += first;
            }

            first = field[SIZE - 1, SIZE - 1];
            if (first != 0)
            {
                for (int i = SIZE - 2; i >= 0 && field[i, SIZE - 1] == first; i--)
                    constFiguresScores += first;
                for (int i = SIZE - 2; i >= 0 && field[SIZE - 1, i] == first; i--)
                    constFiguresScores += first;
            }

            //константы подбирались экспериментально
            return
                ((100 + ComputerScore + PlayerScore) * scoresDiff) +
                (-200 * angleScores) +
                (-150 * endLineScores) +
                (-250 * constFiguresScores);
        }

        public static int GetBestMove(int player, int alpha, int deep, Game game)
        {

            if (deep > MAX_DEEP || game.ComputerScore + game.PlayerScore == Game.SIZE * Game.SIZE)
            {
                //игра или глубина окончена, считаем оценку
                return game.ComputeRating();
            }
            List<int[]> possibleMoves = game.GetPossibleMoves(player);

            //если у компьютера нет ходов, то все плохо 
            if (possibleMoves.Count == 0 && player == COMPUTER)
                return -Int32.MaxValue;

            int maxRating = -Int32.MaxValue;
            foreach (int[] move in possibleMoves)
            {
                //выполняем отсечение
                if (AlphaBeta(alpha, maxRating, deep))
                {
                    return alpha;
                }
                //создаем копию игры и делаем ход
                Game extraGame = game.Copy();
                extraGame.MakeMove(move[1], move[0], player, true);

                //вычисляем оценку после этого хода, меняя игрока
                int result = GetBestMove(-player, alpha, deep + 1, extraGame);

                //вычисляем новую оценку
                maxRating = MinMax(result, maxRating, deep);
            }
            return maxRating;
        }


        public void DoBestMove()
        {
            List<int[]> possibleMoves = GetPossibleMoves(COMPUTER); 
            int bestMove = 0;
            int maxRating = -Int32.MaxValue;
            
            //для каждого возможного хода
            for (int currentMove = 0; currentMove < possibleMoves.Count; currentMove++)
            {
                //создаем копию игры и делаем в ней текущий ход
                Game extraGame = Copy();
                extraGame.MakeMove(possibleMoves[currentMove][1], possibleMoves[currentMove][0], COMPUTER, true);
                
                //вычисляем оценку после этого хода, делая ход игроком
                int result = Game.GetBestMove(PLAYER, -Int32.MaxValue, 0, extraGame);
                if (maxRating < result)
                {
                    //текущий ход - лучший
                    bestMove = currentMove;
                    maxRating = result;
                }
            }

            //если компьютеру есть куда сходить, он ходит

            if (possibleMoves.Count > bestMove)
            {
                MakeMove(possibleMoves[bestMove][1], possibleMoves[bestMove][0], -1, true);
              //  MessageBox.Show(Convert.ToString(possibleMoves[bestMove][0] + 1) + "," + Convert.ToString(possibleMoves[bestMove][1] + 1));
            }
        }



        //проверяет, следует ли выполнять отсечение
        private static bool AlphaBeta(int alpha, int maxRating, int deep)
        {
            return ((alpha != -Int32.MaxValue && maxRating != -Int32.MaxValue) &&
                    ((alpha >= maxRating && deep % 2 == 0) || (alpha <= maxRating && deep % 2 == 1)));
        }


        //вычисляет оптимальную оценку, где
        //result - оценка текущего хода
        //maxRating - лучшая на данный момент оценка
        //deep - глубина рекурсии
        private static int MinMax(int result, int maxRating, int deep)
        {
            
            if (maxRating == -Int32.MaxValue)
                return result;

            //если это ход компа возвращаем лучший вариант, если игрока - худший 
            return (deep % 2 == 1) ? Math.Max(result, maxRating) : Math.Min(result, maxRating);
        }


        //сделать ход, возвращает кол-во замененных фигур, 
        //переменная changeField показывает нужно ли менять поле, если она false, 
        //то метод запускается для проверки хода
        internal int MakeMove(int column, int row, int player, bool changeField)
        {
            //для подсчета
            int res = 0;

            if (field[column, row] == 0)
            {
                //проверяем отдельно каждое направление
                res = UpCheck(column, row - 1, player, changeField) +
                    DownCheck(column, row + 1, player, changeField) +
                    LeftCheck(column - 1, row, player, changeField) +
                    RightCheck(column + 1, row, player, changeField) +
                    UpLeftCheck(column - 1, row - 1, player, changeField) +
                    UpRightCheck(column + 1, row - 1, player, changeField) +
                    DownRightCheck(column + 1, row + 1, player, changeField) +
                    DownLeftCheck(column - 1, row + 1, player, changeField);
                if (res > 0)
                {
                    if (changeField)
                    {
                        //ставим фигуру, если нужно
                        field[column, row] = player;
                        ComputeScores();
                    }
                }
            }
            return res;
        }






        //возвращает список возможных ходов
        public List<int[]> GetPossibleMoves(int player)
        {
            List<int[]> possibleMoves = new List<int[]>(10);

            //проходимся по клеткам
            for (int row = 0; row < SIZE; row++)
            {
                for (int column = 0; column < SIZE; column++)
                {
                    //пытаемся сделать ход, сигнализируя, что менять поле не надо
                    int res = MakeMove(column, row, player, false);
                    if (res > 0)
                    {
                        //если ход проходит, добавляем его в список
                        possibleMoves.Add(new int[] { row, column, res });
                    }
                }
            }
            return possibleMoves;
        }


        #region Проверки хода по горизонтали и вертикали
        private int UpCheck(int moveColumn, int moveRow, int player, bool changeField)
        {
            bool done = false;//был ли переворот
            int count = 0;//кол-во врагов для переворота

            //идем вверх, меняя строки
            for (int row = moveRow; row >= 0; row--)
            {
                //если наткнулись на пустую клетку, дальше идти нет смысла
                if (field[moveColumn, row] == 0)
                    return 0;

                //враг
                if (field[moveColumn, row] == -player)
                    count++;

                //нашли своего
                if (field[moveColumn, row] == player)
                {
                    //узнаем есть ли между клеткой, куда ходим и найденной враги
                    done = count > 0;
                    if (count > 0 && changeField)
                        //враги там есть, можно выполнять переворот
                        for (int i = moveRow; i != row; i--)
                            field[moveColumn, i] = player;
                    //прекращаем for т.к. ход в этом направлении уже сделан
                    break;
                }
            }
            //здесь мы окажемся, если дошли до конца
            return done ? count : 0;
        }


        //комментарии опущены, все аналогично пред. методу
        private int DownCheck(int moveColumn, int moveRow, int player, bool changeField)
        {
            bool done = false;
            int count = 0;

            for (int row = moveRow; row < SIZE; row++)
            {
                if (field[moveColumn, row] == 0)
                    return 0;

                if (field[moveColumn, row] == -player)
                    count++;

                if (field[moveColumn, row] == player)
                {
                    done = count > 0;
                    if (count > 0 && changeField)
                        for (int i = moveRow; i != row; i++)
                            field[moveColumn, i] = player;
                    break;
                }
            }
            return done ? count : 0;
        }

        private int LeftCheck(int moveColumn, int moveRow, int player, bool changeField)
        {
            bool done = false;
            int count = 0;

            for (int column = moveColumn; column >= 0; column--)
            {
                if (field[column, moveRow] == 0)
                    return 0;

                if (field[column, moveRow] == -player)
                    count++;

                if (field[column, moveRow] == player)
                {
                    done = count > 0;
                    if (count > 0 && changeField)
                        for (int i = moveColumn; i != column; i--)
                            field[i, moveRow] = player;
                    break;
                }
            }
            return done ? count : 0;
        }

        private int RightCheck(int moveColumn, int moveRow, int player, bool changeField)
        {
            bool done = false;
            int count = 0;

            for (int column = moveColumn; column < SIZE; column++)
            {
                if (field[column, moveRow] == 0)
                    return 0;

                if (field[column, moveRow] == -player)
                    count++;

                if (field[column, moveRow] == player)
                {
                    done = count > 0;
                    if (count > 0 && changeField)
                        for (int i = moveColumn; i != column; i++)
                            field[i, moveRow] = player;
                    break;
                }
            }
            return done ? count : 0;
        }
        #endregion

        #region Проверки хода по диагонали
        private int UpLeftCheck(int moveColumn, int moveRow, int player, bool changeField)
        {
            bool done = false;

            int count = 0;
            for (int column = moveColumn, row = moveRow; column >= 0 && row >= 0; column--, row--)
            {
                if (field[column, row] == 0) return 0;
                if (field[column, row] == -player) count++;
                if (field[column, row] == player)
                {
                    done = count > 0;
                    if (count > 0 && changeField)
                        for (int i = moveColumn, j = moveRow; i != column && j != row; i--, j--)
                            field[i, j] = player;
                    break;
                }
            }
            return done ? count : 0;
        }

        private int UpRightCheck(int moveColumn, int moveRow, int player, bool changeField)
        {
            bool done = false;
            int count = 0;

            for (int column = moveColumn, row = moveRow; column < SIZE && row >= 0; column++, row--)
            {
                if (field[column, row] == 0)
                    return 0;

                if (field[column, row] == -player)
                    count++;

                if (field[column, row] == player)
                {
                    done = count > 0;
                    if (count > 0 && changeField)
                        for (int i = moveColumn, j = moveRow; i != column && j != row; i++, j--)
                            field[i, j] = player;
                    break;
                }
            }
            return done ? count : 0;
        }



        private int DownRightCheck(int moveColumn, int moveRow, int player, bool changeField)
        {
            bool done = false;
            int count = 0;

            for (int column = moveColumn, row = moveRow; column < SIZE && row < SIZE; column++, row++)
            {
                if (field[column, row] == 0)
                    return 0;

                if (field[column, row] == -player)
                    count++;

                if (field[column, row] == player)
                {
                    done = count > 0;
                    if (count > 0 && changeField)
                        for (int i = moveColumn, j = moveRow; i != column && j != row; i++, j++)
                            field[i, j] = player;
                    break;
                }
            }
            return done ? count : 0;
        }



        private int DownLeftCheck(int moveColumn, int moveRow, int player, bool changeField)
        {
            bool done = false;
            int count = 0;

            for (int column = moveColumn, row = moveRow; column >= 0 && row < SIZE; column--, row++)
            {
                if (field[column, row] == 0)
                    return 0;

                if (field[column, row] == -player)
                    count++;

                if (field[column, row] == player)
                {
                    done = count > 0;
                    if (count > 0 && changeField)
                        for (int i = moveColumn, j = moveRow; i != column && j != row; i--, j++)
                            field[i, j] = player;
                    break;
                }
            }
            return done ? count : 0;
        }
        #endregion
    }
}