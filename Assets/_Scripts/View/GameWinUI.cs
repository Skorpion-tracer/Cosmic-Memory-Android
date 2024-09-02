﻿using CosmicMemory.Controllers;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;
using Zenject;

namespace CosmicMemory.View
{
    public sealed class GameWinUI : MonoBehaviour
    {
        #region Fields
        [SerializeField] private TextMeshProUGUI _textTime;
        [SerializeField] private TextMeshProUGUI _textScores;
        [SerializeField] private RectTransform _panelWinUI;
        [SerializeField] private Button _nextLevel;
        [SerializeField] private Button _btnReward;
        [SerializeField] private float _panelShowDuration = 0.5f;
        [SerializeField] private float _panelDelayShow = 0.7f;

        [Inject] private GameField _gameField;
        [Inject] private LevelController _levelController;
        [Inject] private GameContext _gameContext;

        private const int _idRew = 1;
        //private const float _percentReduce = 0.2f;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            _gameField.EndGame += OnEndGame;
            YandexGame.RewardVideoEvent += Rewarded;
        }

        private void OnDisable()
        {
            _gameField.EndGame -= OnEndGame;
            YandexGame.RewardVideoEvent -= Rewarded;
        }

        private void Start()
        {
            _panelWinUI.gameObject.SetActive(false);
            _btnReward.gameObject.SetActive(YandexGame.auth);
        }
        #endregion

        #region Public Methods
        public void ReloadLevel()
        {
            YandexGame.FullscreenShow();
            SceneManager.LoadSceneAsync(_gameContext.CurrentLevel, LoadSceneMode.Single);
            AudioGame.instance.PlayClick();
        }

        public void NextLevel()
        {
            _gameContext.NextLevel();
            ReloadLevel();
            AudioGame.instance.PlayClick();
        }

        public void AddReward()
        {
            YandexGame.RewVideoShow(_idRew);
            AudioGame.instance.PlayClick();
        }

        public void ExitToMainMenu()
        {
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            AudioGame.instance.PlayClick();
        }
        #endregion

        #region Private Methods
        private void OnEndGame()
        {
            AudioGame.instance.PlayWins();
            _levelController.CalculateScores();
            _panelWinUI.localScale = Vector3.zero;
            _panelWinUI.gameObject.SetActive(true);
            _panelWinUI.DOScale(Vector3.one, _panelShowDuration).SetDelay(_panelDelayShow).SetEase(Ease.OutBack);
            _nextLevel.gameObject.SetActive(_gameContext.IsLastLevel());
            TimeSpan time = TimeSpan.FromSeconds(_levelController.Time);
            _textTime.text = $"{time.Minutes:00}:{time.Seconds:00}";
            _textScores.text = _levelController.Scores.ToString();
            _gameContext.UpdateDataLevel(_levelController.Time, _levelController.Scores);
        }

        private void Rewarded(int id)
        {
            if (id == _idRew)
            {
                //float newResult = _levelController.Time * _percentReduce;
                _levelController.BoostScores();
                //TimeSpan time = TimeSpan.FromSeconds(newResult);
                //_textTime.text = $"{time.Minutes:00}:{time.Seconds:00}";
                _textScores.text = _levelController.Scores.ToString();
                _gameContext.UpdateDataLevelRewarded(_levelController.Scores);
                _btnReward.gameObject.SetActive(false);
            }
        }
        #endregion
    }
}
