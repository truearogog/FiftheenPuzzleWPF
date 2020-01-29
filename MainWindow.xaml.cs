using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public const string GENERATED_NODES_STRING = "Generated nodes: ";
        public const string TIME_STRING = "Elapsed time: ";
        public const byte CONSOLE_LENGTH = 136;
        public const byte AnimationSpeed = 7;
        public bool movingTile = false;

        public static byte[,] matrixToPuz = new byte[4,4]{ { 0,1,2,3},{ 4,5,6,7},{ 8,9,10,11},{ 12,13,14,15} }; 

        private Image[] tiles = new Image[15];
        private Puzzle puz;

        private delegate void EmptyDelegate();

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

        public void SetNodeCount()
        {
            NodesText.Text = GENERATED_NODES_STRING + "\r" + puz.nodecount;
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
            DistanceText.Text = MANHATTAN_DISTANCE_STRING + "\r" + puz.GetManhattanDist();
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
                Println(Dupestring(CONSOLE_LENGTH, "-").ToString());
            }

            SolveButton.Content = "Abort";
            NodesText.Text = GENERATED_NODES_STRING + "\r-";
            TimeText.Text = TIME_STRING + "\r-ms";
            AnimateButton.IsEnabled = false;
            ShuffleButton.IsEnabled = false;
            LoadButton.IsEnabled = false;
            //ConsoleBox.Document.Blocks.Clear();

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

        public void MoveTile(int num, int dir, int am)
        {
            /* dir 0 = left
             * dir 1 = right
             * dir 2 = up
             * dir 3 = down
             */
            switch (dir)
            {
                case 0:
                    Canvas.SetLeft(tiles[num], Canvas.GetLeft(tiles[num]) - AnimationSpeed*am);
                    break;
                case 1:
                    Canvas.SetLeft(tiles[num], Canvas.GetLeft(tiles[num]) + AnimationSpeed*am);
                    break;
                case 2:
                    Canvas.SetTop(tiles[num], Canvas.GetTop(tiles[num]) - AnimationSpeed*am);
                    break;
                case 3:
                    Canvas.SetTop(tiles[num], Canvas.GetTop(tiles[num]) + AnimationSpeed*am);
                    break;
                default:
                    break;
            }
        }

        private void AnimateTile(int num, int dir,int am)
        {
            int to = (int)Math.Floor((TILE_SIZE + TILE_OFFSET) / AnimationSpeed);
            for (int x = 0; x < to; ++x)
            {
                MoveTile(num-1, dir, am);
                DoEvents();
                Thread.Sleep(20);
            }
            DistanceText.Text = MANHATTAN_DISTANCE_STRING + "\r" + puz.GetManhattanDist();
        }

        private void AnimateButton_Clicked(object sender, RoutedEventArgs e)
        {
            for (int i = puz.searchLength - 2; i >= 0; --i)
            {
                int zeroPos = puz.zeroPos();
                for (int dir = 0; dir < 4; ++dir)
                {
                    int newZeroPos = Puzzle.newidx[zeroPos, dir];
                    if (newZeroPos == -1)
                        continue;
                    if (newZeroPos == puz.solution[i])
                    {
                        puz.swapPositions(zeroPos,newZeroPos);
                        AnimateTile(puz[zeroPos], dir, - 1);
                    }
                }
            }
        }

        private void LoadButton_Clicked(object sender, RoutedEventArgs e)
        {
            puz.loadFromFile();
            ChangeTilesPositions();
            Println("Loaded");
        }

        private void MoveTilePressed(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (movingTile)
                return;
            movingTile = true;
            Point mpos = Mouse.GetPosition(TileCanvas);
            byte x = (byte)Math.Floor(mpos.X / (TILE_SIZE+TILE_OFFSET));
            byte y = (byte)Math.Floor(mpos.Y / (TILE_SIZE+TILE_OFFSET));
            byte chosenTile = matrixToPuz[y, x];
            for (int dir = 0; dir < 4; ++dir)
            {
                int zeroPos = Puzzle.newidx[chosenTile, dir];
                if ((zeroPos == -1) || (puz[zeroPos] != 0))
                    continue;
                
                puz.swapPositions(zeroPos, chosenTile);
                AnimateTile(puz[zeroPos], dir, 1);
                break;
            }
            movingTile = false;
        }
    }
}
