using CosmicMemory.Controllers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicMemory.View
{
    [RequireComponent(typeof(Button))]
    public sealed class LevelUI : MonoBehaviour
    {
        #region Fields
        [Inject] private GameContext _gameContext;

        [SerializeField] private TextMeshProUGUI _numberLevel;
        [SerializeField] private TextMeshProUGUI _timeComplete;

        private Button _btnStartLevel;

        private int _indexLevel;
        #endregion

        #region Events
        public event Action LevelStart;
        #endregion

        #region Properties
        public int Index => _indexLevel;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            _btnStartLevel ??= GetComponent<Button>();
        }
        #endregion

        #region Public Methods
        public void SetNumberLevel(int number)
        {
            _indexLevel = number;
            _numberLevel.text = _indexLevel.ToString();
        }

        public void ShowTime(float time, bool isShow)
        {
            _timeComplete.gameObject.SetActive(isShow);
            if (!isShow) return;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            _timeComplete.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        public void StartLevel()
        {
            _gameContext.SetLevel(_indexLevel);
            LevelStart.Invoke();
            AudioGame.instance.PlayClick();
        }

        public void LockLevel(bool isLock)
        {
            _btnStartLevel.interactable = isLock;
        }
        #endregion
    }
}
