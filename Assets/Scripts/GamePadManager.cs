using System;
using XInputDotNetPure;

namespace Assets.Scripts
{
    public static class GamePadManager
    {
        private static bool[] usedIndices = new bool[4] { false, false, false, false };

        private static void Connect(int padIndex)
        {
            if(padIndex >= 0 && padIndex <= 4)
                usedIndices[padIndex] = true;
        }

        private static bool IsInUse(int padIndex)
        {
            if (padIndex >= 0 && padIndex <= 4)
                return usedIndices[padIndex];
            else
                throw new ArgumentOutOfRangeException();        
        }

        public static void ConnectionLost(PlayerIndex playerIndex)
        {
            int index = (int)playerIndex;
            if (index >= 0 && index <= 4)
                usedIndices[index] = false;
        }

        public static PlayerIndex GetPlayerIndex()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!IsInUse(i))
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        Connect(i);
                        return testPlayerIndex;
                    }
                }
            }

            throw new Exception("Free GamePad not found!");
        }
    }
}
