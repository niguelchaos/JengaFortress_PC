using System.Collections.Generic;

public class LobbyConstants {
    public const string JoinKey = "j";
    public const string DifficultyKey = "d";
    public const string GameTypeKey = "t";

    // public static readonly List<string> GameTypes = new() { "Battle Royal", "Capture The Flag", "Creative" };
    // public static readonly List<string> Difficulties = new() { "Easy", "Medium", "Hard" };

    public static readonly LobbyData DefaultLobbyData = new LobbyData{
        Name = "QuickJoinLobby",
        MaxPlayers = 4,
        // Difficulty = 1,
        // Type = 1
    };
}