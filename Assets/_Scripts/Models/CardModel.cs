using UnityEngine;

namespace CosmicMemory.Models
{
    public struct CardModel
    {
        #region Properties
        public int Suit { get; set; }
        public Sprite Sprite { get; set; }
        #endregion

        #region Public Methods
        public override string ToString() => $"Suit: {Suit}; Pic: {Sprite.name}";
        #endregion
    }
}
