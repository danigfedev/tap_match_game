namespace _AppAssets.Code.GameManagement.GameModesSystem
{
    public static class GameModeFactory
    {
        public static GameMode CreateGameMode(GameModes gameMode)
        {
            switch (gameMode)
            {
                default:
                    return new DefaultGameMode();
            }
        }
    }
}