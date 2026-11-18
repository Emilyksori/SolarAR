using UnityEngine;

namespace SolarAR.Games.Quiz
{
    public sealed class QuizProgressStorage
    {
        public const string BestScoreKey = "SolarAR.QuizGalactico.BestScore.v1";
        public const string CompletedGamesKey = "SolarAR.QuizGalactico.CompletedGames.v1";

        public int BestScore => PlayerPrefs.GetInt(BestScoreKey, 0);
        public int CompletedGames => PlayerPrefs.GetInt(CompletedGamesKey, 0);

        public void RecordCompletedGame(int score)
        {
            if (score > BestScore)
            {
                PlayerPrefs.SetInt(BestScoreKey, score);
            }

            PlayerPrefs.SetInt(CompletedGamesKey, CompletedGames + 1);
            PlayerPrefs.Save();
        }
    }
}
