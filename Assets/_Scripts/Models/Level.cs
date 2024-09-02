using CosmicMemory.Helper;
using System;
using UnityEngine;

namespace CosmicMemory.Models
{
    [Serializable]
    public sealed class Level
    {
        #region Fields
        public int Index;
        public LevelHard LevelHard;
        public bool IsComplete;
        public bool IsAcces;
        public float TimeComplete;
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return $" {Index}; {LevelHard} ";
        }
        #endregion
    }
}
