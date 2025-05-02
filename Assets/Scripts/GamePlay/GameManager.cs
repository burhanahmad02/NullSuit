using static MainMenuManager;

public static class GameManager
{
    public static int rows = 2;
    public static int columns = 3;
    public static bool loadSavedGame = false;
    public static Difficulty difficulty = Difficulty.Easy; // or whatever default

    public enum Difficulty
    {
        None,
        Easy,
        Medium,
        Hard
    }
}
