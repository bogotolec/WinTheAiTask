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
            public int Height { get; set; }
            public int Width { get; }

            private List<Cell[]> RowList;

            public override string ToString()
            {
                StringBuilder SB = new StringBuilder();
                SB.Append('#', Width * 2 + 1);

                for (int i = 0; i < Height; i++)
                {
                    SB.Append('\n');
                    SB.Append('#');

                    for (int j = 0; j < Width; j++)
                    {
                        if (RowList[i][j].HasRightBorder)
                            SB.Append(" #");
                        else
                            SB.Append("  ");
                    }

                    SB.Append("\n#");

                    if (i == Height - 1)
                        SB.Append('#', Width * 2);
                    else
                        for (int j = 0; j < Width; j++)
                        {
                            if (RowList[i][j].HasBottomBorder)
                                SB.Append("##");
                            else
                                SB.Append(" #");
                        }
                }

                return SB.ToString();
            }

            private Labyrinth(int Width)
            {
                this.Height = 0;
                this.Width = Width;

                RowList = new List<Cell[]>();
            }

            public static Labyrinth Generate(Int32 seed = 0)
            {
                if (seed <= 0)
                    seed = (new Random()).Next(0, Int32.MaxValue);

                int Height = 45 + (int)(seed % 10);
                int Width = 15 + (int)(seed % 9);

                Labyrinth Resutlt = new Labyrinth(Width);

                for (int i = 0; i < Height; i++)
                {
                    Resutlt.AddRow(seed);
                }

                return Resutlt;
            }

            public void AddRow(Int64 Seed)
            {
                Seed *= Height + 1;

                if (RowList.Count != 0)
                {
                    List<int> Multiplisities = new List<int>();
                    
                    // Add row
                    RowList.Add(new Cell[Width]);
                    for (int i = 0; i < Width; i++)
                    {
                        RowList[Height][i] = new Cell(RowList[Height - 1][i]);
                        RowList[Height][i].HasRightBorder = false;
                        if (RowList[Height][i].HasBottomBorder)
                            RowList[Height][i].Multiplisity = -1;
                        else
                            Multiplisities.Add(RowList[Height][i].Multiplisity);
                        RowList[Height][i].HasBottomBorder = false;
                    }

                    // Give multiplicity
                    int Multiplisity = 0;
                    for (int i = 0; i < Width; i++)
                    {
                        if (RowList[Height][i].Multiplisity == -1)
                        {
                            while (Multiplisities.Contains(Multiplisity))
                                Multiplisity++;
                            RowList[Height][i].Multiplisity = Multiplisity;
                            Multiplisity++;
                        }
                    }

                    // Create right borders
                    for (int i = 0; i < Width - 1; i++)
                    {
                        if ((Seed / (i + 1)) % 3 == 0 && (RowList[Height][i + 1].Multiplisity != RowList[Height][i].Multiplisity))
                            RowList[Height][i + 1].Multiplisity = RowList[Height][i].Multiplisity; // Do not add border
                        else
                            RowList[Height][i].HasRightBorder = true; // Add border

                    }

                    // Create bottom borders
                    for (int i = 0; i < Width; i++)
                    {
                        if ((Seed / (i + 1)) % 4 != 0 && !IsAlone(Height, RowList[Height][i].Multiplisity))
                            RowList[Height][i].HasBottomBorder = true; // Add border
                    }
                }
                else
                {
                    // Add row and give multiplisity
                    RowList.Add(new Cell[Width]);
                    for (int i = 0; i < Width; i++)
                    {
                        RowList[Height][i] = new Cell();
                        RowList[Height][i].Multiplisity = i;
                    }

                    // Create right borders
                    for (int i = 0; i < Width - 1; i++)
                    {
                        if ((Seed / (i + 1)) % 2 == 0)
                            RowList[Height][i].HasRightBorder = true; // Add border
                        else
                            RowList[Height][i + 1].Multiplisity = RowList[Height][i].Multiplisity; // Do not add border

                    }

                    // Create bottom borders
                    for (int i = 0; i < Width; i++)
                    {
                        if ((Seed / (i + 1)) % 4 != 0 && !IsAlone(Height, RowList[Height][i].Multiplisity))
                            RowList[Height][i].HasBottomBorder = true; // Add border
                    }
                }

                RowList[Height][Width - 1].HasRightBorder = true;

                Height++;
            }

            private bool IsAlone(int Row, int Multiplisity)
            {
                int Count = 0;

                for (int i = 0; i < Width; i++)
                {
                    if (RowList[Row][i].Multiplisity == Multiplisity)
                        Count++;
                }

                if (Count == 1)
                    return true;

                return false;
            }

            private class Cell
            {
                public bool HasRightBorder { get; set; }
                public bool HasBottomBorder { get; set; }
                public int Multiplisity { get; set; }

                public Cell()
                {
                    HasRightBorder = false;
                    HasBottomBorder = false;
                }

                public Cell(Cell ToClone)
                {
                    HasBottomBorder = ToClone.HasBottomBorder;
                    HasRightBorder = ToClone.HasRightBorder;
                    Multiplisity = ToClone.Multiplisity;
                }
            }
        }
    }

}
