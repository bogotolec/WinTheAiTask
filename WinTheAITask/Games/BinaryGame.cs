using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTheAITask.Games
{
    class BinaryGame : Game
    {
        public String Rules { get; }
        public String Name { get; }

        private static UInt64[] Numbers = new UInt64[100] { 36569531, 40871198, 45030233, 126696159, 161286131, 206670844, 537076289, 594320809, 629022428, 673060552, 741030257, 749363256, 934307817, 936583196, 963908680, 1138211628, 1156456945, 1270396029, 1364368158, 1385296047, 1452212654, 1496202587, 1527958824, 1851897183, 2026985231, 2152230874, 2176809250, 2293751961, 2307281168, 2486392660, 2538148742, 2614909023, 2736925883, 2779952455, 2822600887, 3000122895, 3168801381, 3224206524, 3388254120, 3435569773, 3518953998, 3591223750, 3612694698, 3807685049, 4041941738, 4054563827, 4114668713, 4131178986, 4146099681, 4333794787, 4334651059, 4335410594, 4435089561, 4569924375, 4570546289, 4611613368, 4840680239, 4845238672, 4918843712, 4976894967, 4996835454, 5095744505, 5114378184, 5131520833, 5551951355, 5860577691, 5884287129, 5903398790, 5925316950, 6284644342, 6332377237, 6485271444, 6534880199, 6841492014, 6872820309, 6943549769, 6993564194, 6999199967, 7097274130, 7202652264, 7390373232, 7617180002, 7643606914, 7672230699, 7847078568, 7935262341, 8360209628, 8529807087, 8591576791, 8641172368, 8778911367, 8891254132, 9245826075, 9412825535, 9413396711, 9450453547, 9547178586, 9734466755, 9836322162, 9983322927 };
        private UInt64 Number;
        private Int16 Attempts;
        

        public BinaryGame()
        {
            Rules = "You have 70 attempts to guess the number. Number is random between 0 and 18446744073709551615. After sending your number, you will get the answer - number that say how many binary digits of your number are equals with my number. If you guess the number, you win.";
            Name = "Guess the number by binary";

            Number = Numbers[(new Random()).Next(0, 99)];
            Attempts = 70;
        }

        public GameResult GetAnswer(String request)
        {
            UInt64 RequestNumber;
            if (!UInt64.TryParse(request, out RequestNumber))
                return new GameResult(State: GameState.Lose, Message: "Incorrect number! You lose");

            if (RequestNumber == Number)
                return new GameResult(State: GameState.Win, Message: "You won! CONGRATULATIONS!!!");

            UInt64 ResultNumber = 0;
            UInt64 Temp = Number;

            GameResult Result = new GameResult();

            for (int i = 0; i < 64; i++)
            {
                ResultNumber += (UInt64)(((RequestNumber & 1) ^ (Temp & 1)) == 1 ? 0 : 1);
                RequestNumber >>= 1;
                Temp >>= 1;
            }

            Attempts--;

            if (Attempts == 0)
                Result.State = GameState.Lose;
            else
                Result.State = GameState.ContinuePlaying;

            Result.Message = "Compares: " + ResultNumber.ToString() + ", Attempts lost: " + Attempts;

            return Result;
        }
    }
}
