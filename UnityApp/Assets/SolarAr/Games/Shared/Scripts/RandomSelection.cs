using System;
using System.Collections.Generic;

namespace SolarAR.Games.Shared
{
    public static class RandomSelection
    {
        public static List<T> SelectDistinct<T>(
            IReadOnlyList<T> source,
            int count,
            Random random)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (count < 0 || count > source.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            List<T> candidates = new(source.Count);
            for (int index = 0; index < source.Count; index++)
            {
                candidates.Add(source[index]);
            }

            Shuffle(candidates, random);
            return candidates.GetRange(0, count);
        }

        public static void Shuffle<T>(IList<T> values, Random random)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            for (int index = values.Count - 1; index > 0; index--)
            {
                int other = random.Next(index + 1);
                (values[index], values[other]) = (values[other], values[index]);
            }
        }
    }
}
