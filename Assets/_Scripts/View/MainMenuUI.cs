using CosmicMemory.Controllers;
using CosmicMemory.Helper;
using CosmicMemory.Models;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;
using Zenject;

namespace CosmicMemory.View
{
    public sealed class MainMenuUI : MonoBehaviour
    {
        #region Fields
        [Inject] private readonly GameContext _gameContext;
        [Inject] private readonly DiContainer _container;

        [SerializeField] private RectTransform _panelMain;
        [SerializeField] private RectTransform _panelLevels;
        [SerializeField] private RectTransform _panelLoading;
        [SerializeField] private RectTransform _panelFailPurchase;

        [Space(5f)]
        [SerializeField] private RectTransform _levelEasy;
        [SerializeField] private RectTransform _levelMedium;
        [SerializeField] private RectTransform _levelHard;

        [Space(5f)]
        [SerializeField] private RectTransform[] _panelsLockLoad;

        [Space(5f)]
        [SerializeField] private TextMeshProUGUI _timeExtreme;

        [Space(5f)]
        [SerializeField] private Button _buttonStart;
        [SerializeField] private Button _buttonLevels;
        [SerializeField] private Button _buttonMedium;
        [SerializeField] private Button _buttonHard;
        [SerializeField] private Button _buttonExtreme;
        [SerializeField] private Button _buttonBuyLevels;
        [SerializeField] private Button _buttonBuyExtreme;
        [SerializeField] private Button _buttonReview;
        [SerializeField] private Button _buttonDropSave;
        [SerializeField] private Button _buttonSaveProgress;

        [Space(5f)]
        [SerializeField] private LevelUI _levelView;
        [SerializeField] private TMP_Dropdown _selectorLang;

        private readonly Dictionary<LevelHard, RectTransform> _levelsDict = new(3);
        private readonly List<LevelUI> _levelsBtns = new(33);
        private RectTransform _currentLevelsPanel;
        #endregion

        #region Unity Methods
        private void Start()
        {
            _levelsDict.Add(LevelHard.Easy, _levelEasy);
            _levelsDict.Add(LevelHard.Medium, _levelMedium);
            _levelsDict.Add(LevelHard.Hard, _levelHard);

            _panelMain.gameObject.SetActive(true);
            _panelLoading.gameObject.SetActive(false);

            _gameContext.SetDefaultLevel();

            _selectorLang.value = _selectorLang.options.IndexOf(_selectorLang.options.FirstOrDefault(e => e.text == YandexGame.lang));
            _selectorLang.onValueChanged.AddListener(SelectLang);

            EnableLevelsBtns();
            EnableExtremeBtn();
            InstantiateLevels();

            if (YandexGame.EnvironmentData.reviewCanShow)
            {
                Debug.Log($"Возможность ставить оценку: {YandexGame.EnvironmentData.reviewCanShow}");
                _buttonReview.gameObject.SetActive(true);
            }

            Debug.Log($"Имя игрока: {YandexGame.playerName}");

            _buttonSaveProgress.gameObject.SetActive(false);
            _buttonDropSave.gameObject.SetActive(false);
            _buttonDropSave.onClick.RemoveAllListeners();

            Debug.Log($"Payload: {YandexGame.EnvironmentData.payload}");
            Debug.Log($"PayloadPassword: {DebuggingModeYG.Instance.payloadPassword}");

            if (YandexGame.EnvironmentData.payload == DebuggingModeYG.Instance.payloadPassword ||
                YandexGame.EnvironmentData.payload == nameof(DropSave))
            {
                Debug.Log("РЕжИм отладки");
                _buttonSaveProgress.gameObject.SetActive(true);
                _buttonDropSave.gameObject.SetActive(true);
                _buttonDropSave.onClick.AddListener(DropSave);
            }
        }

        private void OnEnable()
        {
            YandexGame.PurchaseSuccessEvent += SuccessPurchase;
            YandexGame.PurchaseFailedEvent += FailedPurchased;
        }

        private void OnDisable()
        {
            for (int i = 0; i < _levelsBtns.Count; i++)
            {
                _levelsBtns[i].LevelStart -= OnLevelStart;
            }

            _selectorLang.onValueChanged.RemoveAllListeners();

            YandexGame.PurchaseSuccessEvent -= SuccessPurchase;
            YandexGame.PurchaseFailedEvent -= FailedPurchased;
        }
        #endregion

        #region Public Methods
        public void ContinueGame()
        {
            ShowLoadingPanel();
            YandexGame.FullscreenShow();
            SceneManager.LoadSceneAsync(_gameContext.CurrentLevel, LoadSceneMode.Single);
        }

        public void ShowPanelLevels()
        {
            _panelMain.gameObject.SetActive(false);
            _panelLevels.gameObject.SetActive(true);
            if (_currentLevelsPanel != null)
            {
                _currentLevelsPanel.gameObject.SetActive(false);
            }
        }

        public void ShowPanelMain()
        {
            _panelMain.gameObject.SetActive(true);
            _panelLevels.gameObject.SetActive(false);
        }

        public void StartExtremeLevel()
        {
            _gameContext.SetExtremeLevel();
            ContinueGame();
        }

        public void ShowLevels(RectTransform panelLevels)
        {
            _panelLevels.gameObject.SetActive(false);
            panelLevels.gameObject.SetActive(true);
            _currentLevelsPanel = panelLevels;
        }

        public void ShowPanel(RectTransform newPanel)
        {
            _panelMain.gameObject.SetActive(false);
            newPanel.gameObject.SetActive(true);
            _currentLevelsPanel = newPanel;
        }

        public void BackMain()
        {
            if (_currentLevelsPanel != null)
            {
                _currentLevelsPanel.gameObject.SetActive(false);
                _panelMain.gameObject.SetActive(true);
            }
        }

        public void CallAuth()
        {
            YandexGame.AuthDialog();
            BackMain();
        }

        public void PlaySoundClick()
        {
            AudioGame.instance.PlayClick();
        }

        public void HideBtn(Button btn)
        {
            btn.gameObject.SetActive(false);
        }
        #endregion

        #region Private Methods
        private void EnableExtremeBtn()
        {
            _buttonExtreme.interactable = false;
            _buttonBuyExtreme.gameObject.SetActive(false);
            _timeExtreme.gameObject.SetActive(false);

            if (_gameContext.Levels == null || _gameContext.Levels.Count == 0) return;

            Level extreme = _gameContext.Levels.FirstOrDefault(e => e.LevelHard == LevelHard.Extreme);

            PurchaseYG purchase = _buttonBuyExtreme.GetComponent<PurchaseYG>(); //YandexGame.PurchaseByID("2");

            if (purchase == null)
            {
                _buttonExtreme.gameObject.SetActive(false);
                _buttonBuyExtreme.gameObject.SetActive(false);
                Debug.Log($"не найден объект покупки 2 Extreme");

                _buttonExtreme.interactable = _gameContext.Levels.
                Where(e => e.LevelHard != LevelHard.Extreme).
                All(e => e.IsComplete) || extreme.IsAcces;
            }
            else
            {
                //purchase.UpdateEntries();
                Debug.Log($"найден объект покупки {purchase.data.id}|{purchase.data.title}; Consumed: {purchase.data.consumed}; Price: {purchase.data.price}");

                bool consumed;
                consumed = YandexGame.savesData.BuyContains(purchase.data.id);

#if UNITY_EDITOR
                consumed = purchase.data.consumed;
#endif

                _buttonExtreme.interactable = _gameContext.Levels.
                Where(e => e.LevelHard != LevelHard.Extreme).
                All(e => e.IsComplete) || extreme.IsAcces ||
                consumed;

                if (consumed)
                {
                    _buttonBuyExtreme.gameObject.SetActive(!consumed);
                }
                else
                {
                    _buttonBuyExtreme.gameObject.SetActive(!_buttonExtreme.interactable);
                }
            }

            if (extreme.IsComplete)
            {
                _timeExtreme.gameObject.SetActive(true);
                TimeSpan timeSpan = TimeSpan.FromSeconds(extreme.TimeComplete);
                _timeExtreme.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
            }
        }

        private void EnableLevelsBtns()
        {
            _buttonMedium.interactable = false;
            _buttonHard.interactable = false;
            _buttonBuyLevels.gameObject.SetActive(false);

            if (_gameContext.Levels == null || _gameContext.Levels.Count == 0) return;

            _buttonMedium.interactable = _gameContext.Levels.
                Where(e => e.LevelHard == LevelHard.Easy).
                All(e => e.IsComplete) || _gameContext.Levels.
                FirstOrDefault(i => i.LevelHard == LevelHard.Medium).IsAcces;

            _buttonHard.interactable = _gameContext.Levels.
                Where(e => e.LevelHard == LevelHard.Medium).
                All(e => e.IsComplete) || _gameContext.Levels.
                FirstOrDefault(i => i.LevelHard == LevelHard.Hard).IsAcces;

            PurchaseYG purchase = _buttonBuyLevels.GetComponent<PurchaseYG>(); //YandexGame.PurchaseByID("1");

            if (purchase == null)
            {
                Debug.Log($"не найден объект покупки 1 Уровни");
                _buttonBuyLevels.gameObject.SetActive(false); // Скрыть кнопку покупки
            }
            else
            {
                //purchase.UpdateEntries();
                Debug.Log($"найден объект покупки {purchase.data.id}|{purchase.data.title}; Consumed: {purchase.data.consumed}; Price: {purchase.data.price}");

                bool consumed;
                consumed = YandexGame.savesData.BuyContains(purchase.data.id);

#if UNITY_EDITOR
                consumed = purchase.data.consumed;
#endif

                if (consumed)
                {
                    _buttonBuyLevels.gameObject.SetActive(!consumed);
                }
                else
                {
                    bool isAccess = _buttonMedium.interactable && _buttonHard.interactable ? false : true;
                    _buttonBuyLevels.gameObject.SetActive(isAccess);
                }
            }
        }

        private void InstantiateLevels()
        {
            int count = _gameContext.Levels.Count - 1;

            for (int i = 0; i < count; i++)
            {
                Level level = _gameContext.Levels[i];
                bool isLockLevel = i == 0 || level.IsAcces;
                InitLevelView(_levelsDict[level.LevelHard], level.Index, isLockLevel, level.TimeComplete, level.IsComplete);
            }
        }

        private void UpdateStateLevels()
        {
            int count = _gameContext.Levels.Count - 1;

            for (int i = 0; i < count; i++)
            {
                Level level = _gameContext.Levels[i];
                LevelUI levelUI = _levelsBtns.FirstOrDefault(e => e.Index == level.Index);
                bool isLockLevel = i == 0 || level.IsAcces;
                levelUI.LockLevel(isLockLevel);
                levelUI.ShowTime(level.TimeComplete, level.IsComplete);
            }
        }

        private void InitLevelView(RectTransform parentPanel, int index, bool isLockLevel, float time, bool isComplete)
        {
            LevelUI level = _container.InstantiatePrefab(_levelView).GetComponent<LevelUI>();
            level.transform.SetParent(parentPanel, false);
            level.SetNumberLevel(index);
            level.LockLevel(isLockLevel);
            level.ShowTime(time, isComplete);
            level.LevelStart += OnLevelStart;
            _levelsBtns.Add(level);
        }

        private void OnLevelStart()
        {
            ContinueGame();
        }

        private void ShowLoadingPanel()
        {
            if (_panelsLockLoad != null)
            {
                for (int i = 0; i < _panelsLockLoad.Length; i++)
                {
                    _panelsLockLoad[i].gameObject.SetActive(false);
                }
            }

            _panelLoading.gameObject.SetActive(true);
        }

        private void SelectLang(int index)
        {
            TMP_Dropdown.OptionData option = _selectorLang.options[index];
            YandexGame.SwitchLanguage(option.text);
            YandexGame.savesData.language = YandexGame.lang;
            YandexGame.SaveProgress();
        }

        private void SuccessPurchase(string id)
        {
            switch (id)
            {
                case "1":
                    YandexGame.savesData.AddBuy("1");
                    BuyLevels();
                    break;
                case "2":
                    YandexGame.savesData.buys.Add("2");
                    BuyExtremeLevel();
                    break;
                default:
                    var purchase = YandexGame.purchases.FirstOrDefault(e => e.id == id);
                    if (purchase != null)
                    {
                        YandexGame.savesData.AddBuy(id);
                    }
                    break;
            }
        }

        private void FailedPurchased(string id)
        {
            _panelFailPurchase.gameObject.SetActive(true);
            _panelFailPurchase.DOScale(Vector3.zero, 0.4f).
                SetDelay(3f).SetLink(_panelFailPurchase.gameObject).
                OnComplete(() => _panelFailPurchase.gameObject.SetActive(false));
            Debug.Log("Не удалось совершить покупку");
        }

        private void BuyLevels()
        {
            IEnumerable<Level> levels = _gameContext.Levels.Where(e => e.LevelHard != LevelHard.Extreme);
            foreach (Level level in levels)
            {
                level.IsAcces = true;
            }

            YandexGame.SaveProgress();

            EnableLevelsBtns();
            UpdateStateLevels();
        }

        private void BuyExtremeLevel()
        {
            Level extreme = _gameContext.Levels.FirstOrDefault(e => e.LevelHard == LevelHard.Extreme);
            extreme.IsAcces = true;

            YandexGame.SaveProgress();

            EnableExtremeBtn();
        }

        private void DropSave()
        {
            var buys = YandexGame.savesData.buys;
            YandexGame.ResetSaveProgress();
            YandexGame.savesData.buys = buys;
            _buttonDropSave.gameObject.SetActive(false);
            _buttonDropSave.onClick.RemoveListener(DropSave);
        }
        #endregion
    }
}
