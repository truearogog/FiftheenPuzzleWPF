using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _15puzzleWPF
{
    public partial class MainWindow : Window
    {
        public const byte TILE_SIZE = 128;
        public const byte CONSOLE_LENGTH = 138;
        public const byte TILE_OFFSET = 4;
        public const string SEARCH_DEPTH_STRING = "Search depth:";
        public const string MANHATTAN_DISTANCE_STRING = "Manhattan distance: ";
        public const string GENERATED_NODES_STRING = "Generated nodes:";
        public const string TIME_STRING = "Elapsed time:";

        private Image[] tiles = new Image[15];
        private Puzzle puz;

        public MainWindow()
        {
            InitializeComponent();

            puz = new Puzzle(Puzzle.START_TYPE.NORMALIZED, this);

            //generate 15 puzzle images from source
            for (byte i = 0; i < 15; ++i)
            {
                BitmapImage temp = new BitmapImage();
                temp.BeginInit();
                temp.UriSource = new Uri("PuzzleImages/Tile" + (i+1).ToString() + ".png", UriKind.Relative);
                temp.EndInit();
                tiles[i] = new Image();
                tiles[i].Source = temp;
                tiles[i].Width = temp.PixelWidth - TILE_OFFSET*2;
                tiles[i].Height = temp.PixelHeight - TILE_OFFSET*2;
                TileCanvas.Children.Add(tiles[i]);
            }
            ChangeTilesPositions();
        }

        public void ChangeTilesPositions()
        {
            for (byte i = 0; i < 16; ++i)
            {
                if (puz[i] == 0)
                    continue;
                Canvas.SetLeft(tiles[puz[i]-1], TILE_SIZE * (i % 4) + TILE_OFFSET);
                Canvas.SetTop(tiles[puz[i]-1], TILE_SIZE * (i / 4) + TILE_OFFSET);
            }
        }

        public void MoveTile(byte num, int dir)
        {
            /* dir 0 = left
             * dir 1 = right
             * dir 2 = up
             * dir 3 = down
             */
            switch (dir)
            {
                case 0:
                    Canvas.SetLeft(tiles[num], Canvas.GetLeft(tiles[num]) - TILE_SIZE / 8);
                    break;
                case 1:
                    Canvas.SetLeft(tiles[num], Canvas.GetLeft(tiles[num]) + TILE_SIZE / 8);
                    break;
                case 2:
                    Canvas.SetTop(tiles[num], Canvas.GetTop(tiles[num]) - TILE_SIZE / 8);
                    break;
                case 3:
                    Canvas.SetTop(tiles[num], Canvas.GetTop(tiles[num]) + TILE_SIZE / 8);
                    break;
                default:
                    break;
            }
        }

        public void Print(string message)
        {
            ConsoleBox.AppendText(message);
        }

        public void Println(string message)
        {
            ConsoleBox.AppendText(message + "\r");
        }

        private StringBuilder Dupestring(ulong i, string s)
        {
            StringBuilder res = new StringBuilder("");
            for (ulong x = 0; x < i; ++x)
                res.Append(s);
            return res;
        }

        private void ShuffleButton_Clicked(object sender, RoutedEventArgs e)
        {
            puz.randomize();
            ChangeTilesPositions();
        }

        private void SolveButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (SolveButton.Content.ToString() == "Abort")
            {
                puz.abortIDA = 1;
                SolveButton.Content = "Solve";
                Println("Aborted");
                return;
            }
            else
            {
                ConsoleBox.Document.Blocks.Clear();
                Println(Dupestring(CONSOLE_LENGTH * 2, "-").ToString());
            }

            SolveButton.Content = "Abort";
            AnimateButton.IsEnabled = false;
            ShuffleButton.IsEnabled = false;

            //get system time
            long startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            int l = puz.IDAsearch();

            if (l == -1)
            {
                //Unable to solve
                SearchDepthText.Text = SEARCH_DEPTH_STRING + "\r-";
                NodesText.Text = GENERATED_NODES_STRING + "\r-";
                Println("Unsolvable");
            }
            else if (puz.abortIDA == 0)
            {
                //Solved
                //Reset time label
                long endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                TimeText.Text = TIME_STRING + "\r" + (endTime-startTime).ToString() + " ms";

                //reset nodes label
                NodesText.Text = GENERATED_NODES_STRING + "\r" + puz.nodecount;
                SearchDepthText.Text = SEARCH_DEPTH_STRING + "\r" + l;
            }

            Println("");
            SolveButton.Content = "Solve";
            ShuffleButton.IsEnabled = true;
            AnimateButton.IsEnabled = true;  
        }

        private void AnimateButton_Clicked(object sender, RoutedEventArgs e)
        {

        }
    } 
}
