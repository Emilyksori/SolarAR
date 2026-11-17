using NUnit.Framework;

namespace SolarAR.Games.Shared.Tests
{
    public sealed class MissionNavigationModelTests
    {
        [TestCase(MissionGameId.Quiz)]
        [TestCase(MissionGameId.Memory)]
        [TestCase(MissionGameId.PlanetOrder)]
        [TestCase(MissionGameId.SpaceDetective)]
        public void EveryGame_CanOpenAndReturnToHub(MissionGameId game)
        {
            MissionNavigationModel navigation = new();

            navigation.OpenGame(game);
            Assert.That(navigation.IsActive(game), Is.True);
            Assert.That(navigation.IsAtHub, Is.False);

            navigation.ReturnToHub();
            Assert.That(navigation.IsAtHub, Is.True);
            Assert.That(navigation.ActiveGame, Is.Null);
        }

        [Test]
        public void OpeningAnotherGame_DoesNotKeepPreviousGameActive()
        {
            MissionNavigationModel navigation = new();
            navigation.OpenGame(MissionGameId.Memory);

            navigation.OpenGame(MissionGameId.SpaceDetective);

            Assert.That(navigation.IsActive(MissionGameId.Memory), Is.False);
            Assert.That(navigation.IsActive(MissionGameId.SpaceDetective), Is.True);
        }
    }
}
