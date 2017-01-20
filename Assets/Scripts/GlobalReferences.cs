using System.Collections.Generic;

public static class GlobalReferences
{
    public enum GameState
    {
        Play,
        Pause,
        ConnectionLost
    }

    public static List<PlayerState> PlayerStates = new List<PlayerState>();

    public static GameState CurrentGameState = GameState.Play;
}
