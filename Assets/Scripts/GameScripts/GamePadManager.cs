using System;
using System.Collections.Generic;
using XInputDotNetPure;

namespace Assets.Scripts
{
    public static class GamePadManager
    {
        public static event EventHandler OnPlayerCountChanged;

        private static bool[] usedIndices = new bool[4] { false, false, false, false };

        public static void Connect(int padIndex)
        {
            if (padIndex >= 0 && padIndex <= 4)
            {
                usedIndices[padIndex] = true;
                if(OnPlayerCountChanged != null)
                {
                    OnPlayerCountChanged(null, EventArgs.Empty);
                }
            }
        }

        public static bool IsInUse(int padIndex)
        {
            if (padIndex >= 0 && padIndex <= 4)
                return usedIndices[padIndex];
            else
                throw new ArgumentOutOfRangeException();        
        }

        public static void Disconnect(PlayerIndex playerIndex)
        {
            int index = (int)playerIndex;
            if (index >= 0 && index <= 4)
            {
                usedIndices[index] = false;
                if (OnPlayerCountChanged != null)
                    OnPlayerCountChanged(null, EventArgs.Empty);
            }
        }

        public static bool GetPlayerIndex(out PlayerIndex playerIndex)
        {
            playerIndex = PlayerIndex.One;
            for (int i = 0; i < 4; i++)
            {
                if (!IsInUse(i))
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        Connect(i);
                        usedIndices[i] = true;
                        playerIndex = testPlayerIndex;
                        return true;
                    }
                }
            }
            return false;
        }

        public static PlayerIndex[] GetFreeControllers()
        {
            List<PlayerIndex> availibleControllers = new List<PlayerIndex>();
            for (int i = 0; i < 4; i++)
            {
                if (!IsInUse(i))
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        availibleControllers.Add(testPlayerIndex);
                    }
                }
            }

            return availibleControllers.ToArray();
        }

        public static void DisconnectAll()
        {
            Disconnect(PlayerIndex.One);
            Disconnect(PlayerIndex.Two);
            Disconnect(PlayerIndex.Three);
            Disconnect(PlayerIndex.Four);
        }
    }
}
