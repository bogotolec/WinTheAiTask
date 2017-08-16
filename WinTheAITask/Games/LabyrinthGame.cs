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
            this.Rules += "\n\tTeleport = 0";
            this.Rules += "\nType 'Start' to begin";

            GameLabyrinth = Labyrinth.Generate();
        }

        public GameResult GetAnswer(string Request)
        {
            if (Request.ToLower() == "start")
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
            public int Heigth { get; }
            public int Width { get; }

            private Cell[,] Map;

            public override string ToString()
            {
                StringBuilder SB = new StringBuilder();
                for (int i = 0; i < Heigth; i++)
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

                this.Heigth = Heigth;
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
                Map[Heigth - 2, Width - 2].Type = Cell.CellType.Start;
            }

            public static Labyrinth Generate(UInt32 seed = 0)
            {
                if (seed == 0)
                    seed = (UInt32)(new Random()).Next(0, Int32.MaxValue);

                int Height = 90 + (int)(seed % 10);
                int Width = 30 + (int)(seed % 9);

                Labyrinth Resutlt = new Labyrinth(Height, Width);

                int Modifer = 0;
                while (!Resutlt.IsPassable)
                {
                    Resutlt.DeleteNearestWall(((seed / Modifer) % ((Resutlt.Heigth - 2) / 2) + 1) * 2,
                                               ((seed / Modifer) % ((Resutlt.Width - 2) / 2) + 1) * 2);
                    Modifer++;
                }

                return Resutlt;
            }

            public void DeleteNearestWall(long x, long y)
            {
                Queue<Cell> queue = new Queue<Cell>();

                Cell NowCell = Map[x, y];
                
                while (NowCell.Type != Cell.CellType.Wall)
                {
                    if (NowCell.X + 2 < Heigth - 1)
                        queue.Enqueue(Map[NowCell.X + 2, NowCell.Y]);
                    if (NowCell.X - 2 > 0)
                        queue.Enqueue(Map[NowCell.X - 2, NowCell.Y]);
                    if (NowCell.Y + 2 < Width)
                        queue.Enqueue(Map[NowCell.X, NowCell.Y + 2]);
                    if (NowCell.Y - 2 > 0)
                        queue.Enqueue(Map[NowCell.X, NowCell.Y - 2]);

                    if (queue.Count == 0)
                        return;

                    NowCell = queue.Dequeue();
                }

                NowCell.Type = Cell.CellType.Empty;
            }

            public bool IsPassable
            {
                get
                {
                    int[,] IntMap = new int[Heigth, Width];

                    for (int i = 0; i < Heigth; i++)
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

                        for (int i = 1; i < Heigth - 1; i++)
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

                    if (IntMap[Heigth - 2, Width - 2] != 0)
                        return true;
                    return false;
                }
            }

            private class Cell
            {
                public enum CellType { Empty, Wall, Teleport, Start, Finish }

                public CellType Type { get; set; }
                public int Y { get; }
                public int X { get; }

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

                public Cell(CellType Type, int Y, int X)
                {
                    this.Type = Type;
                    this.Y = Y;
                    this.X = X;
                }
            }
        }
    }

}
