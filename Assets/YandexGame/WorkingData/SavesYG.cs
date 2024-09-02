using CosmicMemory.Helper;
using CosmicMemory.Models;
using System;
using System.Collections.Generic;

namespace YG
{
    [System.Serializable]
    public sealed class SavesYG
    {
        #region Fileds Yandex Game
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;
        #endregion

        #region Fields
        public int scores;
        public List<Level> levels = new();
        public List<string> buys = new();
        public string idBackground = "id001";
        public string idPictureBack = "pb001";
        public bool isOnSounds = true;
        public bool isOnMusic = true;

        [NonSerialized]
        private const int _countLevels = 31;
        #endregion

        #region Public Methods
        public void InitLevels()
        {
            for (int i = 1, j = 0; i <= _countLevels; i++)
            {
                Level level = new()
                {
                    Index = i,
                    LevelHard = (LevelHard)j,
                };
                j = (i % 10) > 0 ? j : (j + 1);
                levels.Add(level);
            }
        }

        public void AddBuy(string id)
        {
            if (buys.Contains(id)) return;
            buys.Add(id);
            YandexGame.SaveProgress();
        }

        public bool BuyContains(string id)
        {
            return buys.Contains(id);
        }
        #endregion
    }
}
