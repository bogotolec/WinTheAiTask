using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTheAITask.Games
{
    class TicTacToeGame : Game
    {
        public String Rules { get; }
        public String Name { get; }

        private int[,] Map;

        public TicTacToeGame()
        {
            Rules = "Standart tic tac toe game 10x10. You should put 2 numbers splitted by space - coords (x, y) of cell where you want to put 'x'. Numeration is startss from top left angle and begins with zero.";
            Name = "Tic tac toe 10x10";

            Map = new int[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Map[i, j] = 0;
                }
            }
        }

        public GameResult GetAnswer(String request)
        {
            String[] StringNumbers = request.Split();
            Int32 x, y;
            try
            {
                x = Int32.Parse(StringNumbers[0]);
                y = Int32.Parse(StringNumbers[1]);
            }
            catch (Exception ex)
            {
                return new GameResult(State: GameState.Lose, Message: "Incorrect format");
            }

            if (Map[y, x] != 0)
                return new GameResult(State: GameState.Lose, Message: "This cell is not empty");

            if (y > 9 || x > 9)
                return new GameResult(State: GameState.Lose, Message: "Impossible cell");

            Map[y, x] = 1;

            if (RowsCount(1, 5) > 0)
                return new GameResult(State: GameState.Win, Message: "GONGRATULATIONS!");

            BotTurn();

            if (RowsCount(2, 5) > 0)
                return new GameResult(State: GameState.Lose, Message: "My bot win!:\n" + MapToString());

            return new GameResult(State: GameState.ContinuePlaying, Message: MapToString());
        }

        private void BotTurn()
        {
            int[,] Priority = new int[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Priority[i, j] = 0;
                }
            }

            #region CheckWins
            // Check if bot can win
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Map[i, j] == 0)
                    {
                        Map[i, j] = 2;
                        if (RowsCount(2, 5) > 0)
                            return;
                        Map[i, j] = 0;
                    }
                }
            }

            //Check if player can win
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Map[i, j] == 0)
                    {
                        Map[i, j] = 1;
                        if (RowsCount(1, 5) > 0)
                        {
                            Map[i, j] = 2;
                            return;
                        }
                        Map[i, j] = 0;
                    }
                }
            }
            #endregion

            int MaxPriority = 0;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int RowLength = 2; RowLength < 5; RowLength++)
                    {
                        if (Map[i, j] == 0)
                        {
                            Map[i, j] = 2;
                            Priority[i, j] += RowsCount(2, RowLength) * RowLength * RowLength;

                            Map[i, j] = 1;
                            Priority[i, j] += RowsCount(1, RowLength) * RowLength * RowLength * RowLength;

                            if (MaxPriority < Priority[i, j])
                                MaxPriority = Priority[i, j];

                            Map[i, j] = 0;
                        }
                    }
                }
            }

            List<int[]> BestCells = new List<int[]>();

            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Priority[i, j] == MaxPriority)
                        BestCells.Add(new int[2] { i, j });
                }
            }

            int[] RandomCell = BestCells[(new Random()).Next(0, BestCells.Count - 1)];

            Map[RandomCell[0], RandomCell[1]] = 2;
        }

        private int RowsCount(int player, int RowLength)
        {
            Int32 Result = 0;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 11 - RowLength; j++)
                {
                    Boolean IsRow = true;

                    for (int offset = 0; offset < RowLength; offset++)
                    {
                        if (Map[i, j + offset] != player)
                            IsRow = false;
                    }

                    if (IsRow)
                        Result++;
                }
            }

            for (int i = 0; i < 11 - RowLength; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    bool IsRow = true;

                    for (int offset = 0; offset < RowLength; offset++)
                    {
                        if (Map[i + offset, j] != player)
                            IsRow = false;
                    }

                    if (IsRow)
                        Result++;
                }
            }

            for (int i = 0; i <  11 -RowLength; i++)
            {
                for (int j = 0; j <  11 - RowLength; j++)
                {
                    bool IsRow = true;

                    for (int offset = 0; offset < RowLength; offset++)
                    {
                        if (Map[i + offset, j + offset] != player)
                            IsRow = false;
                    }

                    if (IsRow)
                        Result++;
                }
            }

            for (int i = RowLength - 1; i < 10; i++)
            {
                for (int j = 0; j < 10 - RowLength; j++)
                {
                    bool IsRow = true;

                    for (int offset = 0; offset < RowLength; offset++)
                    {
                        if (Map[i - offset, j + offset] != player)
                            IsRow = false;
                    }

                    if (IsRow)
                        Result++;
                }
            }

            return Result;
        }

        private String MapToString()
        {
            StringBuilder SB = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    switch (Map[i, j])
                    {
                        case 1:
                            SB.Append("X");
                            break;
                        case 2:
                            SB.Append("O");
                            break;
                        default:
                            SB.Append(" ");
                            break;
                    }

                    if (j != 9)
                        SB.Append("|");
                }

                SB.Append('\n');

                if (i != 10)
                {
                    SB.Append('-', 19);
                    SB.Append('\n');
                }
            }

            return SB.ToString();
        }
        
    }
}
