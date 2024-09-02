using CosmicMemory.Controllers;
using CosmicMemory.Helper;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace CosmicMemory.View
{
    public sealed class GameplayUI : MonoBehaviour
    {
        #region Fields
        [Inject] private GameContext _gameContext;
        [Inject] private GameField _gameField;
        [Inject] private LevelController _levelController;

        private Button[] _btnsPause;

        [SerializeField] private Button _btnPause;
        [SerializeField] private RectTransform _panelPause;
        [SerializeField] private RectTransform _panelLoading;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            _gameField.EndGame += OnEndGame;
        }

        private void OnDisable()
        {
            _gameField.EndGame -= OnEndGame;
        }

        private void Start()
        {
            _btnsPause = _panelPause.gameObject.GetComponentsInChildren<Button>();

            _panelPause.gameObject.SetActive(false);
            _panelPause.localScale = Vector3.zero;
            _panelLoading.DOScale(Vector3.zero, 0.3f).OnComplete(() => _panelLoading.gameObject.SetActive(false));
        }
        #endregion

        #region Public Methods
        public void Pause()
        {
            _gameContext.GameState = GameState.Pause;
            _btnPause.gameObject.SetActive(false);
            _panelPause.gameObject.SetActive(true);
            _panelPause.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack).SetLink(_panelPause.gameObject);
            OnOffBtns(true);
            AudioGame.instance.PlayClick();
        }

        public void Play()
        {
            _gameContext.GameState = GameState.Game;
            _btnPause.gameObject.SetActive(true);
            _panelPause.DOScale(Vector3.zero, 0.4f).
                SetEase(Ease.InBack).
                SetLink(_panelPause.gameObject).
                OnComplete(() => _panelPause.gameObject.SetActive(false));
            OnOffBtns(false);
            AudioGame.instance.PlayClick();
        }

        public void Reload()
        {
            _levelController.UnLoad();
            SceneManager.LoadSceneAsync(_gameContext.CurrentLevel, LoadSceneMode.Single);
            OnOffBtns(false);
            AudioGame.instance.PlayClick();
        }

        public void ExitInMenu()
        {
            AudioGame.instance.PlayClick();
            _gameContext.GameState = GameState.None;
            _levelController.UnLoad();
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            OnOffBtns(false);
        }
        #endregion

        #region Private Methods
        private void OnEndGame()
        {
            gameObject.SetActive(false);
        }

        private void OnOffBtns(bool onOff)
        {
            foreach (Button btn in _btnsPause)
            {
                btn.interactable = onOff;
            }
        }
        #endregion
    }
}
