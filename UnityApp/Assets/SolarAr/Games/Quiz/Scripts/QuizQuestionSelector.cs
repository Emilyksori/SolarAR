using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarAR.Games.Quiz
{
    public sealed class QuizQuestionSelector
    {
        private static readonly QuizDifficulty[] PreferredDifficulties =
        {
            QuizDifficulty.Easy,
            QuizDifficulty.Easy,
            QuizDifficulty.Medium,
            QuizDifficulty.Medium,
            QuizDifficulty.Hard
        };

        private readonly Random _random;

        public QuizQuestionSelector(Random random = null)
        {
            _random = random ?? new Random();
        }

        public IReadOnlyList<QuizRoundQuestion> Select(
            IReadOnlyList<QuizQuestion> source,
            int count,
            out string warning)
        {
            warning = null;
            if (source == null || count <= 0)
            {
                warning = "The quiz question source is missing or the requested count is invalid.";
                return Array.Empty<QuizRoundQuestion>();
            }

            List<QuizQuestion> valid = new();
            HashSet<string> knownIds = new(StringComparer.Ordinal);
            foreach (QuizQuestion question in source)
            {
                if (question == null || !question.TryValidate(out _) || !knownIds.Add(question.Id))
                {
                    continue;
                }

                valid.Add(question);
            }

            if (valid.Count < count)
            {
                warning = $"Only {valid.Count} valid unique questions are available; {count} were requested.";
            }

            Shuffle(valid);
            List<QuizDifficulty> targets = PreferredDifficulties.Take(count).ToList();
            while (targets.Count < count)
            {
                targets.Add(QuizDifficulty.Medium);
            }

            Shuffle(targets);
            List<QuizQuestion> selected = new();
            HashSet<string> selectedIds = new(StringComparer.Ordinal);
            HashSet<string> selectedPlanets = new(StringComparer.Ordinal);

            foreach (QuizDifficulty target in targets)
            {
                QuizQuestion question = FindCandidate(
                    valid,
                    selectedIds,
                    candidate => candidate.Difficulty == target &&
                                 !selectedPlanets.Contains(candidate.PlanetKey));
                question ??= FindCandidate(
                    valid,
                    selectedIds,
                    candidate => !selectedPlanets.Contains(candidate.PlanetKey));
                question ??= FindCandidate(
                    valid,
                    selectedIds,
                    candidate => candidate.Difficulty == target);
                question ??= FindCandidate(valid, selectedIds, _ => true);

                if (question == null)
                {
                    break;
                }

                selected.Add(question);
                selectedIds.Add(question.Id);
                selectedPlanets.Add(question.PlanetKey);
            }

            if (selected.Count < count && warning == null)
            {
                warning = $"The selector safely returned {selected.Count} of {count} requested questions.";
            }

            Shuffle(selected);
            return selected
                .Select(question => QuizRoundQuestion.CreateShuffled(question, _random))
                .ToArray();
        }

        private QuizQuestion FindCandidate(
            IEnumerable<QuizQuestion> source,
            ISet<string> selectedIds,
            Func<QuizQuestion, bool> predicate)
        {
            List<QuizQuestion> candidates = source
                .Where(question => !selectedIds.Contains(question.Id) && predicate(question))
                .ToList();
            return candidates.Count == 0 ? null : candidates[_random.Next(candidates.Count)];
        }

        private void Shuffle<T>(IList<T> items)
        {
            for (int index = items.Count - 1; index > 0; index--)
            {
                int swapIndex = _random.Next(index + 1);
                (items[index], items[swapIndex]) = (items[swapIndex], items[index]);
            }
        }
    }
}
