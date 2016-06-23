using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;

namespace Reversi
{
    //����� �������
    class Game       
    {
        public const int SIZE = 8; //������ ����� 
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




        //������� ����� �����
        public Game Copy()
        {
            //�������� �������
            Game extraBoard = new Game();
            for (int column = 0; column < SIZE; column++)
            {
                for (int row = 0; row < SIZE; row++)
                {
                    extraBoard.field[column, row] = field[column, row];
                }
            }

            //�������� ����
            extraBoard.computerScore = this.ComputerScore;
            extraBoard.playerScore = this.PlayerScore;
            return extraBoard;
        }




        //��������� ���� ������ � ��
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




        //�������������� ������ ����
        private void InitMat()
        {
            computerScore = 0;
            playerScore = 0;

            //�������������� ���� ������
            field = new int[SIZE, SIZE];
            for (int column = 0; column < SIZE; column++)
            {
                for (int row = 0; row < SIZE; row++)
                {
                    field[column, row] = 0;
                }
            }

            //��������� ��������� �������
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


        //��������� ������ ����� ��� ����������
        public int ComputeRating()
        {
            //����� ����, ���� ���� �������, ���� ��������
            if (ComputerScore + PlayerScore == SIZE * SIZE)
                return (ComputerScore > PlayerScore) ? Int32.MaxValue : -Int32.MaxValue;

            int endLineScores = 0;
            int angleScores = 0;
            int scoresDiff = ComputerScore - PlayerScore;
            int constFiguresScores = 0;

            //���� �� ����
            angleScores = field[0, 0] + field[0, SIZE - 1] + field[SIZE - 1, SIZE - 1] + field[SIZE - 1, 0];

            //���� �� ���������� � ������� ������
            for (int i = 0; i < SIZE; i++)
                endLineScores += (field[i, 0] + field[i, SIZE - 1] + field[0, i] + field[SIZE - 1, i]);


            //������� ���� �� ���� ���������� ����� ����� �����
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

            //��������� ����������� ����������������
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
                //���� ��� ������� ��������, ������� ������
                return game.ComputeRating();
            }
            List<int[]> possibleMoves = game.GetPossibleMoves(player);

            //���� � ���������� ��� �����, �� ��� ����� 
            if (possibleMoves.Count == 0 && player == COMPUTER)
                return -Int32.MaxValue;

            int maxRating = -Int32.MaxValue;
            foreach (int[] move in possibleMoves)
            {
                //��������� ���������
                if (AlphaBeta(alpha, maxRating, deep))
                {
                    return alpha;
                }
                //������� ����� ���� � ������ ���
                Game extraGame = game.Copy();
                extraGame.MakeMove(move[1], move[0], player, true);

                //��������� ������ ����� ����� ����, ����� ������
                int result = GetBestMove(-player, alpha, deep + 1, extraGame);

                //��������� ����� ������
                maxRating = MinMax(result, maxRating, deep);
            }
            return maxRating;
        }


        public void DoBestMove()
        {
            List<int[]> possibleMoves = GetPossibleMoves(COMPUTER); 
            int bestMove = 0;
            int maxRating = -Int32.MaxValue;
            
            //��� ������� ���������� ����
            for (int currentMove = 0; currentMove < possibleMoves.Count; currentMove++)
            {
                //������� ����� ���� � ������ � ��� ������� ���
                Game extraGame = Copy();
                extraGame.MakeMove(possibleMoves[currentMove][1], possibleMoves[currentMove][0], COMPUTER, true);
                
                //��������� ������ ����� ����� ����, ����� ��� �������
                int result = Game.GetBestMove(PLAYER, -Int32.MaxValue, 0, extraGame);
                if (maxRating < result)
                {
                    //������� ��� - ������
                    bestMove = currentMove;
                    maxRating = result;
                }
            }

            //���� ���������� ���� ���� �������, �� �����

            if (possibleMoves.Count > bestMove)
            {
                MakeMove(possibleMoves[bestMove][1], possibleMoves[bestMove][0], -1, true);
              //  MessageBox.Show(Convert.ToString(possibleMoves[bestMove][0] + 1) + "," + Convert.ToString(possibleMoves[bestMove][1] + 1));
            }
        }



        //���������, ������� �� ��������� ���������
        private static bool AlphaBeta(int alpha, int maxRating, int deep)
        {
            return ((alpha != -Int32.MaxValue && maxRating != -Int32.MaxValue) &&
                    ((alpha >= maxRating && deep % 2 == 0) || (alpha <= maxRating && deep % 2 == 1)));
        }


        //��������� ����������� ������, ���
        //result - ������ �������� ����
        //maxRating - ������ �� ������ ������ ������
        //deep - ������� ��������
        private static int MinMax(int result, int maxRating, int deep)
        {
            
            if (maxRating == -Int32.MaxValue)
                return result;

            //���� ��� ��� ����� ���������� ������ �������, ���� ������ - ������ 
            return (deep % 2 == 1) ? Math.Max(result, maxRating) : Math.Min(result, maxRating);
        }


        //������� ���, ���������� ���-�� ���������� �����, 
        //���������� changeField ���������� ����� �� ������ ����, ���� ��� false, 
        //�� ����� ����������� ��� �������� ����
        internal int MakeMove(int column, int row, int player, bool changeField)
        {
            //��� ��������
            int res = 0;

            if (field[column, row] == 0)
            {
                //��������� �������� ������ �����������
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
                        //������ ������, ���� �����
                        field[column, row] = player;
                        ComputeScores();
                    }
                }
            }
            return res;
        }






        //���������� ������ ��������� �����
        public List<int[]> GetPossibleMoves(int player)
        {
            List<int[]> possibleMoves = new List<int[]>(10);

            //���������� �� �������
            for (int row = 0; row < SIZE; row++)
            {
                for (int column = 0; column < SIZE; column++)
                {
                    //�������� ������� ���, ������������, ��� ������ ���� �� ����
                    int res = MakeMove(column, row, player, false);
                    if (res > 0)
                    {
                        //���� ��� ��������, ��������� ��� � ������
                        possibleMoves.Add(new int[] { row, column, res });
                    }
                }
            }
            return possibleMoves;
        }


        #region �������� ���� �� ����������� � ���������
        private int UpCheck(int moveColumn, int moveRow, int player, bool changeField)
        {
            bool done = false;//��� �� ���������
            int count = 0;//���-�� ������ ��� ����������

            //���� �����, ����� ������
            for (int row = moveRow; row >= 0; row--)
            {
                //���� ���������� �� ������ ������, ������ ���� ��� ������
                if (field[moveColumn, row] == 0)
                    return 0;

                //����
                if (field[moveColumn, row] == -player)
                    count++;

                //����� ������
                if (field[moveColumn, row] == player)
                {
                    //������ ���� �� ����� �������, ���� ����� � ��������� �����
                    done = count > 0;
                    if (count > 0 && changeField)
                        //����� ��� ����, ����� ��������� ���������
                        for (int i = moveRow; i != row; i--)
                            field[moveColumn, i] = player;
                    //���������� for �.�. ��� � ���� ����������� ��� ������
                    break;
                }
            }
            //����� �� ��������, ���� ����� �� �����
            return done ? count : 0;
        }


        //����������� �������, ��� ���������� ����. ������
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

        #region �������� ���� �� ���������
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