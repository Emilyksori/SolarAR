using System;

namespace SolarAR.Games.Shared
{
    public sealed class MissionNavigationModel
    {
        public MissionGameId? ActiveGame { get; private set; }

        public bool IsAtHub => !ActiveGame.HasValue;

        public void OpenGame(MissionGameId game)
        {
            ActiveGame = game;
        }

        public void ReturnToHub()
        {
            ActiveGame = null;
        }

        public bool IsActive(MissionGameId game)
        {
            return ActiveGame == game;
        }
    }
}
