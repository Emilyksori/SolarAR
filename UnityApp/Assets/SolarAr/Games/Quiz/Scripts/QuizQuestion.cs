using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolarAR.Games.Quiz
{
    [Serializable]
    public sealed class QuizQuestion
    {
        [SerializeField] private string _id;
        [SerializeField] private string _planetKey;
        [SerializeField] private QuizDifficulty _difficulty;
        [SerializeField, TextArea(2, 4)] private string _prompt;
        [SerializeField] private List<string> _alternatives = new();
        [SerializeField] private int _correctAlternativeIndex;
        [SerializeField, TextArea(2, 4)] private string _explanation;

        public QuizQuestion(
            string id,
            string planetKey,
            QuizDifficulty difficulty,
            string prompt,
            IReadOnlyList<string> alternatives,
            int correctAlternativeIndex,
            string explanation)
        {
            _id = id;
            _planetKey = planetKey;
            _difficulty = difficulty;
            _prompt = prompt;
            _alternatives = alternatives == null ? new List<string>() : new List<string>(alternatives);
            _correctAlternativeIndex = correctAlternativeIndex;
            _explanation = explanation;
        }

        public string Id => _id;
        public string PlanetKey => _planetKey;
        public QuizDifficulty Difficulty => _difficulty;
        public string Prompt => _prompt;
        public IReadOnlyList<string> Alternatives => _alternatives;
        public int CorrectAlternativeIndex => _correctAlternativeIndex;
        public string Explanation => _explanation;

        public bool TryValidate(out string error)
        {
            if (string.IsNullOrWhiteSpace(_id) ||
                string.IsNullOrWhiteSpace(_planetKey) ||
                string.IsNullOrWhiteSpace(_prompt) ||
                string.IsNullOrWhiteSpace(_explanation))
            {
                error = "Question identification, planet, prompt and explanation are required.";
                return false;
            }

            if (_alternatives == null || _alternatives.Count != 3)
            {
                error = $"Question '{_id}' must contain exactly three alternatives.";
                return false;
            }

            if (_correctAlternativeIndex < 0 || _correctAlternativeIndex >= _alternatives.Count)
            {
                error = $"Question '{_id}' has an invalid correct alternative index.";
                return false;
            }

            HashSet<string> alternatives = new(StringComparer.OrdinalIgnoreCase);
            foreach (string alternative in _alternatives)
            {
                string normalized = alternative?.Trim();
                if (string.IsNullOrEmpty(normalized) || !alternatives.Add(normalized))
                {
                    error = $"Question '{_id}' contains an empty or duplicate alternative.";
                    return false;
                }
            }

            error = null;
            return true;
        }
    }
}
