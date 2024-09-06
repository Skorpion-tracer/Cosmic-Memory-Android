using CosmicMemory.View;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CosmicMemory
{
    public sealed class GameField
    {
        #region Fields
        [Inject] private GameFieldDatas _data;

        private List<Card> _cards = new(30); 
        private List<Card> _cardsOpen = new(3);

        private const int _maxClick = 2;
        private const int _firstClick = 1;
        private int _countClick;
        #endregion

        #region Events
        public event Action OpenCard;
        public event Action CloseCard;
        public event Action MatchedCards;
        public event Action SwapCards;
        public event Action EndGame;
        #endregion

        #region Properties
        public List<Card> Cards => _cards;
        #endregion

        #region Public Methods
        public void AddCardOpen(Card card)
        {
            _cardsOpen.Add(card);
            if (_countClick < _maxClick)
            {
                _countClick++;
                if (_countClick == _firstClick)
                {
                    OpenCard?.Invoke();
                }
                else if (_countClick == _maxClick)
                {
                    CloseCard?.Invoke();
                }
            }

            if (_cardsOpen.Count == _data.CountDropCards)
            {
                int id = _cardsOpen[0].Id;
                if (_cardsOpen.All(e => e.Id == id))
                {
                    MatchedCards?.Invoke();
                    _cardsOpen[0].PlaySoundMatch();
                    //AudioGame.instance.PlaySoundMatchCards();
                    foreach (Card cardOpen in _cardsOpen)
                    {
                        _cards.Remove(cardOpen);
                        cardOpen.Delete();
                    }
                    _cardsOpen.Clear();
                    if (_cards.Count == 0)
                    {
                        EndGame?.Invoke();
                        Debug.Log("Вы победили!!!");
                    }  
                }
                return;
            }
            
            if (_cardsOpen.Count > _data.CountDropCards)
            {
                List<Card> dropCards = new(_cardsOpen.Take(_data.CountDropCards));
                foreach (Card cardOpen in dropCards)
                {
                    cardOpen.CloseCard();
                }

                if (_data.IsSwap())
                {
                    SwapCards?.Invoke();
                    FindToSwapCards(dropCards);
                }

                _cardsOpen.RemoveRange(0, _data.CountDropCards);
            }
        }

        public void PushCard(Card card)
        {
            _cards.Add(card);
        }
        #endregion

        #region Private Methods
        private void FindToSwapCards(List<Card> cards)
        {
            Card firstCard = cards[0];
            Card secondCard = cards.FirstOrDefault(e => e.Id != firstCard.Id);
            firstCard.SwapPosition(secondCard.gameObject.transform.position);
            secondCard.SwapPosition(firstCard.gameObject.transform.position);
        }
        #endregion
    }
}
