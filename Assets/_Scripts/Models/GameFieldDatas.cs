using CosmicMemory.Models;
using System.Collections.Generic;
using UnityEngine;

namespace CosmicMemory
{
    [CreateAssetMenu(fileName = "Game Data", menuName = "GameFiled/Game Field Data", order = 51)]
    public sealed class GameFieldDatas : ScriptableObject
    {
        #region Fields
        private const int _minCard = 6;
        private const int _maxCard = 60;

        [SerializeField, Range(6, 60)] private int _countCards = 6;
        [SerializeField, Range(2, 3)] private int _countDropCards = 2;
        [SerializeField, Range(2, 5)] private int _countRows = 2;
        [SerializeField, Range(0, 100)] private int _swap = 0;

        [SerializeField] private Vector2 _margin = new(0.2f, 0.2f);
        #endregion

        #region Properties
        public int CountCards => _countCards;
        public int CountDropCards => _countDropCards;
        public int CountRows => _countRows;
        public int Swap => _swap;
        public Vector2 Margin => _margin;
        #endregion

        #region Unity Methods
        private void OnValidate()
        {
            if (_countCards < _minCard) _countCards = _minCard;
            if (_countCards > _maxCard) _countCards = _maxCard;
            while (_countCards % _countDropCards > 0) _countCards++;
            while (_countCards % _countRows > 0) _countRows--;
        }
        #endregion

        #region Public Methods
        public List<CardModel> CreateSuitsOfCards(Sprite[] pictures)
        {
            List<Sprite> sprites = new(pictures);
            List<CardModel> newCards = new(_countCards);

            int index = Random.Range(0, sprites.Count);
            Sprite sprite = sprites[index];
            sprites.RemoveAt(index);

            for (int i = 0, j = 0; i < _countCards; i++)
            {
                int tmpSuit = j;
                j = Mathf.FloorToInt(i / _countDropCards);
                if (tmpSuit != j)
                {
                    index = Random.Range(0, sprites.Count);
                    sprite = sprites[index];
                    sprites.RemoveAt(index);
                }
                CardModel cModel = new() { Suit = j, Sprite = sprite };
                newCards.Add(cModel);
            }

            SwapCards(newCards);

            return newCards;
        }

        public bool IsSwap()
        {
            return Random.Range(1, 99) < _swap;
        }
        #endregion

        #region Private Methods
        private void SwapCards(List<CardModel> cards)
        {
            for (int i = _countCards - 1; i > 0; i--)
            {
                int swap = Mathf.FloorToInt(Random.Range(0f, 1f) * i);
                (cards[i], cards[swap]) = (cards[swap], cards[i]);
            }
        }
        #endregion
    }
}
