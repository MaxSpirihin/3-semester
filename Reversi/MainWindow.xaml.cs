using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;
using System.Reflection;

namespace Reversi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game game;
        bool IsSingleGame;
        int currentPlayer;
        Brush[] colors;
        
        public MainWindow()
        {
            this.InitializeComponent();
            IsSingleGame = true;
            colors = new Brush[4];
            colors[0] = Brushes.Gray;
            colors[1] = Brushes.RosyBrown;
            colors[2] = Brushes.White;
            colors[3] = Brushes.Black;
            CreateTable();
            StartGame();
        }



        private void CreateTable()
        {
            for (int row = 0; row < Game.SIZE; row++)
            {
                for (int column = 0; column < Game.SIZE; column++)
                {
                    Button btn = new Button();
                    DockPanel localDock = new DockPanel();
                    if ((row + column) % 2 == 0)
                        btn.Background = colors[0];
                    else
                        btn.Background = colors[1];


                    btn.Content = localDock;
                    btn.SetValue(Grid.ColumnProperty, column);
                    btn.SetValue(Grid.RowProperty, row);
                    btn.Name = "b" + (Convert.ToString(row) + Convert.ToString(column));
                    btn.Click += new RoutedEventHandler(Move);
                    gameField.Children.Add(btn);
                }
            }
        }


        public void RepaintField()
        {
            int i = 0;
            foreach (Button btn in gameField.Children)
            {

                DockPanel localDock = new DockPanel();
                if ((i%Game.SIZE  + i/Game.SIZE) % 2 == 0)
                    btn.Background = colors[0];
                else
                    btn.Background = colors[1];

                btn.Content = localDock;
                i++;
            }
        }


        private void Move(object sender, System.Windows.RoutedEventArgs e)
        {
            //достаем из кнопки номер строки и столбца
            Button btn = (Button)sender;
            string name = btn.Name;
            int row = Convert.ToInt32(name.Substring(1, 1));
            int column = Convert.ToInt32(name.Substring(2, 1));



            if (IsSingleGame)
            {
                //пытаемся сделать ход
                if (game.MakeMove(column, row, Game.PLAYER, true) > 0)
                {
                    //ход сделан,да
                    game.DoBestMove();
                    PaintGameField();


                    //если игроку некуда ходить
                    while (game.GetPossibleMoves(Game.PLAYER).Count == 0)
                    {
                        if ((game.GetPossibleMoves(Game.PLAYER).Count > 0))
                        {
                            //если компьютеру есть куда сходить
                            MessageBox.Show("У вас нет ходов");
                            game.DoBestMove();
                            PaintGameField();
                        }

                        else
                        {
                            //никто не может ходить, конец
                            EndGame();

                            break;
                        }//end else
                    }//end while
                }//end if make move
            }//end if single game
            else
            {
                //пытаемся сделать ход
                if (game.MakeMove(column, row, currentPlayer, true) > 0)
                {
                    //ход сделан, проверим может ли другой ходить

                    if (game.GetPossibleMoves(-currentPlayer).Count != 0)
                    {
                        //другой может ходить, ну ОК, пускай ходит, чё?
                        currentPlayer = -currentPlayer;
                        PaintGameField();
                    }
                    else
                    {
                        //хм, он не может ходить, а еще разок сходишь?
                        if (game.GetPossibleMoves(currentPlayer).Count != 0)
                        {
                            //ходи
                            MessageBox.Show("Игрок " + Convert.ToString(currentPlayer) + " не может ходить");
                            PaintGameField();
                        }
                        else
                        {
                            //никто не может ходить, конец
                            PaintGameField();
                            EndGame();
                        }
                    }
                }
            }



        }//end method



        private void StartGame()
        {
            game = new Game();
            if (IsSingleGame)
            {
                TittleText.Text = "Single Game";
                FirstPlayer.Text = "Computer";
                SecondPlayer.Text = "Player";
            }
            else
            {
                TittleText.Text = "Multiplayer";
                FirstPlayer.Text = "Player 1";
                SecondPlayer.Text = "Player 2";
            }
            PaintGameField();
        }




        private void colouredEllipsePainter(Button gotBtn, Brush color, bool realColor)
        {
            Ellipse buttonEllipse = new Ellipse();
            buttonEllipse.Height = 40;
            buttonEllipse.Width = 40;
            buttonEllipse.Fill = color;
            if (!realColor)
                buttonEllipse.Opacity = 0.15;

            DockPanel gotDcp = (DockPanel)gotBtn.Content;
            gotDcp.Children.Clear();
            gotDcp.Children.Add(buttonEllipse);
        }



        //рисует доску
        private void PaintGameField()
        {

            Computer.Text = Convert.ToString(game.ComputerScore);
            Player.Text = Convert.ToString(game.PlayerScore);

            for (int row = 0; row < Game.SIZE; row++)
            {
                for (int column = 0; column < Game.SIZE; column++)
                {
                    switch (game.GetCell(row, column))
                    {
                        case 0:
                            {
                                int player;
                                if (IsSingleGame)
                                    player = Game.PLAYER;
                                else
                                    player = currentPlayer;
                                if (game.MakeMove(column, row, player, false) == 0)
                                {
                                    //сделать ход нельзя
                                    int num = (8 * row) + column;
                                    Button gotBtn = (Button)gameField.Children[num];
                                    colouredEllipsePainter(gotBtn, null, true);
                                }
                                else
                                {
                                    //сюда можно сходить
                                    int num = (8 * row) + column;

                                    Brush gotBtnColor;
                                    if (player == Game.PLAYER)
                                        gotBtnColor = colors[2];
                                    else
                                        gotBtnColor = colors[3];
                                    Button gotBtn = (Button)gameField.Children[num];
                                    colouredEllipsePainter(gotBtn, gotBtnColor, false);
                                }
                            }
                            break;

                        case Game.PLAYER:
                            {
                                int num = (8 * row) + column;
                                Brush gotBtnColor = colors[2];
                                Button gotBtn = (Button)gameField.Children[num];
                                colouredEllipsePainter(gotBtn, gotBtnColor, true);
                                break;
                            }
                        case Game.COMPUTER:
                            {
                                int num = (8 * row) + column;
                                Brush gotBtnColor = colors[3];
                                Button gotBtn = (Button)gameField.Children[num];
                                colouredEllipsePainter(gotBtn, gotBtnColor, true);
                                break;
                            }
                    }//end switch
                }//end for
            }//end for

        }//end method


        private void EndGame()
        {
            if (IsSingleGame)
            {
                if (game.ComputerScore > game.PlayerScore)
                    MessageBox.Show("Наш бот победил вас");
                else
                    if (game.ComputerScore == game.PlayerScore)
                    {
                        MessageBox.Show("Ничья");
                    }
                    else
                    {
                        MessageBox.Show("Вы выиграли нашего бота. Нам стыдно.");
                    }
            }
            else
            {
                if (game.ComputerScore > game.PlayerScore)
                    MessageBox.Show("Первый игрок победил");
                else
                    if (game.ComputerScore == game.PlayerScore)
                    {
                        MessageBox.Show("Ничья");
                    }
                    else
                    {
                        MessageBox.Show("Второй игрок победил");
                    }
            }
        }

        private void AboutClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("                        Игру подготовили: \n\n        Спирихин Максим и Румянцев Дмитрий");
        }

        private void SingleGame_Clicked(object sender, RoutedEventArgs e)
        {
            IsSingleGame = true;
            StartGame();
        }

        private void TwoPlayers_Clicked(object sender, RoutedEventArgs e)
        {
            IsSingleGame = false;
            currentPlayer = 1;
            StartGame();
        }

        private void Options_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void FirstDesign_Chousen(object sender, RoutedEventArgs e)
        {
            colors[0] = Brushes.Gray;
            colors[1] = Brushes.RosyBrown;
            colors[2] = Brushes.White;
            colors[3] = Brushes.Black;
            RepaintField();
            PaintGameField();
        }

        private void SecondDesign_Chousen(object sender, RoutedEventArgs e)
        {

            colors[0] = Brushes.White;
            colors[1] = Brushes.White;
            colors[2] = Brushes.Red;
            colors[3] = Brushes.Blue;
            RepaintField();
            PaintGameField();
        }


        private void Thirddesign_Chousen(object sender, RoutedEventArgs e)
        {
            colors[0] = Brushes.LightGray;
            colors[1] = Brushes.Gray;
            colors[2] = Brushes.White;
            colors[3] = Brushes.Black;
            RepaintField();
            PaintGameField();
        }


        private void Forthdesign_Chousen(object sender, RoutedEventArgs e)
        {
            colors[0] = Brushes.Silver;
            colors[1] = Brushes.Gold;
            colors[2] = Brushes.DarkViolet;
            colors[3] = Brushes.Black;

            RepaintField();
            PaintGameField();
        }


          private void RandomDesign_Chousen(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            colors[0] = RandomBrush(rand.Next(1,140));
            colors[1] = RandomBrush(rand.Next(1, 140));
            colors[2] = RandomBrush(rand.Next(1, 140));
            colors[3] = RandomBrush(rand.Next(1, 140));

            RepaintField();
            PaintGameField();
        }


          Brush RandomBrush(int k)
          {
              PropertyInfo[] brushInfo = typeof(Brushes).GetProperties();
              Brush[] brushList = new Brush[brushInfo.Length];
              
              for (int i = 0; i < brushInfo.Length; i++)
              {
                  brushList[i] = (Brush)brushInfo[i].GetValue(null, null);
              }
              return brushList[k];
          }

    }
}