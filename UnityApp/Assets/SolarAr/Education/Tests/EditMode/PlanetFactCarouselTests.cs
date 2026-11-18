using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;

namespace SolarAR.Education.Tests
{
    public sealed class PlanetFactCarouselTests
    {
        [Test]
        public void SetFacts_SortsByStableOrderAndStartsAtFirstFact()
        {
            PlanetFactCarousel carousel = new();
            carousel.SetFacts(new List<PlanetFact>
            {
                CreateFact("third", 3),
                CreateFact("first", 1),
                CreateFact("second", 2)
            });

            Assert.That(carousel.Count, Is.EqualTo(3));
            Assert.That(carousel.Current.Id, Is.EqualTo("first"));
            Assert.That(carousel.ProgressText, Is.EqualTo("1/3"));
        }

        [Test]
        public void Navigation_StopsAtEndsAndUpdatesProgress()
        {
            PlanetFactCarousel carousel = new();
            carousel.SetFacts(new[] { CreateFact("first", 1), CreateFact("second", 2) });

            Assert.That(carousel.MovePrevious(), Is.False);
            Assert.That(carousel.MoveNext(), Is.True);
            Assert.That(carousel.Current.Id, Is.EqualTo("second"));
            Assert.That(carousel.ProgressText, Is.EqualTo("2/2"));
            Assert.That(carousel.MoveNext(), Is.False);
            Assert.That(carousel.MovePrevious(), Is.True);
        }

        [Test]
        public void EmptyCarousel_IsSafeAndReportsZeroProgress()
        {
            PlanetFactCarousel carousel = new();
            carousel.SetFacts(null);

            Assert.That(carousel.HasFacts, Is.False);
            Assert.That(carousel.Current, Is.Null);
            Assert.That(carousel.ProgressText, Is.EqualTo("0/0"));
            Assert.That(carousel.MoveNext(), Is.False);
        }

        [Test]
        public void PlanetFactAssets_ContainEightFactsEachAndFiftySixInTotal()
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:PlanetFacts",
                new[] { "Assets/SolarAr/Education/Facts" });
            HashSet<string> factIds = new();
            HashSet<string> planetKeys = new();
            int total = 0;

            Assert.That(guids, Has.Length.EqualTo(7));
            foreach (string guid in guids)
            {
                PlanetFacts facts = AssetDatabase.LoadAssetAtPath<PlanetFacts>(
                    AssetDatabase.GUIDToAssetPath(guid));
                Assert.That(facts, Is.Not.Null);
                Assert.That(planetKeys.Add(facts.PlanetKey), Is.True);
                Assert.That(facts.Facts, Has.Count.EqualTo(8));

                for (int index = 0; index < facts.Facts.Count; index++)
                {
                    PlanetFact fact = facts.Facts[index];
                    Assert.That(fact, Is.Not.Null);
                    Assert.That(fact.Id, Is.Not.Empty);
                    Assert.That(fact.Title, Is.Not.Empty);
                    Assert.That(fact.Description, Is.Not.Empty);
                    Assert.That(fact.Order, Is.EqualTo(index + 1));
                    Assert.That(factIds.Add(fact.Id), Is.True, $"Duplicate id: {fact.Id}");
                    total++;
                }
            }

            Assert.That(total, Is.EqualTo(56));
        }

        private static PlanetFact CreateFact(string id, int order)
        {
            return new PlanetFact(
                id,
                $"Title {order}",
                $"Description {order}",
                PlanetFactCategory.Observation,
                order);
        }
    }
}
