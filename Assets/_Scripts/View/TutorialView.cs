using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CosmicMemory.View
{
    public sealed class TutorialView : MonoBehaviour
    {
        #region Fields
        [Inject] private GameField _gameField;

        [SerializeField] private RectTransform _learnOpenPanel;
        [SerializeField] private RectTransform _learnMatchedPanel;
        [SerializeField] private RectTransform _learnSwapPanel;

        private float _delayAnimation = 5.0f;
        private float _durationAnimation = 0.3f;
        #endregion

        #region Unity Methods
        private void Start()
        {
            ShowPanel(_learnOpenPanel);
        }

        private void OnEnable()
        {
            _gameField.OpenCard += OnLearnOpenCard;
            _gameField.CloseCard += OnCloseLearnOpenCard;
            _gameField.MatchedCards += OnLearnMatchedCards;
            _gameField.SwapCards += OnLearnSwapCards;
        }

        private void OnDisable()
        {
            _gameField.OpenCard -= OnLearnOpenCard;
            _gameField.CloseCard -= OnCloseLearnOpenCard;
            _gameField.MatchedCards -= OnLearnMatchedCards;
            _gameField.SwapCards -= OnLearnSwapCards;
        }
        #endregion

        #region Private Methods
        private void OnLearnOpenCard()
        {
            ShowPanel(_learnMatchedPanel);
            _gameField.OpenCard -= OnLearnOpenCard;
        }

        private void OnCloseLearnOpenCard()
        {
            HidePanel(_learnOpenPanel);
            _gameField.OpenCard -= OnCloseLearnOpenCard;
        }

        private void OnLearnMatchedCards()
        {
            HidePanel(_learnMatchedPanel);
            _gameField.MatchedCards -= OnLearnMatchedCards;
        }

        private void OnLearnSwapCards()
        {
            ShowPanel(_learnSwapPanel);
            HidePanel(_learnSwapPanel, _delayAnimation);
            _gameField.SwapCards -= OnLearnSwapCards;
        }

        private void ShowPanel(RectTransform panel)
        {
            panel.localScale = Vector3.zero;
            panel.gameObject.SetActive(true);
            panel.DOScale(Vector3.one, _durationAnimation).SetLink(panel.gameObject);
        }

        private void HidePanel(RectTransform panel, float delay = 0.0f)
        {
            panel.DOScale(Vector3.zero, _durationAnimation).
                SetDelay(delay).
                SetLink(panel.gameObject).
                OnComplete(() => panel.gameObject.SetActive(false));
        }
        #endregion
    }
}
