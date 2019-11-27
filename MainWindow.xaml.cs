using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace _15puzzleWPF
{
    public partial class MainWindow : Window
    {
        public const double TILE_SIZE = 65;
        public const double TILE_OFFSET = 5;
        public const string SEARCH_DEPTH_STRING = "Search depth: ";
        public const string MANHATTAN_DISTANCE_STRING = "Manhattan distance: ";
        public const string GENERATED_NODES_STRING = "Generated nodes:";
        public const string TIME_STRING = "Elapsed time:";
        public const byte CONSOLE_LENGTH = 136;
        public const byte AnimationSpeed = 5;

        private Image[] tiles = new Image[15];
        private Puzzle puz;

        private delegate void EmptyDelegate();

        int dir;

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
                tiles[i].Width = temp.PixelWidth-63;
                tiles[i].Height = temp.PixelWidth-63;
                TileCanvas.Children.Add(tiles[i]);
            }
            ChangeTilesPositions();
        }

        public void DoEvents()
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new EmptyDelegate(delegate { }));
        }

        public void ChangeTilesPositions()
        {
            for (byte i = 0; i < 16; ++i)
            {
                if (puz[i] == 0)
                    continue;
                Canvas.SetLeft(tiles[puz[i]-1], (TILE_SIZE + TILE_OFFSET) * (i % 4) + TILE_OFFSET/2);
                Canvas.SetTop(tiles[puz[i]-1],  (TILE_SIZE + TILE_OFFSET) * (i / 4) + TILE_OFFSET/2);
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
                Println(Dupestring(CONSOLE_LENGTH * 2, "-").ToString());
            }

            SolveButton.Content = "Abort";
            NodesText.Text = GENERATED_NODES_STRING + "\r-";
            TimeText.Text = TIME_STRING + "\r-ms";
            AnimateButton.IsEnabled = false;
            ShuffleButton.IsEnabled = false;
            LoadButton.IsEnabled = false;

            //get system time
            long startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            
            int l = puz.IDAsearch();

            if (l == -1)
            {
                //Unable to solve
                SearchDepthText.Text = SEARCH_DEPTH_STRING + "\r-";
                Println("Unsolvable");
            }
            else if (puz.abortIDA == 0)
            {
                //Solved
                //Reset time label
                long endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                TimeText.Text = TIME_STRING + "\r" + (endTime - startTime).ToString() + " ms";

                //reset nodes label
                NodesText.Text = GENERATED_NODES_STRING + "\r" + puz.nodecount;
                SearchDepthText.Text = SEARCH_DEPTH_STRING + "\r" + l;
            }
            SolveButton.Content = "Solve";
            ShuffleButton.IsEnabled = true;
            AnimateButton.IsEnabled = true;
            LoadButton.IsEnabled = true;
        }

        public void MoveTile(int num, int dir)
        {
            /* dir 0 = left
             * dir 1 = right
             * dir 2 = up
             * dir 3 = down
             */
            switch (dir)
            {
                case 0:
                    Canvas.SetLeft(tiles[num], Canvas.GetLeft(tiles[num]) + AnimationSpeed);// TILE_SIZE / 8);
                    break;
                case 1:
                    Canvas.SetLeft(tiles[num], Canvas.GetLeft(tiles[num]) - AnimationSpeed);//TILE_SIZE / 8);
                    break;
                case 2:
                    Canvas.SetTop(tiles[num], Canvas.GetTop(tiles[num]) + AnimationSpeed); //TILE_SIZE / 8);
                    break;
                case 3:
                    Canvas.SetTop(tiles[num], Canvas.GetTop(tiles[num]) - AnimationSpeed); //TILE_SIZE / 8);
                    break;
                default:
                    break;
            }
        }

        private void AnimateButton_Clicked(object sender, RoutedEventArgs e)
        {
            for (int i = puz.searchLength - 1; i >= 0; --i)
            {
                for (int dir = 0; dir < 4; ++dir)
                {
                    int zeropos = puz.zeroPos();
                    int newZeroPos = Puzzle.newidx[zeropos, dir];
                    if (newZeroPos == -1)
                        continue;

                    if (puz[newZeroPos] == puz[puz.solution[i]])
                    {
                        puz.swapPositions(newZeroPos, zeropos);
                        for (int x = 0; x < 14; ++x)
                        {
                            MoveTile(puz[zeropos]-1, dir);
                            DoEvents();
                            Thread.Sleep(20);
                        }
                        break;
                    }
                }
            }

            /*
            for (int i = searchLength - 1; i >= 0; --i)
            {
                sol.Append(pztmp[solution[i]]);
                swap<int>(ref pztmp[solution[i]], ref pztmp[zeroPos(ref pztmp)]);
            }
            */
        }

        private void LoadButton_Clicked(object sender, RoutedEventArgs e)
        {
            puz.loadFromFile();
            ChangeTilesPositions();
            Println("Loaded");
        }
    } 
}
