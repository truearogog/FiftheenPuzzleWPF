using System;
using System.IO;
using System.Text;
using System.Windows.Threading;

namespace _15puzzleWPF
{
    public class Puzzle
    {
        public enum START_TYPE {NORMALIZED, RANDOM};

        /* dir 0 = left
         * dir 1 = right
         * dir 2 = up
         * dir 3 = down
         */

        //new zero positions for basic order
        public static int[,] newidx = new int[16, 4] {{-1,1,-1,4},  {0,2,-1,5},   {1,3,-1,6},    {2,-1,-1,7},
                                                       {-1,5,0,8},   {4,6,1,9},    {5,7,2,10},    {6,-1,3,11},
                                                       {-1,9,4,12},  {8,10,5,13},  {9,11,6,14},   {10,-1,7,15},
                                                       {-1,13,8,-1}, {12,14,9,-1}, {13,15,10,-1}, {14,-1,11,-1}};

        private static byte[] boostrofedon = new byte[16] {0, 1, 2, 3, 7, 6, 5, 4, 8, 9, 10, 11, 15, 14, 13, 12};
        private static byte[] final = new byte[16] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0};

        public byte abortIDA = 0;
        private byte solutionFound = 0;
        public int searchLength = 0;
        public Int64 nodecount;

        private int[] pz = new int[16];
        public int[] solution = new int[85];
        private MainWindow winParent;

        public static byte[,] manhattanDistance = new byte[16, 16];
        public Puzzle(START_TYPE type, MainWindow parent)
        {
            this.winParent = parent;
            buildManhattan();

            /*
            for (int x = 0; x<16; ++x)
            {
                for (int y = 0; y<16; ++y)
                {
                    print(manhattanDistance[x,y] + " ");
                }
                print("\r");
            }
            */

            //PUZZLE GENERATION TYPES
            switch (type)
            {
                case START_TYPE.NORMALIZED:
                    startPos();
                    break;
                case START_TYPE.RANDOM:
                    randomize();
                    break;
            }
        }

        private void buildManhattan()
        {
            for (int i = 0; i < 16; ++i)
            {
                //int it = boostrofedon[i];
                for (int j = 0; j < 16; ++j)
                {
                    //int jt = boostrofedon[j];
                    //manhattanDistance[i, j] = (byte)(Math.Abs(it / 4 - jt / 4) + Math.Abs(it % 4 - jt % 4));
                    manhattanDistance[i, j] = (byte)(Math.Abs(i / 4 - j / 4) + Math.Abs(i % 4 - j % 4));
                }
            }
        }

        public int this[int index]
        {
            get { return pz[index]; }
        }

        public static void swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        public void swapPositions(int pos1, int pos2)
        {
            int tmp = pz[pos1];
            pz[pos1] = pz[pos2];
            pz[pos2] = tmp;
        }

        public void loadFromFile()
        {
            try
            {
                string txt = File.ReadAllText("puzzle.txt");
                pz = Array.ConvertAll<string, int>(txt.Split(' '), x => int.Parse(x));
            }
            catch(IOException e){
                print(e.StackTrace + "\r");
            }
        }

        public void randomize()
        {
            Random rand = new Random();

            do
            {
                for (int i = 0; i < 16; ++i)
                {
                    pz[i] = i;
                }
                for (int i = 16; i > 1; i--)
                {
                    swap<int>(ref pz[i - 1], ref pz[rand.Next(i)]);
                }
            } while (parityToBust(ref pz) != 1);
        }

        private void startPos()
        {
            for (byte i = 1; i <= 15; ++i)
            {
                pz[i - 1] = i;
            }
            pz[15] = 0;
        }

        private void printSolution(ref int[] puz)
        {
            StringBuilder sol = new StringBuilder("Solution[" + searchLength + "] : | ");
            int[] pztmp = new int[16];
            Array.Copy(pz, pztmp, 16);
            for (int i = searchLength-1; i>=0; --i)
            {
                sol.Append(pztmp[solution[i]] + " | ");
                swap<int>(ref pztmp[solution[i]], ref pztmp[zeroPos(ref pztmp)]);
            }
            sol.Append("\r");
            print(sol.ToString());
        }

        public int IDAsearch()
        {
            abortIDA = 0;
            solutionFound = 0;
            nodecount = 0;

            for (int i = 0; i < 85; ++i)
                solution[i] = 0;

            int[] node = new int[16];            
            
            Array.Copy(pz, node, 16);

            printNode(ref node);

            if (parityToBust(ref node) != 1)
                return -1;

            searchLength = 2 - (zeroPos(ref node) % 2);
            do
            {
                searchManhattan(node, zeroPos(ref node), -1, searchLength);
                winParent.SearchDepthText.Text = MainWindow.SEARCH_DEPTH_STRING + "\r" + (searchLength);
                searchLength += 1;
            } while (solutionFound==0 && abortIDA==0);

            return searchLength-1;
        }

        private void searchManhattan(int[] node, int zeropos, int ldir, int nextSearch)
        {
            if (solutionFound == 1 || abortIDA == 1)
                return;

            if (nextSearch == 0)
            {
                solution[nextSearch] = zeropos;

                if (solution[84] == 0)
                {
                    //found solution
                    solution[84] = searchLength;
                    printSolution(ref pz);
                    solutionFound = 1;
                    return;
                }
                else
                {
                    if (solution[84] < searchLength)
                    {
                        //found solution
                        solutionFound = 1;
                        return;
                    }
                    printSolution(ref pz);
                    winParent.DoEvents();
                }
            }
            else
            {
                for (int dir = 0; dir < 4; ++dir)
                {
                    if ((ldir == -1) || ((dir + ldir) % 4 != 1) || (solutionFound != 1))
                    {
                        int newZeroPos = newidx[zeropos, dir];
                        if (newZeroPos == -1)
                            continue;

                        swap<int>(ref node[zeropos], ref node[newZeroPos]);

                        byte manDist = 0;
                        for (byte i = 0; i < 16; ++i)
                        {
                            if (i != newZeroPos || node[i] != 0)
                            {
                                manDist += manhattanDistance[node[i] - 1, i];
                            }
                        }

                        //printNode(ref node);
                        //print("m = " + manDist + " | n = " + nextSearch + "\r");
                        //winParent.DoEvents();

                        if (manDist < nextSearch)
                        {
                            //print("m = " + manDist + " | n = " + nextSearch + "\r");
                            //winParent.DoEvents();
                            if (nodecount % 100000 == 0)
                                winParent.DoEvents();

                            ++nodecount;
                            solution[nextSearch] = zeropos;
                            searchManhattan(node, newZeroPos, dir, nextSearch - 1);
                        }

                        swap<int>(ref node[zeropos], ref node[newZeroPos]);
                    }
                }
            }
        }

        private void print(string message)
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => winParent.Print(message)));
        }

        private void printNode(ref int[] node)
        {
            for (int i = 0; i < 16; ++i)
                print(node[i] + " ");
            print("\r");
        }

        private byte checkNode(ref int[] node)
        {
            byte x = 0;
            for (int i = 0; i < 16; ++i)
                if (node[i] != final[i])
                    x++;
            return x;
        }

        private int parityToBust(ref int[] puz)
        {
            int[] tmp = new int[16];
            for (int i = 0; i < 16; ++i)
            {
                tmp[i] = puz[boostrofedon[i]];
            }

            int mix = 0;
            for (int i = 0; i < 15; ++i)
            {
                if (tmp[i] == 0) continue;
                for (int j = i + 1; j < 16; ++j)
                {
                    if (tmp[j] == 0) continue;
                    if (tmp[i] > tmp[j])
                    {
                        ++mix;
                    }
                }
            }
            return mix % 2;
        }

        private int parity(ref int[] puz)
        {
            int mix = 0;
            for (int i = 0; i < 15; ++i)
            {
                if (puz[i] == 0) continue;
                for (int j = i + 1; j < 16; ++j)
                {
                    if (puz[j] == 0) continue;
                    if (puz[i] > puz[j])
                    {
                        ++mix;
                    }
                }
            }
            return mix % 2;
        }

        static public int zeroPos(ref int[] puz)
        {
            for(byte i = 0; i < 16; ++i)
            {
                if (puz[i] == 0)
                    return i;
            }
            return -1;
        }

        public int zeroPos()
        {
            for (byte i = 0; i < 16; ++i)
            {
                if (pz[i] == 0)
                    return i;
            }
            return -1;
        }

        static public int zeroPos(ref Puzzle puz)
        {
            for (byte i = 0; i < 16; ++i)
            {
                if (puz[i] == 0)
                    return i;
            }
            return -1;
        }
    }
}
