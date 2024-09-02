using CosmicMemory.Helper;
using Zenject;
using UnityEngine;

namespace CosmicMemory.Controllers
{
    public sealed class LevelController : ITickable
    {
        #region Fields
        [Inject] private GameContext _gameContext;
        [Inject] private GameFieldDatas _gameFieldDatas;
        private GameField _gameField;

        private const float _coeffScores = 10.35f;
        private const float _multiplyScores = 1.8f;
        private const int _scoresBoost = 2;

        private float _time;
        private int _scores;
        #endregion

        #region Properties
        public float Time => _time;
        public int Scores => _scores;
        #endregion

        #region Contrusctor
        [Inject]
        private void Constructor(GameField gameField)
        {
            _gameField = gameField;
            _gameField.EndGame += OnEndGame;
            _gameContext.GameState = GameState.Game;
            Debug.Log("Создание уровня");
        }
        #endregion

        #region Public Methods
        public void Tick()
        {
            if (_gameContext.GameState == GameState.Game)
            {
                _time += UnityEngine.Time.deltaTime;
            }
        }

        public void UnLoad()
        {
            _gameField.EndGame -= OnEndGame;
            Debug.Log("Отписка от событий уровня");
        }

        public void CalculateScores()
        {
            _scores = CalculateNewResult(_time);
        }

        public void BoostScores()
        {
            _scores *= _scoresBoost;
        }
        #endregion

        #region Private Methods
        private void OnEndGame()
        {
            _gameContext.GameState = GameState.None;
            _gameField.EndGame -= OnEndGame;
            Debug.Log("Отписка от событий уровня");
        }

        private int CalculateNewResult(float time)
        {
            int scores = (int)Mathf.Ceil(((_gameFieldDatas.CountCards * _multiplyScores) * _coeffScores) - (time - _gameFieldDatas.CountCards));
            return scores <= 0 ? (int)(_gameFieldDatas.CountCards * _multiplyScores) : scores;
        }
        #endregion
    }
}
