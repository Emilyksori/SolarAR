using System.Collections.Generic;
using UnityEngine;

namespace SolarAR.Games.Quiz
{
    [CreateAssetMenu(
        fileName = "QuizQuestionBank",
        menuName = "SolarAR/Games/Quiz Question Bank")]
    public sealed class QuizQuestionBank : ScriptableObject
    {
        [SerializeField] private List<QuizQuestion> _questions = new();

        public IReadOnlyList<QuizQuestion> Questions => _questions;

#if UNITY_EDITOR
        public void ReplaceQuestionsForEditor(IEnumerable<QuizQuestion> questions)
        {
            _questions = questions == null ? new List<QuizQuestion>() : new List<QuizQuestion>(questions);
        }
#endif
    }
}
