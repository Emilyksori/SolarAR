using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolarAR.Games.Quiz
{
    public sealed class QuizAnswerButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Graphic _background;
        [SerializeField] private QuizScreenController _controller;
        [SerializeField] private int _answerIndex;

        private Color _defaultColor;

        private void Awake()
        {
            if (_background != null)
            {
                _defaultColor = _background.color;
            }
        }

        public void HandlePressed()
        {
            _controller?.SelectAnswer(_answerIndex);
        }

        public void Prepare(string label)
        {
            if (_label != null)
            {
                _label.text = label;
            }

            if (_background != null)
            {
                _background.color = _defaultColor;
            }

            if (_button != null)
            {
                _button.interactable = true;
            }
        }

        public void ShowResult(bool isCorrect, bool wasSelected, Color correctColor, Color selectedColor)
        {
            if (_background != null)
            {
                if (isCorrect)
                {
                    _background.color = correctColor;
                }
                else if (wasSelected)
                {
                    _background.color = selectedColor;
                }
            }

            if (_button != null)
            {
                _button.interactable = false;
            }
        }
    }
}
