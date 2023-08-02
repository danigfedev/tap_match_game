namespace _AppAssets.Code.GameManagement.GameModesSystem
{
    public class DefaultGameMode : GameMode
    {
        public override EndGameStatus CheckEndOfGameStatus()
        {
            return EndGameStatus.KEEP_PLAYING;
        }
    }
}