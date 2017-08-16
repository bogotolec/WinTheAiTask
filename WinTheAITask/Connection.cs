using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WinTheAITask.Games;

namespace WinTheAITask
{
    class Connection
    {
        private Socket Socket;
        private List<Game> GameList = new List<Game>();

        public Connection(Socket socket)
        {
            Socket = socket;

            GameList.Add(new BinaryGame());
            GameList.Add(new TicTacToeGame());
        }

        public void Start()
        {
            while (GameList.Count > 0)
            {
                int ans = -1;
                string rawans = "";

                do
                {
                    SendData(Question());
                    rawans = GetData();
                }
                while (!int.TryParse(rawans, out ans) && (ans < 0 || ans >= GameList.Count));

                Game NowGame = GameList[ans];

                SendData(NowGame.Rules);

                GameResult Result = new GameResult();

                do
                {
                    Result = NowGame.GetAnswer(GetData());
                    SendData(Result.Message);

                    if (Result.State == GameState.Lose)
                    {
                        Socket.Close();
                        return;
                    }
                }
                while (Result.State != GameState.Win);

                GameList.Remove(NowGame);
            }

            Console.WriteLine("Succes");

            SendData("CRG{BigTaskSolved}");
        }

        private void SendData(String data)
        {
            if (!data.EndsWith("\n"))
                data += "\n";

            Byte[] Buffer = Encoding.UTF8.GetBytes(data);
            Socket.Send(Buffer);
        }

        private string Question()
        {
            StringBuilder SB = new StringBuilder();
            SB.Append("Select the game:\n");

            int i = 0;
            foreach (var game in GameList)
            {
                SB.Append("\t" + i.ToString() + ": " + game.Name + "\n");
                i++;
            }

            return SB.ToString();
        }

        private String GetData()
        {
            while (Socket.Available == 0)
                Thread.Sleep(100);

            Byte[] Buffer = new Byte[Socket.Available];
            Socket.Receive(Buffer);

            return Encoding.UTF8.GetString(Buffer);
        }
    }
}
