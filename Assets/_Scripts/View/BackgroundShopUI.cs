using CosmicMemory.Helper;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CosmicMemory.View
{
    public sealed class BackgroundShopUI : MonoBehaviour
    {
        #region Fields
        [SerializeField] private RectTransform _backgroundContainer;

        private List<BaseChangeImage> _backgrounds = new(15);
        #endregion

        #region Unity Methods
        private void Start()
        {
            _backgrounds.AddRange(_backgroundContainer.GetComponentsInChildren<BaseChangeImage>());

            BaseChangeImage sb = _backgrounds.FirstOrDefault(e => e.IdBack == Instances.GetBackImage(_backgrounds[0]));
            if (sb != null)
            {
                sb.Switch(true);
            }
        }
        #endregion
    }
}
