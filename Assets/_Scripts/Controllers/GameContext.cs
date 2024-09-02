using CosmicMemory.Helper;
using CosmicMemory.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        public List<Level> Levels => SaveHelper.savesData.levels;
        public int CurrentLevel => _curentLevel;
        #endregion

        #region Public Methods
        public void Initialize()
        {
            SaveHelper.LoadData();
            LoadData();
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
            return _curentLevel != _extremeLevel - 1 || _curentLevel == _extremeLevel;
        }

        public void UpdateDataLevel(float time, int scores)
        {
            SaveDataLevel(time);

            _scores = scores;

            SaveHelper.savesData.scores += _scores;
            SaveHelper.SaveData();
        }
        #endregion

        #region Private Methods
        private void LoadData()
        {
            if (SaveHelper.savesData.levels.Count == 0)
            {
                SaveHelper.savesData.InitLevels();
                Debug.Log("Данные не загружены, Созданы поумолчанию");
            }

            _extremeLevel = Levels.FirstOrDefault(e => e.LevelHard == LevelHard.Extreme).Index;
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
