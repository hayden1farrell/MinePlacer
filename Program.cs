using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeperish
{
    // Guided study code
    // Complete the two empty routines

    struct Cell
    {
        public bool mine;
        public int X;
        public int Y;
        public int displayValue;
        public bool visited;
    }

    class Program
    {
        const char SPACE = ' ';
        const int playSize = 4;

        static Cell[,] Minefield = new Cell[playSize + 2, playSize + 2];
        const int MINES = 4;
        static void Main(string[] args)
        {
            Memset();
            Console.ForegroundColor = ConsoleColor.White;
            PlaceMines();
            Console.WriteLine("Mines Placed");
            //DisplayMineField(null);
            Scan();
            Console.WriteLine("Minefield scanned");
            //DisplayMineField(null);
            Console.ReadLine();

            FloodIshFillSetUp();
            Console.WriteLine("-------------");
            Safeist();
            Console.WriteLine("-------------");
            RecursiveSafeSetUp();
        }

        private static void RecursiveSafeSetUp()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.Write("LOWEST RISK Recursive: ");
            ResetMaze();
            Scan();
            Stack path = new Stack();
            Minefield[1, 1].visited = true;
            path.Push(Minefield[1, 1]);
            bool pathPossible = SmartFindRecusive(1, 1, ref path);
            Console.WriteLine($"Path found: {pathPossible} found in: {stopWatch.Elapsed.TotalSeconds}");
            if (pathPossible == false) return;
            List<Cell> pathway = StackToList(path);
            DisplayCoordinates(pathway);
            DisplayMineField(pathway);
            Console.ReadKey();
        }

        private static void Safeist()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.Write("LOWEST RISK: ");
            ResetMaze();
            Scan();
            Stack path = new Stack();
            bool pathPossible = SmartFind(1, 1, ref path);
            stopWatch.Stop();
            Console.WriteLine($"Path found: {pathPossible} found in: {stopWatch.Elapsed.TotalSeconds}");
            if (pathPossible == false) return;
            List<Cell> pathway = StackToList(path);
            DisplayCoordinates(pathway);
            DisplayMineField(pathway);
            Console.ReadKey();
        }

        private static void DisplayCoordinates(List<Cell> pathway)
        {
            Console.WriteLine("Safest path coordinates");
            pathway.Reverse();
            foreach (Cell cell in pathway)
            {
                Console.Write($"({cell.X},{cell.Y}) ");
            }
            Console.WriteLine(" ");
        }

        private static void FloodIshFillSetUp()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.Write("FLOOD FILL: ");
            List<Cell> list = new List<Cell>();
            Cell start = new Cell(); start.X = 1; start.Y = 1; start.mine = false;
            list.Add(start);
            bool pathPossible = Traverse(1, 1, ref list, false);
            Console.WriteLine($"Path found: {pathPossible} found in: {stopWatch.Elapsed.TotalSeconds}");
            DisplayMineField(list);
            Console.ReadKey();
        }

        private static List<Cell> StackToList(Stack path)
        {
            List<Cell> pathway = new List<Cell>();
            while(path.Count > 0)
            {
                pathway.Add((Cell)path.Peek());
                path.Pop();
            }
            return pathway;
        }

        private static void ResetMaze()
        {
            for (int x = 0; x < playSize + 2; x++)
            {
                for (int y = 0; y < playSize + 2; y++)
                {
                    if (Minefield[x, y].mine == false)
                        Minefield[x, y].displayValue = 0;
                    else
                        Minefield[x, y].displayValue = -1;
                    Minefield[x, y].visited = false;
                }
            }
        }

        private static bool SmartFindRecusive(int row, int col, ref Stack path)
        {

            Cell currentCell = Minefield[row, col];
            (int lowRow, int lowCol) = GetLowestNeighbor(currentCell.X, currentCell.Y);

            if (lowRow != int.MaxValue)
            {
                currentCell.visited = true;
                Minefield[currentCell.X, currentCell.Y] = currentCell;
                Cell nextCell = Minefield[lowRow, lowCol];
                nextCell.visited = true;
                Minefield[nextCell.X, nextCell.Y] = nextCell;
                path.Push(nextCell);
                row = lowRow; col = lowCol;
            }
            else
            {
                if (path.Count == 1) return false;
                path.Pop();
                currentCell = (Cell)path.Peek();
                row = currentCell.X; col = currentCell.Y;
            }
            if (row == playSize && col == playSize)
                return true;

            return SmartFindRecusive(row, col, ref path);
        }

        private static bool SmartFind(int row, int col, ref Stack path)
        {

            Minefield[row, col].visited = true;
            path.Push(Minefield[row, col]);

            while (true)
            {
                Cell currentCell = Minefield[row, col];
                (int lowRow, int lowCol) = GetLowestNeighbor(currentCell.X, currentCell.Y);

                if(lowRow != int.MaxValue)
                {
                    currentCell.visited = true;
                    Minefield[currentCell.X, currentCell.Y] = currentCell;
                    Cell nextCell = Minefield[lowRow, lowCol];
                    nextCell.visited = true;
                    Minefield[nextCell.X, nextCell.Y] = nextCell;


                    path.Push(nextCell);
                    row = lowRow; col = lowCol;
                }
                else
                {
                    if(path.Count == 1) return false;
                    path.Pop();
                    currentCell = (Cell)path.Peek();
                    row = currentCell.X;
                    col = currentCell.Y;
                }

                if(row == playSize && col == playSize)
                {
                    return true;
                }
            }
        }

        private static (int lowRow, int lowCol) GetLowestNeighbor(int row, int col)
        {
            int lowRow = int.MaxValue; int lowCol = int.MaxValue; int currentLow = int.MaxValue;
            for (int currentRow = row - 1; currentRow <= row + 1; currentRow += 2)
            {
                bool valid = ValidCell(currentRow, col, currentLow);
                if (valid == true)
                {
                    currentLow = Minefield[currentRow, col].displayValue; lowRow = currentRow; lowCol = col;
                }
            }

            for (int currentCol= col - 1; currentCol <= col + 1; currentCol += 2)
            {
                bool valid = ValidCell(row, currentCol, currentLow);
                if (valid == true)
                {
                    currentLow = Minefield[row, currentCol].displayValue; lowRow = row; lowCol = currentCol;
                }
            }
            return (lowRow, lowCol);
        }

        static bool ValidCell(int currentRow, int currentCol, int currentLow)
        {
            if (ValidSpace(currentRow, currentCol) == false) return false; 
            if (Minefield[currentRow, currentCol].visited == true) return false;
            if (Minefield[currentRow, currentCol].mine == true) return false;
            if (Minefield[currentRow, currentCol].displayValue > currentLow) return false;

            return true;
        }

        private static bool ValidSpace(int row, int col)
        {
            if (row > playSize || row <= 0) return false;
            if (col > playSize || col <= 0) return false;
            return true;
        }

        static void Memset()
        {
            for (int x = 0; x < Minefield.GetLength(0); x++)
            {
                for (int y = 0; y < Minefield.GetLength(0); y++)
                {
                    Cell cell = new Cell();
                    cell.X = x; cell.Y = y; cell.mine = false; cell.displayValue = 0; cell.visited = false;
                    Minefield[x, y] = cell;
                }
            }
        }

        static void PlaceMines()
        {
            int minesPlaced = 0;
            Random rand = new Random();
            while(minesPlaced < MINES)
            {
                int row = rand.Next(1, playSize + 1);
                int col = rand.Next(1, playSize + 1);

                if (Minefield[row, col].mine == false)
                {
                    if (row == 1 && col == 1) continue;
                    else if (row == playSize && col == playSize) continue;
                    else
                    {
                        Minefield[row, col].mine = true;
                        Minefield[row, col].displayValue = -1;
                        minesPlaced += 1;
                    }
                }
            }
        }

        static void Scan()
        {
            for (int row = 1; row <= playSize; row++)
            {
                for (int col = 1; col <= playSize; col++)
                {
                    if(Minefield[row, col].mine == false)
                    {
                        int Count = 0;
                        for (int chkrow =  row - 1; chkrow  <= row + 1; chkrow++)
                        {
                            for (int chkcol = col - 1; chkcol <= col + 1; chkcol++)
                            {
                                if(Minefield[chkrow, chkcol].mine == true)
                                    Count++;
                            }
                        }
                        Minefield[row, col].displayValue = Count;
                    }
                }
            }
        }

        static bool Traverse(int row, int col, ref List<Cell> been, bool found)
        {
            if(row > playSize || row <= 0) return false;
            if (col > playSize || col <= 0) return false;
            if (Minefield[row, col].mine == true || Minefield[row, col].visited == true)
            {
                return false;
            }

            if(row == playSize && col == playSize)
            {
                Minefield[row, col].visited = true; Minefield[row, col].displayValue = 9;
                been.Add(Minefield[row, col]);
                found = true;
                return found;
            }
            Minefield[row, col].visited = true; Minefield[row, col].displayValue = 9;
            been.Add(Minefield[row, col]);
            if(found == false) found = Traverse(row + 1, col, ref been, found);
            if (found == false) found = Traverse(row - 1, col, ref been, found);
            if (found == false) found = Traverse(row, col + 1, ref been, found);
            if (found == false) found = Traverse(row, col - 1, ref been, found);

            return found;
        }

        static void DisplayMineField(List<Cell> been)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" | 1 | 2 | 3 | 4 |");
            Console.WriteLine("------------------");
            for (int r = 1; r < playSize + 1; r++)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(r + "|");
                Console.ForegroundColor = ConsoleColor.White;
                for (int c = 1; c < playSize + 1; c++)
                {
                    bool path = IsPath(been, r, c);
                    string space = "";
                    string toDisplay = Minefield[r,c].displayValue.ToString();
                    if (path == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        toDisplay = "+";
                    }
                    else Console.ForegroundColor = ConsoleColor.White;

                    if (Minefield[r, c].mine == false)
                       space = " ";
                   Console.Write(" " + space + toDisplay );
                   Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" |");
                }
                Console.WriteLine();
            }
        }

        private static bool IsPath(List<Cell> been, int x, int y)
        {
            if (been == null) return false;

            foreach (var dot in been)
            {
                if (dot.X == x && dot.Y == y)
                    return true;
            }

            return false;
        }
    }
}