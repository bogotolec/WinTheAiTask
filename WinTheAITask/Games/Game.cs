using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTheAITask.Games
{
    interface Game
    {
        string Rules { get; }
        string Name { get; }
        GameResult GetAnswer(string Request);
    }
}
