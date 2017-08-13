using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTheAITask
{
    public enum GameState { Win, Lose, ContinuePlaying } 

    class GameResult
    {
        public GameState State;
        public String Message;

        public GameResult(GameState State = GameState.ContinuePlaying,  String Message = "")
        {
            this.State = State;
            this.Message = Message;
        }
    }
}
