using CosmicMemory.Controllers;
using CosmicMemory.Helper;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CosmicMemory.View
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Fields
        private Sprite _cardShirt;
        private Sprite _cardPricture;

        private GameField _gameField;
        private GameContext _gameContext;

        private Tween _tweenEnterCard;
        private Sequence _closeCard;

        [SerializeField] private Transform _cardBack;
        [SerializeField] private Transform _cardFace;
        [SerializeField] private SpriteRenderer _pictureView;
        [SerializeField] private SpriteRenderer _backSide;
        [SerializeField] private ParticleSystem _effectFade;
        [SerializeField] private AudioSource _soundEnter;
        [SerializeField] private AudioSource _soundOpen;
        [SerializeField] private AudioSource _soundMatch;

        [Space(10f)]
        [SerializeField] private float _durationSwapPos = 1f;
        [SerializeField] private float _durationOpen = 0.3f;
        [SerializeField] private float _durationClose = 0.3f;
        [SerializeField] private float _durationDestroy = 0.5f;
        [SerializeField] private float _durationEnterCard = 0.3f;
        [SerializeField] private float _durationOverCard = 0.3f;
        [SerializeField] private float _durationScaleOpenCard = 0.1f;
        [SerializeField] private float _durationScaleShowCard = 0.3f;
        [SerializeField] private float _scaleEnterCard = 1.3f;
        [SerializeField] private float _delayDestroy = 0.1f;

        [Space(10f)]
        [SerializeField] private Ease _easeSwap = Ease.InOutElastic;
        [SerializeField] private Ease _destroySwap = Ease.InOutBack;
        [SerializeField] private Ease _easeEnter = Ease.OutSine;
        [SerializeField] private Ease _easeOver = Ease.OutSine;
        [SerializeField] private Ease _easeShow = Ease.InBack;

        private const int _sortOrderSwap = 10;
        private const float _delayMatchPlay = 0.7f;
        private bool _isMove = false;
        #endregion

        #region Constructor
        [Inject]
        private void Construct(GameField gameField, GameContext gameContext)
        {
            _gameField = gameField;
            _gameContext = gameContext;
        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public bool IsOpen { get; private set; } = true;
        #endregion

        #region Public Methods
        public void SetPicture(Sprite sprite)
        {
            _cardPricture = sprite;
            _pictureView.sprite = _cardPricture;
            gameObject.name = _pictureView.sprite.name;
        }

        public void SetSuit(Sprite sprite)
        {
            _cardShirt = sprite;
            _backSide.sprite = _cardShirt;
        }

        public void ShowCard()
        {
            transform.DOScale(Vector3.one, _durationScaleShowCard).
                SetEase(_easeShow).OnComplete(() => IsOpen = false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsOpen || _isMove || _gameContext.GameState != GameState.Game) return;

            IsOpen = true;

            _gameField.AddCardOpen(this);

            //ResetScaleCard(_durationScaleOpenCard);

            DOTween.Sequence().
                Append(transform.DORotate(new Vector3(0, 90f, 0), _durationOpen)).
                AppendCallback(() =>
                {
                    _cardFace.gameObject.SetActive(true);
                    _cardBack.gameObject.SetActive(false);
                }).
                Append(transform.DORotate(Vector3.zero, _durationOpen)).
                SetLink(gameObject).SetEase(Ease.InSine);

            if (SaveHelper.savesData.isOnSounds)
                _soundOpen.Play();
        }

        public void CloseCard()
        {
            _closeCard = DOTween.Sequence().
                Append(transform.DORotate(new Vector3(0, -90f, 0), _durationClose)).
                AppendCallback(() =>
                {
                    _cardFace.gameObject.SetActive(false);
                    _cardBack.gameObject.SetActive(true);
                }).
                Append(transform.DORotate(Vector3.zero, _durationClose)).
                SetLink(gameObject).SetEase(Ease.InSine).
                AppendCallback(() =>
                {
                    IsOpen = false;
                });
        }

        public void SwapPosition(Vector3 newPos)
        {
            _backSide.sortingOrder = _sortOrderSwap;
            _isMove = true;

            if (_closeCard.IsComplete())
            {
                SetStateCardAfterSwap(newPos);
                return;
            }
            else
            {
                _closeCard.OnComplete(() =>
                {
                    SetStateCardAfterSwap(newPos);
                });
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
#if UNITY_ANDROID
            return;
#endif
            if (IsOpen || _isMove || _gameContext.GameState == GameState.Pause) return;

            _backSide.sortingOrder = _sortOrderSwap;

            _tweenEnterCard = transform.DOScale(new Vector2(_scaleEnterCard, _scaleEnterCard), _durationEnterCard)
                .SetLink(gameObject).SetEase(_easeEnter);

            if (SaveHelper.savesData.isOnSounds)
                _soundEnter.Play();
        }

        public async void OnPointerExit(PointerEventData eventData)
        {
#if UNITY_ANDROID
            return;
#endif
            if (IsOpen || _isMove || _gameContext.GameState != GameState.Game) return;

            _backSide.sortingOrder = 0;

            if (_tweenEnterCard.IsActive())
                await _tweenEnterCard.AsyncWaitForCompletion();

            _ = ResetScaleCard(_durationOverCard);
        }

        public void Delete()
        {
            _effectFade.Play();
            _cardFace.DOScale(Vector2.zero, _durationDestroy)
                .SetLink(gameObject).SetDelay(_delayDestroy)
                .SetEase(_destroySwap)
                .OnComplete(() =>
                {
                    Destroy(gameObject, _effectFade.main.startDelay.constant + _effectFade.main.duration); 
                });
        }

        public void PlaySoundMatch()
        {
            if (SaveHelper.savesData.isOnSounds)
                _soundMatch.PlayDelayed(_delayMatchPlay);
        }
        #endregion

        #region Private Methods
        private Tween ResetScaleCard(float duration)
        {
            _tweenEnterCard.Complete();

            return transform.DOScale(Vector2.one, duration)
                .SetLink(gameObject).SetEase(_easeOver);
        }

        private void SetStateCardAfterSwap(Vector3 newPos)
        {
            transform.DOMove(newPos, _durationSwapPos)
                .SetEase(_easeSwap).SetLink(gameObject)
                .OnComplete(() =>
                {
                    _backSide.sortingOrder = 0;
                    _isMove = false;
                });
        }
        #endregion
    }
}
