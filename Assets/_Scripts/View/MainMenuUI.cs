using CosmicMemory.Controllers;
using CosmicMemory.Helper;
using CosmicMemory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

        [Space(5f)]
        [SerializeField] private RectTransform _levelEasy;
        [SerializeField] private RectTransform _levelMedium;
        [SerializeField] private RectTransform _levelHard;

        [Space(5f)]
        [SerializeField] private RectTransform[] _panelsLockLoad;

        [Space(5f)]
        [SerializeField] private TextMeshProUGUI _timeExtreme;
        [SerializeField] private TextMeshProUGUI _scores;

        [Space(5f)]
        [SerializeField] private Button _buttonStart;
        [SerializeField] private Button _buttonLevels;
        [SerializeField] private Button _buttonMedium;
        [SerializeField] private Button _buttonHard;
        [SerializeField] private Button _buttonExtreme;

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
            HelperSizeCamera.ResizeCamera(Camera.main);

            _levelsDict.Add(LevelHard.Easy, _levelEasy);
            _levelsDict.Add(LevelHard.Medium, _levelMedium);
            _levelsDict.Add(LevelHard.Hard, _levelHard);

            _panelMain.gameObject.SetActive(true);
            _panelLoading.gameObject.SetActive(false);

            _gameContext.SetDefaultLevel();

            _selectorLang.value = _selectorLang.options.IndexOf(_selectorLang.options.FirstOrDefault(e => e.text == SaveHelper.savesData.language));
            _selectorLang.onValueChanged.AddListener(SelectLang);

            _scores.text = SaveHelper.savesData.scores.ToString();

            EnableLevelsBtns();
            EnableExtremeBtn();
            InstantiateLevels();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _levelsBtns.Count; i++)
            {
                _levelsBtns[i].LevelStart -= OnLevelStart;
            }

            _selectorLang.onValueChanged.RemoveAllListeners();
        }
        #endregion

        #region Public Methods
        public void ContinueGame()
        {
            ShowLoadingPanel();
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

        public void ExitGame()
        {
            Application.Quit();
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
            _timeExtreme.gameObject.SetActive(false);

            if (_gameContext.Levels == null || _gameContext.Levels.Count == 0) return;

            Level extreme = _gameContext.Levels.FirstOrDefault(e => e.LevelHard == LevelHard.Extreme);

            _buttonExtreme.interactable = _gameContext.Levels.
            Where(e => e.LevelHard != LevelHard.Extreme).
            All(e => e.IsComplete) || extreme.IsAcces;

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

            if (_gameContext.Levels == null || _gameContext.Levels.Count == 0) return;

            _buttonMedium.interactable = _gameContext.Levels.
                Where(e => e.LevelHard == LevelHard.Easy).
                All(e => e.IsComplete) || _gameContext.Levels.
                FirstOrDefault(i => i.LevelHard == LevelHard.Medium).IsAcces;

            _buttonHard.interactable = _gameContext.Levels.
                Where(e => e.LevelHard == LevelHard.Medium).
                All(e => e.IsComplete) || _gameContext.Levels.
                FirstOrDefault(i => i.LevelHard == LevelHard.Hard).IsAcces;
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

            // TODO настроить смену языка в игре
        }
        #endregion
    }
}
