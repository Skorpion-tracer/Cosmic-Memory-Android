using CosmicMemory.Models;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CosmicMemory.View
{
    public sealed class SpawnCard : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Card _cardPrefab;

        [Inject] private GameFieldDatas _data;
        [Inject] private GameField _gameField;
        [Inject] private CardPictures _cardPictures;
        [Inject] private DiContainer _container;

        private float _heightCard;
        private float _widthCard;
        private float _startPointY;
        private float _startPointX;

        private float _widthAreaCards;
        private float _heightAreaCards;
        private int _countCardInRow;

        private List<CardModel> _cards;
        #endregion

        #region Unity Methods
        private void Start()
        {
            _cards = _data.CreateSuitsOfCards(_cardPictures.Pictures);

            BoxCollider2D cardCollider = _cardPrefab.GetComponent<BoxCollider2D>();
            _heightCard = cardCollider.size.y;
            _widthCard = cardCollider.size.x;

            _countCardInRow = _data.CountCards / _data.CountRows;
            _widthAreaCards = _widthCard * _countCardInRow + (_data.Margin.x * (_countCardInRow - 1));
            _heightAreaCards = _heightCard * _data.CountRows + (_data.Margin.y * (_data.CountRows - 1));
            _startPointX = 0f - ((_widthAreaCards - _widthCard) * 0.5f);
            _startPointY = (_heightAreaCards - _heightCard) * 0.5f;

            PlacementCards();
        }
        #endregion

        #region Private Methods
        private void PlacementCards()
        {
            int pic = 0;
            for (int i = 0; i < _data.CountRows; i++)
            {
                float startX = _startPointX;
                for (int j = 0; j < _countCardInRow; j++)
                {
                    Card card = _container.InstantiatePrefab(_cardPrefab, new Vector3(startX, _startPointY, 0), Quaternion.identity, null).GetComponent<Card>();
                    card.Id = _cards[pic].Suit;
                    card.SetPicture(_cards[pic].Sprite);
                    card.SetSuit(_cardPictures.PictureBack);
                    _gameField.PushCard(card);
                    card.ShowCard();
                    startX += _widthCard + _data.Margin.x;
                    pic++;
                }
                _startPointY -= _heightCard + _data.Margin.y;
            }
        }
        #endregion
    }
}
