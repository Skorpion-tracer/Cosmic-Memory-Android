using CosmicMemory.Helper;
using CosmicMemory.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YG;
using Zenject;

namespace CosmicMemory.Controllers
{
    public sealed class GameContext : IInitializable
    {
        #region Fields
        private int _curentLevel = 1;
        private int _extremeLevel;
        private int _scores;
        #endregion

        #region Properties
        public GameState GameState { get; set; } = GameState.None;
        public List<Level> Levels => YandexGame.savesData.levels;
        public int CurrentLevel => _curentLevel;
        #endregion

        #region Public Methods
        public void Initialize()
        {
            YandexGame.GetDataEvent += LoadData;
        }

        public void SetDefaultLevel()
        {
            Level[] levels = Levels.Where(e => e.LevelHard != LevelHard.Extreme).ToArray();
            if (levels.All(e => e.IsComplete))
            {
                _curentLevel = levels[^1].Index;
            }
            else
            {
                _curentLevel = levels.FirstOrDefault(e => !e.IsComplete).Index;
            }
        }

        public void SetLevel(int index)
        {
            if (Levels.Any(e => e.Index == index))
            {
                _curentLevel = index;
            }
            else
            {
                _curentLevel = 1;
            }
        }

        public void SetExtremeLevel()
        {
            _curentLevel = _extremeLevel;
        }

        public void NextLevel()
        {
            if (_curentLevel != _extremeLevel)
            {
                _curentLevel++;
            }
        }

        public bool IsLastLevel()
        {
            return _curentLevel != _extremeLevel - 1;
        }

        public void UpdateDataLevel(float time, int scores)
        {
            SaveDataLevel(time);

            _scores = scores;
            YandexGame.savesData.scores += _scores;

            YandexGame.SaveProgress();

            if (YandexGame.auth)
            {
                YandexGame.NewLeaderboardScores("Scores", YandexGame.savesData.scores);
            }
        }

        public void UpdateDataLevelRewarded(int scores)
        {
            //SaveDataLevel(time);

            YandexGame.savesData.scores -= _scores;
            YandexGame.savesData.scores += scores;

            YandexGame.SaveProgress();

            YandexGame.NewLeaderboardScores("Scores", YandexGame.savesData.scores);
        }
        #endregion

        #region Private Methods
        private void LoadData()
        {
            if (YandexGame.savesData.levels.Count == 0)
            {
                YandexGame.savesData.InitLevels();
                Debug.Log("Данные не загружены, Созданы поумолчанию");
            }

            _extremeLevel = Levels.FirstOrDefault(e => e.LevelHard == LevelHard.Extreme).Index;

            StringBuilder data = new();

            foreach (Level level in Levels)
            {
                data.AppendLine($"Level: {level.Index} {level.LevelHard}; Access: {level.IsAcces}; Complete: {level.IsComplete};");
            }

            Debug.Log(data.ToString());
            Debug.Log($"Осуществленные покупки: {string.Join("   ;", YandexGame.savesData.buys)}");

            YandexGame.GetDataEvent -= LoadData;
        }

        private void SaveDataLevel(float time)
        {
            Level level = Levels.FirstOrDefault(e => e.Index == _curentLevel);

            if (level.TimeComplete == 0.0f)
            {
                level.TimeComplete = time;
            }
            else
            {
                level.TimeComplete = time < level.TimeComplete ? time : level.TimeComplete;
            }

            level.IsComplete = true;

            if (level.LevelHard == LevelHard.Extreme) return;

            Level nextLevel = Levels.FirstOrDefault(e => e.Index == (_curentLevel + 1));
            nextLevel.IsAcces = true;
        }
        #endregion
    }
}
