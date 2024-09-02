using CosmicMemory.Models;
using System;
using System.Collections.Generic;

namespace CosmicMemory.Helper
{
    [System.Serializable]
    public sealed class Saves
    {
        #region Fields
        public string language = "ru";
        public int scores;
        public List<Level> levels = new();
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
        #endregion
    }
}
