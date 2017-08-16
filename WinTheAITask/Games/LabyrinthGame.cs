using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTheAITask.Games
{
    class LabyrinthGame : Game
    {
        public string Rules { get; }
        public string Name { get; }

        private Labyrinth GameLabyrinth;

        public LabyrinthGame()
        {
            this.Name = "Labyrinth";
            this.Rules = "You must sent only one string that contains sybmols 'u', 'l', 'd' and 'r' (up, left, down and right) - the shortest way to the finish.";
            this.Rules += "\n\tStart point = s";
            this.Rules += "\n\tEnd point = e";
            this.Rules += "\nType 'Start' to begin";

            GameLabyrinth = Labyrinth.Generate();
        }

        public GameResult GetAnswer(string Request)
        {
            if (Request.ToLower().StartsWith("start"))
            {
                GameResult Result = new GameResult();
                Result.State = GameState.ContinuePlaying;
                Result.Message = GameLabyrinth.ToString();
                return Result;
            }

            return new GameResult(State: GameState.Lose, Message: "Wtf?");
        }

        private class Labyrinth
        {
            public int Height { get; }
            public int Width { get; }

            private Cell[,] Map;

            public override string ToString()
            {
                StringBuilder SB = new StringBuilder();
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        SB.Append(Map[i, j]);
                    }
                    SB.Append('\n');
                }

                return SB.ToString();
            }

            private Labyrinth(int Height, int Width)
            {
                if (Height % 2 == 0)
                    Height++;
                if (Width % 2 == 0)
                    Width++;

                this.Height = Height;
                this.Width = Width;

                Map = new Cell[Height, Width];

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        if ((i & 1) == 0 || (j & 1) == 0)
                            Map[i, j] = new Cell(Cell.CellType.Wall, i, j);
                        else
                            Map[i, j] = new Cell(Cell.CellType.Empty, i, j);
                    }
                }

                Map[1, 1].Type = Cell.CellType.Start;
                Map[Height - 2, Width - 2].Type = Cell.CellType.Finish;
            }

            public static Labyrinth Generate(UInt32 seed = 0)
            {
                if (seed == 0)
                    seed = (UInt32)(new Random()).Next(0, Int32.MaxValue);

                int Height = 90 + (int)(seed % 10);
                int Width = 30 + (int)(seed % 9);

                Labyrinth Resutlt = new Labyrinth(Height, Width);

                int Modifer = 1;
                while (!Resutlt.IsPassable)
                {
                    Resutlt.DeleteNearestWall((seed / Modifer) % (Resutlt.Height - 2) + 1,
                                               (seed / Modifer) % (Resutlt.Width - 2) + 1);
                    Modifer++;
                }

                return Resutlt;
            }

            public void DeleteNearestWall(long y, long x)
            {
                Queue<Cell> queue = new Queue<Cell>();
                bool[,] Used = new bool[Height, Width];

                if ((y % 2 == 0 && x % 2 == 0) || (y % 2 == 1 && x % 2 == 1))
                {
                    switch (((x + y) / 2) % 4)
                    {
                        case 0:
                            x += 1; break;
                        case 1:
                            y += 1; break;
                        case 2:
                            x -= 1; break;
                        case 3:
                            y -= 1; break;
                    }
                }

                if (y >= Height - 1)
                    y -= 2;
                if (x >= Width - 1)
                    x -= 2;
                if (y <= 0)
                    y += 2;
                if (x <= 0)
                    x += 2;

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        Used[i, j] = false;
                    }
                }

                Cell NowCell = Map[y, x];
                Used[y, x] = true;
                
                while (NowCell.Type != Cell.CellType.Wall)
                {
                    if (NowCell.Y + 2 < Height - 1 && !Used[NowCell.Y + 2, NowCell.X])
                    {
                        queue.Enqueue(Map[NowCell.Y + 2, NowCell.X]);
                        Used[NowCell.Y + 2, NowCell.X] = true;
                    }
                    if (NowCell.Y - 2 > 0 && !Used[NowCell.Y - 2, NowCell.X])
                    {
                        queue.Enqueue(Map[NowCell.Y - 2, NowCell.X]);
                        Used[NowCell.Y - 2, NowCell.X] = true;
                    }
                    if (NowCell.X + 2 < Width - 1 && !Used[NowCell.Y, NowCell.X + 2])
                    {
                        queue.Enqueue(Map[NowCell.Y, NowCell.X + 2]);
                        Used[NowCell.Y, NowCell.X + 2] = true;
                    }
                    if (NowCell.X - 2 > 0 && !Used[NowCell.Y, NowCell.X - 2])
                    {
                        queue.Enqueue(Map[NowCell.Y, NowCell.X - 2]);
                        Used[NowCell.Y, NowCell.X - 2] = true;
                    }

                    if (queue.Count == 0)
                        return;
                    if (queue.Count > 30)
                        return;
                    NowCell = queue.Dequeue();
                }

                NowCell.Type = Cell.CellType.Empty;
            }

            public bool IsPassable
            {
                get
                {
                    int[,] IntMap = new int[Height, Width];

                    for (int i = 0; i < Height; i++)
                    {
                        for (int j = 0; j < Width; j++)
                        {
                            if (Map[i, j].Type == Cell.CellType.Wall)
                                IntMap[i, j] = -1;
                            else
                                IntMap[i, j] = 0;
                        }
                    }

                    IntMap[1, 1] = 1;

                    bool HasNewCell = true;
                    int Counter = 2;

                    while (HasNewCell)
                    {
                        HasNewCell = false;

                        for (int i = 1; i < Height - 1; i++)
                        {
                            for (int j = 1; j < Width - 1; j++)
                            {
                                if (IntMap[i, j] == 0 &&
                                    (IntMap[i + 1, j] == Counter - 1 ||
                                     IntMap[i - 1, j] == Counter - 1 ||
                                     IntMap[i, j - 1] == Counter - 1 ||
                                     IntMap[i, j - 1] == Counter - 1))
                                {
                                    IntMap[i, j] = Counter;
                                    HasNewCell = true;
                                }
                            }
                        }

                        Counter++;
                    }

                    if (IntMap[Height - 2, Width - 2] != 0)
                        return true;
                    return false;
                }
            }

            private class Cell
            {
                public int Y { get; }
                public int X { get; }
                public bool HasRightBorder { get; set; }
                public bool HasBottomBorder { get; set; }

                public override string ToString()
                {
                    switch (Type)
                    {
                        case CellType.Empty:
                            return " ";
                        case CellType.Wall:
                            return "#";
                        case CellType.Teleport:
                            return "0";
                        case CellType.Start:
                            return "s";
                        case CellType.Finish:
                            return " ";
                        default:
                            return null;
                    }
                }

                public Cell(int Y, int X)
                {
                    this.Y = Y;
                    this.X = X;
                }
            }
        }
    }

}
