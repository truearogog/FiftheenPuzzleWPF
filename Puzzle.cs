using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

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

        private static int[,] newidx = new int[16, 4] {{-1,1,-1,7},{0,2,-1,6},{1,3,-1,5},{2,-1,-1,4},
                                                {-1,5,0,8},{4,6,1,9},{5,7,2,10},{6,-1,3,11},
                                                {-1,9,4,12},{8,10,5,13},{9,11,6,14},{10,-1,7,15},
                                                {-1,13,8,-1},{12,14,9,-1},{13,15,10,-1},{14,-1,11,-1}
                                                };

        private static byte[] boostrofedon = new byte[16] {0, 1, 2, 3, 7, 6, 5, 4, 8, 9, 10, 11, 15, 14, 13, 12};
        private static byte[] final = new byte[16] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0};

        public byte abortIDA = 0;
        private byte solutionFound = 0;
        public int searchLength = 0;
        public Int64 nodecount;

        private int[] pz = new int[16];
        public int[] solution = new int[85];
        private object parent;

        public static byte[,] manhattanDistance = new byte[16, 16];

        public Puzzle(START_TYPE type, object parent)
        {
            this.parent = parent;
            buildManhattan();

            /*
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
            */

            /*
            //PRINT MANHATTAN MATRIX 
            for (byte i = 0; i < 16; ++i)
            {
                for (byte j = 0; j < 16; ++j)
                {
                    print(manhattanDistance[i, j] + "  ");
                }
                print("\r");
            }
            */

            //training set
            pz = new int[16] { 1, 2, 3, 4, 5, 6, 8, 0, 9, 10, 7, 11, 13, 14, 15, 12 };
        }

        private void buildManhattan()
        {
            for (int i = 0; i < 16; ++i)
            {
                int ik = boostrofedon[i];
                for (int j = 0; j < 16; ++j)
                {
                    int jk = boostrofedon[j];
                    manhattanDistance[i, j] = (byte) (Math.Abs(ik / 4 - jk / 4) + Math.Abs(ik % 4 - jk % 4));
                }
            }
        }

        public int this[byte index]
        {
            get { return pz[index]; }
        }

        private void swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        public void randomize()
        {
            Random rand = new Random();

            do
            {
                for (int i = 0; i < 16; ++i)
                {
                    pz[i] = (byte)i;
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
            StringBuilder sol = new StringBuilder("");
            sol.Append("Solution : ");
            for (int i = searchLength-1; i>=0; --i)
            {
                sol.Append(solution[i]+1);
                if (i != 0)
                    sol.Append(" | ");
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

            /*
            int[] pzr = new int[16];
            for (int i = 0; i < 16; ++i)
            {
                pzr[i] = (16 - pz[15 - i]) % 16;
            }

            for (int i = 0; i < 16; ++i)
            {
                node[i] = boostrofedon[pzr[boostrofedon[i]]];
            }
            */

            for (int i = 0; i < 16; ++i)
            {
                print(node[i] + " ");
            }
            print(" | " + parityToBust(ref node) + "\r");

            if (parityToBust(ref node) != 1)
                return -1;

            searchLength = 2 - zeroPosBust(ref node) % 2;

            do
            {
                searchManhattan(node, zeroPos(ref node), -1, searchLength);
                searchLength += 2;
            } while (solutionFound==0 && abortIDA==0);

            return searchLength - 2;
        }

        private void searchManhattan(int[] node, int zeropos, int ldir, int nextSearch)
        {
            if (solutionFound == 1 || abortIDA == 1)
            {
                return;
            }

            //print((zeropos+1) + " " + ldir + " " + searchLength + " " + nextSearch + " " + nodecount + "\r");
            //print(searchLength + "\r");
            //printNode(ref node);

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
                        printSolution(ref pz);
                        solutionFound = 1;
                        return; 
                    }
                    printSolution(ref pz);
                }
            }
            else
            {
                for (int dir = 0; dir < 4; ++dir)
                {
                    if (ldir == -1 || (dir + ldir) % 4 != 1 || solutionFound != 1)
                    {
                        int newZeroPos = newidx[zeropos, dir];
                        if (newZeroPos == -1)
                            continue;

                        swap<int>(ref node[zeropos], ref node[newZeroPos]);

                        int manDist = 0;
                        for (byte i = 0; i < 16; ++i)
                        {
                            if (i != newZeroPos)
                                manDist += manhattanDistance[node[i]-1, i];
                        }
                        ((MainWindow)parent).DistanceText.Text = MainWindow.MANHATTAN_DISTANCE_STRING + manDist;
                        DoEvents();

                        //printNode(ref node);
                        //print("m = " + manDist + " | n = " + nextSearch + "\r");

                        if (manDist < nextSearch)
                        {
                            nodecount++;
                            solution[nextSearch] = zeropos;
                            searchManhattan(node, newZeroPos, dir, nextSearch - 1);
                        }

                        swap<int>(ref node[zeropos], ref node[newZeroPos]);
                    }
                }
            }
        }

        public event EventHandler MyEventHandler = delegate { };
        private void DoEvents()
        {
            try
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, MyEventHandler);
            }
            catch (Exception e) { }
        }

        private void print(string message)
        {
            ((MainWindow)parent).Print(message);
        }

        private void printNode(ref int[] node)
        {
            for (int i = 0; i < 16; ++i)
                print(node[i] + " ");
            print("\r");
        }

        private bool checkNode(ref int[] node)
        {
            for (int i = 0; i < 16; ++i)
                if (node[i] != final[i])
                    return false;
            return true;
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
                        mix++;
                    }
                }
            }
            return mix % 2;
        }

        private int zeroPos(ref int[] puz)
        {
            for(byte i = 0; i < 16; ++i)
            {
                if (puz[i] == 0)
                    return i;
            }
            return -1;
        }

        private int zeroPosBust(ref int[] puz)
        {
            int[] tmp = new int[16];
            for (int i = 0; i < 16; ++i)
            {
                tmp[i] = puz[boostrofedon[i]];
            }

            for (byte i = 0; i < 16; ++i)
            {
                if (tmp[i] == 0)
                    return i;
            }
            return -1;
        }
    }
}
