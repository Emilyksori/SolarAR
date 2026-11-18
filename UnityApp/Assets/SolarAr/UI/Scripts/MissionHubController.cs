using SolarAR.Games.Memory;
using SolarAR.Games.PlanetOrder;
using SolarAR.Games.Quiz;
using SolarAR.Games.Shared;
using SolarAR.Games.SpaceDetective;
using UnityEngine;

namespace SolarAR.UI
{
    public sealed class MissionHubController : MonoBehaviour
    {
        [SerializeField] private GameObject _hubRoot;
        [SerializeField] private GameObject _quizRoot;
        [SerializeField] private QuizScreenController _quizController;
        [SerializeField] private GameObject _memoryRoot;
        [SerializeField] private MemoryScreenController _memoryController;
        [SerializeField] private GameObject _planetOrderRoot;
        [SerializeField] private PlanetOrderScreenController _planetOrderController;
        [SerializeField] private GameObject _spaceDetectiveRoot;
        [SerializeField] private SpaceDetectiveScreenController _spaceDetectiveController;

        private readonly MissionNavigationModel _navigation = new();

        public MissionNavigationModel Navigation => _navigation;

        public void SetPlanetTextures(Texture[] planetTextures)
        {
            _memoryController?.SetPlanetTextures(planetTextures);
            _planetOrderController?.SetPlanetTextures(planetTextures);
            _spaceDetectiveController?.SetPlanetTextures(planetTextures);
        }

        public void OnTabOpened()
        {
            ShowHub();
        }

        public void OnTabClosed()
        {
            CloseActiveGame();
            _navigation.ReturnToHub();
            ShowOnlyHub();
        }

        public void OpenQuiz()
        {
            CloseActiveGame();
            _navigation.OpenGame(MissionGameId.Quiz);
            ShowOnly(_quizRoot);
            _quizController?.OnTabOpened();
        }

        public void OpenMemory()
        {
            CloseActiveGame();
            _navigation.OpenGame(MissionGameId.Memory);
            ShowOnly(_memoryRoot);
            _memoryController?.OnGameOpened();
        }

        public void OpenPlanetOrder()
        {
            CloseActiveGame();
            _navigation.OpenGame(MissionGameId.PlanetOrder);
            ShowOnly(_planetOrderRoot);
            _planetOrderController?.OnGameOpened();
        }

        public void OpenSpaceDetective()
        {
            CloseActiveGame();
            _navigation.OpenGame(MissionGameId.SpaceDetective);
            ShowOnly(_spaceDetectiveRoot);
            _spaceDetectiveController?.OnGameOpened();
        }

        public void BackToHub()
        {
            ShowHub();
        }

        private void ShowHub()
        {
            CloseActiveGame();
            _navigation.ReturnToHub();
            ShowOnlyHub();
        }

        private void CloseActiveGame()
        {
            if (_navigation.IsActive(MissionGameId.Quiz))
            {
                _quizController?.OnTabClosed();
            }
            else if (_navigation.IsActive(MissionGameId.Memory))
            {
                _memoryController?.OnGameClosed();
            }
            else if (_navigation.IsActive(MissionGameId.PlanetOrder))
            {
                _planetOrderController?.OnGameClosed();
            }
            else if (_navigation.IsActive(MissionGameId.SpaceDetective))
            {
                _spaceDetectiveController?.OnGameClosed();
            }
        }

        private void ShowOnlyHub()
        {
            ShowOnly(_hubRoot);
        }

        private void ShowOnly(GameObject activeRoot)
        {
            SetActive(_hubRoot, activeRoot == _hubRoot);
            SetActive(_quizRoot, activeRoot == _quizRoot);
            SetActive(_memoryRoot, activeRoot == _memoryRoot);
            SetActive(_planetOrderRoot, activeRoot == _planetOrderRoot);
            SetActive(_spaceDetectiveRoot, activeRoot == _spaceDetectiveRoot);
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null && target.activeSelf != active)
            {
                target.SetActive(active);
            }
        }
    }
}
